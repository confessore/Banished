#!/bin/sh

sudo systemctl stop banished.discord.service

sudo systemctl disable banished.discord.service

sudo rm /etc/systemd/system/banished.discord.service
