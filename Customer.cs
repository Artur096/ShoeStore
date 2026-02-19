using System;
using System.Collections.Generic;
using System.Linq;

namespace ShoeStore
{
    public class Customer
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime RegistrationDate { get; set; }

        // Добавленные свойства
        public string CustomerCategory { get; set; } // Категория: обычный, VIP, постоянный
        public List<int> PreferredSizes { get; set; } // Предпочитаемые размеры
        public List<string> PreferredStyles { get; set; } // Предпочитаемые стили

        private List<PurchaseRecord> purchaseHistory = new List<PurchaseRecord>();
        private List<PurchaseRecord> currentPurchases = new List<PurchaseRecord>(); // Для учета возвратов
        private decimal totalSpent = 0; // Общая сумма покупок
        private decimal discountPercent = 0; // Текущая скидка в процентах

        public class PurchaseRecord
        {
            public Shoe Shoe { get; set; }
            public DateTime PurchaseDate { get; set; }
            public decimal PricePaid { get; set; }
            public bool IsReturned { get; set; }       // Возвращена ли покупка
            public DateTime? ReturnDate { get; set; }  // Дата возврата
        }

        public Customer()
        {
            PreferredSizes = new List<int>();
            PreferredStyles = new List<string>();
        }

        // Купить обувь
        public bool BuyShoe(Shoe shoe)
        {
            // Проверка наличия обуви
            if (!shoe.IsAvailable())
            {
                return false;
            }

            // Создание записи о покупке
            PurchaseRecord record = new PurchaseRecord
            {
                Shoe = shoe,
                PurchaseDate = DateTime.Now,
                PricePaid = shoe.Price * (1 - discountPercent / 100), // Цена со скидкой
                IsReturned = false,
                ReturnDate = null
            };

            // Добавление в историю покупок
            purchaseHistory.Add(record);
            currentPurchases.Add(record);

            // Продажа обуви
            if (!shoe.Sell())
            {
                return false;
            }

            // Обновление общей суммы
            totalSpent += record.PricePaid;

            // Пересчет скидки
            UpdateDiscount();

            return true;
        }

        // Вернуть обувь
        public bool ReturnShoe(Shoe shoe)
        {
            // Поиск записи о покупке
            PurchaseRecord recordToReturn = null;

            foreach (var record in currentPurchases)
            {
                if (record.Shoe.Id == shoe.Id && !record.IsReturned)
                {
                    recordToReturn = record;
                    break;
                }
            }

            if (recordToReturn == null)
            {
                Console.WriteLine("Покупка не найдена или уже возвращена.");
                return false;
            }

            // Проверка срока возврата (14 дней)
            TimeSpan timeSincePurchase = DateTime.Now - recordToReturn.PurchaseDate;
            if (timeSincePurchase.Days > 14)
            {
                Console.WriteLine($"Срок возврата истек. Прошло {timeSincePurchase.Days} дней.");
                return false;
            }

            // Возврат обуви
            recordToReturn.IsReturned = true;
            recordToReturn.ReturnDate = DateTime.Now;
            currentPurchases.Remove(recordToReturn);
            shoe.Return();

            // Обновление суммы
            totalSpent -= recordToReturn.PricePaid;
            UpdateDiscount();

            return true;
        }

        // Обновить уровень скидки
        public void UpdateDiscount()
        {
            if (totalSpent >= 30000)
            {
                discountPercent = 15; // Более 30000 руб.: 15%
            }
            else if (totalSpent >= 15000)
            {
                discountPercent = 10; // 15000-30000 руб.: 10%
            }
            else if (totalSpent >= 5000)
            {
                discountPercent = 5;  // 5000-15000 руб.: 5%
            }
            else
            {
                discountPercent = 0;  // До 5000 руб.: 0%
            }
        }

        // Получить статистику покупок
        public (int totalPurchases, decimal totalSpent, int returnsCount) GetPurchaseStats()
        {
            int returnsCount = 0;

            foreach (var record in purchaseHistory)
            {
                if (record.IsReturned)
                {
                    returnsCount++;
                }
            }

            return (purchaseHistory.Count, this.totalSpent, returnsCount);
        }

        // Добавить предпочитаемый размер
        public void AddPreferredSize(int size)
        {
            if (!PreferredSizes.Contains(size))
            {
                PreferredSizes.Add(size);
            }
        }

        // Добавить предпочитаемый стиль
        public void AddPreferredStyle(string style)
        {
            if (!PreferredStyles.Contains(style))
            {
                PreferredStyles.Add(style);
            }
        }

        // Показать информацию о покупателе
        public void ShowCustomerInfo()
        {
            Console.WriteLine($"=== ИНФОРМАЦИЯ О ПОКУПАТЕЛЕ ===");
            Console.WriteLine($"ФИО: {FullName}");
            Console.WriteLine($"ID: {Id}");
            Console.WriteLine($"Дата рождения: {BirthDate:dd.MM.yyyy}");
            Console.WriteLine($"Телефон: {Phone}");
            Console.WriteLine($"Email: {Email}");
            Console.WriteLine($"Категория: {CustomerCategory}");
            Console.WriteLine($"Зарегистрирован: {RegistrationDate:dd.MM.yyyy}");

            var stats = GetPurchaseStats();
            Console.WriteLine($"Всего покупок: {stats.totalPurchases}");
            Console.WriteLine($"Всего возвратов: {stats.returnsCount}");
            Console.WriteLine($"Общая сумма покупок: {stats.totalSpent} руб.");
            Console.WriteLine($"Текущая скидка: {discountPercent}%");

            Console.Write("Предпочитаемые размеры: ");
            if (PreferredSizes.Count > 0)
            {
                Console.WriteLine(string.Join(", ", PreferredSizes));
            }
            else
            {
                Console.WriteLine("не указаны");
            }

            Console.Write("Предпочитаемые стили: ");
            if (PreferredStyles.Count > 0)
            {
                Console.WriteLine(string.Join(", ", PreferredStyles));
            }
            else
            {
                Console.WriteLine("не указаны");
            }
            Console.WriteLine("================================");
        }

        // Дополнительные методы для удобства
        public List<PurchaseRecord> GetCurrentPurchases()
        {
            return currentPurchases;
        }

        public List<PurchaseRecord> GetPurchaseHistory()
        {
            return purchaseHistory;
        }

        public decimal GetTotalSpent()
        {
            return totalSpent;
        }

        public decimal GetDiscountPercent()
        {
            return discountPercent;
        }
    }
}