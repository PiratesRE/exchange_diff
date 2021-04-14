using System;
using Microsoft.Exchange.Diagnostics.Components.SenderId;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.SenderId
{
	internal sealed class ExpSpfModifier
	{
		public ExpSpfModifier(SenderIdValidationContext context, MacroTermSpfNode domainSpec)
		{
			this.context = context;
			this.domainSpec = domainSpec;
		}

		public IAsyncResult BeginGetExplanation(AsyncCallback asyncCallback, object asyncState)
		{
			this.asyncResult = new SenderIdAsyncResult(asyncCallback, asyncState);
			SpfMacro.BeginExpandDomainSpec(this.context, this.domainSpec, new AsyncCallback(this.ExpandDomainSpecCallback), null);
			return this.asyncResult;
		}

		private void ExpandDomainSpecCallback(IAsyncResult ar)
		{
			SpfMacro.ExpandedMacro expandedMacro = SpfMacro.EndExpandDomainSpec(ar);
			if (!expandedMacro.IsValid)
			{
				ExTraceGlobals.ValidationTracer.TraceError((long)this.GetHashCode(), "Could not expand DomainSpec for exp modifier");
				this.context.ValidationCompleted(SenderIdStatus.PermError);
				return;
			}
			string value = expandedMacro.Value;
			ExTraceGlobals.ValidationTracer.TraceDebug<string>((long)this.GetHashCode(), "Using expanded domain for TXT lookup: {0}", value);
			if (!Util.AsyncDns.IsValidName(value))
			{
				this.asyncResult.InvokeCompleted(string.Empty);
				return;
			}
			Util.AsyncDns.BeginTxtRecordQuery(value, new AsyncCallback(this.TxtCallback), null);
		}

		private void TxtCallback(IAsyncResult ar)
		{
			string[] array;
			if (Util.AsyncDns.EndTxtRecordQuery(ar, out array) == DnsStatus.Success && array.Length == 1 && array[0].Length > 0)
			{
				SpfMacro.BeginExpandExp(this.context, array[0], new AsyncCallback(this.ExpandExpCallback), null);
				return;
			}
			this.asyncResult.InvokeCompleted(string.Empty);
		}

		private void ExpandExpCallback(IAsyncResult ar)
		{
			SpfMacro.ExpandedMacro expandedMacro = SpfMacro.EndExpandExp(ar);
			if (expandedMacro.IsValid)
			{
				ExTraceGlobals.ValidationTracer.TraceDebug<string>((long)this.GetHashCode(), "Using expanded EXP string: {0}", expandedMacro.Value);
				this.asyncResult.InvokeCompleted(expandedMacro.Value);
				return;
			}
			ExTraceGlobals.ValidationTracer.TraceError((long)this.GetHashCode(), "Could not expand EXP string");
			this.asyncResult.InvokeCompleted(string.Empty);
		}

		public string EndGetExplanation(IAsyncResult ar)
		{
			return (string)((SenderIdAsyncResult)ar).GetResult();
		}

		private MacroTermSpfNode domainSpec;

		private SenderIdValidationContext context;

		private SenderIdAsyncResult asyncResult;
	}
}
