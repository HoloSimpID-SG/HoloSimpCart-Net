FROM fedora:42 AS build
WORKDIR /src
COPY ./ ./

RUN dnf install -y \
  dotnet-sdk-8.0 \
  zig-0.14.1 \
  swig-4.3.1 \
  make-4.4.1 && \
  dnf clean all

RUN make

FROM fedora:42 AS final
WORKDIR /app
COPY --from=build /src/bin ./

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

