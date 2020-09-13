#!/bin/bash
if [ ! -d "./build" ]; then
    echo No build folder, nothing to do, exiting...
    exit
fi

if [ ! -f "/usr/bin/cmd-r" ]; then
    echo Installing CMD-R...
    
    mkdir /usr/lib/cmd-r
    cp build/* /usr/lib/cmd-r -r -f
    
    cp lib/cmd-r /usr/bin/cmd-r -f
    cp lib/update-cmdr /usr/bin/update-cmdr -f
    chmod 777 /usr/lib/cmd-r
    
    echo CMD-R has been installed, you can run it by running \'cmd-r\'
else
    read -p "CMD-R is already installed, do you want to update the installation? [Y/n] " -n 1 -r
    echo
    if [[ $REPLY =~ ^[Yy]$ ]]
    then
        echo
        echo Updating CMD-R...
    
        cp build/* /usr/lib/cmd-r -r -f
    
        cp lib/cmd-r /usr/bin/cmd-r -f
        cp lib/update-cmdr /usr/bin/update-cmdr -f
        chmod 777 /usr/lib/cmd-r
        
        echo Update completed, run CMD-R by running \'cmd-r\'
    fi
fi
