using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace UnifaceLibrary
{
    internal interface IUnifaceSourceCode
    {
        string CodeField { get; }

        string IdField { get; }

        string PrimaryTable { get; } 

        string TypeFilter { get; }

        string LibraryField { get; }

        UnifaceObjectData Extract(SqlConnection connection, UnifaceObjectId objectId); 
    }
}