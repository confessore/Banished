#!/bin/sh

sudo systemctl stop banished.discord.service

sudo systemctl disable banished.discord.service

read -p "Banished Discord Application Token: " discordToken

sudo cp ./services/banished.discord.service ./services/banished.discord.service.backup

sudo sed -i '/BanishedDiscordToken=/s/$/'"$discordToken"'/' ./services/banished.discord.service.backup

sudo mv ./services/banished.discord.service.backup /etc/systemd/system/banished.discord.service

sudo systemctl enable banished.discord.service

sudo systemctl start banished.discord.service
