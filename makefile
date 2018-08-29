# variables
modname = AngryGuards
version = $(shell cat modInfo.json | awk '/"version"/ {print $$3}' | head -1 | sed 's/[",]//g')
basedir = "../../../../"
build_dir = "adrenalynn/$(modname)"
dllname = $(modname).dll
zipname = $(modname)-$(version).zip
zip_files_extra = "types.json" "angryguards-config.json" "texturemapping.json" "recipes_crafter.json" "icons" "localization" "textures" "meshes"

build:
	mcs /target:library -r:$(basedir)/colonyserver_Data/Managed/Assembly-CSharp.dll,$(basedir)/gamedata/mods/Pipliz/APIProvider/APIProvider.dll,$(basedir)/gamedata/mods/Pipliz/BaseGame/BaseGame.dll,$(basedir)/colonyserver_Data/Managed/UnityEngine.dll -out:"$(dllname)" -sdk:2 src/*.cs src/Research/*.cs

clean:
	rm -f "$(dllname)"
	rm -rf $(build_dir)

checkjson:
	find . -type f -name "*.json" | while read f; do echo $$f; json_pp <$$f >/dev/null; done

default: build

all: checkjson build zip

serverlog:
	less ../../../logs/server/$$(ls -1rt ../../../logs/server | tail -1)

clientlog:
	less ../../../logs/client/$$(ls -1rt ../../../logs/client | tail -1)

zip: default
	rm -f "$(zipname)"
	mkdir -p $(build_dir)
	cp -rp modInfo.json LICENSE README.md $(dllname) $(zip_files_extra) $(build_dir)/
	zip -r "$(zipname)" $(build_dir)
	rm -r $(build_dir)
