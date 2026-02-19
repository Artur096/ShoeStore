using System;
using System.Collections.Generic;
using System.Linq;

namespace ShoeStore
{
    public class StoreMenu
    {
        private StoreManager manager;

        public StoreMenu()
        {
            manager = new StoreManager();
            InitializeStoreData();
        }

        private void InitializeStoreData()
        {
            // Инициализация тестовых данных - обувь
            manager.AddShoeToCatalog(new Shoe(1, "Air Max 270", "Nike",
                "NIKE-AM270-BLK", 2023, "Nike Inc.",
                "Сетка/кожа", 42, "Черный", 12999, 15,
                "демисезонная", "спортивная"));

            manager.AddShoeToCatalog(new Shoe(2, "Classic Leather", "Reebok",
                "RBL-CL-WHT", 2022, "Reebok International",
                "Кожа", 39, "Белый", 8999, 8,
                "летняя", "повседневная"));

            manager.AddShoeToCatalog(new Shoe(3, "Timberland Premium", "Timberland",
                "TIMB-PRM-BRN", 2023, "Timberland LLC",
                "Натуральная кожа", 44, "Коричневый", 18999, 5,
                "зимняя", "повседневная"));

            manager.AddShoeToCatalog(new Shoe(4, "Gazelle", "Adidas",
                "ADID-GAZ-RED", 2023, "Adidas AG",
                "Замша", 38, "Красный", 10999, 12,
                "демисезонная", "повседневная"));

            manager.AddShoeToCatalog(new Shoe(5, "Chelsea Boots", "Clarks",
                "CLK-CHEL-BLK", 2022, "Clarks Shoes",
                "Кожа", 43, "Черный", 14999, 6,
                "демисезонная", "классическая"));

            // Инициализация тестовых покупателей
            manager.RegisterCustomer("Иванов Иван Иванович",
                new DateTime(1985, 5, 15), "79161234567", "ivanov@mail.ru");
            manager.RegisterCustomer("Петрова Мария Сергеевна",
                new DateTime(1992, 8, 22), "79167654321", "petrova@gmail.com");
            manager.RegisterCustomer("Сидоров Алексей Петрович",
                new DateTime(1978, 3, 10), "79031234567", "sidorov@yandex.ru");
        }

        // Поиск обуви
        public void SearchShoes()
        {
            Console.Clear();
            Console.WriteLine("=== ПОИСК ОБУВИ ===");
            Console.WriteLine("1. По бренду");
            Console.WriteLine("2. По модели");
            Console.WriteLine("3. По размеру");
            Console.WriteLine("4. По цене (диапазон)");
            Console.WriteLine("5. Показать весь каталог");
            Console.Write("Выберите критерий поиска: ");

            string choice = Console.ReadLine();
            List<Shoe> results = new List<Shoe>();

            switch (choice)
            {
                case "1":
                    Console.Write("Введите бренд: ");
                    string brand = Console.ReadLine();
                    results = manager.GetCatalog().FindByBrand(brand);
                    break;
                case "2":
                    Console.Write("Введите модель: ");
                    string model = Console.ReadLine();
                    results = manager.GetCatalog().FindByModel(model);
                    break;
                case "3":
                    Console.Write("Введите размер: ");
                    if (int.TryParse(Console.ReadLine(), out int size))
                        results = manager.GetCatalog().FindBySize(size);
                    break;
                case "4":
                    Console.Write("Введите минимальную цену: ");
                    decimal min = decimal.Parse(Console.ReadLine());
                    Console.Write("Введите максимальную цену: ");
                    decimal max = decimal.Parse(Console.ReadLine());
                    results = manager.GetCatalog().FindByPriceRange(min, max);
                    break;
                case "5":
                    results = manager.GetCatalog().GetAllShoes();
                    break;
                default:
                    Console.WriteLine("Неверный выбор!");
                    return;
            }

            Console.WriteLine("\n=== РЕЗУЛЬТАТЫ ПОИСКА ===");
            if (results.Count > 0)
            {
                Console.WriteLine($"Найдено моделей: {results.Count}");
                foreach (var shoe in results)
                {
                    Console.WriteLine($"  ID: {shoe.Id} - {shoe}");
                }
            }
            else
            {
                Console.WriteLine("Обувь не найдена");
            }
        }

