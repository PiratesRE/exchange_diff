using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[Serializable]
	public abstract class OwaTransientException : LocalizedException
	{
		public OwaTransientException(string message) : this(message, null, null)
		{
		}

		public OwaTransientException(string message, Exception innerException) : this(message, innerException, null)
		{
		}

		protected OwaTransientException(string message, Exception innerException, object thisObject) : base(new LocalizedString(message), innerException)
		{
			ExTraceGlobals.ExceptionTracer.TraceDebug((long)((thisObject != null) ? thisObject.GetHashCode() : 0), (message != null) ? message : "<Exception has no message associated>");
		}
	}
}
