using System;
using System.Globalization;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class GalGrammarFile : SearchGrammarFile
	{
		internal GalGrammarFile(CultureInfo culture, string filePath) : base(culture, true)
		{
			this.galGrammarFilePath = filePath;
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
				return this.galGrammarFilePath;
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
			GrammarIdentifier grammarIdentifier = null;
			switch (callContext.CallType)
			{
			case 1:
			{
				UMDialPlan dialPlan = callContext.DialPlan;
				switch (dialPlan.ContactScope)
				{
				case CallSomeoneScopeEnum.DialPlan:
					grammarIdentifier = new GrammarIdentifier(dialPlan.OrganizationId, callContext.Culture, GrammarFileNames.GetFileNameForDialPlan(dialPlan));
					break;
				case CallSomeoneScopeEnum.GlobalAddressList:
					grammarIdentifier = new GrammarIdentifier(dialPlan.OrganizationId, callContext.Culture, GrammarFileNames.GetFileNameForGALUser());
					break;
				case CallSomeoneScopeEnum.AddressList:
					if (dialPlan.ContactAddressList != null)
					{
						grammarIdentifier = new GrammarIdentifier(dialPlan.OrganizationId, callContext.Culture, GrammarFileNames.GetFileNameForCustomAddressList(dialPlan.ContactAddressList));
					}
					else
					{
						grammarIdentifier = new GrammarIdentifier(dialPlan.OrganizationId, callContext.Culture, GrammarFileNames.GetFileNameForGALUser());
					}
					break;
				}
				break;
			}
			case 2:
			{
				UMAutoAttendant autoAttendantInfo = callContext.AutoAttendantInfo;
				switch (autoAttendantInfo.ContactScope)
				{
				case DialScopeEnum.DialPlan:
					grammarIdentifier = new GrammarIdentifier(autoAttendantInfo.OrganizationId, callContext.Culture, GrammarFileNames.GetFileNameForDialPlan(callContext.DialPlan));
					break;
				case DialScopeEnum.GlobalAddressList:
					grammarIdentifier = new GrammarIdentifier(autoAttendantInfo.OrganizationId, callContext.Culture, GrammarFileNames.GetFileNameForGALUser());
					break;
				case DialScopeEnum.AddressList:
					if (autoAttendantInfo.ContactAddressList != null)
					{
						grammarIdentifier = new GrammarIdentifier(autoAttendantInfo.OrganizationId, callContext.Culture, GrammarFileNames.GetFileNameForCustomAddressList(autoAttendantInfo.ContactAddressList));
					}
					else
					{
						grammarIdentifier = new GrammarIdentifier(autoAttendantInfo.OrganizationId, callContext.Culture, GrammarFileNames.GetFileNameForGALUser());
					}
					break;
				}
				break;
			}
			case 3:
			{
				UMSubscriber callerInfo = callContext.CallerInfo;
				ADRecipient adrecipient = callerInfo.ADRecipient;
				ExAssert.RetailAssert(adrecipient != null, "subscriber.ADRecipient = null");
				ExAssert.RetailAssert(adrecipient.OrganizationId != null, "subscriber.ADRecipient.OrganizationId = null");
				if (adrecipient != null && adrecipient.AddressBookPolicy != null && adrecipient.GlobalAddressListFromAddressBookPolicy != null)
				{
					grammarIdentifier = new GrammarIdentifier(adrecipient.OrganizationId, callContext.Culture, GrammarFileNames.GetFileNameForCustomAddressList(adrecipient.GlobalAddressListFromAddressBookPolicy));
				}
				else
				{
					grammarIdentifier = new GrammarIdentifier(adrecipient.OrganizationId, callContext.Culture, GrammarFileNames.GetFileNameForGALUser());
				}
				break;
			}
			default:
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Unhandled CallType {0}", new object[]
				{
					callContext.CallType
				}));
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, null, "GalGrammarFile::GetGrammarIdentifier() - Grammar = '{0}'", new object[]
			{
				grammarIdentifier
			});
			return grammarIdentifier;
		}

		internal static void LogErrorEvent(CallContext callContext)
		{
			if (callContext.CallType == 3)
			{
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_ContactsNoGrammarFileWarning, null, new object[0]);
				return;
			}
			string name = callContext.AutoAttendantInfo.Name;
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_AutoAttendantNoGrammarFileWarning, null, new object[]
			{
				name
			});
		}

		private readonly string galGrammarFilePath;
	}
}
