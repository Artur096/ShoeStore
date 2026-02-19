using System;
using System.Collections.Generic;
using System.Linq;

namespace ShoeStore
{
    public class Catalog
    {
        private List<Shoe> shoes = new List<Shoe>();
        private Dictionary<string, int> brandStatistics = new Dictionary<string, int>(); // Статистика по брендам
        private Dictionary<int, int> salesCount = new Dictionary<int, int>(); // Количество продаж каждой модели (ключ: Id обуви)

        // Добавить обувь в каталог
        public void AddShoe(Shoe shoe)
        {
            shoes.Add(shoe);

            // Обновление статистики по брендам
            if (brandStatistics.ContainsKey(shoe.Brand))
            {
                brandStatistics[shoe.Brand]++;
            }
            else
            {
                brandStatistics[shoe.Brand] = 1;
            }

            // Инициализация счетчика продаж
            salesCount[shoe.Id] = 0;
        }

        // Найти обувь по бренду
        public List<Shoe> FindByBrand(string brand)
        {
            List<Shoe> result = new List<Shoe>();

            foreach (var shoe in shoes)
            {
                if (shoe.Brand.ToLower().Contains(brand.ToLower()))
                {
                    result.Add(shoe);
                }
            }

            return result;
        }

        // Найти обувь по модели
        public List<Shoe> FindByModel(string model)
        {
            List<Shoe> result = new List<Shoe>();

            foreach (var shoe in shoes)
            {
                if (shoe.Model.ToLower().Contains(model.ToLower()))
                {
                    result.Add(shoe);
                }
            }

            return result;
        }

        // Найти обувь по размеру
        public List<Shoe> FindBySize(int size)
        {
            List<Shoe> result = new List<Shoe>();

            foreach (var shoe in shoes)
            {
                if (shoe.Size == size && shoe.IsAvailable())
                {
                    result.Add(shoe);
                }
            }

            return result;
        }

        // Найти обувь по цене (в диапазоне)
        public List<Shoe> FindByPriceRange(decimal minPrice, decimal maxPrice)
        {
            List<Shoe> result = new List<Shoe>();

            foreach (var shoe in shoes)
            {
                if (shoe.Price >= minPrice && shoe.Price <= maxPrice && shoe.IsAvailable())
                {
                    result.Add(shoe);
                }
            }

            return result;
        }

        // Рекомендовать обувь для покупателя
        public List<Shoe> RecommendForCustomer(Customer customer)
        {
            List<Shoe> recommendations = new List<Shoe>();

            // Получение предпочтений покупателя
            var preferredSizes = customer.PreferredSizes;
            var preferredStyles = customer.PreferredStyles;

            // Если предпочтений нет, возвращаем все доступные
            if (preferredSizes.Count == 0 && preferredStyles.Count == 0)
            {
                foreach (var shoe in shoes)
                {
                    if (shoe.IsAvailable())
                    {
                        recommendations.Add(shoe);
                    }
                }
            }
            else
            {
                // Фильтрация по предпочтениям
                foreach (var shoe in shoes)
                {
                    if (!shoe.IsAvailable()) continue;

                    bool sizeMatch = preferredSizes.Count == 0 || preferredSizes.Contains(shoe.Size);
                    bool styleMatch = preferredStyles.Count == 0 || preferredStyles.Contains(shoe.Purpose);

                    if (sizeMatch && styleMatch)
                    {
                        recommendations.Add(shoe);
                    }
                }
            }

            // Сортировка по популярности
            recommendations.Sort((s1, s2) =>
            {
                int sales1 = salesCount.ContainsKey(s1.Id) ? salesCount[s1.Id] : 0;
                int sales2 = salesCount.ContainsKey(s2.Id) ? salesCount[s2.Id] : 0;
                return sales2.CompareTo(sales1);
            });

            // Возврат топ-5 рекомендаций
            return recommendations.Take(5).ToList();
        }

        // Зафиксировать продажу обуви
        public void RecordSale(Shoe shoe)
        {
            if (salesCount.ContainsKey(shoe.Id))
            {
                salesCount[shoe.Id]++;
            }
            else
            {
                salesCount[shoe.Id] = 1;
            }
        }

        // Получить самые популярные модели
        public List<Shoe> GetMostPopularShoes(int count = 10)
        {
            List<Shoe> popular = new List<Shoe>();

            // Сортировка по количеству продаж
            var sortedShoes = shoes.OrderByDescending(s =>
                salesCount.ContainsKey(s.Id) ? salesCount[s.Id] : 0).ToList();

            // Возврат указанного количества
            for (int i = 0; i < Math.Min(count, sortedShoes.Count); i++)
            {
                popular.Add(sortedShoes[i]);
            }

            return popular;
        }

        // Получить статистику по брендам
        public Dictionary<string, int> GetBrandStatistics()
        {
            return brandStatistics;
        }

        // Дополнительные методы
        public int GetTotalModelsCount()
        {
            return shoes.Count;
        }

        public List<Shoe> GetAllAvailableShoes()
        {
            return shoes.Where(s => s.IsAvailable()).ToList();
        }

        public Shoe FindShoeById(int id)
        {
            foreach (var shoe in shoes)
            {
                if (shoe.Id == id)
                {
                    return shoe;
                }
            }
            return null;
        }

        // Показать информацию о каталоге
        public void ShowCatalogInfo()
        {
            Console.WriteLine($"Всего моделей в каталоге: {shoes.Count}");
            Console.WriteLine($"Всего брендов: {brandStatistics.Count}");

            Console.WriteLine("\nСтатистика по брендам:");
            foreach (var stat in brandStatistics)
            {
                Console.WriteLine($"  {stat.Key}: {stat.Value} моделей");
            }
        }

        // Получить все модели обуви
        public List<Shoe> GetAllShoes()
        {
            return shoes;
        }
    }
}