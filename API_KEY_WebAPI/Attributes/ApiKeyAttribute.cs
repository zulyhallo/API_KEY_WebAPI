using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace API_KEY_WebAPI.Attributes                            // AttributeUsage:ApiKeyAttribute'nin nerede kullanılacağını belirten bir Attribute'dir. Controllers gibi sınıflarda kullanılacağı belirtilir.
{                                                             // AttributeTargets:Flag uygulayan bir uygulamadır. We can use pipe|operator (bitwise or) to specify more usages for your custom attribute, 
    [AttributeUsage(validOn: AttributeTargets.Class)]        // so we declare "AttributeUsage(validOn: AttributeTargets.Class | AttributeTargets.Method)]" that used on both classes and/ormethods
    public class ApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        private const string APIKEYNAME = "ApiKey";
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) 
        {
            if(!context.HttpContext.Request.Headers.TryGetValue(APIKEYNAME,out var extractedApiKey)) // if it has a key with name "ApiKey" so first cheking  the request headers collection object 
            {                                                                                       // Of Course we can use whatever header name  you like, some prefer to use X-API-Key etc.
                context.Result = new ContentResult()
                {                                                                                  // if the header doesnt include the ApiKey as key, 
                    StatusCode = 401,                                                             // then we will return a 401 Unauthorized response code and 
                    Content = "API KEY oluşturulmadı"                                            //  with a message  indicating that the  API KEY was  not provided
                };
                return;                                // if the ApiKey header was sent, We will move to the next step which is  validating  that the value of ApiKey header matches the ApiKey defined in our API project.
            }
            var appSettings = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();   // We have obtained our API KEY value that is defined within the appsettings.json
            var apiKey = appSettings.GetValue<string>(APIKEYNAME);                             
            if (!apiKey.Equals(extractedApiKey))                                              // We will match  it versus the extracted  API KEY value  from the request headers collection. 
            {                                                                                // If both match , then we route to the request to the controller to run its intended http action.(post,get etc)
                context.Result = new ContentResult()                        
                {
                    StatusCode = 401,                                                      // If ıt not match , failed the request with a 401 Unauthorized response 
                    Content = "API KEY geçerli değil"                                     // and a message indicating   
                };
            }
            return;

            await next();                                                            // Next step is we decorate our controller  with the ApiKey Attribute
                                                                                    // so that we can authenticate the client that is calling  our endpoint .  
        }
    }
}
