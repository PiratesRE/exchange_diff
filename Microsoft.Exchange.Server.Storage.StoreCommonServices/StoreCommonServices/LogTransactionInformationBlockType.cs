using System;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public enum LogTransactionInformationBlockType : byte
	{
		Unknown,
		ForTestPurposes,
		Identity,
		AdminRpc,
		MapiRpc,
		Task,
		Digest,
		MaxValue
	}
}
