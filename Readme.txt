usage examples:
    
yCopy.exe c:\tmp\bla c:\tmp\dest -v -r -o -V -d -f "12 jan 08" -t "30-jan-08 18:00"
 
Copy all files from c:\tmp\bla 
To c:\tmp\dest 
-v = Verify the files after the copy - This check that the destination is the same size as the source and also performs a CRC check by reading the whole file.  May slow the process down a tad for larger files ;) 
-r = Recursively 
-o = Overwrite destination if it exists 
-V = Verbose mode 
-d = Delete the source after the copy, if the verify is OK 
-f -t = Only process files older than 12 Jan 08 00:00:00 and younger than 30 Jan 18:00
 
yCopy.exe c:\tmp\bla c:\tmp\dest -v -r -o -V -d -T
 
Same as above but sets the from and to date filters to only process files created today.
 
yCopy.exe c:\tmp\bla c:\tmp\dest -v -r -o -V -d -Y
 
Same as above but sets the from and to date filters to only process files created yesterday.
 
yCopy.exe c:\tmp\bla c:\tmp\dest
 
Very simple usage, copies all files non-recursively from c:\tmp\bla to c:\tmp\dest
 
For more information. Execute: yCopy -?
 
