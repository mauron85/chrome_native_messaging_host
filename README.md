# Chrome Native Messaging Host for KeePassXC (aka keepassxc-proxy)

Self-contained app intented to act as proxy between Google Chrome browser and KeePassXC app.

Rationale: https://github.com/varjolintu/keepassxc-browser/issues/1#issuecomment-310135201

## Create app binary
Binaries can be created for multiple runtimes/platforms supported by dotnet core.

Supported runtimes:

* centos.7-x64;
* debian.8-x64
* ol.7.0-x64
* ol.7.1-x64
* ol.7.2-x64
* opensuse.13.2-x64
* osx.10.10-x64
* osx.10.11-x64
* linuxmint.17-x64
* linuxmint.17.1-x64
* linuxmint.17.2-x64
* linuxmint.17.3-x64
* rhel.7.2-x64
* ubuntu.14.04-x64
* ubuntu.14.10-x64
* ubuntu.15.04-x64
* ubuntu.15.10-x64
* ubuntu.16.04-x64
* win7-x64
* win7-x86]

All possible runtimes can be found in  [chrome_native_messaging_host.csproj](chrome_native_messaging_host.csproj).

Run docker container for building app:

    docker run -it -v $(pwd):/native_messaging_host microsoft/dotnet

From running container:

    cd /native_messaging_host

Build binary with selected runtime ubuntu.16.04-x64:

    dotnet restore
    dotnet build
    dotnet publish -c Release -o out -r ubuntu.16.04-x64

Binary can now be accesible from docker host:
    
    ./out/chrome_native_messaging_host


All dotnet runtime libraries included.
*More info: https://github.com/dotnet/dotnet-docker-samples/tree/master/dotnetapp-selfcontained

## Register Google Chrome native messaging host

Following snippet will register native messaging host. Adjust accordingly.

    CHROME_NATIVE_HOSTS_PATH="${HOME}/.config/google-chrome/NativeMessagingHosts"

    mkdir -p $CHROME_NATIVE_HOSTS_PATH

    PROXY_PATH="$(pwd)/out/chrome_native_messaging_host"
    cat << EOF > "${CHROME_NATIVE_HOSTS_PATH}/com.varjolintu.keepassxc_browser.json" && echo DONE
    {
      "name": "com.varjolintu.keepassxc_browser",
      "description": "KeepassXC integration with Chrome with Native Messaging support",
      "path" : "${PROXY_PATH}",
      "type": "stdio",
      "allowed_origins": [
        "chrome-extension://iopaggbpplllidnfmcghoonnokmjoicf/",
        "chrome-extension://fhakpkpdnjecjfceboihdjpfmgajebii/"
      ]
    }
    EOF

## Debug

* Start Chrome in Terminal. All logging is done to stderr.
* Monitor traffic with tcpdump
>
    tcpdump -i lo 'port 19700' -vv -A
