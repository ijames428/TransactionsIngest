@echo off

start cmd /k "cd .\TransactionsIngest\Frontend\ & npm install & npm start"

start cmd /k "cd .\TransactionsIngest\Backend & dotnet watch"