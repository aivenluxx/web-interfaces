using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json; // Підключення методу PostAsJsonAsync
using System.Text.Json;
using System.Threading.Tasks;

public class HttpClientWrapper
{
    private readonly HttpClient _httpClient; // Поле для зберігання екземпляра HttpClient

    // Конструктор, який приймає екземпляр HttpClient для ініціалізації
    public HttpClientWrapper(HttpClient httpClient)
    {
        _httpClient = httpClient; // Ініціалізація поля _httpClient
    }

    // Асинхронний метод для виконання GET-запиту
    public async Task<ApiResponse<T>> GetAsync<T>(string url)
    {
        try
        {
            // Виконуємо GET-запит за заданою URL-адресою
            var response = await _httpClient.GetAsync(url);
            // Зчитуємо відповідь як рядок
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode) // Якщо запит успішний
            {
                T data;
                try
                {
                    // Спроба десеріалізації в один об'єкт типу T
                    data = JsonSerializer.Deserialize<T>(content);
                }
                catch
                {
                    // Якщо не вдалося десеріалізувати як один об'єкт, пробуємо як список
                    var dataList = JsonSerializer.Deserialize<List<T>>(content);
                    return new ApiResponse<T>
                    {
                        Message = "Success", // Повідомлення про успішну операцію
                        StatusCode = response.StatusCode, // Статус код відповіді
                        Data = dataList // Повертаємо колекцію даних
                    };
                }

                // Повертаємо дані у вигляді списку, навіть якщо це один об'єкт
                return new ApiResponse<T>
                {
                    Message = "Success", // Повідомлення про успіх
                    StatusCode = response.StatusCode, // Статус код відповіді
                    Data = new List<T> { data } // Список з одним елементом
                };
            }

            // Якщо статус код відповіді не є успішним, повертаємо помилку
            return new ApiResponse<T>
            {
                Message = $"Error: {response.ReasonPhrase}", // Причина помилки
                StatusCode = response.StatusCode // Статус код відповіді
            };
        }
        catch (Exception ex)
        {
            // Якщо виникла помилка під час запиту або обробки, повертаємо виняток
            return new ApiResponse<T>
            {
                Message = $"Exception: {ex.Message}", // Повідомлення про виняток
                StatusCode = HttpStatusCode.InternalServerError // Код помилки сервера
            };
        }
    }

    // Асинхронний метод для виконання POST-запиту з передачею даних
    public async Task<ApiResponse<T>> PostAsync<T, TPayload>(string url, TPayload payload)
    {
        try
        {
            // Використовуємо PostAsJsonAsync для відправки JSON-даних на сервер
            var response = await _httpClient.PostAsJsonAsync(url, payload);
            // Зчитуємо відповідь як рядок
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode) // Якщо запит успішний
            {
                // Десеріалізація відповіді у тип T
                var data = JsonSerializer.Deserialize<T>(content);
                return new ApiResponse<T>
                {
                    Message = "Success", // Повідомлення про успіх
                    StatusCode = response.StatusCode, // Статус код відповіді
                    Data = new List<T> { data } // Повертаємо дані у вигляді списку
                };
            }

            // Якщо статус код відповіді не є успішним, повертаємо помилку
            return new ApiResponse<T>
            {
                Message = $"Error: {response.ReasonPhrase}", // Причина помилки
                StatusCode = response.StatusCode // Статус код відповіді
            };
        }
        catch (Exception ex)
        {
            // Якщо виникла помилка під час запиту або обробки, повертаємо виняток
            return new ApiResponse<T>
            {
                Message = $"Exception: {ex.Message}", // Повідомлення про виняток
                StatusCode = HttpStatusCode.InternalServerError // Код помилки сервера
            };
        }
    }
}
