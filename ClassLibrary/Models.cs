using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace ClassLibrary
{
    public class ModelBrand
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }

    public class ModelDetail
    {
        public string Filename { get; set; }
        public string Size { get; set; }
        public string Type { get; set; }
        public string Download_Url { get; set; }
    }
    public static class ModelService
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public static async Task<List<ModelBrand>> GetAllBrandsAsync()
        {
            var response = await httpClient.GetStringAsync("https://sc.n0v4.site/models/all.json");
            return JsonConvert.DeserializeObject<List<ModelBrand>>(response);
        }

        public static async Task<List<ModelDetail>> GetModelDetailsAsync(string modelUrl)
        {
            var response = await httpClient.GetStringAsync($"https://sc.n0v4.site/models/{modelUrl}");
            return JsonConvert.DeserializeObject<List<ModelDetail>>(response);
        }
    }
    public class ChatHistory
    {
        public string role { get; set; }
        public string content { get; set; }
    }

    public class Session
    {
        public Session(string sessionName)
        {
            Name = sessionName;
        }
        public string CreatDate { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        public string Name { get; set; }
        public string ModelName { get; set; }
        public string ID { get; set; }
        public ChatHistory[] Chats { get; set; }
    }
}