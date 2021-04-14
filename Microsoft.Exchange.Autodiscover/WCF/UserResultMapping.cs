using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics.Components.Autodiscover;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	internal class UserResultMapping
	{
		internal UserResultMapping(string mailbox, CallContext callContext)
		{
			ExTraceGlobals.FrameworkTracer.TraceDebug<string>((long)this.GetHashCode(), "UserResultMapping constructor called for '{0}'.", mailbox);
			this.mailbox = mailbox;
			this.isValidSmtpAddress = SmtpAddress.IsValidSmtpAddress(mailbox);
			this.smtpAddress = (this.isValidSmtpAddress ? new SmtpAddress(mailbox) : SmtpAddress.Empty);
			this.smtpProxyAddress = (this.isValidSmtpAddress ? new SmtpProxyAddress(mailbox, true) : null);
			this.callContext = callContext;
		}

		internal string Mailbox
		{
			get
			{
				return this.mailbox;
			}
		}

		internal bool IsValidSmtpAddress
		{
			get
			{
				return this.isValidSmtpAddress;
			}
		}

		internal SmtpAddress SmtpAddress
		{
			get
			{
				return this.smtpAddress;
			}
		}

		internal SmtpProxyAddress SmtpProxyAddress
		{
			get
			{
				return this.smtpProxyAddress;
			}
		}

		internal CallContext CallContext
		{
			get
			{
				return this.callContext;
			}
		}

		internal ResultBase Result { get; set; }

		private string mailbox;

		private bool isValidSmtpAddress;

		private SmtpAddress smtpAddress;

		private SmtpProxyAddress smtpProxyAddress;

		private CallContext callContext;
	}
}
