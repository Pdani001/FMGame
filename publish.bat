dotnet publish . -p:PublishProfile=Portable
gamebundle -wlM --mg
butler push ./bin/Bundled/win/ pdani/fazbear-multiplayer:win-x64 --userversion-file version.txt
butler push ./bin/Bundled/linux/ pdani/fazbear-multiplayer:linux-x64 --userversion-file version.txt
butler push ./bin/Bundled/mac-arm/ pdani/fazbear-multiplayer:osx-arm64 --userversion-file version.txt
