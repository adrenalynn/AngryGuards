# variables
modname = AngryGuards
dllname = $(modname).dll
version = $(shell cat modInfo.json | awk '/"version"/ {print $$3}' | head -1 | sed 's/[",]//g')
zipname = $(modname)-$(version).zip
zip_files_extra = "types.json" "angryguards-config.json" "texturemapping.json" "recipes_crafter.json" "icons" "localization" "textures" "meshes"
builddir = "adrenalynn/$(modname)"
gamedir = /local/games/Steam/steamapps/common/Colony\ Survival

$(dllname): src/*.cs
	mcs /target:library -r:$(gamedir)/colonyserver_Data/Managed/Assembly-CSharp.dll,$(gamedir)/gamedata/mods/Pipliz/APIProvider/APIProvider.dll,$(gamedir)/gamedata/mods/Pipliz/BaseGame/BaseGame.dll,$(gamedir)/colonyserver_Data/Managed/UnityEngine.dll -out:"$(dllname)" -sdk:2 src/*.cs src/Research/*.cs

$(zipname): $(dllname)
	rm -f $(zipname)
	mkdir -p $(builddir)
	cp -r modInfo.json LICENSE README.md $(dllname) $(zip_files_extra) $(builddir)/
	zip -r $(zipname) $(builddir)
	rm -rf $(builddir)

.PHONY: build default clean all zip install serverlog clientlog
build: $(dllname)

zip: $(zipname)

default: build

all: checkjson build zip

clean:
	rm -f $(dllname) $(zipname)
	rm -rf $(builddir)

install: build zip
	rm -rf $(gamedir)/gamedata/mods/$(builddir)
	unzip $(zipname) -d $(gamedir)/gamedata/mods

checkjson:
	find . -type f -name "*.json" | while read f; do echo $$f; json_pp <$$f >/dev/null; done

serverlog:
	less ../../../logs/server/$$(ls -1rt ../../../logs/server | tail -1)

clientlog:
	less ../../../logs/client/$$(ls -1rt ../../../logs/client | tail -1)

