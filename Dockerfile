FROM        fsharp/fsharp
MAINTAINER  Miroslav Veith <mveith@hotmail.cz>
			
ADD ./build/app/ app/
EXPOSE 8083
WORKDIR ./app/
ENTRYPOINT ["mono", "/app/WebToEpub.Website.exe"]