## Simple Proof-of-Concept for Consul service discovery in ASP.NET Core

### Background

Local microservice development can be a pain in the backside, especially for service discovery. Adding tons and tons of services to e.g. `appsettings.json` is cumbersome, error prone and nearly impossible to keep up to date. Docker compose is one solution, but debugging C# apps running in Docker in Rider is... let's just say "far from ideal".

Spinning up a Consul instance and automatically registering the services there is closer to how things work when the services are deployed with a container orchestrator like Kubernetes, and you can then use a composite run configuration in Rider to spin up whatever services you need to support the local development.

### Explanation

HelloService is a simple ASP.NET Core app that returns "Hello, world!".

If the environment is Development, it will register a Hosted Service called `ConsulBackgroundService` which hooks into the host application lifetime events. 

When the application has started, `ConsulBackgroundService` grabs the (first) address that the application has registered (in `launchSettings.json`) and registers itself with Consul as a service using that address and the name of the assembly containing `Program.cs`. When the application stops, it tries to unregister itself from Consul. This way the registration itself shouldn't need to be modified for each service.

This could be changed so it takes the name and ID of the service from some configuration setting if it's not flexible enough as is.

CallerService is a simple console application that queries the Consul agent for services and tries to find one called "HelloService".

Services need to - not very surprisingly - know the name of the service they're calling, so abstracting away the service address discovery to using Consul when running in development and something else when running in production (note that this might also be Consul) should not be too hard.

Consul **DOES** come with it's own DNS server that can resolve services and can be set to listen to port 53/udp. This would bring the local development experience even closer to the Kubernetes experience, but would require modifying `hosts`/`resolv.conf` or so.

![image](https://user-images.githubusercontent.com/464166/208651128-2cc31587-c901-485c-8ed3-e409a9a2d018.png)

![image](https://user-images.githubusercontent.com/464166/208651222-be52eddb-21b5-450f-9ead-6ef775093d92.png)

![image](https://user-images.githubusercontent.com/464166/208651307-585752a9-7139-4745-988b-f0d42bda9725.png)



