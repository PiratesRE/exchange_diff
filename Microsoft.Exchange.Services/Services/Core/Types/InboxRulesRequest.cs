using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.Core.Types
{
	public abstract class InboxRulesRequest : BaseRequest
	{
		[XmlElement(Order = 0)]
		public string MailboxSmtpAddress { get; set; }

		protected InboxRulesRequest(Trace tracer, bool isWriteOperation)
		{
			this.tracer = tracer;
			this.isWriteOperation = isWriteOperation;
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			this.tracer.TraceDebug((long)this.GetHashCode(), "InboxRulesRequest.GetProxyInfo(callContext) called;");
			BaseServerIdInfo result;
			try
			{
				if (callContext is ExternalCallContext)
				{
					result = null;
				}
				else if (!string.IsNullOrEmpty(this.MailboxSmtpAddress))
				{
					result = MailboxIdServerInfo.Create(this.MailboxSmtpAddress);
				}
				else if (callContext.AccessingPrincipal == null)
				{
					result = null;
				}
				else
				{
					result = callContext.GetServerInfoForEffectiveCaller();
				}
			}
			finally
			{
				this.tracer.TraceDebug((long)this.GetHashCode(), "InboxRulesRequest.GetProxyInfo(callContext) call finished;");
			}
			return result;
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return base.GetResourceKeysFromProxyInfo(this.isWriteOperation, callContext);
		}

		private readonly bool isWriteOperation;

		private Trace tracer;
	}
}
