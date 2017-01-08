using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using ZipCodeRadius.Models;

namespace ZipCodeRadius.Controllers
{
    [Route("api/search")]
    public class SearchController : Controller
    {
        private readonly ApiContext _context;

        public SearchController(ApiContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var zipCodes = await _context.ZipCodes.Take(20).ToListAsync();

            return Ok(zipCodes);
        }

        [HttpGet("{zipcode}")]
        public async Task<IActionResult> GetByZipCode(string zipcode, [FromQuery]double distance)
        {
            var zipCodeInfo = await _context.ZipCodes.FirstOrDefaultAsync(z => z.ZipCode == zipcode);

            var maxLattitude = CalculateLatitudePlusDistance(zipCodeInfo.Lattitude, distance);

            var maxLongitude = CalculateLongitudePlusDistance(zipCodeInfo.Longitude, zipCodeInfo.Lattitude, distance);

            var minLattitude = 2 * zipCodeInfo.Lattitude - maxLattitude;

            var minLongitude = 2 * zipCodeInfo.Longitude - maxLongitude;

            var nearbyZipCodes = await _context.ZipCodes.Where(z => 
                    (z.Lattitude >= minLattitude && z.Lattitude <= maxLattitude) &&
                    (z.Longitude >= minLongitude && z.Longitude <= maxLongitude) &&
                    (CalculateDistance(zipCodeInfo.Lattitude, zipCodeInfo.Longitude, z.Lattitude, z.Longitude) <= distance)
                ).ToListAsync();

            return Ok(nearbyZipCodes);
        }

        private double CalculateLatitudePlusDistance(double startLatitude, double distance)
        {
            var result = startLatitude + Math.Sqrt(distance * distance / 4766.8999155991);

            return result;
        }

        private double CalculateLongitudePlusDistance(double startLongititude, double startLatitude, double distance)
        {
            var result = startLongititude + Math.Sqrt(distance * distance / (4766.8999155991 * Math.Cos(2 * startLatitude / 114.591559026165) * Math.Cos(2 * startLatitude / 114.591559026165)));

            return result;
        }

        private double CalculateDistance(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            var temp = Math.Sin(latitude1 / 57.2957795130823) * Math.Sin(latitude2 / 57.2957795130823)
                + Math.Cos(latitude1 / 57.2957795130823) * Math.Cos(latitude2 / 57.2957795130823) * 
                  Math.Cos(longitude2 / 57.2957795130823 - longitude1 / 57.2957795130823);

            if (temp > 1)
            {
                temp = 1;
            }
            else if (temp < -1)
            {
                temp = -1;
            }

            var result = 3958.75586574 * Math.Acos(temp);

            return result;
        }
    }
}
