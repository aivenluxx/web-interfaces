// Клас, що представляє пост (наприклад, в блозі або соціальній мережі)
public class Post
{
    // Ідентифікатор поста (унікальний для кожного поста)
    public int Id { get; set; }

    // Ідентифікатор користувача, який створив цей пост
    public int UserId { get; set; }

    // Заголовок поста
    public string Title { get; set; }

    // Текстова частина поста (основний контент)
    public string Body { get; set; }

    // Перевизначення методу ToString для надання зручного текстового представлення об'єкта
    public override string ToString()
    {
        // Повертає відформатований рядок, що містить ID, Title і Body поста
        return $"ID: {Id}, Title: {Title}, Body: {Body}";
    }
}
