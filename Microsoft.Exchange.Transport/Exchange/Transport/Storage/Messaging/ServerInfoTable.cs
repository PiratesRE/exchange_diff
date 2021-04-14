using System;

namespace Microsoft.Exchange.Transport.Storage.Messaging
{
	internal sealed class ServerInfoTable : DataTable
	{
		[DataColumnDefinition(typeof(int), ColumnAccess.CachedProp, PrimaryKey = true, Required = true, AutoIncrement = true)]
		public const int RowId = 0;

		[DataColumnDefinition(typeof(string), ColumnAccess.CachedProp, Required = true)]
		public const int DatabaseState = 1;

		[DataColumnDefinition(typeof(string), ColumnAccess.CachedProp, Required = true)]
		public const int ServerFqdn = 2;

		[DataColumnDefinition(typeof(DateTime), ColumnAccess.CachedProp, Required = true)]
		public const int StartTime = 3;

		[DataColumnDefinition(typeof(DateTime), ColumnAccess.CachedProp, Required = true)]
		public const int EndTime = 4;

		[DataColumnDefinition(typeof(int), ColumnAccess.CachedProp, Required = true)]
		public const int Version = 5;
	}
}
