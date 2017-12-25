FROM microsoft/dotnet:2.0-sdk
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY  . ./
CMD 
RUN dotnet publish -c Release -o out
ENTRYPOINT ["dotnet", "out/MineId.dll"]