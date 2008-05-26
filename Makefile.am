AUTOMAKE_OPTIONS=dist-bzip2 dist-zip
BUILD = $(top_builddir)/build

# Tesselate stolen from agg-sharp
mfgames_runningbomb_ASSEMBLY_NAME = MfGames.RunningBomb
mfgames_runningbomb_DLL = $(mfgames_runningbomb_ASSEMBLY_NAME).dll
mfgames_runningbomb_ASSEMBLY = $(BUILD)/$(mfgames_runningbomb_DLL)
mfgames_runningbomb_FILES =  \
	MfGames.RunningBomb/ConstructionControl.cs \
	MfGames.RunningBomb/Draw/CommonDrawShape.cs \
	MfGames.RunningBomb/Draw/DrawCircle.cs \
	MfGames.RunningBomb/Draw/IDrawable.cs \
	MfGames.RunningBomb/Draw/DrawingCommands.cs \
	MfGames.RunningBomb/Draw/DrawingCommandsReader.cs \
	MfGames.RunningBomb/Physics.cs \
	MfGames.RunningBomb/Mobile.cs \
	MfGames.RunningBomb/Player.cs \
	MfGames.RunningBomb/State.cs \
	MfGames.RunningBomb/Junction.cs \
	MfGames.RunningBomb/JunctionManager.cs \
	MfGames.RunningBomb/JunctionManagerPreload.cs \
	MfGames.RunningBomb/IJunctionFactory.cs \
	MfGames.RunningBomb/RadialJunctionFactory.cs \
	MfGames.RunningBomb/SimpleJunctionFactory.cs \
	MfGames.RunningBomb/Geometry.cs \
	MfGames.RunningBomb/FactoryManager.cs \
	MfGames.RunningBomb/ISegmentFactory.cs \
	MfGames.RunningBomb/SimpleSegmentFactory.cs \
	MfGames.RunningBomb/Segment.cs \
	MfGames.RunningBomb/Functions.cs \
	MfGames.RunningBomb/Score.cs \
	MfGames.RunningBomb/CenterPoint.cs \
	MfGames.RunningBomb/CenterPointList.cs \
	MfGames.RunningBomb/Constants.cs

gtk_drawer_ASSEMBLY_NAME = gtk-drawer
gtk_drawer_EXE = $(gtk_drawer_ASSEMBLY_NAME).exe
gtk_drawer_ASSEMBLY = $(BUILD)/$(gtk_drawer_EXE)
gtk_drawer_FILES =  \
	gtk-drawer/GtkDrawerEntry.cs \
	gtk-drawer/GtkDrawerFrame.cs \
	gtk-drawer/GtkDrawerCanvas.cs

gtk_tunneler_ASSEMBLY_NAME = gtk-tunneler
gtk_tunneler_EXE = $(gtk_tunneler_ASSEMBLY_NAME).exe
gtk_tunneler_ASSEMBLY = $(BUILD)/$(gtk_tunneler_EXE)
gtk_tunneler_FILES =  \
	gtk-tunneler/GtkTunnelerEntry.cs \
	gtk-tunneler/GtkTunnelerFrame.cs \
	gtk-tunneler/GtkTunnelerCanvas.cs

