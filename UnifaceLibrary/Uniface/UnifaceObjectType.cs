using System;
using System.Linq;

namespace UnifaceLibrary
{
    public sealed class UnifaceObjectType
    {
        public static UnifaceObjectType Form { get; } = new UnifaceObjectType("Form");
        public static UnifaceObjectType Service { get; } = new UnifaceObjectType("Service");
        public static UnifaceObjectType Message { get; } = new UnifaceObjectType("Message");
        public static UnifaceObjectType Procedure { get; } = new UnifaceObjectType("Proc");
        public static UnifaceObjectType IncludeProcedure { get; } = new UnifaceObjectType("IncludeProc");

        public static UnifaceObjectType[] All { get; } = new[] { Form, Service, Message, Procedure, IncludeProcedure };

        public string Name { get; private set; }

        private UnifaceObjectType(string name)
        {
            Name = name;
        }

        public UnifaceObjectTypeTableSource TableSource => UnifaceObjectTypeTableSource.Get(this);

        public static UnifaceObjectType Get(string type)
        {
            var typeOrNull = All.SingleOrDefault(_ => _.Name.Equals(type, StringComparison.InvariantCultureIgnoreCase));

            if (typeOrNull == null)
                throw new ArgumentOutOfRangeException($"{type} is not a valid {nameof(UnifaceObjectType)}");

            return typeOrNull;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            
            return this.Name.Equals(((UnifaceObjectType)obj).Name, StringComparison.InvariantCultureIgnoreCase);
        }
        
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}