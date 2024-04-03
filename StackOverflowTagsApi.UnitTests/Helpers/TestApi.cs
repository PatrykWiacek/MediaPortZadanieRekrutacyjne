using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace StackOverflowTagsApi.Tests.Helpers
{
    public class TestApi : WebApplicationFactory<Program>
    {
        public HttpClient Client { get;  }

        public TestApi(Action<IServiceCollection>? services = null)
        {
            Client = WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("test");
                if (services is not null)
                {
                    builder.ConfigureServices(services);
                }
            }).CreateClient();
        }
    }
}
