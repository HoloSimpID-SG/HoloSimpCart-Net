FROM fedora:42 AS build
RUN dnf install -y \
  dotnet-sdk-8.0 \
  # zig-0.14.1 \
  swig-4.3.1 \
  make-4.4.1 && \
  dnf clean all

# Fedora zig repo is behind
WORKDIR /zig
RUN curl -o ./zig.tar.xz "https://ziglang.org/download/0.15.2/zig-x86_64-linux-0.15.2.tar.xz" && \
  tar -xf ./zig.tar.xz --strip-components=1 && \
  rm ./zig.tar.xz

ENV PATH="/zig:$PATH"

WORKDIR /src
COPY ./ ./

RUN make ARCH=x86_64

FROM alpine:3.22 AS final
WORKDIR /app
COPY --from=build /src/bin ./

RUN apk add --no-cache \
  dotnet8-runtime

ENV LANG=en_US.UTF-8 \
  LC_ALL=en_US.UTF-8

ENTRYPOINT ["dotnet", "RuTakingTooLong.dll"]

