// import {io} from "socket.io-client";

// // "undefined" means the URL will be computed from the `window.location` object
// const URL = process.env.NODE_ENV === 'production' ? undefined : 'http://localhost:6643/WeatherForecast/ws';

// export const socket = io('http://localhost:6643/WeatherForecast/ws', {
//     autoConnect: true,
// });

const HTTPS = false;
const WS_HOST = "localhost:6643";
const URL = `${HTTPS ? "wss" : "ws"}://${WS_HOST}/ws`;

export const socket = new WebSocket(URL);
