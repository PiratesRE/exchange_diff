using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class NullPeopleIKnowPublisherFactory : IPeopleIKnowPublisherFactory
	{
		private NullPeopleIKnowPublisherFactory()
		{
		}

		public IPeopleIKnowPublisher Create()
		{
			return NullPeopleIKnowPublisher.Instance;
		}

		public static readonly NullPeopleIKnowPublisherFactory Instance = new NullPeopleIKnowPublisherFactory();
	}
}
