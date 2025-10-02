import os
from fastapi import APIRouter
from fastapi.responses import PlainTextResponse
from pydantic import BaseModel
from manim import Scene, Text, Write, config


class TextScene(Scene):
    def __init__(self, text, **kwargs):
        self.text_to_render = text
        super().__init__(**kwargs)

    def construct(self):
        text = Text(self.text_to_render, font_size=72)
        self.play(Write(text))
        self.wait(1)


def _manim_text(input_text: str) -> str:
    config.media_dir = os.path.join("/shared", "manim_media")
    config.quality = "low_quality"

    scene = TextScene(input_text)
    scene.render()

    return str(scene.renderer.file_writer.movie_file_path)


router = APIRouter()


class Input(BaseModel):
    text: str


@router.post("/manim_text", response_class=PlainTextResponse)
def manim_text(input: Input) -> str:
    return _manim_text(input.text)
