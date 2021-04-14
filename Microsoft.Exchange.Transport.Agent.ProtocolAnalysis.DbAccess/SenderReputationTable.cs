using System;
using Microsoft.Exchange.Transport.Storage;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.DbAccess
{
	internal class SenderReputationTable : DataTable
	{
		[DataColumnDefinition(typeof(byte[]), ColumnAccess.CachedProp, PrimaryKey = true, Required = true)]
		public const int SenderAddrHash = 0;

		[DataColumnDefinition(typeof(int), ColumnAccess.CachedProp, Required = true)]
		public const int Srl = 1;

		[DataColumnDefinition(typeof(bool), ColumnAccess.CachedProp, Required = true)]
		public const int OpenProxy = 2;

		[DataColumnDefinition(typeof(DateTime), ColumnAccess.CachedProp, Required = true)]
		public const int ExpirationTime = 3;
	}
}
