using System;
using System.Collections.Generic;

namespace UnifaceLibrary
{
    public class UnifaceObjectTypeTableSource
    {
        /// <summary>
        /// Name of the table where the data resides.
        /// </summary>
        /// <returns></returns>
        public string PrimaryTable { get; }

        /// <summary>
        /// Overflow database table where if the code does not reside in the primary table
        /// then it is divided into 8k chunks and as separate records in this table.
        /// </summary>
        public string OverflowTable => "o" + PrimaryTable;

        /// <summary>
        /// Only select records for this particular object type.
        /// </summary>
        public string TypeFilter { get; }

        /// <summary>
        /// The database field where the source code resides.
        /// </summary>
        public string CodeField { get; }

        /// <summary>
        /// The database field which indicates the library that the object is associated with.
        /// </summary>
        public string LibraryField { get; }

        /// <summary>
        /// The unique identifier for the object (within the scope of the type and library of the object).
        /// </summary>
        public string IdField => "ulabel";

        // public string FilterSql => String.IsNullOrEmpty(FilterSql) ? String.Empty : $"WHERE {FilterSql}";

        private UnifaceObjectTypeTableSource(string primaryTable, string codeField, string libraryField, string typeFilter = "")
        {
            PrimaryTable = primaryTable;
            CodeField = codeField;
            LibraryField = libraryField;
            TypeFilter = typeFilter;
        }

        private static Dictionary<UnifaceObjectType, UnifaceObjectTypeTableSource> _all = new Dictionary<UnifaceObjectType, UnifaceObjectTypeTableSource>
        {
            { UnifaceObjectType.Form, new UnifaceObjectTypeTableSource("uform", codeField: "tplactual", libraryField: "librar", typeFilter: "uform.utransact = 0") },
            { UnifaceObjectType.Service, new UnifaceObjectTypeTableSource("uform", codeField: "tplactual", libraryField: "librar", typeFilter: "uform.utransact = 1") },
            { UnifaceObjectType.Procedure, new UnifaceObjectTypeTableSource("usource", codeField: "ucomment", libraryField: "uvar", typeFilter: "usource.usub = 'P'") },
            { UnifaceObjectType.IncludeProcedure, new UnifaceObjectTypeTableSource("usource", codeField: "ucomment", libraryField: "uvar", typeFilter: "usource.usub = 'I'") },
            { UnifaceObjectType.Message, new UnifaceObjectTypeTableSource("usource", codeField: "ucomment", libraryField: "uvar", typeFilter: "usource.usub = 'M' AND usource.uvar = 'SCA'") }
        };

        internal static UnifaceObjectTypeTableSource Get(UnifaceObjectType type)
        {
            return _all[type];
        }
    }
}
    