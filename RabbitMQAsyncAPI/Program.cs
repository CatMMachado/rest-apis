using System;

class Program
{
    static void Main(string[] args)
    {
        // Enviar uma mensagem usando o produtor
        Producer.SendMessage("Mensagem de teste");

        // Iniciar o consumidor em outra thread ou processo
        // Consumidor.StartConsuming();  // Para fins de teste, pode ser comentado

        // Gerar e exibir a especificação AsyncAPI
        string asyncApiSpec = AsyncAPIGenerator.GenerateAsyncAPISpec();
        Console.WriteLine("\nEspecificação AsyncAPI Gerada:");
        Console.WriteLine(asyncApiSpec);
    }
}
