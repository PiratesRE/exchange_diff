using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IConversionParticipantList
	{
		int Count { get; }

		Participant this[int index]
		{
			get;
			set;
		}

		bool IsConversionParticipantAlwaysResolvable(int index);
	}
}
