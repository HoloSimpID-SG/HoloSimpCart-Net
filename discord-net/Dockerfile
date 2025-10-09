FROM fedora:latest AS build
WORKDIR /src
COPY ./ ./

RUN dnf install -y \
  dotnet-sdk-8.0 \
  zig \
  swig

WORKDIR /src/swig
RUN mkdir bindings && \
  swig -csharp -c++ -outdir ./bindings -namespace Native swig.i && \
  zig build --release=fast

WORKDIR /src
RUN dotnet restore && \
    dotnet publish -c Release -o ./out && \
    cp ./swig/zig-out/lib/** ./out/

FROM fedora:latest AS final
WORKDIR /app
COPY --from=build /src/out ./

RUN dnf install -y \
  dotnet-runtime-8.0 \
  glibc

ENV LANG=en_US.UTF-8 \
  LC_ALL=en_US.UTF-8

ENTRYPOINT ["dotnet", "RuTakingTooLong.dll"]

