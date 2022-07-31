using RabbitMQ.Client;

namespace Event.Process.Services.Main.Extensions;

public static class ExtensionsRabbitMQ
{
    public static IBasicProperties SetMessageId(this IBasicProperties prop, string messageId)
    {
        prop.MessageId = messageId;
        return prop;
    }

    public static IBasicProperties SetCorrelationId(this IBasicProperties prop, string correlationId)
    {
        prop.CorrelationId = correlationId;
        return prop;
    }

    public static IBasicProperties SetDeliveryMode(this IBasicProperties prop, byte deliveryMode)
    {
        prop.DeliveryMode = deliveryMode;
        return prop;
    }

    public static IModel SetPrefetchCount(this IModel model, ushort prefetchCount)
    {
        model.BasicQos(0, prefetchCount, false);
        return model;
    }

    public static IBasicProperties CreatePersistentBasicProperties(this IModel model) => model.CreateBasicProperties().SetDeliveryMode(2);
}
