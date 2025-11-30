FROM ghcr.io/prefix-dev/pixi:latest
WORKDIR /python-uvicorn

COPY ./ ./
RUN pixi install --verbose

EXPOSE 8000

ENV PATH="/python-uvicorn/.pixi/envs/default/bin:$PATH"

CMD ["pixi", "run", "run"]
