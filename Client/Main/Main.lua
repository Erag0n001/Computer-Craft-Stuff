require("Data")
monitor.clear()
monitor.setTextScale(0.5)
local width, height = monitor.getSize()
local currentLine = 1
function MainLoop()
    while true do
        local event, side, channel, replyChannel, message, distance = os.pullEvent("modem_message")
        print(channel)
        if channel == 0 then --Basic chat
            local toLog = ChatMessage:New(message.message or nil, message.owner or nil)
            local messageToSend = textutils.serializeJSON(message)
            local headers = {
                ["action"] = "Log"
            }
            http.post({url = url, body = textutils.serialiseJSON(toLog), headers = headers})
            WriteLine(toLog:GetDisplayName() .. " " .. toLog.message, false)
        elseif channel == 1 then --Websocket
            --Do Stuff
        elseif channel == 3 then --Monitor
            WriteLine(message.message, message.system)
        elseif channel == 4 then
            if message == false then
                error("System restart, malfunctioning")
            end
        end
        modem.transmit(4,4,true)
    end
end

function WriteLine(message, system)
    if(system) then
        monitor.setTextColor(colors.orange)
    end 
    local wrappedMessage = {}
    while #message > 0 do
        local chunk = string.sub(message, 1, width)
        table.insert(wrappedMessage, chunk)
        message = string.sub(message, width + 1)
    end

    for _, line in ipairs(wrappedMessage) do
        if currentLine > height then
            monitor.scroll(1)
            currentLine = height
        end
        monitor.setCursorPos(1, currentLine)
        monitor.write(line)
        currentLine = currentLine + 1
    end    
    monitor.setTextColor(colors.white)
end 
WriteLine("Main Network booting", true)
local success, result = pcall(MainLoop)
print(result)
modem.transmit(4,4,false)
shell.run("startup.lua")