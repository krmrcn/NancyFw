using Nancy;
using Nancy.Hosting.Self;
using Nancy.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NancyFw
{
    class Program
    {
        static void Main()
        {
            var host = new Uri("http://localhost:3406");
            using (var server = new NancyHost(host))
            {
                server.Start();
                Console.WriteLine("Host is up!");
                Console.ReadKey();
                server.Stop();
            }
        }
    }

    public class TestModule : Nancy.NancyModule
    {
        static List<Watch> watches;
        static TestModule()
        {
            GenerateItems();
        }

        public TestModule()
        {
            //Get["/"] = _ => "Hello World!";
            Get["/"] = _ =>
            {
                const string firstMessage = "Hello World!";
                return firstMessage;
            };

            Get["/items"] = AllItems;

            Get["/item/{Id}"] = FindItemById;

            Post["add"] = AddItem;

            Get["/delete/{Id}"] = DeleteItem;

            Get["/drop"] = _ => Response.AsJson(HttpStatusCode.MethodNotAllowed);
        }

        dynamic DeleteItem(dynamic param)
        {
            var item = watches.FirstOrDefault(d => d.Id == param.Id);
            if (item == null)
                return Response.AsJson(HttpStatusCode.NoContent);

            watches.Remove(item);
            return Response.AsJson(HttpStatusCode.OK);
        }

        dynamic AddItem(dynamic param)
        {
            var newItem = this.Bind<Watch>();
            watches.Add(newItem);
            return $"Successful! Id: {newItem.Id}";
        }

        dynamic FindItemById(dynamic param)
        {
            var item = watches.FirstOrDefault(d => d.Id == param.Id);
            return Response.AsJson(item);
        }

        dynamic AllItems(dynamic param)
        {
            return Response.AsXml(watches);
        }

        private static void GenerateItems()
        {
            watches = new List<Watch>();
            for (var i = 1; i <= 5; i++)
            {
                var item = new Watch
                {
                    Id = i,
                    Brand = FakeData.NameData.GetCompanyName(),
                    Type = FakeData.TextData.GetAlphaNumeric(10)
                };

                watches.Add(item);
            }
        }
    }
}
