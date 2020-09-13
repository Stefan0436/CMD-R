compile:
	@echo Compiling CMD-R...
	@msbuild -verbosity:quiet
	@echo
	@echo Copying start script...
	@cp lib/Start\ CMD-R ./CMD-R/bin/Debug/cmd-r -T -f
	@echo Copying update scripts and commands...
	@cp lib/Update\ and\ start\ CMD-R.sh ./CMD-R/bin/Debug/update-and-run -f -T
	@cp lib/Update\ CMD-R.sh ./CMD-R/bin/Debug/update -f -T
	@echo Copying compiled files to build folder...
	@rm -r -f build
	@mkdir build
	@cp ./CMD-R/bin/Debug/* ./build -r -f
	@echo Deleting bin and obj folder...
	@rm -r -f ./CMD-R/bin
	@rm -r -f ./CMD-R/obj
	@echo
	@echo Compiling finished, run \'make run\' to start the bot, which is located in the folder build.
	@echo Optionally, run \'make install\' to install the bot into your system.
run:
	@echo Starting CMD-R...
	@cd build
	@./cmd-r
	@cd ..
install:
	@echo Installing CMD-R to system...
	@lib/install.sh
uninstall:
	@echo Removing CMD-R from your system...
	@lib/uninstall.sh
remove:
	@echo Removing CMD-R from your system...
	@lib/uninstall.sh
