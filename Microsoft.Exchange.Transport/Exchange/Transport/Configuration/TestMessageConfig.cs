using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverDelivery;

namespace Microsoft.Exchange.Transport.Configuration
{
	internal class TestMessageConfig
	{
		public TestMessageConfig(EmailMessage message)
		{
			HeaderList headers = message.RootPart.Headers;
			Header header = headers.FindFirst("X-MS-Exchange-Organization-Test-Message-Send-Report-To");
			if (header != null)
			{
				TestMessageConfig.tracer.TraceDebug<string>(0L, "Report to address is {0}", header.Value);
				this.reportToAddress = new SmtpAddress(header.Value ?? string.Empty);
				if (!this.reportToAddress.IsValidAddress)
				{
					this.reportToAddress = SmtpAddress.Empty;
				}
			}
			header = headers.FindFirst("X-MS-Exchange-Organization-Test-Message");
			if (header != null)
			{
				TestMessageConfig.tracer.TraceDebug(0L, "Current message is a test message");
				this.isTestMessage = true;
				if (!string.Equals(header.Value, "Deliver", StringComparison.OrdinalIgnoreCase))
				{
					TestMessageConfig.tracer.TraceDebug(0L, "The test message will not be delivered");
					this.suppressDelivery = true;
				}
			}
			header = headers.FindFirst("X-MS-Exchange-Organization-Test-Message-Log-For");
			if (header != null)
			{
				this.logTypes = this.TryParseEnumValue<LogTypesEnum>(header.Value);
				TestMessageConfig.tracer.TraceDebug<LogTypesEnum>(0L, "Log types requested for test message is {0}", this.logTypes);
			}
			header = headers.FindFirst("X-MS-Exchange-Organization-Test-Message-Options");
			if (header != null)
			{
				this.options = header.Value;
				TestMessageConfig.tracer.TraceDebug<string>(0L, "Options for test message is {0}", this.options);
				this.SetFlagsBasedOnOptions();
			}
		}

		public bool IsTestMessage
		{
			get
			{
				return this.isTestMessage;
			}
		}

		public LogTypesEnum LogTypes
		{
			get
			{
				return this.logTypes;
			}
		}

		public bool SuppressDelivery
		{
			get
			{
				return this.suppressDelivery;
			}
		}

		public SmtpAddress ReportToAddress
		{
			get
			{
				return this.reportToAddress;
			}
		}

		public bool ShouldExecuteDisabledAndInErrorRules { get; private set; }

		private T TryParseEnumValue<T>(string value)
		{
			if (!typeof(T).IsEnum)
			{
				throw new ArgumentException("Type must be enum.");
			}
			if (string.IsNullOrEmpty(value))
			{
				return default(T);
			}
			T result;
			try
			{
				result = (T)((object)Enum.Parse(typeof(T), value));
			}
			catch (ArgumentException)
			{
				result = default(T);
			}
			return result;
		}

		private void SetFlagsBasedOnOptions()
		{
			if (this.isTestMessage && string.Equals("ExecuteDisabledAndInErrorRules", this.options, StringComparison.OrdinalIgnoreCase))
			{
				this.ShouldExecuteDisabledAndInErrorRules = true;
			}
		}

		private static readonly Trace tracer = ExTraceGlobals.MailboxRuleTracer;

		private readonly string options;

		private readonly SmtpAddress reportToAddress;

		private readonly bool suppressDelivery;

		private readonly LogTypesEnum logTypes;

		private readonly bool isTestMessage;
	}
}
