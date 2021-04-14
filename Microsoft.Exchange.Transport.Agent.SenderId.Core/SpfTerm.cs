using System;
using Microsoft.Exchange.Diagnostics.Components.SenderId;

namespace Microsoft.Exchange.SenderId
{
	internal abstract class SpfTerm
	{
		public SpfTerm(SenderIdValidationContext context)
		{
			this.context = context;
		}

		public SpfTerm Next
		{
			get
			{
				return this.next;
			}
		}

		public virtual SpfTerm Append(SpfTerm newTerm)
		{
			SpfTerm spfTerm = this;
			while (spfTerm.next != null)
			{
				spfTerm = spfTerm.next;
			}
			spfTerm.next = newTerm;
			return newTerm;
		}

		public abstract void Process();

		protected void ProcessNextTerm()
		{
			if (this.next == null)
			{
				throw new InvalidOperationException("Next pointer cannot be null.");
			}
			ExTraceGlobals.ValidationTracer.TraceDebug((long)this.GetHashCode(), "No match, processing next term");
			this.next.Process();
		}

		private SpfTerm next;

		protected SenderIdValidationContext context;
	}
}
