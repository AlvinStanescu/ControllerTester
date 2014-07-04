using FM4CC.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FM4CC.FaultModels.Step.Parsers
{
    internal class SingleStateSearchParser
    {
        internal static SerializableDictionary<string, object> Parse(string worstPointFile)
        {
            IEnumerable<string> lines = System.IO.File.ReadLines(worstPointFile);

            if (lines == null || lines.Count() < 2) throw new ArgumentException("Invalid file");
            else
            {
                lines = lines.Skip(1);
                SerializableDictionary<string, object> input = new SerializableDictionary<string, object>();
                string[] numbers = (lines.First()).Split(',');
                input.Add("Initial", Double.Parse(numbers[0], CultureInfo.InvariantCulture));
                input.Add("Final", Double.Parse(numbers[1], CultureInfo.InvariantCulture));
                input.Add("ObjectiveFunctionValue", Double.Parse(numbers[2], CultureInfo.InvariantCulture));

                return input;
            }

        }
    }
}
