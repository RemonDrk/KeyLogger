# KeyLogger
Just a simple keylogger that tracks key strokes and saves them to the `../AppData/Roaming/preferences.userdata` file and sends the file content as e-mail after the file is big enough. You have to fill in the blank parts about email. Code is probably not efficient, and can easily be detected.

#### Disclaimer
This script is only written for *learning* purposes. It is illegal to use, and I am not responsible for your malicious activities.

####How to close the program after you run it
- If you run the program, it will add itself to startup list. To remove from there, you can go to Task Manager > Startup and disable KeyLogger. *Or* as removeFromStartup.bat file does for you, you can run the program with a single "removeFromStartup" command line arg.
- After on, you can **End Task** of **KeyLogger.exe** from **Task Manager** (Should be under Background processes).