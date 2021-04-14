using System;
using Microsoft.Exchange.Transport.Storage;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.DbAccess
{
	internal class OpenProxyStatusTable : DataTable
	{
		[DataColumnDefinition(typeof(string), ColumnAccess.CachedProp, PrimaryKey = true, Required = true)]
		public const int SenderAddress = 0;

		[DataColumnDefinition(typeof(DateTime), ColumnAccess.CachedProp, Required = true)]
		public const int LastAccessTime = 1;

		[DataColumnDefinition(typeof(DateTime), ColumnAccess.CachedProp)]
		public const int LastDetectionTime = 2;

		[DataColumnDefinition(typeof(bool), ColumnAccess.CachedProp, Required = true)]
		public const int Processing = 3;

		[DataColumnDefinition(typeof(string), ColumnAccess.CachedProp)]
		public const int Message = 4;

		[DataColumnDefinition(typeof(int), ColumnAccess.CachedProp, Required = true)]
		public const int OpenProxyStatus = 5;
	}
}
