local key =KEYS[1]
local logidentifier=KEYS[2]

local hash=redis.call('hgetall',key)
if #hash == 0 then
  return { err = 'The key "'..key..'" does not exist' }
end

local lookup="";

if logidentifier=="" then
   lookup="log:"..key.."/";
else
   lookup="log:"..key.."/"..logidentifier;
end

redis.call('hmset', lookup, unpack(hash));
redis.call('xadd','Stream:Customer',"*","key",key, "lookup",lookup);