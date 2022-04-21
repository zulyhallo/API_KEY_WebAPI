using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace API_KEY_WebAPI.Middleware
{
    public class ApiKeyMiddleware                   // Middleware: We are able to intercept and process the request object in a single place, we can each and every request that comes  to your puvblished web APIs 
    {                                               // and tap into the request headers collection  and search for the API  key header and validate  its value. 


        private readonly RequestDelegate _requestDelegate;
        private const string APIKEYNAME = "ApiKey";

        public ApiKeyMiddleware(RequestDelegate requestDelegate)
        {
            _requestDelegate = requestDelegate;

        }
        public async Task InvokeAsync (HttpContext context)
        {
            if(!context.Request.Headers.TryGetValue(APIKEYNAME,out var extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("ApiKey oluşturulmadı(Using ApiKey Middleware");
                return;
            }
            var appSettings = context.RequestServices.GetRequiredService<IConfiguration>();
            var apiKey = appSettings.GetValue<string>(APIKEYNAME);
            if (!apiKey.Equals(extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("apikey geçerli değil(using middleware");
            }

            await _requestDelegate(context);
        }
        

    }
}
