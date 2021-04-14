using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	internal class ExtensionListEvaluator : IRuleEvaluator
	{
		public ExtensionListEvaluator(IList<string> paaExtensions)
		{
			this.paaExtensions = paaExtensions;
		}

		public bool Evaluate(IDataLoader dataLoader)
		{
			if (this.paaExtensions == null || this.paaExtensions.Count == 0)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "ExtensionListEvaluator:Evaluate() no extensions defined. Returning true", new object[0]);
				return true;
			}
			string diversionForCall = dataLoader.GetDiversionForCall();
			PhoneNumber phoneNumber = null;
			if (!PhoneNumber.TryParse(diversionForCall, out phoneNumber))
			{
				CallIdTracer.TraceError(ExTraceGlobals.PersonalAutoAttendantTracer, this, "ExtensionListEvaluator:Evaluate() Diversion '{0}' failed to parse as a PhoneNumber. Condition will fail evaluation", new object[]
				{
					diversionForCall
				});
				return false;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "ExtensionListEvaluator:Evaluate() Diversion={0} #extension conditions = {1}", new object[]
			{
				diversionForCall,
				this.paaExtensions.Count
			});
			for (int i = 0; i < this.paaExtensions.Count; i++)
			{
				string text = this.paaExtensions[i];
				PhoneNumber phoneNumber2 = null;
				if (!PhoneNumber.TryParse(text, out phoneNumber2))
				{
					CallIdTracer.TraceError(ExTraceGlobals.PersonalAutoAttendantTracer, this, "ExtensionListEvaluator:Evaluate() Primary Extension '{0}' failed to parse as a PhoneNumber. Continuing evaluation of other extensions", new object[]
					{
						text
					});
				}
				else
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "ExtensionListEvaluator:Evaluate() Diversion={0}[Parsed = {1}] conditionValue = {2}[Parsed = {3}]", new object[]
					{
						diversionForCall,
						phoneNumber.Number,
						text,
						phoneNumber2.Number
					});
					if (string.Compare(phoneNumber2.Number, phoneNumber.Number, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		private IList<string> paaExtensions;
	}
}
