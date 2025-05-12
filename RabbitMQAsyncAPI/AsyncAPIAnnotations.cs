using System;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class ChannelAttribute : Attribute
{
    public string ChannelName { get; }

    public ChannelAttribute(string channelName)
    {
        ChannelName = channelName;
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class MessageAttribute : Attribute
{
    public string MessageName { get; }

    public MessageAttribute(string messageName)
    {
        MessageName = messageName;
    }
}
