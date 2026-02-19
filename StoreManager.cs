using System;
using System.Collections.Generic;
using System.Linq;

namespace ShoeStore
{
    public class StoreManager
    {
        private List<Customer> customers = new List<Customer>();
        private Catalog catalog;

        private int nextCustomerId = 1000;
        private int maxItemsPerCustomer = 10; // Максимум товаров в одной покупке

        public StoreManager()
        {
            catalog = new Catalog();
        }

        // Регистрация нового покупателя
        public Customer RegisterCustomer(string fullName, DateTime birthDate, string phone, string email)
        {
            // Проверка уникальности телефона и email
            foreach (var customer in customers)
            {
                if (customer.Phone == phone)
                {
                    Console.WriteLine($"Покупатель с телефоном {phone} уже зарегистрирован.");
                    return null;
                }

                if (customer.Email == email)
                {
                    Console.WriteLine($"Покупатель с email {email} уже зарегистрирован.");
                    return null;
                }
            }

            // Создание нового покупателя
            Customer newCustomer = new Customer();
            newCustomer.Id = nextCustomerId;
            newCustomer.FullName = fullName;
            newCustomer.BirthDate = birthDate;
            newCustomer.Phone = phone;
            newCustomer.Email = email;
            newCustomer.RegistrationDate = DateTime.Now;

            // Определение категории покупателя по возрасту
            int age = DateTime.Now.Year - birthDate.Year;
            if (DateTime.Now.DayOfYear < birthDate.DayOfYear)
                age--;

            if (age < 18)
            {
                newCustomer.CustomerCategory = "Подросток";
            }
            else if (age >= 60)
            {
                newCustomer.CustomerCategory = "Пенсионер";
            }
            else
            {
                newCustomer.CustomerCategory = "Обычный";
            }

            // Добавление покупателя в список
            customers.Add(newCustomer);
            nextCustomerId++;

            Console.WriteLine($"Покупатель {fullName} успешно зарегистрирован. ID: {newCustomer.Id}");
            return newCustomer;
        }

        // Найти покупателя по телефону
        public Customer FindCustomerByPhone(string phone)
        {
            foreach (var customer in customers)
            {
                if (customer.Phone == phone)
                {
                    return customer;
                }
            }
            return null;
        }

        // Найти покупателя по email
        public Customer FindCustomerByEmail(string email)
        {
            foreach (var customer in customers)
            {
                if (customer.Email == email)
                {
                    return customer;
                }
            }
            return null;
        }

        // Продать обувь покупателю
        public bool SellShoeToCustomer(Customer customer, Shoe shoe)
        {
            // Проверка доступности обуви
            if (!shoe.IsAvailable())
            {
                Console.WriteLine($"Обувь {shoe.Model} недоступна для продажи.");
                return false;
            }

            // Проверка лимита покупок
            if (customer.GetCurrentPurchases().Count >= maxItemsPerCustomer)
            {
                Console.WriteLine($"Покупатель достиг лимита в {maxItemsPerCustomer} покупок.");
                return false;
            }

            // Продажа обуви
            bool success = customer.BuyShoe(shoe);

            if (success)
            {
                // Фиксация продажи в каталоге
                catalog.RecordSale(shoe);
                Console.WriteLine($"Продажа успешно оформлена для покупателя {customer.FullName}.");
            }

            return success;
        }

        // Принять возврат обуви
        public bool AcceptReturn(Customer customer, Shoe shoe)
        {
            // Принятие возврата
            bool success = customer.ReturnShoe(shoe);

            if (success)
            {
                Console.WriteLine($"Возврат успешно оформлен для покупателя {customer.FullName}.");
            }

            return success;
        }

        // Получить список самых активных покупателей
        public List<Customer> GetMostActiveCustomers(int count = 5)
        {
            List<Customer> activeCustomers = new List<Customer>();

            // Сортировка покупателей по общей сумме покупок
            var sortedCustomers = customers.OrderByDescending(c => c.GetTotalSpent()).ToList();

            // Возврат указанного количества
            for (int i = 0; i < Math.Min(count, sortedCustomers.Count); i++)
            {
                activeCustomers.Add(sortedCustomers[i]);
            }

            return activeCustomers;
        }

        // Готовые методы:
        public void AddShoeToCatalog(Shoe shoe)
        {
            catalog.AddShoe(shoe);
        }

        public Catalog GetCatalog()
        {
            return catalog;
        }

        public List<Customer> GetAllCustomers()
        {
            return customers;
        }

        public int GetCustomerCount()
        {
            return customers.Count;
        }

        public int GetMaxItemsPerCustomer()
        {
            return maxItemsPerCustomer;
        }

        // Дополнительный метод для поиска покупателя по ID
        public Customer FindCustomerById(int id)
        {
            foreach (var customer in customers)
            {
                if (customer.Id == id)
                {
                    return customer;
                }
            }
            return null;
        }
    }
}