using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Data.Directory.GlobalLocatorService
{
	internal class GlsAsyncState
	{
		internal GlsAsyncState(AsyncCallback clientCallback, object clientAsyncState, LocatorService serviceProxy) : this(clientCallback, clientAsyncState, serviceProxy, 0, null, null, null, null, null, true, null)
		{
		}

		internal GlsAsyncState(AsyncCallback clientCallback, object clientAsyncState, LocatorService serviceProxy, int retryCount, GlsLoggerContext loggerContext, IExtensibleDataObject request, string methodName, string glsApiName, object parameterValue, bool isRead, Func<LocatorService, GlsLoggerContext, IAsyncResult> methodToRetry)
		{
			this.clientCallback = clientCallback;
			this.clientAsyncState = clientAsyncState;
			this.serviceProxy = serviceProxy;
			this.retryCount = retryCount;
			this.loggerContext = loggerContext;
			this.request = request;
			this.methodName = methodName;
			this.glsApiName = glsApiName;
			this.parameterValue = parameterValue;
			this.isRead = isRead;
			this.methodToRetry = methodToRetry;
		}

		internal AsyncCallback ClientCallback
		{
			get
			{
				return this.clientCallback;
			}
		}

		internal object ClientAsyncState
		{
			get
			{
				return this.clientAsyncState;
			}
		}

		internal LocatorService ServiceProxy
		{
			get
			{
				return this.serviceProxy;
			}
		}

		internal GlsLoggerContext LoggerContext
		{
			get
			{
				return this.loggerContext;
			}
		}

		internal int RetryCount
		{
			get
			{
				return this.retryCount;
			}
		}

		internal Exception AsyncExeption
		{
			get
			{
				return this.asyncException;
			}
			set
			{
				this.asyncException = value;
			}
		}

		internal IExtensibleDataObject Request
		{
			get
			{
				return this.request;
			}
		}

		internal Func<LocatorService, GlsLoggerContext, IAsyncResult> MethodToRetry
		{
			get
			{
				return this.methodToRetry;
			}
		}

		internal string MethodName
		{
			get
			{
				return this.methodName;
			}
		}

		internal string GlsApiName
		{
			get
			{
				return this.glsApiName;
			}
		}

		internal object ParameterValue
		{
			get
			{
				return this.parameterValue;
			}
		}

		internal bool IsRead
		{
			get
			{
				return this.isRead;
			}
		}

		private readonly AsyncCallback clientCallback;

		private readonly object clientAsyncState;

		private readonly int retryCount;

		private readonly Func<LocatorService, GlsLoggerContext, IAsyncResult> methodToRetry;

		private readonly IExtensibleDataObject request;

		private readonly LocatorService serviceProxy;

		private readonly GlsLoggerContext loggerContext;

		private readonly string methodName;

		private readonly string glsApiName;

		private readonly object parameterValue;

		private readonly bool isRead;

		private Exception asyncException;
	}
}
