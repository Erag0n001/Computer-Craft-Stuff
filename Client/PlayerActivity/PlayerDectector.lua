function MainLoop()
    while true do
        local event, username, message, uuid, isHidden = os.pullEvent()
        print(event)
        if event == "playerJoin" then
            local messageObject = 
            {
            message = username .. " joined the game"
            }
            modem.transmit(0,0, messageObject)
        elseif event == "playerLeave" then
            local messageObject = 
            {
            message = username .. " left the game"
            }
            modem.transmit(0,0, messageObject)
        elseif event == "modem_message" then
            if(message == 4) then
                if isHidden == false then
                    error("System restart, malfunctioning")
                end
            end
        end
    end
end
local success, result = pcall(MainLoop)
print(result)
modem.transmit(4,4,false)
shell.run("startup.lua")