using System;
using System.Diagnostics;
using System.Globalization;
using System.Security.Principal;
using Microsoft.Exchange.Data.ApplicationLogic.ConnectionSettingsDiscovery;
using Microsoft.Exchange.Data.ApplicationLogic.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class LoggingAdapter : ILogAdapter
	{
		public LoggingAdapter(CallContext callContext, Microsoft.Exchange.Diagnostics.Trace log)
		{
			this.callContext = callContext;
			this.log = log;
			this.stopwatch = new Stopwatch();
		}

		public void Trace(string messageTemplate, params object[] args)
		{
			string text = string.Format(CultureInfo.InvariantCulture, messageTemplate, args);
			this.log.TraceInformation(this.GetHashCode(), 0L, "{0}. Mailbox: {1}. AccessType: {2}. AccessingAs SID: {3}", new object[]
			{
				text,
				this.Mailbox,
				this.callContext.MailboxAccessType,
				this.CallerSid
			});
		}

		public void LogError(string messageTemplate, params object[] args)
		{
			string text = string.Format(CultureInfo.InvariantCulture, messageTemplate, args);
			this.log.TraceError(this.GetHashCode(), 0L, "{0}. Mailbox: {1}. AccessType: {2}. AccessingAs SID: {3}", new object[]
			{
				text,
				this.Mailbox,
				this.callContext.MailboxAccessType,
				this.CallerSid
			});
		}

		public void LogException(Exception exception, string additionalMessage, params object[] args)
		{
			string str = string.Format(CultureInfo.InvariantCulture, additionalMessage, args);
			string str2 = string.Format(CultureInfo.InvariantCulture, "Exception [{0}]: {1}. Stack trace: {2}. ", new object[]
			{
				exception.GetType().Name,
				exception.Message,
				exception.StackTrace
			});
			this.log.TraceError(this.GetHashCode(), 0L, "{0}. Mailbox: {1}. AccessType: {2}. AccessingAs SID: {3}", new object[]
			{
				str + str2,
				this.Mailbox,
				this.callContext.MailboxAccessType,
				this.CallerSid
			});
		}

		public void ExecuteMonitoredOperation(Enum logMetadata, Action operation)
		{
			this.stopwatch.Restart();
			operation();
			this.stopwatch.Stop();
			this.callContext.ProtocolLog.Set(logMetadata, this.stopwatch.ElapsedMilliseconds);
		}

		public void LogOperationResult(Enum logMetadata, string domain, bool succeeded)
		{
			this.callContext.ProtocolLog.Set(logMetadata, succeeded);
			this.callContext.ProtocolLog.Set(ConnectionSettingsDiscoveryMetadata.Domain, domain);
		}

		public void RegisterLogMetaData(string actionName, Type logMetaDataEnumType)
		{
			OwsLogRegistry.Register(actionName, logMetaDataEnumType, new Type[0]);
		}

		public void LogOperationException(Enum logMetadata, Exception ex)
		{
			this.callContext.ProtocolLog.Set(logMetadata, ex.ToString());
		}

		private string Mailbox
		{
			get
			{
				if (this.callContext.AccessingPrincipal == null)
				{
					return null;
				}
				return this.callContext.AccessingPrincipal.MailboxInfo.PrimarySmtpAddress.ToString();
			}
		}

		private SecurityIdentifier CallerSid
		{
			get
			{
				if (this.callContext.AccessingPrincipal == null)
				{
					return null;
				}
				return this.callContext.EffectiveCallerSid;
			}
		}

		private readonly CallContext callContext;

		private readonly Microsoft.Exchange.Diagnostics.Trace log;

		private readonly Stopwatch stopwatch;
	}
}
