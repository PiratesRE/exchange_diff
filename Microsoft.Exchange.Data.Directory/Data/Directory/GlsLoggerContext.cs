using System;

namespace Microsoft.Exchange.Data.Directory
{
	internal class GlsLoggerContext
	{
		internal int TickStart { get; private set; }

		internal string MethodName { get; private set; }

		internal string ParameterValue { get; private set; }

		internal string EndpointHostName { get; private set; }

		internal bool IsRead { get; private set; }

		internal Guid RequestTrackingGuid { get; private set; }

		internal string ConnectionId { get; set; }

		internal GlsLoggerContext(string methodName, object parameterValue, string endpointHostName, bool isRead, Guid requestTrackingGuid)
		{
			this.TickStart = Environment.TickCount;
			this.MethodName = methodName;
			this.ParameterValue = parameterValue.ToString();
			this.EndpointHostName = endpointHostName;
			this.IsRead = isRead;
			this.RequestTrackingGuid = requestTrackingGuid;
		}

		internal string ResolveEndpointToIpAddress(bool flushCache)
		{
			return this.EndpointHostName;
		}
	}
}