runningbomb_ASSEMBLY_NAME = running-bomb
runningbomb_EXE = $(runningbomb_ASSEMBLY_NAME).exe
runningbomb_ASSEMBLY = $(BUILD)/$(runningbomb_EXE)
runningbomb_FILES =  \
	running-bomb/NewGameMode.cs \
	running-bomb/HudAbstractMode.cs \
	running-bomb/DisplayUniverseAbstractMode.cs \
	running-bomb/RunningGameMode.cs \
	running-bomb/SpeedMeter.cs \
	running-bomb/ViewState.cs \
	running-bomb/EndOfGameMode.cs \
	running-bomb/TestThemeGameMode.cs \
	running-bomb/Themes/ThemeManager.cs \
	running-bomb/Themes/Theme.cs \
	running-bomb/Themes/ThemeFont.cs \
	running-bomb/Themes/IThemeCallback.cs \
	running-bomb/Themes/ThemeContext.cs \
	running-bomb/Themes/ThemeLayout.cs \
	running-bomb/Themes/ThemeSingle.cs \
	running-bomb/Themes/ThemeColor.cs \
	running-bomb/Themes/ThemeViewport.cs \
	running-bomb/Themes/ThemeString.cs \
	running-bomb/Themes/IThemeCommand.cs \
	running-bomb/RunningBombEntry.cs \
	Tesselate/ActiveRegion.cs \
	Tesselate/ContourVertex.cs \
	Tesselate/Dictionary.cs \
	Tesselate/Face.cs \
	Tesselate/HalfEdge.cs \
	Tesselate/mesh.cs \
	Tesselate/Tesselator.cs

# Generic
all: $(mfgames_runningbomb_ASSEMBLY) $(gtk_drawer_ASSEMBLY)
all: $(gtk_tunneler_ASSEMBLY) $(runningbomb_ASSEMBLY)
	cp libs/*.dll libs/*.dll.config build

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
		@mfgames_sprite3_works_LIBS@ \
		@gpc_LIBS@ \
		-r:libs/C5.dll \
		-r:libs/AdvanceMath.dll \
		-r:libs/Physics2DDotNet.dll \
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
		@gpc_LIBS@ \
		-r:System.Drawing \
		-r:$(mfgames_runningbomb_ASSEMBLY) \
		-r:/usr/lib/mono/2.0/Mono.Cairo.dll \
		-pkg:gtk-sharp-2.0 \
		-pkg:rsvg-sharp-2.0 \
		-resource:gtk-drawer/ui.xml \
		$(realpath $(gtk_drawer_FILES))
	chmod a+x $@

$(gtk_tunneler_ASSEMBLY): $(gtk_tunneler_FILES) $(mfgames_runningbomb_ASSEMBLY)
$(gtk_tunneler_ASSEMBLY): gtk-tunneler/ui.xml
	mkdir -p $(BUILD)
	$(CSC) -debug $(CSC_FLAGS) -target:winexe -out:$@ \
		/d:TRACE /d:DEBUG \
		-r:libs/C5.dll \
		@mfgames_utility_LIBS@ \
		@gpc_LIBS@ \
		-r:System.Drawing \
		-r:$(mfgames_runningbomb_ASSEMBLY) \
		-r:/usr/lib/mono/2.0/Mono.Cairo.dll \
		-pkg:gtk-sharp-2.0 \
		-pkg:rsvg-sharp-2.0 \
		-resource:gtk-tunneler/ui.xml \
		$(realpath $(gtk_tunneler_FILES))
	chmod a+x $@

$(runningbomb_ASSEMBLY): $(runningbomb_FILES) $(mfgames_runningbomb_ASSEMBLY)
	mkdir -p $(BUILD)
	$(CSC) -debug $(CSC_FLAGS) -target:winexe -out:$@ \
		-unsafe \
		/d:TRACE /d:DEBUG \
		@boogame_LIBS@ \
		@mfgames_input_LIBS@ \
		@mfgames_utility_LIBS@ \
		@mfgames_sprite3_works_LIBS@ \
		@mfgames_sprite3_boogamebe_LIBS@ \
		@gpc_LIBS@ \
		@tao_opengl_LIBS@ \
		-r:libs/C5.dll \
		-r:libs/AdvanceMath.dll \
		-r:libs/Physics2DDotNet.dll \
		-r:System.Drawing \
		-r:$(mfgames_runningbomb_ASSEMBLY) \
		$(realpath $(runningbomb_FILES))
	chmod a+x $@

# Installation
#install-data-local:
# TODO Install the game in the appropriate place

# Uninstallation
#uninstall-local:


# Definition of the various files that need to be cleaned up properly.
CLEANFILES = $(BUILD)/MfGames.RunningBomb.xml 

EXTRA_DIST = $(mfgames_runningbomb_FILES)
