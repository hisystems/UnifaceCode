using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace UnifaceLibrary
{
    public class UnifaceEntitySourceCode : IUnifaceSourceCode
    {
        private readonly IUnifaceSourceCode primarySourceTable = new UnifacePrimarySourceCode("ucgroup", codeField: "tplactual", libraryField: "u_vlab", idField: "u_glab", applyLibraryConditionToOverflowTable: true);

        string IUnifaceSourceCode.CodeField => primarySourceTable.CodeField;

        string IUnifaceSourceCode.IdField => primarySourceTable.IdField;

        string IUnifaceSourceCode.PrimaryTable => primarySourceTable.PrimaryTable;

        string IUnifaceSourceCode.TypeFilter => primarySourceTable.TypeFilter;

        string IUnifaceSourceCode.LibraryField => primarySourceTable.LibraryField;

        /// <summary>
        /// Entity field code
        /// </summary>
        private class UnifaceObjectSourceCodeBlockUcfield
        {
            public string Utlab { get; set; }
            public string Uvlab { get; set; }
            public string Uflab { get; set; }
            public StringBuilder SourceCode { get; } = new StringBuilder();
        }

        UnifaceObjectData IUnifaceSourceCode.Extract(SqlConnection connection, UnifaceObjectId objectId)
        {
            var objectData = primarySourceTable.Extract(connection, objectId);

            var codeBlockGroupEntityFields = GetCodeBlockGroupEntityFields(connection, objectId);

            foreach (var codeBlockEntityField in codeBlockGroupEntityFields)
                objectData.SourceCodeBlockGroups.Add(new UnifaceObjectSourceCodeBlockGroup(codeBlockEntityField.Uvlab + "." + codeBlockEntityField.Utlab + "." + codeBlockEntityField.Uflab, codeBlockEntityField.SourceCode));

            return objectData;
        }
        
        private List<UnifaceObjectSourceCodeBlockUcfield> GetCodeBlockGroupEntityFields(SqlConnection connection, UnifaceObjectId entity)
        {
            var codeBlockGroups = new List<UnifaceObjectSourceCodeBlockUcfield>();

            var command = new SqlCommand(
                $"SELECT * FROM ucfield " +
                $"WHERE u_tlab = '{entity.ObjectName}' AND u_vlab = '{entity.LibraryName}'", connection);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var codeBlockGroup = new UnifaceObjectSourceCodeBlockUcfield();
                    codeBlockGroup.Utlab = reader["u_tlab"].ToString().Trim();
                    codeBlockGroup.Uvlab = reader["u_vlab"].ToString().Trim();
                    codeBlockGroup.Uflab = reader["u_flab"].ToString().Trim();
                    codeBlockGroup.SourceCode.Append(reader["u_desc"].ToString());
                    codeBlockGroups.Add(codeBlockGroup);
                }
            }

            // TODO - load overflow data from oucfield. e.g.: select * admin.oucfield  u_tlab = 'CDCATTP'

            return codeBlockGroups;
        }
    }
}