# Notes
- Please note that I did this project all by myself, I tried to get into contact with my group and none responded except one, five days before the assignment was due
- Also note that there is a video demonstrating the client and server running on separate VMs connecting to one another over my public IP address, not over a LAN network
- My TUI library will fully use the thread it is rendering the ui on, if that is the only thread on the machine packets may start dropping
- Running the "Client" program runs an SSH server that you can connect to, by default you can access the application by running ``ssh 127.0.0.1``. If you have already connected to an ssh server on the host 127.0.0.1, you will need to clear out the existing identity in the known_hosts (under ~/.ssh/ on Linux systems) file to be able to connect. 
- Running the client from outside the Client folder will break the lookup process for the configuration file

# Prerequisites
- .net 10 SDK installed on your machine

# Configuration
By default, the SSH server runs on port 22 and looks for the backend on the loopback (127.0.0.1) address. The backend runs on port 4343. If you want to change this, go to the corresponding Program.cs file and edit the constants there.

# Running
```
git clone https://github.com/TypicalNameTag23/TuiTacToe.git
cd TuiTacToe
cd Scripts
dotnet run

# Run all the initial setup scripts

cd ../Client
dotnet run

# In separate terminal
cd Path/To/TuiTacToe/Server
dotnet run

ssh 127.0.0.1
```

# AI Disclosure
No AI was used in the making of this project.