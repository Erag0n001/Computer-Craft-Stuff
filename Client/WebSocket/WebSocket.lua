local foundSocket = false
local socket
while foundSocket == false do
    local ws, err = http.websocket("ws://70.51.50.99:25565")
    if ws then
        socket = ws
        foundSocket = true
        print("Found socket!")
    end
    os.sleep(2)
end


local chatbox = peripheral.find("chatBox")
function Main()
    local sucess, result = pcall(function()
        while true do
            local message, err = socket.receive(2)

            if err then
                chat.sendMessageToPlayer(err, "Erag0n001")
            end 

            if(message) then
                local chatMessage = textutils.unserializeJSON(message)
                if chatMessage.message == "Ping" then
                    print("Ping")
                else
                    modem.transmit(3,3,"<" .. chatMessage.owner .. "> " .. chatMessage.message)
                    chatbox.sendMessage(chatMessage.message, chatMessage.owner, "<>", "&3")
                end
            end
            os.sleep(0.1)
        end
    end)
    if success then
        print("Result:", result)
    else
        print("Error:", result)
        Main()
    end
end

function KeepAlive()
    local sucess, result = pcall(function()
        while true do
            socket.send("Ping")
            os.sleep(2)
        end
    end)
    if success then
        print("Result:", result)
    else
        print("Error:", result)
        KeepAlive()
    end
end

parallel.waitForAny(KeepAlive, Main)
