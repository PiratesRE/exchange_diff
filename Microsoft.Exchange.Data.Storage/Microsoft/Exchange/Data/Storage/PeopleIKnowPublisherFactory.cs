using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PeopleIKnowPublisherFactory : IPeopleIKnowPublisherFactory
	{
		public PeopleIKnowPublisherFactory(IXSOFactory xsoFactory, ITracer tracer, int traceId)
		{
			ArgumentValidator.ThrowIfNull("xsoFactory", xsoFactory);
			ArgumentValidator.ThrowIfNull("tracer", tracer);
			this.xsoFactory = xsoFactory;
			this.tracer = tracer;
			this.traceId = traceId;
		}

		public IPeopleIKnowPublisher Create()
		{
			return new PeopleIKnowEmailAddressCollectionFolderProperty(this.xsoFactory, this.tracer, this.traceId);
		}

		private readonly ITracer tracer;

		private readonly int traceId;

		private readonly IXSOFactory xsoFactory;
	}
}
