using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FM4CC.Util
{
    public static class ObjectiveFunctionValueParser
    {
        public static double Parse(string value)
        {
            if (value == "Inf")
            {
                return Double.PositiveInfinity;
            }
            else
            {
                return Double.Parse(value, CultureInfo.InvariantCulture);
            }
        }
    }
}
