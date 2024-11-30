using System.Collections.Generic;
using System.Net;

public class ApiResponse<T>
{
    // Повідомлення, яке містить додаткову інформацію про результат операції
    public string Message { get; set; }

    // Статусний код HTTP-відповіді (наприклад, 200 OK, 404 Not Found, 500 Internal Server Error)
    public HttpStatusCode StatusCode { get; set; }

    // Колекція даних типу T, що містить результати запиту
    // Це може бути одиничний об'єкт або список об'єктів, в залежності від запиту
    public List<T> Data { get; set; } = new List<T>(); // Ініціалізація порожнього списку за замовчуванням
}
