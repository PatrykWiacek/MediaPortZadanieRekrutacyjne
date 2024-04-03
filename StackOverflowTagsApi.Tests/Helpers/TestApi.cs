using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using StackOverflowTagsApi;

namespace StackOverflowTagsApi.Tests.Helpers
{
    public class TestApi : WebApplicationFactory<Program>
    {
        public HttpClient Client { get;  }

        public TestApi(Action<IServiceCollection>? services = null)
        {

            Client = WithWebHostBuilder(builder =>
            {
                if (services is not null)
                {
                    builder.ConfigureServices(services);
                }
            }).CreateClient();
        }
    }
}
