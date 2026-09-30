# DAoC Connect
Dawn of Light's Connect.exe reverse engineered from assembly and modified to patch clients below v1.68 for a classic daoc experience.

## How it works
The goal of connect.exe is to mimic the behaviors of camelot.exe but with runtime memory modifications to bypass expansion cd key startup & launcher integrity checks. The launcher will become the parent process of game.dll then inject 0x90:NOP on specific memory regions to disable these checks. I believe the original connect.exe was obfuscated with the name mangling to hide how the bypass method works from mythic/broadsword.

### Creating a custom patch (Advanced)
Depending on which version of DAoC you have the dolloader.exe/connect.exe will not work because it looks for signatures in the memory and if that version is older than 1.68 or part of the 3 expansion set(classic, SI & ToA) it requires multiple rewrites to the memory at 4 different locations. Essentially disabling the required expansion set checks.

First switch the client version in the program.cs to the version you are patching. Currently it is set to SI (1.56)

Use Ghidra & WinDbgx32 to inspect the game.dll memory during runtime. Depending on which version of the game you are debugging you want to search for the flag checks in the memory. An example of these memory regions can be found in the catacombs client by searching for the signature in v1.68+ patch. Using the SI patch I have implemented is good reference for older clients since the memory layout is drastically different than future versions. The goal is to find the "signature" or range of memory that contains the flag checks and disable it using NOP. Find the range. Update the signature. Replace the bytes at (75 07) to (90 90) bypassing the check.