        // Продать обувь
        public void SellShoeToCustomer()
        {
            Console.Clear();
            Console.WriteLine("=== ПРОДАЖА ОБУВИ ===");

            // Поиск покупателя
            Console.Write("Введите телефон покупателя: ");
            string phone = Console.ReadLine();
            Customer customer = manager.FindCustomerByPhone(phone);

            if (customer == null)
            {
                Console.WriteLine("Покупатель не найден.");
                Console.Write("Зарегистрировать нового? (да/нет): ");
                if (Console.ReadLine().ToLower() == "да")
                {
                    RegisterNewCustomer();
                    customer = manager.FindCustomerByPhone(phone);
                    if (customer == null) return;
                }
                else
                {
                    return;
                }
            }

            // Показать информацию о покупателе
            customer.ShowCustomerInfo();

            // Поиск обуви
            Console.Write("\nВведите ID модели обуви: ");
            if (int.TryParse(Console.ReadLine(), out int shoeId))
            {
                Shoe shoe = manager.GetCatalog().FindShoeById(shoeId);

                if (shoe == null)
                {
                    Console.WriteLine("Модель не найдена.");
                    return;
                }

                // Показать информацию об обуви
                Console.WriteLine($"\nВыбранная модель: {shoe}");

                // Подтверждение
                Console.Write("Подтвердить покупку? (да/нет): ");
                if (Console.ReadLine().ToLower() == "да")
                {
                    bool success = manager.SellShoeToCustomer(customer, shoe);
                    if (success)
                    {
                        Console.WriteLine("Покупка успешно оформлена!");
                    }
                }
            }
        }

        // Принять возврат
        public void AcceptReturnFromCustomer()
        {
            Console.Clear();
            Console.WriteLine("=== ПРИЕМ ВОЗВРАТА ===");

            // Поиск покупателя
            Console.Write("Введите телефон покупателя: ");
            string phone = Console.ReadLine();
            Customer customer = manager.FindCustomerByPhone(phone);

            if (customer == null)
            {
                Console.WriteLine("Покупатель не найден.");
                return;
            }

            // Показать текущие покупки
            var purchases = customer.GetCurrentPurchases();
            if (purchases.Count == 0)
            {
                Console.WriteLine("У покупателя нет активных покупок.");
                return;
            }

            Console.WriteLine("\nТекущие покупки:");
            for (int i = 0; i < purchases.Count; i++)
            {
                var purchase = purchases[i];
                Console.WriteLine($"{i + 1}. {purchase.Shoe.Brand} {purchase.Shoe.Model} - " +
                                 $"Куплено: {purchase.PurchaseDate:dd.MM.yyyy}");
            }

            // Выбор покупки для возврата
            Console.Write("Выберите номер покупки для возврата: ");
            if (int.TryParse(Console.ReadLine(), out int choice) &&
                choice >= 1 && choice <= purchases.Count)
            {
                var purchase = purchases[choice - 1];

                // Проверка срока возврата
                TimeSpan timeSincePurchase = DateTime.Now - purchase.PurchaseDate;
                if (timeSincePurchase.Days > 14)
                {
                    Console.WriteLine($"Срок возврата истек (прошло {timeSincePurchase.Days} дней).");
                    return;
                }

                // Подтверждение
                Console.Write($"Вернуть {purchase.Shoe.Model}? (да/нет): ");
                if (Console.ReadLine().ToLower() == "да")
                {
                    bool success = manager.AcceptReturn(customer, purchase.Shoe);
                    if (success)
                    {
                        Console.WriteLine("Возврат успешно оформлен!");
                    }
                }
            }
        }

        // Консультация по подбору обуви
        public void ProvideShoeRecommendations()
        {
            Console.Clear();
            Console.WriteLine("=== КОНСУЛЬТАЦИЯ ПО ПОДБОРУ ОБУВИ ===");

            // Поиск покупателя
            Console.Write("Введите телефон покупателя: ");
            string phone = Console.ReadLine();
            Customer customer = manager.FindCustomerByPhone(phone);

            if (customer == null)
            {
                Console.WriteLine("Покупатель не найден.");
                return;
            }

            // Если нет предпочтений, запросить их
            if (customer.PreferredSizes.Count == 0)
            {
                Console.Write("Введите предпочитаемый размер обуви: ");
                if (int.TryParse(Console.ReadLine(), out int size))
                {
                    customer.AddPreferredSize(size);
                }
            }

            if (customer.PreferredStyles.Count == 0)
            {
                Console.Write("Введите предпочитаемый стиль (спортивная/повседневная/классическая): ");
                string style = Console.ReadLine();
                customer.AddPreferredStyle(style);
            }

            // Получение рекомендаций
            var recommendations = manager.GetCatalog().RecommendForCustomer(customer);

            Console.WriteLine("\n=== РЕКОМЕНДАЦИИ ===");
            if (recommendations.Count > 0)
            {
                Console.WriteLine($"Для {customer.FullName}:");
                for (int i = 0; i < recommendations.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {recommendations[i]}");
                }
            }
            else
            {
                Console.WriteLine("К сожалению, нет подходящих рекомендаций.");
            }
        }

