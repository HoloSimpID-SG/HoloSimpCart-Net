from fastapi import FastAPI
from fastapi.responses import PlainTextResponse
from pydantic import BaseModel

app = FastAPI()

class Input(BaseModel):
  name: str
  number: int

@app.post("/hello_from_python", response_class=PlainTextResponse)
def hello_from_python(input: Input):
  text = f"Hello from Python: {input.name}, your assigned number is: {input.number}"
  return text 