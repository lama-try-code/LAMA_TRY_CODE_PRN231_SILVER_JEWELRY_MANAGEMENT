using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LamDT_SE172525.DAO
{
    public class Util
    {
        public string GenerateRandomString()
        {
            Random random = new Random();

            char firstChar = 'S';
            char secondChar = random.Next(0, 3) switch
            {
                0 => 'B',
                1 => 'M',
                _ => 'N'
            };

            int digit = random.Next(0, 10);
            char letter1 = (char)random.Next('A', 'Z' + 1);
            char letter2 = (char)random.Next('A', 'Z' + 1);

            string numberPart = random.Next(10000, 100000).ToString();
            string decimalPart = random.Next(0, 100).ToString("D3");

            return $"{firstChar}{secondChar}{digit}{letter1}{letter2}{numberPart}.{decimalPart}";
        }
    }
}
