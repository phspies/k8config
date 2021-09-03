dotnet publish --os linux --self-contained true /p:PublishSingleFile=true -o dist/linux/
dotnet publish --os win --self-contained true /p:PublishSingleFile=true -o dist/win/
dotnet publish --os osx --self-contained true /p:PublishSingleFile=true -o dist/osx/
