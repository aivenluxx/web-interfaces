using System;
using System.Reflection;

public class Person
{
    public string Name { get; set; }
    private int Age { get; set; }
    protected string Nationality { get; set; }
    internal double Height { get; set; }

    private bool isMarried;

    public Person(string name, int age, string nationality, double height, bool married)
    {
        Name = name;
        Age = age;
        Nationality = nationality;
        Height = height;
        isMarried = married;
    }

    public void Greet()
    {
        Console.WriteLine($"Hello, my name is {Name}");
    }

    private int GetAge()
    {
        return Age;
    }

    internal bool IsMarried()
    {
        return isMarried;
    }
}

class Program
{
    static void Main()
    {
        Person person = new Person("John", 30, "American", 1.85, true);

        // Робота з Type і TypeInfo
        Type type = person.GetType();
        Console.WriteLine($"Type Name: {type.Name}");
        Console.WriteLine($"Namespace: {type.Namespace}");
        Console.WriteLine($"Is Class: {type.IsClass}");

        TypeInfo typeInfo = type.GetTypeInfo();
        Console.WriteLine("\nTypeInfo:");
        Console.WriteLine($"Base Type: {typeInfo.BaseType}");
        Console.WriteLine($"Is Abstract: {typeInfo.IsAbstract}");
        Console.WriteLine($"Is Public: {typeInfo.IsPublic}");

        // Робота з MemberInfo
        Console.WriteLine("\nMembers of Person class:");
        MemberInfo[] members = type.GetMembers();
        foreach (var member in members)
        {
            Console.WriteLine($"{member.MemberType}: {member.Name}");
        }

        // Робота з FieldInfo
        Console.WriteLine("\nFields of Person class:");
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var field in fields)
        {
            Console.WriteLine($"Field: {field.Name}, Type: {field.FieldType}");
        }

        FieldInfo isMarriedField = type.GetField("isMarried", BindingFlags.NonPublic | BindingFlags.Instance);
        Console.WriteLine($"\nOriginal value of isMarried: {isMarriedField.GetValue(person)}");
        isMarriedField.SetValue(person, false);
        Console.WriteLine($"Modified value of isMarried: {isMarriedField.GetValue(person)}");

        // Робота з MethodInfo
        Console.WriteLine("\nInvoking Greet method:");
        MethodInfo greetMethod = type.GetMethod("Greet");
        greetMethod.Invoke(person, null);

        MethodInfo getAgeMethod = type.GetMethod("GetAge", BindingFlags.NonPublic | BindingFlags.Instance);
        int age = (int)getAgeMethod.Invoke(person, null);
        Console.WriteLine($"\nAge: {age}");
    }
}
