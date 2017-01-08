using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ZipCodeRadius.Models
{
    public class InitializeDatabase
    {
        IHostingEnvironment _env;
        ApiContext _context;

        public InitializeDatabase(IHostingEnvironment env, ApiContext context)
        {
            _env = env;
            _context = context;
        }

        public void Seed()
        {
            ICollection<ZipCodeInfo> list = new List<ZipCodeInfo>();

            var pathToFile = Path.Combine(_env.WebRootPath, "data", "US.txt");

            string fileContent;

            using (StreamReader reader = File.OpenText(pathToFile))
            {
                fileContent = reader.ReadToEnd();
            }

            if (!string.IsNullOrWhiteSpace(fileContent))
            {
                string[] lines = fileContent.Split(new string[] { "\n" }, StringSplitOptions.None);

                int lineCount = lines.Count();

                for (int i = 0; i < lineCount; i++)
                {
                    string[] fields = lines[i].Split(new string[] { "\t" }, StringSplitOptions.None);

                    var fieldCount = fields.Count();

                    if (fieldCount == 12)
                    {
                        var item = new ZipCodeInfo()
                        {
                            Country = fields[0],
                            ZipCode = fields[1],
                            City = fields[2],
                            StateName = fields[3],
                            StateAbbreviation = fields[4],
                            CountyName = fields[5],
                            CountyFipsCode = fields[6],
                            Lattitude = double.Parse(fields[9]),
                            Longitude = double.Parse(fields[10])
                        };

                        list.Add(item);
                    }
                }
            }

            _context.ZipCodes.AddRange(list);

            _context.SaveChanges();
        }
    }
}
