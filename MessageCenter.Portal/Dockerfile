FROM microsoft/aspnetcore:2.0

RUN apt-get update -y \
	&& apt-get upgrade -y \
	&& apt-get install -y vim \
	&& apt-get install -y libgdiplus 
#	&& ln -sf /usr/share/zoneinfo/Asia/Shanghai /etc/localtime \
#   && echo "Asia/Shanghai" > /etc/timezone

WORKDIR /app

COPY PublishOutput /app/

EXPOSE 80

ENTRYPOINT ["dotnet", "/app/MessageCenter.Portal.dll"]
