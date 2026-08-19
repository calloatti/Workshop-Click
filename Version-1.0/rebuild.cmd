@echo off
echo Rebuilding Workshop Click (Version-1.0)...
cd /d "C:\Users\calloatti\source\repos\Mods\Workshop Click\Version-1.0"
dotnet build "Workshop Click.csproj" -c Release -t:Rebuild /nodeReuse:false
pause
