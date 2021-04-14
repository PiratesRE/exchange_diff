using System;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class DialPlanGrammar : DirectoryGrammar
	{
		public DialPlanGrammar(Guid dialPlanGuid)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, this, "DialPlanGrammar constructor - dialPlanGuid='{0}'", new object[]
			{
				dialPlanGuid
			});
			this.dialPlanGuid = dialPlanGuid;
			this.fileName = dialPlanGuid.ToString();
		}

		public override string FileName
		{
			get
			{
				return this.fileName;
			}
		}

		protected override bool ShouldAcceptGrammarEntry(ADEntry entry)
		{
			return entry.DialPlanGuid == this.dialPlanGuid;
		}

		private readonly Guid dialPlanGuid;

		private readonly string fileName;
	}
}
