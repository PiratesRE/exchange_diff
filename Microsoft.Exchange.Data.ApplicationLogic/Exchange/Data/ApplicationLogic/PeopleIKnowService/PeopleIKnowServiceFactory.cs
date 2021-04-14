using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.PeopleIKnowService
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PeopleIKnowServiceFactory : IPeopleIKnowServiceFactory
	{
		internal PeopleIKnowServiceFactory(IPeopleIKnowSerializerFactory serializerFactory)
		{
			this.serializerFactory = serializerFactory;
		}

		public IPeopleIKnowService CreatePeopleIKnowService(ITracer tracer)
		{
			return new PeopleIKnowService(this.serializerFactory, tracer);
		}

		private readonly IPeopleIKnowSerializerFactory serializerFactory;
	}
}
