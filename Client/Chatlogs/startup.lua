_G.modem = peripheral.find("modem") or error("No modem attached", 0)

modem.open(0)
modem.open(1)
modem.open(4)
modem.open(2000)
repeat
    modem.transmit(2000,2000,true)
    os.startTimer(10)
    event, side, channel, replyChannel, message, distance = os.pullEvent()
until channel == 4
print("test")
local toTransmit = {message = "Chatlogs booting", system = true}
modem.transmit(3,3,toTransmit)
shell.run("ChatLogs.lua")