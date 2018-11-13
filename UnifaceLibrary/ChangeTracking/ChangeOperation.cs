using System;
using System.Linq;

namespace UnifaceLibrary
{
    /// <summary>
    /// SQL Server change tracking code mapping.
    /// </summary>
    public class ChangeOperation
    {
        public static ChangeOperation Insert = new ChangeOperation("I", "Insert");

        public static ChangeOperation Update = new ChangeOperation("U", "Update");

        public static ChangeOperation Delete = new ChangeOperation("D", "Delete");

        private static readonly ChangeOperation[] _all = new[] { Insert, Update, Delete };

        private readonly string _operationCode;
        private readonly string _operationDescription;

        private ChangeOperation(string operationCode, string operationDescription)
        {
            _operationCode = operationCode;
            _operationDescription = operationDescription;
        }

        public static explicit operator ChangeOperation(string operationCode)
        {
            var operation = _all.FirstOrDefault(o => o._operationCode.Equals(operationCode, StringComparison.InvariantCultureIgnoreCase));

            if (operation == null)
                throw new InvalidOperationException($"Invalid ChangeOperation code {operationCode}");

            return operation;
        }

        public override string ToString()
        {
            return _operationDescription;
        }
    }
}
