all: run

configure:
  @just discord-net/configure

reset:
  podman compose down -v

clean:
  podman compose down

run:
  podman compose up --build
