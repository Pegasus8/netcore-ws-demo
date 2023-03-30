import React, { useState, useEffect } from 'react';
import { socket } from './socket';
import { ConnectionState } from './components/ConnectionState';
import { ConnectionManager } from './components/ConnectionManager';
import { MyForm } from './components/MyForm';
import {Events} from './components/Events';

export default function App() {
  const [connectionState, setConnectionState] = useState(socket.readyState);
  const [fooEvents, setFooEvents] = useState([]);

  useEffect(() => {
    function onConnect() {
      setConnectionState(socket.readyState);
      socket.send("Sending message on open");
    } 

    function onDisconnect() {
      setConnectionState(socket.readyState);
    }

    function onFooEvent(value) {
      setFooEvents(previous => [...previous, value]);
      socket.send("Message received!");
    }
    
    socket.addEventListener("open", onConnect);
    socket.addEventListener("close", onDisconnect);
    socket.addEventListener("message", onFooEvent);

    return () => {
      socket.removeEventListener("open", onConnect);
      socket.removeEventListener("close", onDisconnect);
      socket.removeEventListener("message", onFooEvent);
    };
  }, []);

  return (
    <div className="App">
      <ConnectionState connectionState={ connectionState } />
      <ConnectionManager />
      <MyForm />
      <Events events={ fooEvents } />
    </div>
  );
}
