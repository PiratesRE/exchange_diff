using System;
using Microsoft.Exchange.Diagnostics.Components.SenderId;

namespace Microsoft.Exchange.SenderId
{
	internal abstract class SpfMechanism : SpfTerm
	{
		protected SpfMechanism(SenderIdValidationContext context, SenderIdStatus prefix) : base(context)
		{
			this.prefix = prefix;
		}

		protected void SetMatchResult()
		{
			ExTraceGlobals.ValidationTracer.TraceDebug((long)this.GetHashCode(), "Mechanism matched");
			if (this.prefix != SenderIdStatus.Fail)
			{
				this.context.ValidationCompleted(this.prefix);
				return;
			}
			if (this.context.ProcessExpModifier && this.context.Exp != null)
			{
				this.context.Exp.BeginGetExplanation(new AsyncCallback(this.ExplanationCallback), null);
				return;
			}
			this.context.ValidationCompleted(SenderIdStatus.Fail, SenderIdFailReason.NotPermitted, string.Empty);
		}

		private void ExplanationCallback(IAsyncResult ar)
		{
			string explanation = this.context.Exp.EndGetExplanation(ar);
			this.context.ValidationCompleted(SenderIdStatus.Fail, SenderIdFailReason.NotPermitted, explanation);
		}

		private SenderIdStatus prefix;
	}
}
