import React from 'react';

export function ConnectionState({ connectionState }) {
    let status;
    
    switch(connectionState) {
        case WebSocket.CONNECTING:
            status = "connecting";
            break;
        case WebSocket.OPEN:
            status = "open";
            break;
        case WebSocket.CLOSING:
            status = "closing";
            break;
        case WebSocket.CLOSED:
            status = "closed";
            break;
        default:
            throw new Error(`Unrecognized state code ${connectionState}`);
    }

  return <p>State: { status }</p>;
}
