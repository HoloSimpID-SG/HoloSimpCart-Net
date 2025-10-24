OUTDIR = ./bin

all: install

lock: configure
	jq -s 'reduce .[] as $$item ({}; . * $$item)' \
		**/packages.lock.json > packages.lock.json

swig:
	mkdir -p ./Native/bindings/
	rm -f ./Native/bindings/*.cs
	rm -f ./Native/swig_wrap.cxx
	swig -csharp -c++ -outdir ./Native/bindings -namespace Native -v ./Native/swig.i

configure: swig
	dotnet restore

native:	swig
	cd ./Native && zig build --release=fast && cd ..

build: native configure
	dotnet build -c Release

install: build
	dotnet publish -c Release -o "$(OUTDIR)" ./RuTakingTooLong/RuTakingTooLong.csproj
	cp ./Native/zig-out/lib/** "$(OUTDIR)"

run: install
	dotnet ./bin/RuTakingTooLong.dll

