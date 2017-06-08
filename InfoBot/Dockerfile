FROM microsoft/dotnet:1.1-runtime

ENV ApiKeys:DarkSky $APIKEYS_DARKSKY

WORKDIR /app
COPY . .

CMD ASPNETCORE_URLS=http://*:$PORT dotnet InfoBot.dll