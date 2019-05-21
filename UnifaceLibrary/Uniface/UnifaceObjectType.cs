using System;
using System.Linq;

namespace UnifaceLibrary
{
    public sealed class UnifaceObjectType
    {
        public static UnifaceObjectType Form { get; } = new UnifaceObjectType("Form", new UnifaceFormSourceCode());
        public static UnifaceObjectType Service { get; } = new UnifaceObjectType("Service", new UnifacePrimarySourceCode("uform", codeField: "tplactual", libraryField: "librar", typeFilter: "uform.utransact = 1"));
        public static UnifaceObjectType Entity { get; } = new UnifaceObjectType("Entity", new UnifaceEntitySourceCode());
        public static UnifaceObjectType Message { get; } = new UnifaceObjectType("Message", new UnifacePrimarySourceCode("usource", codeField: "ucomment", libraryField: "uvar", typeFilter: "usource.usub = 'M' AND usource.uvar = 'SCA'"));
        public static UnifaceObjectType Procedure { get; } = new UnifaceObjectType("Proc", new UnifacePrimarySourceCode("usource", codeField: "ucomment", libraryField: "uvar", typeFilter: "usource.usub = 'P'"));
        public static UnifaceObjectType IncludeProcedure { get; } = new UnifaceObjectType("IncludeProc", new UnifacePrimarySourceCode("usource", codeField: "ucomment", libraryField: "uvar", typeFilter: "usource.usub = 'I'") );

        public static UnifaceObjectType[] All { get; } = new[] { Form, Service, Entity, Message, Procedure, IncludeProcedure };

        public string Name { get; private set; }

        internal IUnifaceSourceCode SourceCode { get; private set; }

        private UnifaceObjectType(string name, IUnifaceSourceCode sourceCode)
        {
            Name = name;
            SourceCode = sourceCode;
        }

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