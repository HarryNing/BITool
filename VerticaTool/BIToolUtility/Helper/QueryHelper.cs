using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIToolUtility.Helper
{
    public static class QueryHelper
    {
        public static string GetSourceTableQuery = @"
SELECT tb.Name AS TableName, 'CREATE TABLE @@.' + tb.name + ' (' + LEFT(cn.list, LEN(cn.list) - 1) +
',PRIMARY KEY ('+ LEFT(pn.list, LEN(pn.list) - 1)+
 '));' AS CreateTableText, tc.rows AS RowsCount
FROM sysobjects tb
CROSS APPLY ( SELECT ' ' + column_name + ' '
			   + CASE data_type
				  WHEN 'bit' THEN 'TINYINT'
				  WHEN 'nvarchar' THEN 'VARCHAR'
				  ELSE data_type
			   END
                + CASE data_type
                    WHEN 'nvarchar' THEN
                          COALESCE('('
                                  + CASE WHEN character_maximum_length = -1
                                         THEN '4000'
                                         ELSE CAST(character_maximum_length*2 AS VARCHAR)
                                    END + ')', '')
                    WHEN 'decimal'
                    THEN '('
                         + CAST(numeric_precision AS VARCHAR)
                         + ', '
                         + CAST(numeric_scale AS VARCHAR)
                         + ')'
                    ELSE COALESCE('('
                                  + CASE WHEN character_maximum_length = -1
                                         THEN '4000'
                                         ELSE CAST(character_maximum_length AS VARCHAR)
                                    END + ')', '')
                  END + ' '
                + 'NULL, '
          FROM INFORMATION_SCHEMA.COLUMNS tc
          WHERE table_name = tb.name 
          AND tc.DATA_TYPE NOT IN('image','ntext','text','sql_variant','xml') 
          AND tc.COLUMN_NAME NOT IN('desc','asc')
          ORDER BY ordinal_position
          FOR XML PATH('')
) cn (list)
CROSS APPLY
(SELECT '' + Column_Name + ', '
	FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE kcu
	INNER JOIN sysobjects so ON so.name=kcu.CONSTRAINT_NAME AND so.xtype='PK'
	WHERE kcu.TABLE_NAME=tb.name
	ORDER BY ORDINAL_POSITION
	FOR XML PATH('')
) pn (list)
LEFT JOIN 
(
	SELECT si.id,max(si.rows) as rows
	FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE kcu
	INNER JOIN sysindexes si ON si.name=kcu.CONSTRAINT_NAME 
	group by si.id
) tc on tc.id = tb.id
WHERE tb.xtype = 'U' AND pn.list IS NOT NULL
ORDER BY tb.Name";

        public static string GetTargetTableQuery = @"
SELECT t.table_name,
       ISNULL(SUM(ps.wos_row_count + ps.ros_row_count),0) AS row_count,
	   cn.column_names
FROM tables t
INNER JOIN
(select table_id,group_concat(column_name) over (partition by table_id) as column_names from columns) cn ON cn.table_id=t.table_id
LEFT JOIN projections p ON t.table_id = p.anchor_table_id and p.is_super_projection='t'
LEFT JOIN projection_storage ps on p.projection_id = ps.projection_id
WHERE t.table_schema='@@'
GROUP BY t.table_name,cn.column_names
ORDER BY t.table_name; ";

        public static string GetTargetColumnsQuery = @"
select group_concat(column_name) over() as column_names 
from columns where table_schema='{0}' and table_name='{1}';";

        public static string GetPKsMaxUpdateDateQuery = @"
select pk.primary_keys, mu.max_updatedate from
(
select group_concat(column_name)over() AS primary_keys from primary_keys where table_schema='{0}' and table_name='{1}'
) pk
cross join
(
select max(updatedate) AS max_updatedate from {0}.{1}
)mu;";
    }
}
