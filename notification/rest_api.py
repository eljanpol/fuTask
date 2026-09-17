from fastapi import FastAPI
import uvicorn


app = FastAPI()


@app.post("/api/webhooks/task_created")
async def task_created(data: dict):
    print(data)

    return {"status": 200}


if __name__ == "__main__":
    uvicorn.run("notification.rest_api:app", port=8000, reload=True)
