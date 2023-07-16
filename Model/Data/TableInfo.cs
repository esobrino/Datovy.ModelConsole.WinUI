using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Data
{

   public class TableInfo
   {
      public string CatalogName { get; set; }
      public string SchemaName { get; set; }
      public string TableName { get; set; }
      public List<ColumnInfo> Columns { get; set; }

      public void Copy(TableInfo table)
      {
         CatalogName = table.CatalogName;
         SchemaName = table.SchemaName;
         TableName = table.TableName;
         Columns = table.Columns;
      }
   }

}
