ARCH = x86_64
OUTDIR = ./bin

all: install

clean:
	dotnet clean
	rm -rf ./**/[Oo]bj/
	rm -rf ./**/[Bb]in/
	rm -f ./**/compile_commands.json
	rm -rf ./Native/.zig-cache/
	rm -rf ./Native/zig-out/
	rm -f ./Native/bindings/*.cs
	rm -f ./**/*_wrap.cxx
	rm -f ./**/__*.zig

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
	cd ./Native && zig build cdb

native:	swig
	cd ./Native && zig build --release=fast -Dtarget=$(ARCH)-linux-musl --verbose

build: native configure
	dotnet build -c Release

install: build
	dotnet publish -c Release -o "$(OUTDIR)" ./RuTakingTooLong/RuTakingTooLong.csproj
	cp ./Native/zig-out/lib/** "$(OUTDIR)"

run: install
	dotnet ./bin/RuTakingTooLong.dll

