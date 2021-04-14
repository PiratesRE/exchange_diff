using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Inference.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IRecipientInfo
	{
		uint SentCount { get; }

		ExDateTime? FirstSentTimeUtc { get; }

		ExDateTime? LastSentTimeUtc { get; }

		string Address { get; }
	}
}
