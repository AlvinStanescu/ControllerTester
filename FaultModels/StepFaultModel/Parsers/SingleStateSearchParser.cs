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
        internal static SerializableDictionary<string, object> Parse(string worstPointFile, int requirementIndex)
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
                if (requirementIndex == 6)
                {
                    input.Add("ObjectiveFunctionValue", ObjectiveFunctionValueParser.Parse(numbers[2]) / Double.Parse(numbers[1], CultureInfo.InvariantCulture));
                }
                else
                {
                    input.Add("ObjectiveFunctionValue", ObjectiveFunctionValueParser.Parse(numbers[2]));
                }

                return input;
            }

        }
    }
}