        // Показать статистику магазина
        public void ShowStoreStats()
        {
            Console.Clear();
            Console.WriteLine("=== СТАТИСТИКА МАГАЗИНА ===");

            // Количество покупателей
            Console.WriteLine($"Всего покупателей: {manager.GetCustomerCount()}");

            // Количество моделей
            Console.WriteLine($"Всего моделей в каталоге: {manager.GetCatalog().GetTotalModelsCount()}");

            // Популярные модели
            var popularShoes = manager.GetCatalog().GetMostPopularShoes(3);
            Console.WriteLine("\nСамые популярные модели:");
            if (popularShoes.Count > 0)
            {
                foreach (var shoe in popularShoes)
                {
                    Console.WriteLine($"  {shoe.Brand} {shoe.Model}");
                }
            }

            // Статистика по брендам
            var brandStats = manager.GetCatalog().GetBrandStatistics();
            Console.WriteLine("\nСтатистика по брендам:");
            foreach (var stat in brandStats)
            {
                Console.WriteLine($"  {stat.Key}: {stat.Value} моделей");
            }

            // Активные покупатели
            var activeCustomers = manager.GetMostActiveCustomers(3);
            Console.WriteLine("\nСамые активные покупатели:");
            if (activeCustomers.Count > 0)
            {
                foreach (var customer in activeCustomers)
                {
                    var stats = customer.GetPurchaseStats();
                    Console.WriteLine($"  {customer.FullName}: {stats.totalSpent} руб., {stats.totalPurchases} покупок");
                }
            }
        }

        // Зарегистрировать нового покупателя
        public void RegisterNewCustomer()
        {
            Console.Clear();
            Console.WriteLine("=== РЕГИСТРАЦИЯ НОВОГО ПОКУПАТЕЛЯ ===");

            try
            {
                Console.Write("Введите ФИО: ");
                string fullName = Console.ReadLine();

                Console.Write("Введите дату рождения (дд.мм.гггг): ");
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime birthDate))
                {
                    Console.WriteLine("Неверный формат даты.");
                    return;
                }

                Console.Write("Введите телефон: ");
                string phone = Console.ReadLine();

                Console.Write("Введите email: ");
                string email = Console.ReadLine();

                // Регистрация
                Customer newCustomer = manager.RegisterCustomer(fullName, birthDate, phone, email);

                if (newCustomer != null)
                {
                    // Предпочтения
                    Console.Write("Добавить предпочитаемый размер? (да/нет): ");
                    if (Console.ReadLine().ToLower() == "да")
                    {
                        Console.Write("Введите размер: ");
                        if (int.TryParse(Console.ReadLine(), out int size))
                        {
                            newCustomer.AddPreferredSize(size);
                        }
                    }

                    Console.Write("Добавить предпочитаемый стиль? (да/нет): ");
                    if (Console.ReadLine().ToLower() == "да")
                    {
                        Console.Write("Введите стиль: ");
                        string style = Console.ReadLine();
                        newCustomer.AddPreferredStyle(style);
                    }

                    Console.WriteLine("\n=== КАРТА КЛИЕНТА ===");
                    newCustomer.ShowCustomerInfo();
                    Console.WriteLine("Регистрация завершена успешно!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        // Главное меню
        public void ShowMainMenu()
        {
            bool running = true;

            while (running)
            {
                Console.Clear();
                Console.WriteLine("=== ОБУВНОЙ МАГАЗИН 'СТЕППЕР' ===");
                Console.WriteLine("1. Поиск обуви");
                Console.WriteLine("2. Продать обувь");
                Console.WriteLine("3. Принять возврат");
                Console.WriteLine("4. Консультация по подбору");
                Console.WriteLine("5. Статистика магазина");
                Console.WriteLine("6. Регистрация нового покупателя");
                Console.WriteLine("7. Поиск покупателя");
                Console.WriteLine("8. Выход");
                Console.Write("Выберите: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        SearchShoes();
                        break;
                    case "2":
                        SellShoeToCustomer();
                        break;
                    case "3":
                        AcceptReturnFromCustomer();
                        break;
                    case "4":
                        ProvideShoeRecommendations();
                        break;
                    case "5":
                        ShowStoreStats();
                        break;
                    case "6":
                        RegisterNewCustomer();
                        break;
                    case "7":
                        SearchCustomer();
                        break;
                    case "8":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Неверный выбор!");
                        break;
                }

                if (running)
                {
                    Console.WriteLine("\nНажмите Enter для продолжения...");
                    Console.ReadLine();
                }
            }
        }

        // Метод поиска покупателя
        private void SearchCustomer()
        {
            Console.Clear();
            Console.WriteLine("=== ПОИСК ПОКУПАТЕЛЯ ===");
            Console.WriteLine("1. По телефону");
            Console.WriteLine("2. По email");
            Console.Write("Выберите способ: ");

            string choice = Console.ReadLine();

            if (choice == "1")
            {
                Console.Write("Введите телефон: ");
                string phone = Console.ReadLine();
                Customer customer = manager.FindCustomerByPhone(phone);
                if (customer != null)
                {
                    customer.ShowCustomerInfo();
                }
                else
                {
                    Console.WriteLine("Покупатель не найден");
                }
            }
            else if (choice == "2")
            {
                Console.Write("Введите email: ");
                string email = Console.ReadLine();
                Customer customer = manager.FindCustomerByEmail(email);
                if (customer != null)
                {
                    customer.ShowCustomerInfo();
                }
                else
                {
                    Console.WriteLine("Покупатель не найден");
                }
            }
        }
    }
}