using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace UnifaceLibrary
{
    public class UnifacePrimarySourceCode : IUnifaceSourceCode
    {
        /// <summary>
        /// Name of the table where the data resides.
        /// </summary>
        /// <returns></returns>
        public string PrimaryTable { get; private set; }

        /// <summary>
        /// Overflow database table where if the code does not reside in the primary table
        /// then it is divided into 8k chunks and as separate records in this table.
        /// </summary>
        private string overflowTable => "o" + PrimaryTable;

        /// <summary>
        /// Only select records for this particular object type.
        /// </summary>
        public string TypeFilter { get; private set; }

        /// <summary>
        /// The database field where the source code resides.
        /// </summary>
        public string CodeField { get; private set; }

        /// <summary>
        /// The database field which indicates the library that the object is associated with.
        /// </summary>
        public string LibraryField { get; private set;}

        /// <summary>
        /// The unique identifier for the object (within the scope of the type and library of the object).
        /// </summary>
        public string IdField { get; private set; }

        private readonly bool applyLibraryConditionToOverflowTable;

        internal UnifacePrimarySourceCode(string primaryTable, string codeField, string libraryField, string typeFilter = "1=1", string idField = "ulabel", bool applyLibraryConditionToOverflowTable = false)
        {
            this.PrimaryTable = primaryTable;
            this.CodeField = codeField;
            this.LibraryField = libraryField;
            this.TypeFilter = typeFilter;
            this.IdField = idField;
            this.applyLibraryConditionToOverflowTable = applyLibraryConditionToOverflowTable;
        }

        UnifaceObjectData IUnifaceSourceCode.Extract(SqlConnection connection, UnifaceObjectId objectId) 
        {
            var sourceCodeBlock = new StringBuilder();
            Dictionary<string, object> objectData;

            var libraryCondition = objectId.LibraryNameIsGlobal ?
                $"{LibraryField} IS NULL" : $"{LibraryField} = '{objectId.LibraryName}'";
            
            var command = new SqlCommand(
                $"SELECT * FROM {PrimaryTable} "+
                $"WHERE {IdField} = '{objectId.ObjectName}' AND {libraryCondition} AND {TypeFilter}", connection);

            using (var reader = command.ExecuteReader())
            {
                if (!reader.Read())
                    throw new InvalidOperationException($"Could not find {objectId} in database. Check that the object identifier is correct. SQL: '{command.CommandText}'");

                objectData = reader.ToDictionary();
                sourceCodeBlock.Append(reader[CodeField].ToString());
            }

            var overflowLibraryCondition = this.applyLibraryConditionToOverflowTable ? " AND " + libraryCondition : "";
            var getSegmentsCommand = new SqlCommand($"SELECT data FROM {overflowTable} WHERE {IdField} = '{objectId.ObjectName}' {overflowLibraryCondition} ORDER BY segm", connection);

            using (var reader = getSegmentsCommand.ExecuteReader())
                UnifaceSourceCodeParser.LoadOverflowSegments(reader, sourceCodeBlock);

            return new UnifaceObjectData(objectData, new UnifaceObjectSourceCodeBlockGroup("", sourceCodeBlock));
        }
    }
}