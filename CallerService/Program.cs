// See https://aka.ms/new-console-template for more information

using Consul;

var client = new HttpClient();

var consulClient = new ConsulClient();
var services = 
    await consulClient
        .Agent
        .Services();

var helloService = services
    .Response
    .FirstOrDefault(s => s.Value.Service == "HelloService");

var address = $"{helloService.Value.Address}:{helloService.Value.Port}/HelloWorld";

var result = await client.GetAsync(address);

Console.WriteLine($"Request address: {address}, result {await result.Content.ReadAsStringAsync()}");