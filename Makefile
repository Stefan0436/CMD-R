compile:
	@lib/installscripts.sh
	@echo
	@echo Compiling finished, run \'make run\' to start the bot, which is located in the folder build.
	@echo Optionally, run \'make install\' to install the bot into your system.
run:
	@echo Starting CMD-R...
	@(cd build; ./cmd-r)
install:
	@echo Installing CMD-R to system...
	@lib/install.sh
install-no-requests:
	@echo Installing CMD-R to system...
	@sudo lib/do-install-c.sh
uninstall:
	@echo Removing CMD-R from your system...
	@lib/uninstall.sh
remove:
	@echo Removing CMD-R from your system...
	@lib/uninstall.sh
uninstall-no-requests:
	@echo Removing CMD-R from your system...
	@sudo lib/do-uninstall-c.sh
remove-no-requests:
	@echo Removing CMD-R from your system...
	@sudo lib/do-uninstall-c.sh
