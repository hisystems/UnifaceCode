using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace UnifaceLibrary
{
    public static class DataExtensions
    {
        public static Dictionary<string, object> ToDictionary(this SqlDataReader reader)
        {
            return new int[reader.FieldCount]
                .Select((v, i) => new { Name = reader.GetName(i), Value = reader.GetValue(i) })
                .ToDictionary(a => a.Name, a => a.Value);
        }
    }
}
