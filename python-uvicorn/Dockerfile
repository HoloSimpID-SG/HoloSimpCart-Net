FROM ghcr.io/prefix-dev/pixi:latest
WORKDIR /python-uvicorn

COPY ./ ./
RUN pixi install --verbose

ENV PATH="/python-uvicorn/.pixi/envs/default/bin:$PATH"

CMD ["pixi", "run", "run"]
