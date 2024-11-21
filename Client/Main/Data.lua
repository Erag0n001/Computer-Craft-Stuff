---------------------------------------------------------
ChatMessage = {}
ChatMessage.__index = ChatMessage

function ChatMessage:New(message, name)
    local obj= setmetatable({}, self)
    if name and name ~= "System" then
        obj.owner = name
        obj.ownerCharacters = "<>"
    else
        obj.owner = "‎"
        obj.ownerCharacters = "‎"   
    end
    obj.message = message or "‎"
    return obj
end

function ChatMessage:GetDisplayName()
    local char1 = string.sub(self.ownerCharacters, 1, 1)
    local char2 = string.sub(self.ownerCharacters, -1, -1)
    local result = char1 .. self.owner .. char2
    return result
end

---------------------------------------------------------
HttpRequest = {}

function HttpRequest:New(action, content)
    setmetatable(request, self)
    self.headers = {
        ["action"] = action,
        ["content"] = content
    }
end
---------------------------------------------------------
Queue = {}

function Queue:New()
    local queue = {list = {}}
    setmetatable(queue, self)
    self.__index = self
    return queue
end

function Queue:enqueue(item)
    table.insert(self.list, item)
end

function Queue:dequeue()
    return table.remove(self.list, 1)
end

function Queue:peek()
    return self.list[1]
end

function Queue:isEmpty()
    return #self.list == 0
end

function Queue:size()
    return #self.list
end
