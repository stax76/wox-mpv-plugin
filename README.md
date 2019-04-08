# wox-mpv-plugin
 
Wox launcher plugin that controls the mpv media player.

### Installation

Extract the files to your plugin directory, the location depends on the Wox version you are using.

```
C:\Users\frank\AppData\Local\Wox\app-1.3.578\Plugins\Wox.Plugin.mpv
```

Add the following line to your mpv.conf:

```
input-ipc-server = \\.\pipe\mpvsocket
```

Restart Wox.

### Usage

Start mpv(.net).

Type mpv in Wox and search, select and execute mpv commands.

### Screenshot

![](https://github.com/stax76/wox-mpv-plugin/blob/master/wox-mpv-plugin.png)