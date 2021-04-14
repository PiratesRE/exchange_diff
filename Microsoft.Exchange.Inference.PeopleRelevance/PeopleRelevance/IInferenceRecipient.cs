using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Inference.PeopleRelevance
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IInferenceRecipient : IMessageRecipient, IEquatable<IMessageRecipient>
	{
		long TotalSentCount { get; set; }

		DateTime FirstSentTime { get; set; }

		DateTime LastSentTime { get; set; }

		int RecipientRank { get; set; }

		double RawRecipientWeight { get; set; }

		bool HasUpdatedData { get; set; }

		int RelevanceCategory { get; }

		int RelevanceCategoryAtLastCapture { get; set; }

		long LastUsedInTimeWindow { get; set; }

		int CaptureFlag { get; set; }
	}
}
