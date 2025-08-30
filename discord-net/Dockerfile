FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /discord-bot
COPY ./ ./

RUN dotnet restore && \
    dotnet publish -c Release -o build

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /discord-bot
COPY --from=build /discord-bot/build .

ENV LANG=en_US.UTF-8 \
  LC_ALL=en_US.UTF-8 \

ENTRYPOINT ["dotnet", "RuTakingTooLong.dll"]
