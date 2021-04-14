using System;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class AddressListGrammar : DirectoryGrammar
	{
		public AddressListGrammar(Guid addressListGuid)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, this, "AddressListGrammar constructor - addressListGuid='{0}'", new object[]
			{
				addressListGuid
			});
			this.addressListGuid = addressListGuid;
			this.fileName = addressListGuid.ToString();
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
			return entry.AddressListGuids.Contains(this.addressListGuid);
		}

		private readonly Guid addressListGuid;

		private readonly string fileName;
	}
}
