using System;
using Microsoft.Exchange.Diagnostics.Components.SenderId;

namespace Microsoft.Exchange.SenderId
{
	internal abstract class SpfMechanismWithDomainSpec : SpfMechanism
	{
		public SpfMechanismWithDomainSpec(SenderIdValidationContext context, SenderIdStatus prefix, MacroTermSpfNode domainSpec) : base(context, prefix)
		{
			this.domainSpec = domainSpec;
		}

		public override void Process()
		{
			if (this.domainSpec != null)
			{
				SpfMacro.BeginExpandDomainSpec(this.context, this.domainSpec, new AsyncCallback(this.ExpandDomainSpecCallback), null);
				return;
			}
			this.ProcessWithExpandedDomainSpec(this.context.PurportedResponsibleDomain);
		}

		private void ExpandDomainSpecCallback(IAsyncResult ar)
		{
			SpfMacro.ExpandedMacro expandedMacro = SpfMacro.EndExpandDomainSpec(ar);
			if (expandedMacro.IsValid)
			{
				this.ProcessWithExpandedDomainSpec(expandedMacro.Value);
				return;
			}
			ExTraceGlobals.ValidationTracer.TraceError((long)this.GetHashCode(), "Domain spec could not be expanded");
			this.context.ValidationCompleted(SenderIdStatus.PermError);
		}

		protected abstract void ProcessWithExpandedDomainSpec(string expandedDomain);

		protected MacroTermSpfNode domainSpec;
	}
}
