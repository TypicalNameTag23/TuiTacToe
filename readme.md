# Running the Code
## Prerequisites
- .net 10 SDK installed on your machine
- postgresql installed on your machine
- postgresql server running on your machine

## Running
```
git clone git@github.com:TypicalNameTag23/TuiTacToe.git
cd TuiTacToe
cd Scripts
dotnet run

# Run all the initial setup scripts

cd ../Client
dotnet run

# In separate terminal
cd ../Server
dotnet run
```

# Todo
- game logic
- sql database
- tls encryption
- session id
- heartbeat
- id packets so you know what you are responding to
- better handle asynchronous operations; take a look at what calls are blocking and which ones are not

# AI Disclosure
No AI was used in the making of this project.