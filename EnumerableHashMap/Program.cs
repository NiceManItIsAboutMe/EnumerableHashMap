using EnumerableHashMap.HashMap;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        MyHashMap<String, String> hashMap = new(4);

        String res = hashMap.Put("+79005554433", "Андрей");
        res = hashMap.Put("+79005554432", "Алексей");
        res = hashMap.Put("+79005554432", "Дарья1");
        res = hashMap.Put("+79005554433", "Дарья2");
        res = hashMap.Put("+79005554434", "Дарья3");
        res = hashMap.Put("+79005554435", "Дарья4");
        res = hashMap.Put("+79005554436", "Дарья5");
        res = hashMap.Put("+79005554437", "Дарья6");
        res = hashMap.Put("+79005554438", "Дарья7");
        res = hashMap.Put("+79005554439", "Дарья8");

        res = hashMap.Get("+79005554435");
        hashMap.Remove("+79005554438");

        foreach (MyHashMap<String, String>.Entity element in hashMap)
        {
            Console.WriteLine(element.Key + " - " + element.Value);
        }
    }
}