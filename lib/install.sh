#!/bin/bash
echo This will install CMD-R into your system, so it can be executed with cmd-r.
read -p "Are you sure you want to continue? [Y/n] " -n 1 -r
echo
if [[ $REPLY =~ ^[Yy]$ ]]
then
    echo
    sudo lib/install-c.sh
fi
