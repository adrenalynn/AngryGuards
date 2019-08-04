# variables
modname = AngryGuards
dllname = $(modname).dll
version = $(shell cat modInfo.json | awk '/"version"/ {print $$3}' | head -1 | sed 's/[",]//g')
zipname = $(modname)-$(version).zip
zip_files_extra = types.json angryguards-config.json texturemapping.json recipes_crafter.json science.json icons localization textures meshes
builddir = adrenalynn/$(modname)
gamedir = /local/games/Steam/steamapps/common/Colony\ Survival

$(dllname): src/*.cs
	mcs /target:library -nostdlib -r:$(gamedir)/colonyserver_Data/Managed/Assembly-CSharp.dll,$(gamedir)/gamedata/mods/Pipliz/BaseGame/BaseGame.dll,$(gamedir)/colonyserver_Data/Managed/UnityEngine.CoreModule.dll,$(gamedir)/colonyserver_Data/Managed/mscorlib.dll,$(gamedir)/colonyserver_Data/Managed/System.dll,$(gamedir)/colonyserver_Data/Managed/System.Core.dll,$(gamedir)/colonyserver_Data/Managed/Steamworks.NET.dll -out:"$(dllname)" -sdk:4 -recurse:'src/*.cs'

$(zipname): $(dllname) $(zip_files_extra)
	$(RM) $(zipname)
	mkdir -p $(builddir)
	cp -r modInfo.json LICENSE README.md $(dllname) $(zip_files_extra) $(builddir)/
	zip -r $(zipname) $(builddir)
	$(RM) -r $(builddir)

.PHONY: build default clean all zip install serverlog clientlog
build: $(dllname)

zip: $(zipname)

default: build

all: checkjson build zip

clean:
	$(RM) $(dllname) $(zipname)
	$(RM) -r $(builddir)

install: build zip
	$(RM) -r $(gamedir)/gamedata/mods/$(builddir)
	unzip $(zipname) -d $(gamedir)/gamedata/mods

checkjson:
	find . -type f -name "*.json" | while read f; do echo $$f; json_pp <$$f >/dev/null; done

serverlog:
	less $(gamedir)/logs/server/$$(ls -1rt $(gamedir)/logs/server | tail -1)

clientlog:
	less $(gamedir)/logs/client/$$(ls -1rt $(gamedir)/logs/client | tail -1)

