using System;
using Microsoft.Exchange.Diagnostics.Components.SenderId;

namespace Microsoft.Exchange.SenderId
{
	internal sealed class RedirectSpfModifier : SpfTerm
	{
		public RedirectSpfModifier(SenderIdValidationContext context, MacroTermSpfNode domainSpec) : base(context)
		{
			this.domainSpec = domainSpec;
		}

		public override void Process()
		{
			ExTraceGlobals.ValidationTracer.TraceDebug((long)this.GetHashCode(), "Processing redirect modifier");
			SpfMacro.BeginExpandDomainSpec(this.context, this.domainSpec, new AsyncCallback(this.ExpandDomainSpecCallback), null);
		}

		private void ExpandDomainSpecCallback(IAsyncResult ar)
		{
			SpfMacro.ExpandedMacro expandedMacro = SpfMacro.EndExpandDomainSpec(ar);
			if (expandedMacro.IsValid)
			{
				string value = expandedMacro.Value;
				ExTraceGlobals.ValidationTracer.TraceDebug<string>((long)this.GetHashCode(), "Using expanded RedirectDomain for SenderId validation: {0}", value);
				this.context.BaseContext.SenderIdValidator.BeginCheckHost(this.context, value, true, new AsyncCallback(this.RedirectCallback), null);
				return;
			}
			this.context.ValidationCompleted(SenderIdStatus.PermError);
		}

		private void RedirectCallback(IAsyncResult ar)
		{
			SenderIdResult senderIdResult = this.context.BaseContext.SenderIdValidator.EndCheckHost(ar);
			ExTraceGlobals.ValidationTracer.TraceDebug<SenderIdStatus>((long)this.GetHashCode(), "Result of redirect: {0}", senderIdResult.Status);
			this.context.ValidationCompleted(senderIdResult);
		}

		private MacroTermSpfNode domainSpec;
	}
}
