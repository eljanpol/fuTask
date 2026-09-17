using System.Text.Json;
using TaskService.Models;

namespace TaskService.Utils.Retry;

static public class Retry
{
    public async static Task SendWebHook(IHttpClientFactory httpFactory, JsonSerializerOptions jsonOptions, TaskItem task)
    {
        for (int i = 0; i<3; i++)
        {
            try
            {
                var client = httpFactory.CreateClient("NotificationService");
                var response = await client.PostAsJsonAsync("/api/webhooks/task_created",task, jsonOptions);
                if (response.IsSuccessStatusCode)
                {
                    return ;
                }
                Console.WriteLine($"Attempt {i+1}: status {response.StatusCode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Attempt \"{i+1}\" | Message {ex.Message}");
            }
            if (i<2)
            {
                await Task.Delay(1000*(i+1));
            }
        }
    }
}
