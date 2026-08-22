using System;

namespace Twitch.Data;

internal class EventSubTableTypeHolder(Type conditionType, Type dataType)
{
    public Type ConditionType = conditionType;
    public Type DataType = dataType;
}