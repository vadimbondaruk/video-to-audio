FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
RUN apt-get update \
    && apt-get install -y ffmpeg
COPY bin/Release/net5.0/ App/
WORKDIR /App
ENTRYPOINT ["dotnet", "video-to-audio.dll"]