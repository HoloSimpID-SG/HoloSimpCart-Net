FROM registry.fedoraproject.org/fedora:43 AS build
RUN dnf --setopt=install_weak_deps=False install -y \
  dotnet-sdk-10.0 \
  swig && \
  dnf clean all

# Fedora zig repo is behind
WORKDIR /zig
RUN curl -o ./zig.tar.xz "https://ziglang.org/download/0.15.2/zig-$(uname -m)-linux-0.15.2.tar.xz" && \
  tar -xf ./zig.tar.xz --strip-components=1 && \
  rm ./zig.tar.xz
ENV PATH="/zig:$PATH"

WORKDIR /src
COPY ./ ./

RUN ([ "$(uname -m)" = "x86_64" ] && echo "x64" || echo "arm64") >> .tmp-arch

RUN dotnet publish ./RuTakingTooLong \
  --runtime "linux-musl-$(cat .tmp-arch)" \
  --configuration Release \
  --output ./bin

FROM alpine:3.22 AS final
WORKDIR /app
COPY --from=build /src/bin ./

RUN apk add --no-cache \
  dotnet9-runtime

ENV LANG=en_US.UTF-8 \
  LC_ALL=en_US.UTF-8

ENTRYPOINT ["dotnet", "RuTakingTooLong.dll"]

