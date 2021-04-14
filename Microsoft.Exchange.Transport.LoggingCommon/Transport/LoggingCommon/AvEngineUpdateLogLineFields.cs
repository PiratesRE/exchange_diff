using System;

namespace Microsoft.Exchange.Transport.LoggingCommon
{
	internal enum AvEngineUpdateLogLineFields : short
	{
		Timestamp,
		EngineCategory,
		EngineName,
		EngineVersion,
		SignatureVersion,
		SignatureDateTime,
		RUSId,
		UpdatedDateTime
	}
}
