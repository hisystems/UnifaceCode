using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace UnifaceLibrary
{
    public class UnifaceFormSourceCode : IUnifaceSourceCode
    {
        private readonly IUnifaceSourceCode primarySourceTable = new UnifacePrimarySourceCode("uform", codeField: "tplactual", libraryField: "librar", typeFilter: "uform.utransact = 0");

        string IUnifaceSourceCode.CodeField => primarySourceTable.CodeField;

        string IUnifaceSourceCode.IdField => primarySourceTable.IdField;

        string IUnifaceSourceCode.PrimaryTable => primarySourceTable.PrimaryTable;

        string IUnifaceSourceCode.TypeFilter => primarySourceTable.TypeFilter;

        string IUnifaceSourceCode.LibraryField => primarySourceTable.LibraryField;

        /// <summary>
        /// Override entity code
        /// </summary>
        private class UnifaceObjectSourceCodeBlockUxgroup
        {
            public string Ulabel { get; set; }
            public string Ubase { get; set; }
            public StringBuilder SourceCode { get; } = new StringBuilder();
        }

        /// <summary>
        /// Override entity field code
        /// </summary>
        private class UnifaceObjectSourceCodeBlockUxfield
        {
            public string Ulabel { get; set; }
            public string Grp { get; set; }
            public string Ubase { get; set; }
            public StringBuilder SourceCode { get; } = new StringBuilder();
        }

        UnifaceObjectData IUnifaceSourceCode.Extract(SqlConnection connection, UnifaceObjectId objectId)
        {
            var objectData = primarySourceTable.Extract(connection, objectId);

            var codeBlockEntityGroups = GetCodeBlockEntitiyGroups(connection, objectId);
            var codeBlockEntityFields = GetCodeBlockEntityFields(connection, objectId);

            // Add the groups to the object data.
            foreach (var codeBlockGroup in codeBlockEntityGroups)
                objectData.SourceCodeBlockGroups.Add(new UnifaceObjectSourceCodeBlockGroup(codeBlockGroup.Ubase + "." + codeBlockGroup.Ulabel, codeBlockGroup.SourceCode));

            // Add the fields to the object data.
            foreach (var codeBlockField in codeBlockEntityFields)
                objectData.SourceCodeBlockGroups.Add(new UnifaceObjectSourceCodeBlockGroup(codeBlockField.Ubase + "." + codeBlockField.Grp + "." + codeBlockField.Ulabel, codeBlockField.SourceCode));

            return objectData;
        }

        private static List<UnifaceObjectSourceCodeBlockUxgroup> GetCodeBlockEntitiyGroups(SqlConnection connection, UnifaceObjectId objectId)
        {
            var codeBlockGroups = new List<UnifaceObjectSourceCodeBlockUxgroup>();

            var command = new SqlCommand(
                $"SELECT * FROM uxgroup " +
                $"WHERE uform = '{objectId.ObjectName}'", connection);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var codeBlockGroup = new UnifaceObjectSourceCodeBlockUxgroup();
                    codeBlockGroup.Ubase = reader["ubase"].ToString().Trim();
                    codeBlockGroup.Ulabel = reader["ulabel"].ToString().Trim();
                    codeBlockGroup.SourceCode.Append(reader["tplactual"].ToString());
                    codeBlockGroups.Add(codeBlockGroup);
                }
            }

            // Load the overflow data.
            foreach (var codeBlockGroup in codeBlockGroups)
            {
                var overflowCommand = new SqlCommand(
                    $"SELECT * FROM ouxgroup " +
                    $"WHERE uform = '{objectId.ObjectName}' AND ulabel = '{codeBlockGroup.Ulabel}' AND ubase = '{codeBlockGroup.Ubase}' ORDER BY segm", connection);

                using (var reader = overflowCommand.ExecuteReader())
                    UnifaceSourceCodeParser.LoadOverflowSegments(reader, codeBlockGroup.SourceCode);
            }

            return codeBlockGroups;
        }

        private static List<UnifaceObjectSourceCodeBlockUxfield> GetCodeBlockEntityFields(SqlConnection connection, UnifaceObjectId objectId)
        {
            var codeBlockFields = new List<UnifaceObjectSourceCodeBlockUxfield>();

            var command = new SqlCommand(
                $"SELECT * FROM uxfield " +
                $"WHERE uform = '{objectId.ObjectName}'", connection);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var codeBlockField = new UnifaceObjectSourceCodeBlockUxfield();
                    codeBlockField.Ulabel = reader["ulabel"].ToString().Trim();
                    codeBlockField.Grp = reader["grp"].ToString().Trim();
                    codeBlockField.Ubase = reader["ubase"].ToString().Trim();
                    codeBlockField.SourceCode.Append(reader["startmod"].ToString());
                    codeBlockFields.Add(codeBlockField);
                }
            }

            // Load the overflow data.
            foreach (var codeBlockField in codeBlockFields)
            {
                var overflowCommand = new SqlCommand(
                    $"SELECT * FROM ouxfield " +
                    $"WHERE uform = '{objectId.ObjectName}' AND ulabel = '{codeBlockField.Ulabel}' AND grp = '{codeBlockField.Grp}' AND ubase = '{codeBlockField.Ubase}' ORDER BY segm", connection);

                using (var reader = overflowCommand.ExecuteReader())
                    UnifaceSourceCodeParser.LoadOverflowSegments(reader, codeBlockField.SourceCode);
            }

            return codeBlockFields;
        }
    }
}