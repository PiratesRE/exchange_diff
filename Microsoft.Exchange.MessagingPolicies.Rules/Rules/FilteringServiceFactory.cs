using System;
using Microsoft.Filtering;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class FilteringServiceFactory
	{
		public static void Create(out IFipsDataStreamFilteringService filteringService)
		{
			filteringService = ((FilteringServiceFactory.InstanceBuilder != null) ? FilteringServiceFactory.InstanceBuilder() : new FipsDataStreamFilteringService());
		}

		public static FilteringServiceFactory.FilteringServiceBuilder InstanceBuilder;

		public delegate IFipsDataStreamFilteringService FilteringServiceBuilder();
	}
}
