#coc-proxy-csharp

Build, then run with:

    coc-proxy-csharp.exe

**Warning**: This does not automatically close the other side when the connection to the client or server closes.  In fact, it doesn't even detect that either side has closed and will still try to send data on the closed socket.

##Installation

Install [libsodium-net](https://github.com/adamcaudill/libsodium-net) with NuGet.

Patch `libg.so` with:

    adb pull /data/data/com.supercell.clashofclans/lib/libg.so
    dd if=libg.so of=key-backup.bin skip=$(<libg.so.offset) bs=1 count=32
    xxd -p -c 32 key-backup.bin
    echo 72f1a4a4c48e44da0c42310f800e96624e6dc6a641a9d41c3b5039d8dfadc27e | xxd -r -p > key-new.bin
    xxd -p -c 32 key-new.bin
    dd if=key-new.bin of=libg.so seek=$(<libg.so.offset) bs=1 count=32 conv=notrunc
    adb push libg.so /data/data/com.supercell.clashofclans/lib/libg.so

Be sure the correct offset for your client version and device platform is in your `libg.so.offset` file, or replace `$(<libg.so.offset)` with the appropriate offset in the above commands.  Also, be sure to confirm the remote location of the `libg.so` file on your platform.   For example, the file is stored at `/data/app-lib/com.supercell.clashofclans-2/libg.so` on `Kitkat`.

See the [Key Offsets](https://github.com/clugh/coc-proxy/wiki/Key-Offsets) wiki page for the offsets in the various versions of `libg.so`.

###Routing Packets

This topic is expansive and is outside the scope of this document.  See the [Routing Packets](https://github.com/clugh/coc-proxy/wiki/Routing-Packets) wiki page for more information.
