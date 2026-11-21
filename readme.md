# Prerequisites
- .net 10 SDK installed on your machine

# Configuration
By default, the SSH server runs on port 22, looks for the backend on the loopback address, and the backend runs on port 4343. If you want to change this, go to the corresponding Program.cs file and edit the constants there.

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