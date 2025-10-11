FROM fedora:42 AS build
WORKDIR /src
COPY ./ ./

RUN dnf install -y \
  dotnet-sdk-8.0 \
  zig-0.14.1 \
  swig-4.3.1 && \
  dnf clean all

WORKDIR /src/swig
RUN mkdir bindings && \
  swig -csharp -c++ -outdir ./bindings -namespace Native -v swig.i && \
  zig build --release=fast --verbose

WORKDIR /src
RUN dotnet restore && \
    dotnet publish -c Release -o ./out && \
    cp ./swig/zig-out/lib/** ./out/

FROM fedora:42 AS final
WORKDIR /app
COPY --from=build /src/out ./

RUN dnf install -y \
  dotnet-runtime-8.0 && \
  dnf clean all
# FROM alpine:latest AS final
# WORKDIR /app
# COPY --from=build /src/out ./
#
# RUN apk add \
#   dotnet8-runtime \
#   gcompat \
#   libucontext

ENV LANG=en_US.UTF-8 \
  LC_ALL=en_US.UTF-8

ENTRYPOINT ["dotnet", "RuTakingTooLong.dll"]

