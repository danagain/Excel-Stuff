import asyncio
import random

import websockets


async def feed(websocket):
    print("Client connected!")
    while True:
        value = round(random.uniform(0, 100), 2)
        message = f"sensorA:{value}"
        await websocket.send(message)
        await asyncio.sleep(1)

async def main():
    async with websockets.serve(feed, "localhost", 8765):
        await asyncio.Future()  # run forever

if __name__ == "__main__":
    asyncio.run(main())