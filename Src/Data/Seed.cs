

using System.Text.Json;
using access_service.Src.Models;

namespace access_service.Src.Data{

    public class Seed
    {
        /// <summary>
        /// Seed the database with examples models in the json files if the database is empty.
        /// </summary>
        public void SeedData(DataContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            SeedCareers(context, options);
        }

        /// <summary>
        /// Seed the database with the careers in the json file and save changes if the database is empty.
        /// </summary>
        /// <param name="options">Options to deserialize json</param>
        private void SeedCareers(DataContext context, JsonSerializerOptions options)
        {
            var result = context.Careers?.Any();
            if (result is true or null) return;
            var path = "Src/Data/DataSeeder/CareersData.json";
            var careersData = File.ReadAllText(path);
            var careersList = JsonSerializer.Deserialize<List<Career>>(careersData, options) ?? 
                throw new Exception("CareersData.json is Empty");
            careersList.ForEach(s => 
            {
                s.Name = s.Name.ToLower();
            });

            context.Careers?.AddRange(careersList);
            context.SaveChanges();
        }

       
    }
}

