/*
 * AUTHOR: David B. Dickey (KN4TEM) 
 * Written 8/31/2022 for CARC special project.
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepeaterController.Extensions
{
    public static class ThermometerExtensions
    {
        public static bool BetweenInclusive(this Double val, Double minValue, Double maxValue)
        {
            return val.CompareTo(minValue) >= 0 && val.CompareTo(maxValue) <= 0;
        }
    }
}
