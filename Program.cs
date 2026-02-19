using System;

namespace ShoeStore
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== ОБУВНОЙ МАГАЗИН 'СТЕППЕР' ===\n");

            StoreMenu menu = new StoreMenu();
            menu.ShowMainMenu();

            Console.WriteLine("\nДо новых встреч в мире стильной обуви!");
            Console.ReadKey();
        }
    }
}