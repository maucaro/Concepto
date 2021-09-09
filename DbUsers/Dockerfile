FROM mcr.microsoft.com/mssql/server:2019-latest
ENV ACCEPT_EULA=Y
ENV SA_PASSWORD=P@ssw0rd
COPY ./DbUsers /var/opt/DbUsers
COPY ./DbUsers.Build/DbUsers.Build.dacpac /var/opt/DbUsers
USER root
RUN apt-get update \
    && apt install unzip \
    && wget -q -O /var/opt/sqlpackage.zip https://go.microsoft.com/fwlink/?linkid=2165213 \
    && unzip -qq /var/opt/sqlpackage.zip -d /var/opt/sqlpackage \
    && rm /var/opt/sqlpackage.zip \
    && chmod +x /var/opt/sqlpackage/sqlpackage \    
    && (/opt/mssql/bin/sqlservr --accept-eula & ) | grep -q "Service Broker manager has started" \
    && /var/opt/sqlpackage/sqlpackage /a:Publish /tsn:localhost /tdn:DbUsers \
         /pr:/var/opt/DbUsers/DbUsers.publish.xml /sf:/var/opt/DbUsers/DbUsers.Build.dacpac \
         /p:IncludeCompositeObjects=true /tu:sa /tp:$SA_PASSWORD \
    && (/opt/mssql-tools/bin/sqlcmd -S localhost -d DbUsers -U SA -P $SA_PASSWORD -Q 'ALTER DATABASE DbUsers SET RECOVERY SIMPLE') \
    && (/opt/mssql-tools/bin/sqlcmd -S localhost -d DbUsers -U SA -P $SA_PASSWORD -i /var/opt/DbUsers/Data.sql)
