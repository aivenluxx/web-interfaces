using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

public class Program
{
    // Головний асинхронний метод програми
    public static async Task Main(string[] args)
    {
        // Ініціалізація HttpClient з базовою адресою API (у даному випадку - для роботи з jsonplaceholder)
        using var httpClient = new HttpClient { BaseAddress = new Uri("https://jsonplaceholder.typicode.com/") };

        // Створення екземпляра HttpClientWrapper для використання наших методів GET і POST
        var clientWrapper = new HttpClientWrapper(httpClient);

        // GET запит: Отримання даних (постів)
        Console.WriteLine("Fetching posts...");
        // Викликаємо метод GetAsync з класу HttpClientWrapper для отримання поста з id = 1
        var getResponse = await clientWrapper.GetAsync<Post>("posts/1");

        // Перевірка, чи успішно виконано запит (статус код OK)
        if (getResponse.StatusCode == HttpStatusCode.OK) // Используем HttpStatusCode для перевірки статусу
        {
            // Якщо запит успішний, виводимо заголовок поста
            foreach (var post in getResponse.Data)
            {
                Console.WriteLine($"Post Title: {post.Title}");
            }
        }
        else
        {
            // Якщо запит не вдалося виконати успішно, виводимо повідомлення про помилку
            Console.WriteLine($"GET Error: {getResponse.Message}");
        }

        // POST запит: Створення нового поста
        Console.WriteLine("Creating a post...");
        // Створення нового об'єкта Post для відправки на сервер
        var newPost = new Post
        {
            UserId = 1, // Вказуємо користувача, який створює пост
            Title = "New Post", // Заголовок нового поста
            Body = "This is the content of the new post." // Текст поста
        };

        // Викликаємо метод PostAsync з класу HttpClientWrapper для відправки нового поста на сервер
        var postResponse = await clientWrapper.PostAsync<Post, Post>("posts", newPost);

        // Перевірка, чи успішно створено пост (статус код Created)
        if (postResponse.StatusCode == HttpStatusCode.Created) // Используем HttpStatusCode для перевірки статусу
        {
            // Якщо запит успішний, виводимо ID створеного поста
            foreach (var post in postResponse.Data)
            {
                Console.WriteLine($"Created Post ID: {post.Id}");
            }
        }
        else
        {
            // Якщо запит не вдалося виконати успішно, виводимо повідомлення про помилку
            Console.WriteLine($"POST Error: {postResponse.Message}");
        }
    }
}
