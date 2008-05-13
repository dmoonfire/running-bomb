AUTOMAKE_OPTIONS=dist-bzip2 dist-zip
BUILD = $(top_builddir)/build

mfgames_runningbomb_ASSEMBLY_NAME = MfGames.RunningBomb
mfgames_runningbomb_DLL = $(mfgames_runningbomb_ASSEMBLY_NAME).dll
mfgames_runningbomb_ASSEMBLY = $(BUILD)/$(mfgames_runningbomb_DLL)
mfgames_runningbomb_FILES =  \
	MfGames.RunningBomb/ConstructionControl.cs \
	MfGames.RunningBomb/Draw/CommonDrawShape.cs \
	MfGames.RunningBomb/Draw/DrawCircle.cs \
	MfGames.RunningBomb/Draw/IDrawable.cs \
	MfGames.RunningBomb/Draw/DrawingCommands.cs \
	MfGames.RunningBomb/Draw/DrawingCommandsReader.cs

gtk_drawer_ASSEMBLY_NAME = gtk-drawer
gtk_drawer_EXE = $(gtk_drawer_ASSEMBLY_NAME).exe
gtk_drawer_ASSEMBLY = $(BUILD)/$(gtk_drawer_EXE)
gtk_drawer_FILES =  \
	gtk-drawer/GtkDrawerEntry.cs \
	gtk-drawer/GtkDrawerFrame.cs \
	gtk-drawer/GtkDrawerCanvas.cs

# Generic
all: $(mfgames_runningbomb_ASSEMBLY) $(gtk_drawer_ASSEMBLY)
	cp libs/C5.dll build

clean-local:
	$(RM) -rf $(BUILD)
	$(RM) -f *~ */*~ */*/*~
	$(RM) -rf autom4te.cache config.log

$(mfgames_runningbomb_ASSEMBLY): $(mfgames_runningbomb_FILES) Makefile
	mkdir -p $(BUILD)
	$(CSC) -debug $(CSC_FLAGS) -target:library -out:$@ \
		-doc:$(BUILD)/MfGames.RunningBomb.xml \
		/d:TRACE /d:DEBUG \
		@mfgames_utility_LIBS@ \
		-r:libs/C5.dll \
		-r:System.Drawing \
		$(realpath $(mfgames_runningbomb_FILES))
	chmod a-x $@

$(gtk_drawer_ASSEMBLY): $(gtk_drawer_FILES) $(mfgames_runningbomb_ASSEMBLY)
$(gtk_drawer_ASSEMBLY): gtk-drawer/ui.xml
	mkdir -p $(BUILD)
	$(CSC) -debug $(CSC_FLAGS) -target:winexe -out:$@ \
		/d:TRACE /d:DEBUG \
		@mfgames_utility_LIBS@ \
		-r:libs/C5.dll \
		-r:System.Drawing \
		-r:$(mfgames_runningbomb_ASSEMBLY) \
		-r:/usr/lib/mono/2.0/Mono.Cairo.dll \
		-pkg:gtk-sharp-2.0 \
		-pkg:rsvg-sharp-2.0 \
		-resource:gtk-drawer/ui.xml \
		$(realpath $(gtk_drawer_FILES))
	chmod a+x $@

# Installation
#install-data-local:
# TODO Install the game in the appropriate place

# Uninstallation
#uninstall-local:


# Definition of the various files that need to be cleaned up properly.
CLEANFILES = $(BUILD)/MfGames.RunningBomb.xml 

EXTRA_DIST = $(mfgames_runningbomb_FILES)
