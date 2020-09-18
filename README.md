<p align="center">
  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+<br />
  THIS PROJECT IS WORK IN PROGRESS AND NOT CURRENTLY USABLE<br />
  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
</p>



# CMD-R
CMD-R is a discord management, scheduling and logging bot.

# Compiling guide
## Linux compiling guide
1.  Clone the git using the command `git clone https://github.com/Stefan0436/CMD-R.git`
2.  Enter the cloned repo by running `cd CMD-R`
3.  Mark `configure` as executable by running `chmod +x ./configure`
4.  Configure the project and download C# dependencies by running `./configure`
5.  Build the project by running `make`

### Optional steps (to run or install the bot)
6.  Run the bot inside the build folder by running `make run`
7.  Install the bot to your system by running `make install` so you can run it with `cmd-r`


## Windows compiling guide
Coming soon.

## Guide to get a discord bot token
1.  Head over to https://discord.com/developers/applications/
2.  Click New Application
3.  Give the application a name (preffered you name it something like `CMD-R fork of <your name here>`)
4.  Then, click on `Bot` in the settings menu (the left side of the page)
5.  Next, click `Add Bot`
6.  I know it says `Reveal token` but we are not there yet, scroll down until you see the settings category `Bot Permissions` and select the required permissions (see the bottom of this page)
7.  Scroll all the way down and copy the number in `Permissions Integer` and save it in a text file named bot info which can be saved anywhere you like, but it is an important file, because it will hold your bots information for later in the guides. (line example: Perm Int: \<integer\>)
8.  Scroll back up and click the link `Click to Reveal Token` and copy the token to the text file (line example: Token: \<token\>)
9.  Next, go to the tab `OAuth` in the settings menu (the left side of the page), look for `CLIENT ID` and copy it to the text file (line example: ID: \<client id\>)
10. Then copy the following url to the text file: `https://discord.com/api/oauth2/authorize?client_id=<client id>&permissions=<perm int>&scope=bot` (save it twice, so that you can change it later using the template, in one of them, change the id and the perm integer, remove the '\<' and the '\>', example line: URL-1/2: \<url\>)
11. Use the url you just created to add the bot to servers (copy it and open it in your browser, see 'Guide to create a discord server' for creating a server)
12. Optional: Add a profile icon to the bot in the `Bot` page in the settings menu (the left side of the page)
13. Optional: Add the profile icon as a app icon in the `General Information` page in the settings menu (the left side of the page)

## Guide to create a discord server (for those who don't know how)
1. Open discord (or the browser version: `https://discord.com/app`)
2. Click on the '+' in the menu bar on the right side of the app/page. (2nd/3rd option from the bottom)
3. Click `Create My Own`
4. Enter a server name
5. Optional: Upload a server icon
6. Click `Create`

## Guide for the main configuration
1.  Start the bot (usually `make run` but `cmd-r` if you installed the bot to your system)
2.  Enter the bot token you saved in the text file
3.  Open the url you saved in the text file in a browser, and add the bot to a server.
(incomplete guide, todo, add the rest of the guide)

###  My bot crashes on startup: 'Discord.Net.HttpException: The server responded with error 401: 401: Unauthorized'
It looks like you entered a invalid token, delete the token config by running the following:
1. (Installed to system): rm -r /usr/lib/cmd-r/Bot.cfg
1. (Locally in cloned repo): rm -r ./build/Bot.cfg
2. Start your bot and enter your token again (from the file)

## Guide for module development
Coming soon

# Other guides

### Updating an installed version
Updating is simple, just run `update-cmdr` to download and install the latest version, but make sure the bot is completely shut down.
Or alternatively, do the compiling guide again, which overwrites the current installation.


### Uninstalling an CMD-R installation
Sadly, you cannot uninstall the bot without cloning the repo again, to simplify the process, here are the commands to uninstall the bot:
```
git clone https://github.com/Stefan0436/CMD-R.git
cd CMD-R
chmod +x ./configure
./configure
make
make uninstall
```


### Direct commands for installing without prompts
If you are trying to create a direct install script (for example, to add the bot to a package manager), these are the commands you need:
```
git clone https://github.com/Stefan0436/CMD-R.git
cd CMD-R
chmod +x ./configure
./configure
make
make install-no-requests
```

### Direct commands for uninstalling without prompts
If you are trying to create a direct uninstall script (for example, to add the bot to a package manager), these are the commands you need to uninstall it:
```
git clone https://github.com/Stefan0436/CMD-R.git
cd CMD-R
chmod +x ./configure
./configure
make
make uninstall-no-requests
```


## Required bot permissions:
Administrator (temporary, specific perms are coming soon!)
