using System.Drawing;
using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace server.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    [HttpGet("ws")]
    public async Task Websocket()
    {
        var context = ControllerContext.HttpContext;

        if (context.WebSockets.IsWebSocketRequest)
        {
            Console.WriteLine("Connection to websocket initiated");
            
            using (WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync())
            {
                Console.WriteLine("Connection to websocket established");
                
                await GetColorData(context, webSocket);
            }
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }

    private async Task GetColorData(HttpContext context, WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        var result =
            await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

        while (!result.CloseStatus.HasValue)
        {
            var rnd = new Random();
            var b = new byte[3];
            
            rnd.NextBytes(b);

            var color = Color.FromArgb(b[0], b[1], b[2]);

            var jsonResult = string.Format("{'red' =  {0}, 'green' = {1}, 'blue' = {2} }", color.R.ToString(), color.G.ToString(), color.B.ToString());

            var bytes = Encoding.ASCII.GetBytes(jsonResult);
            var arraySegment = new ArraySegment<byte>(bytes);

            await webSocket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);

            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        }
        
        await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
    }
}
