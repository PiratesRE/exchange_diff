using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Transport.Storage.IPFiltering
{
	internal class IPFilteringTable : DataTable
	{
		[DataColumnDefinition(typeof(int), ColumnAccess.CachedProp, PrimaryKey = true, Required = true, AutoIncrement = true)]
		public const int Identity = 0;

		[DataColumnDefinition(typeof(int), ColumnAccess.CachedProp, Required = true)]
		public const int Type = 1;

		[DataColumnDefinition(typeof(IPvxAddress), ColumnAccess.CachedProp, Required = true)]
		public const int LowAddress = 2;

		[DataColumnDefinition(typeof(IPvxAddress), ColumnAccess.CachedProp, Required = true)]
		public const int HighAddress = 3;

		[DataColumnDefinition(typeof(DateTime), ColumnAccess.CachedProp, Required = true)]
		public const int DateExpired = 4;

		[DataColumnDefinition(typeof(string), ColumnAccess.CachedProp)]
		public const int Comment = 5;
	}
}
