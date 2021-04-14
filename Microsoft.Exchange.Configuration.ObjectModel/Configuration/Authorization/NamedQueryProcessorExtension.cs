using System;

namespace Microsoft.Exchange.Configuration.Authorization
{
	internal static class NamedQueryProcessorExtension
	{
		public static void Register(this INamedQueryProcessor processor)
		{
			RbacQuery.RegisterQueryProcessor(processor.Name, (RbacQuery.RbacQueryProcessor)processor);
		}
	}
}
