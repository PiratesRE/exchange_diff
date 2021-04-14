using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IAttendeeCollectionImpl : IAttendeeCollection, IRecipientBaseCollection<Attendee>, IList<Attendee>, ICollection<Attendee>, IEnumerable<Attendee>, IEnumerable
	{
		void Cleanup();

		void LoadIsDistributionList();
	}
}
