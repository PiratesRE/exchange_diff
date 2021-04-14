using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public abstract class OwaPermanentException : LocalizedException
	{
		public OwaPermanentException(string message) : this(message, null, null)
		{
		}

		public OwaPermanentException(string message, Exception innerException) : this(message, innerException, null)
		{
		}

		protected OwaPermanentException(string message, Exception innerException, object thisObject) : base(new LocalizedString(message), innerException)
		{
			ExTraceGlobals.ExceptionTracer.TraceDebug((long)((thisObject != null) ? thisObject.GetHashCode() : 0), (message != null) ? message : "<Exception has no message associated>");
		}
	}
}
