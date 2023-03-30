using System.Drawing;
using System.Net.WebSockets;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("TestAppPolicy",
        policy =>
        {
            policy.WithOrigins("*")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
});

var app = builder.Build();

app.UseWebSockets();

app.Map("/ws", builder =>
{
    builder.Use(async (context, next) =>
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            Console.WriteLine("Connection to websocket initiated");

            using (WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync())
            {
                Console.WriteLine("Connection to websocket established");

                var buffer = new byte[1024 * 4];
                var result =
                    await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                while (!result.CloseStatus.HasValue)
                {
                    var rnd = new Random();
                    var b = new byte[3];
            
                    rnd.NextBytes(b);

                    var color = Color.FromArgb(b[0], b[1], b[2]);

                    var jsonResult = "{'red' = " + color.R.ToString() + ", 'green' = " + color.G.ToString() + ", 'blue' = " + color.B.ToString() + "}";

                    var bytes = Encoding.ASCII.GetBytes(jsonResult);
                    var arraySegment = new ArraySegment<byte>(bytes);
                    
                    Console.WriteLine("Sending message");

                    await webSocket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);

                    Console.WriteLine("Message sent. Waiting for a response");
                    
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    
                    Console.WriteLine("Message received");
                }
        
                await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);

                return;
            }
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }

        await next();
    });
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
