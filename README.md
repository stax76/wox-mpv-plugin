# wox-mpv-plugin
 
Wox plugin that controls the mpv media player

### Installion

Extract the files to your plugin directory, the location depends on the Wox version you are using.

```
C:\Users\frank\AppData\Local\Wox\app-1.3.578\Plugins\Wox.Plugin.mpv
```

Furthermore add the following line to your mpv.conf:

```
input-ipc-server = \\.\pipe\mpvsocket
```

Start Wox and type mpv, Wox might take time for indexing in the beginning.