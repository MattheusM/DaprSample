# DaprSample Encontraras la traduccion al espanol en la parte de abajo

This is an example that you can use to understand how Dapr works, in this sample, we will be using the self-hosted mode.

First of all, you need to install dapr, you can find how to install dapr in the next link: https://docs.dapr.io/getting-started/install-dapr-cli/
There are some steps you need to follow to download and install dapr depending on you OS. After installing dapr you'll need to install Docker containers, you will be able to follow the steps in the official page of DOCKER: https://docs.docker.com/desktop/install/windows-install/

Now that you have dapr and docker you can run the following command: dapr init , this will allow dapr to create some containers in your docker.
You should see this 3 containers running in docker after running the previous command.

<img width="1266" alt="Captura de pantalla 2023-02-13 a la(s) 11 32 47" src="https://user-images.githubusercontent.com/65357617/218501167-c8fd9216-36c1-4af1-9120-fb798c259c0f.png">

If you are able to see the redis, zipkin and placement containers you are all set.

After doing all of this, you can use dapr in your local machine.

# Introduction

Dapr is an open-source, event-driven, portable runtime for building distributed applications. It's a set of building blocks and tools that simplify the process of building microservices-based applications by providing a common set of abstractions for the underlying infrastructure. Dapr is language-agnostic, meaning that it can be used to build applications in any language.

With Dapr, you can build scalable and resilient microservices applications that can run on any platform, including cloud and on-premises. It provides a wide range of abstractions, such as state management, pub/sub messaging, service invocation, and more, that can be used to build applications without having to worry about the underlying infrastructure details.

Overall, Dapr is designed to make it easier for developers to build and manage modern, cloud-native applications by abstracting away much of the complexity associated with building distributed systems.

To continue with, I'm going to show you how to implement a simple funtionality in your microservice using dapr.

<img width="1094" alt="Captura de pantalla 2023-02-13 a la(s) 11 39 18" src="https://user-images.githubusercontent.com/65357617/218502860-59d0c49e-e21d-4c87-8f6b-235d56d2a792.png">

So imagine that you have to services, one that send a message and another one that receive the message sent.
To use the dapr Client you need to add 2 nuggets in your nugget packege --> dapr.ASPNetCore and dapr.Client

after adding these 2 nuggets, you need to add dapr into your program.cs 
This is an example on how you can do it.

<img width="1106" alt="Captura de pantalla 2023-02-13 a la(s) 12 35 46" src="https://user-images.githubusercontent.com/65357617/218516777-75049c82-23cf-48a0-8281-1b1c014dbaf4.png">

after adding dapr into your program.cs, you know can start implementing dapr.

We are going to start with the sender service.

using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Dapr.Client;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using static Dapr.Client.Autogen.Grpc.v1.Dapr;
using DaprClient = Dapr.Client.DaprClient;

namespace EventPublisher.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly ILogger<EventController> _logger;
        public EventController(ILogger<EventController> logger = null)
        {
            _logger = logger ?? NullLogger<EventController>.Instance;
        }


        [HttpPost]
        public async Task<IActionResult> PublishEvent()
        {
            var httpClient = DaprClient.CreateInvokeHttpClient("myapps1");

            //mensaje
            var requestBody = "{\"data\":{\"message\":\"Hola Mundo desde Sender\"}}";

            // Configura la solicitud POST
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(httpClient.BaseAddress+ "subscribe"),
                Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
            };
            var value = request.RequestUri;
            _logger.LogInformation("BaseAdress: {value}", value);
            // Envía la solicitud
            var response = await httpClient.SendAsync(request);

            // Verifica si la solicitud fue exitosa
            if (response.IsSuccessStatusCode)
            {
                return Ok("Evento publicado con éxito");
            }
            else
            {
                return BadRequest("Error al publicar evento");
            }
        }
    }
}


as you can see, first you need to create a Dapr client with the type HttpClient specifying the name of the carside that its communicating, then you need to specify the route (for example subscribe) where it's sending it.

So if the response is SuccessStatusCode (200) it returns a message that says Event published succeded, if it is another response, it means that the message did not arrive.


Now we're going to the Receiver service. In this one you also need to add dapr into your program.cs too.

using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Dapr.Client;

