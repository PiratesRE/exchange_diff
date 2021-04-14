using System;
using System.Globalization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class DistributionListGrammarFile : SearchGrammarFile
	{
		internal DistributionListGrammarFile(CultureInfo culture, string filePath) : base(culture, true)
		{
			this.filePath = filePath;
		}

		public override Uri BaseUri
		{
			get
			{
				return new Uri(Utils.GrammarPathFromCulture(base.Culture));
			}
		}

		internal override string FilePath
		{
			get
			{
				return this.filePath;
			}
		}

		internal override bool HasEntries
		{
			get
			{
				return true;
			}
		}

		internal static GrammarIdentifier GetGrammarIdentifier(CallContext callContext)
		{
			ExAssert.RetailAssert(callContext.CallType == 3, "Invalid call type = {0}", new object[]
			{
				callContext.CallType.ToString()
			});
			UMSubscriber callerInfo = callContext.CallerInfo;
			ADRecipient adrecipient = callerInfo.ADRecipient;
			ExAssert.RetailAssert(adrecipient != null, "subscriber.ADRecipient = null");
			ExAssert.RetailAssert(adrecipient.OrganizationId != null, "subscriber.ADRecipient.OrganizationId = null");
			ADObjectId organizationalUnit = adrecipient.OrganizationId.OrganizationalUnit;
			PIIMessage data = PIIMessage.Create(PIIType._User, adrecipient);
			CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, null, data, "DistributionListGrammarFile::GetGrammarIdentifier() - User = '_User', OU = '{0}'", new object[]
			{
				(organizationalUnit != null) ? organizationalUnit.ToString() : "<null>"
			});
			return new GrammarIdentifier(adrecipient.OrganizationId, callContext.Culture, GrammarFileNames.GetFileNameForDL());
		}

		internal static void LogErrorEvent()
		{
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_ContactsNoGrammarFileWarning, null, new object[0]);
		}

		private readonly string filePath;
	}
}
