#!/bin/bash
echo This will completely remove CMD-R from your system including server files.
echo Attatched servers will lose their access to the bot.
read -p "Are you sure you want to continue? [Y/n] " -n 1 -r
echo
if [[ $REPLY =~ ^[Yy]$ ]]
then
    echo
    sudo lib/uninstall-c.sh
fi
