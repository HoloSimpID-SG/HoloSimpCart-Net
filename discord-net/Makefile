all: install

configure:
	mkdir -p ./Native/bindings/
	rm -f ./Native/bindings/*.cs
	rm -f ./Native/swig_wrap.cxx
	swig -csharp -c++ -outdir ./Native/bindings -namespace Native -v ./Native/swig.i
	dotnet restore

build: configure
	cd ./Native && zig build --release=fast && cd ..
	dotnet build -c Release

install: build
	dotnet publish -c Release -o ./bin ./RuTakingTooLong/RuTakingTooLong.csproj
	cp ./Native/zig-out/lib/libNative.so ./bin

run: install
	dotnet ./bin/RuTakingTooLong.dll

