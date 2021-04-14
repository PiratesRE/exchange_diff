﻿using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Local.Components.Rws
{
	internal class RwsSqlQueries
	{
		internal const string GetDatabaseDataFileUsage = "\r\n                DECLARE @sql VARCHAR(8000), @sql2 VARCHAR(8000)\r\n                SET @sql = 'SELECT DB_NAME() AS [Database], SUM(reserved_page_count) * 8.0 / 1024 as [Size (MB)] FROM sys.dm_db_partition_stats'\r\n                SET @sql2 = 'USE [' + @DatabaseName + '];' + @sql\r\n\r\n                DECLARE @database_usage TABLE(\r\n                    database_name SYSNAME,\r\n                    space_used FLOAT\r\n                );\r\n\r\n                INSERT INTO @database_usage EXEC (@sql2)\r\n\r\n                SELECT\r\n                    db.[name] AS [Database],\r\n                    dbf.[physical_name] AS [File Name],\r\n                    convert(FLOAT, dbf.[size] * 8 / 1024.0) AS [Current Size (MB)],\r\n                    convert(FLOAT, dbu.[space_used]) AS [UsedSizeInMB],\r\n                    CASE WHEN (dbf.[max_size] <> -1) THEN CAST(dbf.[max_size]/1024 * 8 AS VARCHAR) ELSE 'Unlimited' END AS [Max Size (MB)],\r\n                    CASE WHEN (dbf.[is_percent_growth] = 1) THEN 'By ' + CAST(dbf.[growth] AS VARCHAR) + '%' ELSE 'By ' + CAST(dbf.[growth] * 8 / 1024 AS VARCHAR) + 'MB' END AS [AutoGrowthSetting]\r\n                FROM\r\n                    sys.[databases] AS db\r\n                    LEFT OUTER JOIN sys.[master_files] AS dbf ON [db].[database_id] = [dbf].[database_id], @database_usage AS dbu\r\n                WHERE\r\n                    dbf.[type] = 0 AND\r\n                    db.[name] = @DatabaseName AND\r\n                    dbu.[database_name] = db.[name]\r\n                ORDER BY \r\n                    db.[name], dbf.[file_id]\r\n            ";

		internal const string GetDatabaseLogFileUsage = "\r\n                DECLARE @tran_log_space_usage TABLE(\r\n                    database_name SYSNAME,\r\n                    log_size_mb FLOAT,\r\n                    log_space_used FLOAT,\r\n                    [status] INT\r\n                );\r\n\r\n                INSERT INTO @tran_log_space_usage EXEC('DBCC SQLPERF (LOGSPACE)') \r\n\r\n                SELECT\r\n                    db.[name] AS [Database],\r\n                    dbf.[physical_name] AS [File Name],\r\n                    convert(FLOAT, dbf.[size] * 8 / 1024.0) AS [Current Size (MB)],\r\n                    convert(FLOAT, tlog.[log_space_used] * dbf.[size] * 8 / (1024 * 100)) AS [Used Size (MB)],\r\n                    CASE WHEN (dbf.[max_size] <> -1) THEN CAST(dbf.[max_size]/1024 * 8 AS VARCHAR) ELSE 'Unlimited' END AS [Max Size (MB)],\r\n                    CASE WHEN (dbf.[is_percent_growth] = 1) THEN 'By ' + CAST(dbf.[growth] AS VARCHAR) + '%' ELSE 'By ' + CAST(dbf.[growth] * 8 / 1024 AS VARCHAR) + 'MB' END AS [AutoGrowthSetting]\r\n                FROM \r\n                    sys.[databases] AS db\r\n                    LEFT OUTER JOIN sys.[master_files] AS dbf ON [db].[database_id] = [dbf].[database_id], @tran_log_space_usage AS tlog\r\n                WHERE\r\n                    dbf.[type] = 1 AND\r\n                    db.[name] = @DatabaseName AND\r\n                    tlog.[database_name] = db.[name]\r\n                ORDER BY \r\n                    db.[name], dbf.[file_id]\r\n            ";

		internal const string GetDatabaseTableUsage = "\r\n                SELECT\r\n                    sys.objects.name as [TableName], convert(FLOAT, sum(reserved_page_count) * 8.0 / 1024) as [Size (MB)]\r\n                FROM \r\n                    sys.dm_db_partition_stats, sys.objects \r\n                WHERE \r\n                    sys.dm_db_partition_stats.object_id = sys.objects.object_id AND\r\n                    sys.objects.type = 'U'\r\n                GROUP BY \r\n                    sys.objects.name\r\n            ";

		internal const string InsertDatabaseDataFileUsage = "\r\n                INSERT INTO [dbo].[Database_DataFileUsage]\r\n                (\r\n                    [Instance],\r\n                    [Database],\r\n                    [FileName],\r\n                    [CurrentSizeInMB],\r\n                    [UsedSizeInMB],\r\n                    [MaxSizeInMB]\r\n                )\r\n                VALUES\r\n                (\r\n                    @Instance,\r\n                    @Database,\r\n                    @FileName,\r\n                    @CurrentSizeInMB,\r\n                    @UsedSizeInMB,\r\n                    @MaxSizeInMB\r\n                )\r\n            ";

		internal const string InsertDatabaseLogFileUsage = "\r\n                INSERT INTO [dbo].[Database_LogFileUsage]\r\n                (\r\n                    [Instance],\r\n                    [Database],\r\n                    [FileName],\r\n                    [CurrentSizeInMB],\r\n                    [UsedSizeInMB],\r\n                    [MaxSizeInMB]\r\n                )\r\n                VALUES\r\n                (\r\n                    @Instance,\r\n                    @Database,\r\n                    @FileName,\r\n                    @CurrentSizeInMB,\r\n                    @UsedSizeInMB,\r\n                    @MaxSizeInMB\r\n                )\r\n            ";

		internal const string InsertDatabaseTableUsage = "\r\n                INSERT INTO [dbo].[Database_TableUsage]\r\n                (\r\n                    [Instance],\r\n                    [Database],\r\n                    [Table],\r\n                    [CurrentSizeInMB]\r\n                )\r\n                VALUES\r\n                (\r\n                    @Instance,\r\n                    @Database,\r\n                    @Table,\r\n                    @CurrentSizeInMB\r\n                )\r\n            ";
	}
}