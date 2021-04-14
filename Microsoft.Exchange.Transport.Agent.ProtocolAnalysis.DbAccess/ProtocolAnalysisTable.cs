using System;
using Microsoft.Exchange.Transport.Storage;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.DbAccess
{
	internal class ProtocolAnalysisTable : DataTable
	{
		[DataColumnDefinition(typeof(string), ColumnAccess.CachedProp, PrimaryKey = true, Required = true)]
		public const int SenderAddress = 0;

		[DataColumnDefinition(typeof(DateTime), ColumnAccess.CachedProp, Required = true)]
		public const int LastUpdateTime = 1;

		[DataColumnDefinition(typeof(byte[]), ColumnAccess.CachedProp)]
		public const int DataBlob = 2;

		[DataColumnDefinition(typeof(string), ColumnAccess.CachedProp)]
		public const int ReverseDns = 3;

		[DataColumnDefinition(typeof(DateTime), ColumnAccess.CachedProp)]
		public const int LastQueryTime = 4;

		[DataColumnDefinition(typeof(bool), ColumnAccess.CachedProp, Required = true)]
		public const int Processing = 5;
	}
}
