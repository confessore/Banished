#!/bin/sh

sudo service banished.discord stop

cd /home/$USER/banished
sudo git pull origin master

cd /home/$USER/banished/src/Banished.Discord
sudo dotnet publish -c Release -o /var/dotnetcore/Banished.Discord

sudo service banished.discord start
