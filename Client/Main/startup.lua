_G.modem = peripheral.find("modem") or error("No modem attached", 0)
_G.monitor = peripheral.find("monitor") or error("No monitor found", 0)
_G.url = "http://70.51.50.99:25565"
modem.open(0)
modem.open(1)
modem.open(2)
modem.open(3)
modem.open(2000)
shell.run("Main.lua")