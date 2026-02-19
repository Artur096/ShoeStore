using System;

namespace ShoeStore
{
    public class Shoe
    {
        public int Id { get; set; }                    // Артикул
        public string Model { get; set; }              // Модель
        public string Brand { get; set; }              // Бренд
        public string SKU { get; set; }                // SKU
        public int YearProduced { get; set; }          // Год выпуска
        public string Manufacturer { get; set; }       // Производитель
        public string Material { get; set; }           // Материал верха
        public int Size { get; set; }                  // Размер
        public string Color { get; set; }              // Цвет
        public decimal Price { get; set; }             // Цена
        public int TotalQuantity { get; set; }         // Всего пар на складе
        public int AvailableQuantity { get; set; }     // Доступно пар

        // Добавленные свойства для классификации обуви
        public string Season { get; set; }             // Сезон: летняя, зимняя, демисезонная
        public string Purpose { get; set; }            // Назначение: повседневная, спортивная, вечерняя

        public Shoe(int id, string model, string brand, string sku, int year, string manufacturer,
                   string material, int size, string color, decimal price, int quantity,
                   string season, string purpose)
        {
            Id = id;
            Model = model;
            Brand = brand;
            SKU = sku;
            YearProduced = year;
            Manufacturer = manufacturer;
            Material = material;
            Color = color;

            // Проверка корректности размера (15-50)
            if (size < 15 || size > 50)
            {
                Console.WriteLine($"Предупреждение: размер {size} вне диапазона 15-50 для модели {model}. Установлен размер 40.");
                Size = 40;
            }
            else
            {
                Size = size;
            }

            // Проверка цены (не должна быть отрицательной)
            if (price < 0)
            {
                Console.WriteLine($"Предупреждение: отрицательная цена {price} для модели {model}. Установлена цена 0.");
                Price = 0;
            }
            else
            {
                Price = price;
            }

            // Проверка количества (не должно быть отрицательным)
            if (quantity < 0)
            {
                Console.WriteLine($"Предупреждение: отрицательное количество {quantity} для модели {model}. Установлено количество 0.");
                TotalQuantity = 0;
            }
            else
            {
                TotalQuantity = quantity;
            }

            AvailableQuantity = quantity; // Изначально все пары доступны

            // Сохранение сезонности и назначения
            Season = season;
            Purpose = purpose;
        }

        // Информативное строковое представление обуви
        public override string ToString()
        {
            return $"{Brand}: {Model} (размер {Size}, {Color}) - Цена: {Price} руб. [Доступно: {AvailableQuantity}/{TotalQuantity}]";
        }

        // Продать пару обуви (уменьшить количество доступных)
        public bool Sell()
        {
            if (AvailableQuantity > 0)
            {
                AvailableQuantity--;
                Console.WriteLine($"Продана 1 пара {Model}. Осталось: {AvailableQuantity}");
                return true;
            }
            Console.WriteLine($"Не удалось продать {Model}. Нет в наличии.");
            return false;
        }

        // Вернуть пару обуви (увеличить количество доступных)
        public void Return()
        {
            if (AvailableQuantity < TotalQuantity)
            {
                AvailableQuantity++;
                Console.WriteLine($"Возвращена 1 пара {Model}. Теперь доступно: {AvailableQuantity}");
            }
            else
            {
                Console.WriteLine($"Нельзя вернуть {Model}. Максимальное количество уже на складе.");
            }
        }

        // Проверить наличие
        public bool IsAvailable()
        {
            return AvailableQuantity > 0;
        }

        // Получить информацию о модели
        public string GetFullInfo()
        {
            return $"{Brand} {Model}\n" +
                   $"Артикул: {Id}\n" +
                   $"Размер: {Size}, Цвет: {Color}\n" +
                   $"Материал: {Material}\n" +
                   $"Сезон: {Season}, Назначение: {Purpose}\n" +
                   $"Цена: {Price} руб.\n" +
                   $"Доступно: {AvailableQuantity} из {TotalQuantity}";
        }
    }
}