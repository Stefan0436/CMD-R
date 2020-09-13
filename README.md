# CMD-R
CMD-R is a discord management, scheduling and logging bot.



# Compiling guide
## Linux compiling guide
1.  Clone the git using the command `git clone https://github.com/Stefan0436/CMD-R.git`
2.  Enter the cloned repo by running `cd CMD-R`
3.  Mark `configure` as executable by running `chmod +x ./configure`
4.  Configure the project and download dependencies by running `./configure`
5.  Build the project by running `make`

### Optional steps (to run or install the bot)
6.  Run the bot inside the build folder by running `make run`
7.  Install the bot to your system by running `make install` so you can run it with `cmd-r`

## Windows compiling guide
Coming soon.

# Other guides

## Updating an installed version
Updating is simple, just run `update-cmdr` to download and install the latest version, but make sure the bot is completely shut down.
Or alternatively, do the compiling guide again, which overwrites the current installation.


## Uninstalling an installed version
Sadly, you cannot uninstall the bot without cloning the repo again, to simplify the process, here are the commands to uninstall the bot:
```
git clone https://github.com/Stefan0436/CMD-R.git
cd CMD-R
chmod +x ./configure
make
make uninstall
```


## Direct commands for installing without prompts
If you are trying to create a direct install script (for example, to add the bot to a package manager), these are the commands you need:
```
git clone https://github.com/Stefan0436/CMD-R.git
cd CMD-R
chmod +x ./configure
make
make install-no-requests
```

## Direct commands for uninstalling without prompts
If you are trying to create a direct uninstall script (for example, to add the bot to a package manager), these are the commands you need to uninstall it:
```
git clone https://github.com/Stefan0436/CMD-R.git
cd CMD-R
chmod +x ./configure
make
make uninstall-no-requests
```
