using System;

namespace Microsoft.Exchange.Transport.Storage.Messaging
{
	internal class QueueTable : DataTable
	{
		[DataColumnDefinition(typeof(long), ColumnAccess.CachedProp, PrimaryKey = true, Required = true)]
		public const int QueueRowId = 0;

		[DataColumnDefinition(typeof(Guid), ColumnAccess.CachedProp, Required = true)]
		public const int NextHopConnector = 1;

		[DataColumnDefinition(typeof(string), ColumnAccess.CachedProp, Required = true)]
		public const int NextHopDomain = 2;

		[DataColumnDefinition(typeof(int), ColumnAccess.CachedProp, Required = true)]
		public const int State = 3;

		[DataColumnDefinition(typeof(int), ColumnAccess.CachedProp, Required = true)]
		public const int NextHopType = 4;
	}
}
