using Event.Process.Services.Main.Extensions;
using Event.Process.Services.Main.Model;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Event.Process.Services.Main.RabbitMQ;

public class Consumer
{
    private readonly IModel model;
    private readonly IConnection connection;
    private readonly ILogger<Consumer> logger;
    // private readonly SqlConnection sqlConnection;
    // private readonly MessageDataService messageDataService;
    private string queue;
    private EventingBasicConsumer eventingBasicConsumer;
    private volatile bool isRunning;
    private bool isInitialized;

    public string ConsumerTag { get; private set; }
    public int MessagesPerSecond { get; private set; }

    public string Id { get; }

    public Consumer(IModel model, IConnection connection, ILogger<Consumer> logger
    // , SqlConnection sqlConnection, MessageDataService messageDataService
    )
    {
        this.model = model;
        this.connection = connection;
        this.logger = logger;
        // this.sqlConnection = sqlConnection;
        // this.messageDataService = messageDataService;
        this.Id = Guid.NewGuid().ToString("D");
        this.eventingBasicConsumer = new EventingBasicConsumer(model);
        this.eventingBasicConsumer.Received += this.OnMessage;
    }

    public void Initialize(string queue, int messagesPerSecond)
    {
        if (this.isInitialized) throw new InvalidOperationException("Initialize só pode ser chamado uma vez");
        this.queue = queue;
        this.MessagesPerSecond = messagesPerSecond;
        this.isInitialized = true;
    }


    private void OnMessage(object sender, BasicDeliverEventArgs eventArgs)
    {
        //Thread.Sleep(TimeSpan.FromSeconds(2));
        EventModel @event = new();

        if (this.isRunning == false)
        {
            if (this.model.IsOpen)
            {
                this.model.BasicReject(eventArgs.DeliveryTag, true);
                this.logger.LogInformation("Mensagem sofreu rejeição leve em função do desligamento do consumidor");
            }
            return;
        }
        if (this.MessagesPerSecond != 0)
            this.MessagesPerSecond.AsMessageRateToSleepTimeSpan().Wait();

        MessageModel message = null;
        string messageString = string.Empty;

        try
        {
            message = eventArgs.Body.ToArray().ToUTF8String().Deserialize<MessageModel>();

            messageString = eventArgs.Body.ToArray().ToUTF8String();
            @event = eventArgs.Body.ToArray().ToUTF8String().Deserialize<EventModel>();
        }
        catch (Exception ex)
        {
            if (this.model.IsOpen)
            {
                this.model.BasicReject(eventArgs.DeliveryTag, false);
                this.logger.LogError(ex, "Mensagem sofreu uma rejeição grave em função de um erro na desserialização. A mensagem será descartada");
            }
            return;
        }


        if (this.isRunning)
        {
            //using var sqlTransaction = this.sqlConnection.BeginTransaction();
            try
            {
                if (this.isRunning)
                {
                    //this.messageDataService.MarkAsProcessed(message, this.sqlConnection, sqlTransaction);

                    if (this.isRunning)
                    {
                        this.logger.LogInformation("Processando mensagem {0}", eventArgs.ConsumerTag);
                        this.logger.LogInformation("CorrelationID: {0} - EventName: {1}", @event.CorrelationId, @event.Metadata.Id);
                        //sqlTransaction.Commit();
                        this.model.BasicAck(eventArgs.DeliveryTag, false);
                    }
                    else
                    {
                        this.logger.LogInformation("Abordando procesamento sem ack e sem commit");
                    }
                }
                else
                {
                    if (this.model.IsOpen)
                    {
                        this.model.BasicReject(eventArgs.DeliveryTag, true);
                        this.logger.LogInformation("Mensagem sofreu rejeição leve em função do desligamento do consumidor");
                    }
                    //sqlTransaction.Rollback();
                }
            }
            catch (Exception ex)
            {
                if (this.model.IsOpen)
                {
                    this.model.BasicNack(eventArgs.DeliveryTag, false, true);
                    this.logger.LogError(ex, "Mensagem foi reenfileirada para processamento futuro, o consumidor atual não conseguiu processá-la.");
                }

                // if (sqlTransaction.Connection != null)
                //     sqlTransaction.Rollback();
            }
        }
        else
        {
            if (this.model.IsOpen) this.model.BasicReject(eventArgs.DeliveryTag, true);
        }
    }


    public Consumer Start()
    {
        if (this.isInitialized == false) throw new InvalidOperationException("Instancia não inicializada");

        if (this.MessagesPerSecond > 0)
            this.model.SetPrefetchCount((ushort)(this.MessagesPerSecond * 5));
        else
            this.model.SetPrefetchCount((ushort)(1000));

        this.isRunning = true;

        this.ConsumerTag = this.model.BasicConsume(this.queue, false, this.eventingBasicConsumer);

        return this;
    }

    public Consumer Stop()
    {
        this.isRunning = false;

        if (!string.IsNullOrWhiteSpace(this.ConsumerTag))
        {
            this.model.BasicCancel(this.ConsumerTag);
        }


        this.model.Close();

        this.model.Dispose();

        this.connection.Close();

        this.connection.Dispose();

        //this.sqlConnection.Close();

        return this;
    }
}