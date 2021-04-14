using System;
using System.Web;

namespace Microsoft.Exchange.Configuration.Core
{
	internal sealed class RequestMonitorContext
	{
		internal RequestMonitorContext(Guid requestId)
		{
			this[RequestMonitorMetadata.StartTime] = DateTime.UtcNow;
			this[RequestMonitorMetadata.RequestId] = requestId;
			RequestMonitorContext.Current = this;
		}

		internal static RequestMonitorContext Current
		{
			get
			{
				HttpContext httpContext = HttpContext.Current;
				if (httpContext == null)
				{
					return null;
				}
				return (RequestMonitorContext)httpContext.Items["X-RequestMonitor-Item-Key"];
			}
			set
			{
				HttpContext httpContext = HttpContext.Current;
				if (httpContext != null)
				{
					httpContext.Items["X-RequestMonitor-Item-Key"] = value;
				}
			}
		}

		internal object[] Fields
		{
			get
			{
				return this.fields;
			}
		}

		internal object this[RequestMonitorMetadata key]
		{
			get
			{
				return this.fields[(int)key];
			}
			set
			{
				this.fields[(int)key] = value;
			}
		}

		internal object this[int index]
		{
			get
			{
				return this.fields[index];
			}
			set
			{
				this.fields[index] = value;
			}
		}

		private const string HttpContextItemKey = "X-RequestMonitor-Item-Key";

		private static readonly int Length = Enum.GetNames(typeof(RequestMonitorMetadata)).Length;

		private object[] fields = new object[RequestMonitorContext.Length];
	}
}
