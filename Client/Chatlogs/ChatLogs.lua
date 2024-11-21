function MainLoop()
    while true do
        local event, username, message, uuid, isHidden = os.pullEvent()
        print(event)
        if event == "chat" then
            if not username then username = nil end
            if not message then message = nil end
            local messageObject = 
            {
            owner = username,
            message = message
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
shell.run("startup.lua")