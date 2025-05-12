using System;
using System.Linq;
using System.Reflection;
using YamlDotNet.Serialization;
using System.IO;

public class AsyncAPIGenerator
{
    public static string GenerateAsyncAPISpec()
    {
        var asyncApiSpec = new
        {
            asyncapi = "2.6.0",
            info = new
            {
                title = "Exemplo de API Assíncrona",
                version = "1.0.0",
                description = "API gerada automaticamente a partir de anotações",
            },
            servers = new
            {
                production = new
                {
                    url = "rabbitmq://localhost",
                    protocol = "amqp",
                }
            },
            channels = new object(),
            messages = new object()
        };

        var serviceType = typeof(MessageService);
        var methods = serviceType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

        var channels = methods
            .Where(m => m.GetCustomAttributes(typeof(ChannelAttribute), false).Any())
            .Select(m =>
            {
                var channelAttr = m.GetCustomAttributes(typeof(ChannelAttribute), false).Cast<ChannelAttribute>().FirstOrDefault();
                var messageAttr = m.GetCustomAttributes(typeof(MessageAttribute), false).Cast<MessageAttribute>().FirstOrDefault();

                return new
                {
                    channelName = channelAttr?.ChannelName,
                    method = m.Name,
                    message = new
                    {
                        name = messageAttr?.MessageName,
                        payload = new
                        {
                            type = "object",
                            properties = new
                            {
                                userId = new { type = "string" },
                                email = new { type = "string" }
                            }
                        }
                    }
                };
            })
            .ToArray();

        var yaml = new SerializerBuilder().Build().Serialize(asyncApiSpec);
        return yaml;
    }
}
