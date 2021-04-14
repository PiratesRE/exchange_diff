using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.PeopleIKnowService
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class PeopleIKnowSerializerFactory : IPeopleIKnowSerializerFactory
	{
		public IPeopleIKnowSerializer CreatePeopleIKnowSerializer()
		{
			return PeopleIKnowJsonSerializer.Singleton;
		}
	}
}
