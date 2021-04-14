using System;
using System.IO;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class FilteringServiceInvokerFactory
	{
		public static FilteringServiceInvoker Create(ITracer tracer)
		{
			FilteringServiceInvoker result;
			try
			{
				result = new FipsFilteringServiceInvoker();
			}
			catch (Exception e)
			{
				FilteringServiceInvokerFactory.LogFipsInvokerCreationException(e, tracer);
				result = NullFilteringServiceInvoker.Factory();
			}
			return result;
		}

		protected static void LogFipsInvokerCreationException(Exception e, ITracer tracer)
		{
			if (e is FileNotFoundException)
			{
				tracer.TraceWarning("Fips does not appear to be installed. Acting as if there are no fips results (Exception: '{0}')", new object[]
				{
					e
				});
				return;
			}
			tracer.TraceError("Unexpected exception while creating FipsFilteringServiceInvoker (Exception: '{0}')", new object[]
			{
				e
			});
		}
	}
}
