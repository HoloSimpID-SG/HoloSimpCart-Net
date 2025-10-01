from fastapi import APIRouter
from fastapi.responses import PlainTextResponse
from pydantic import BaseModel

router = APIRouter()

class Input(BaseModel):
  name: str
  number: int

@router.post("/hello_from_python", response_class=PlainTextResponse)
def hello_from_python(input: Input) -> str:
  text = f"Hello from Python: {input.name}, your assigned number is: {input.number}"
  return text
