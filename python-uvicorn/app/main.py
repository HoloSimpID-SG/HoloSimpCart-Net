from fastapi import FastAPI

from app.services.manim_text import router as manim_text_router
from app.services.hello import router as hello_router

app = FastAPI()
app.include_router(manim_text_router)
app.include_router(hello_router)
