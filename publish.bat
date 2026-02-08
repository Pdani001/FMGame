dotnet publish . -p:PublishProfile=win-x64
dotnet publish . -p:PublishProfile=linux-x64
dotnet publish . -p:PublishProfile=osx-arm64
butler push ./bin/Release/win-x64/ pdani/fazbear-multiplayer:win-x64 --userversion-file version.txt
butler push ./bin/Release/linux-x64/ pdani/fazbear-multiplayer:linux-x64 --userversion-file version.txt
butler push ./bin/Release/osx-arm64/ pdani/fazbear-multiplayer:osx-arm64 --userversion-file version.txt
