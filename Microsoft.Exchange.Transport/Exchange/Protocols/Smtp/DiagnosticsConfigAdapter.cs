using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class DiagnosticsConfigAdapter : IDiagnosticsConfigProvider
	{
		public TimeSpan SmtpRecvLogAsyncInterval
		{
			get
			{
				return this.appConfig.Logging.SmtpRecvLogAsyncInterval;
			}
		}

		public int SmtpRecvLogBufferSize
		{
			get
			{
				return this.appConfig.Logging.SmtpRecvLogBufferSize;
			}
		}

		public TimeSpan SmtpRecvLogFlushInterval
		{
			get
			{
				return this.appConfig.Logging.SmtpRecvLogFlushInterval;
			}
		}

		public static IDiagnosticsConfigProvider Create(ITransportAppConfig appConfig)
		{
			ArgumentValidator.ThrowIfNull("appConfig", appConfig);
			return new DiagnosticsConfigAdapter(appConfig);
		}

		private DiagnosticsConfigAdapter(ITransportAppConfig appConfig)
		{
			this.appConfig = appConfig;
		}

		private readonly ITransportAppConfig appConfig;
	}
}