[ApiController]
public class EventReceiverController : ControllerBase
{
    private readonly ILogger<EventReceiverController> _logger;
    private readonly DaprClient _daprClient;

    public EventReceiverController(ILogger<EventReceiverController> logger = null, DaprClient daprClient = null)
    {
        _logger = logger ?? NullLogger<EventReceiverController>.Instance;
        _daprClient = daprClient ?? new DaprClientBuilder().Build();
    }

    [Route("subscribe")]
    [HttpPost]
    public async Task<IActionResult> Subscribe()
    {
        string value = await ReadRequestBodyAsString(Request);
        _logger.LogInformation("Received event: {value}", value);
        return Ok();
    }

    private async Task<string> ReadRequestBodyAsString(HttpRequest request)
    {
        using (var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
        {
            return await reader.ReadToEndAsync();
        }
    }
}

Here we can see, how the controller is receiving the message.


This is how dapr works in this two microservices.

![Diagrama sin título-Página-4 drawio](https://user-images.githubusercontent.com/65357617/218523417-fd037af4-7b4e-489d-89fb-7b09a52ca121.png)




# Ejemplo DAPR
PARTE EN ESPANOL

Este es un ejemplo que puede ser usado para comprender cómo funciona Dapr. En este ejemplo, usaremos el modo self-hosted.

En primer lugar, debes instalar dapr, puedes encontrar cómo instalar dapr en el siguiente enlace: https://docs.dapr.io/getting-started/install-dapr-cli/
Hay algunos pasos que debes seguir para descargar e instalar dapr según su sistema operativo. Después de instalar dapr, deberás instalar los contenedores Docker, podrás seguir los pasos en la página oficial de DOCKER: https://docs.docker.com/desktop/install/windows-install/

Ahora que tienes dapr y docker, puedes ejecutar el siguiente comando: dapr init, esto permitirá que dapr cree algunos contenedores en tu docker.
Deberías ver estos 3 contenedores ejecutándose en la ventana acoplable después de ejecutar el comando anterior.

<img width="1266" alt="Captura de pantalla 2023-02-13 a la(s) 11 32 47" src="https://user-images.githubusercontent.com/65357617/218501167-c8fd9216-36c1-4af1-9120-fb798c259c0f.png">

Si puede ver los contenedores redis, zipkin y placement, estás listo.

Después de hacer todo esto, puede usar dapr en tu máquina local.

# Introduccion

Dapr es ejecutador portátil de código abierto, impulsado por eventos, para crear aplicaciones distribuidas. Es un conjunto de componentes básicos y herramientas que simplifican el proceso de creación de aplicaciones basadas en microservicios al proporcionar un conjunto común de abstracciones para la infraestructura subyacente. Dapr es independiente del lenguaje de programacion, lo que significa que se puede utilizar para crear aplicaciones en cualquier lenguaje.

Con Dapr, puede crear aplicaciones de microservicios escalables y resistentes que pueden ejecutarse en cualquier plataforma, incluidas la nube y las instalaciones. Proporciona una amplia gama de abstracciones, como administración de estado, mensajería de publicación/suscripción, invocación de servicios y más, que se pueden usar para crear aplicaciones sin tener que preocuparse por los detalles de la infraestructura subyacente.

En general, Dapr está diseñado para facilitar a los desarrolladores la creación y administración de aplicaciones modernas nativas de la nube al abstraer gran parte de la complejidad asociada con la creación de sistemas distribuidos.

Para continuar, les mostraré cómo implementar una funcionalidad simple en su microservicio usando dapr.

<img width="1094" alt="Captura de pantalla 2023-02-13 a la(s) 11 39 18" src="https://user-images.githubusercontent.com/65357617/218502860-59d0c49e-e21d-4c87-8f6b-235d56d2a792.png">

Imagina que tienes dos servicios, uno que envía un mensaje y otro que recibe el mensaje enviado.
Para usar el dapr client, debes agregar 2 nuggets en su paquete nugget --> dapr.ASPNetCore y dapr.Client

después de agregar estas 2 pepitas, debe agregar dapr a su programa.cs
Este es un ejemplo de cómo puedes hacerlo.

<img width="1106" alt="Captura de pantalla 2023-02-13 a la(s) 12 35 46" src="https://user-images.githubusercontent.com/65357617/218516777-75049c82-23cf-48a0-8281-1b1c014dbaf4.png">



