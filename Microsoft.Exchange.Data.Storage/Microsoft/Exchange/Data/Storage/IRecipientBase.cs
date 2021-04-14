using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IRecipientBase
	{
		RecipientId Id { get; }

		bool? IsDistributionList();

		Participant Participant { get; }
	}
}
