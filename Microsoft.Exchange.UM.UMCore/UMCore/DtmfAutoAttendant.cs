using System;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class DtmfAutoAttendant : AutoAttendantCore
	{
		internal DtmfAutoAttendant(IAutoAttendantUI autoAttendantManager, BaseUMCallSession voiceObject) : base(autoAttendantManager, voiceObject)
		{
		}

		internal override void Configure()
		{
			base.Configure();
			base.WriteVariable("directorySearchEnabled", base.Config.NameLookupEnabled);
			if (base.CustomizedMenuConfigured)
			{
				foreach (CustomMenuKeyMapping customMenuKeyMapping in base.CustomMenu)
				{
					if (customMenuKeyMapping.MappedKey == CustomMenuKey.Timeout)
					{
						base.WriteVariable(string.Format(CultureInfo.InvariantCulture, "nameOfDepartment{0}", new object[]
						{
							"TimeOut"
						}), customMenuKeyMapping.Description);
					}
					else
					{
						int mappedKey = (int)customMenuKeyMapping.MappedKey;
						base.WriteVariable(string.Format(CultureInfo.InvariantCulture, "nameOfDepartment{0}", new object[]
						{
							mappedKey
						}), customMenuKeyMapping.Description);
					}
				}
			}
		}

		internal override bool ExecuteAction(string action, BaseUMCallSession voiceObject, ref string autoEvent)
		{
			bool result;
			if (string.Compare(action, "setExtensionNumber", true, CultureInfo.InvariantCulture) == 0)
			{
				result = true;
				autoEvent = this.Action_SetExtensionNumber();
			}
			else
			{
				if (string.Compare(action, "processResult", true, CultureInfo.InvariantCulture) != 0)
				{
					return base.ExecuteAction(action, voiceObject, ref autoEvent);
				}
				result = true;
				autoEvent = this.Action_ProcessResult();
			}
			return result;
		}

		internal string Action_ProcessResult()
		{
			ADRecipient adrecipient = (ADRecipient)base.ReadVariable("directorySearchResult");
			string result = AnonCallerUtils.ProcessResult(base.VoiceObject, (ActivityManager)base.AutoAttendantManager, adrecipient);
			ActivityManager activityManager = (ActivityManager)base.AutoAttendantManager;
			bool flag = false;
			ActivityManager activityManager2 = activityManager;
			string variableName = "userName";
			string recipientName;
			if ((recipientName = adrecipient.DisplayName) == null)
			{
				recipientName = (adrecipient.Alias ?? string.Empty);
			}
			activityManager2.SetRecordedName(variableName, recipientName, adrecipient, false, Util.GetDisambiguationField(base.CallContext), ref flag);
			return result;
		}

		private string Action_SetExtensionNumber()
		{
			string dtmfDigits = ((ActivityManager)base.AutoAttendantManager).DtmfDigits;
			string result = null;
			if (Constants.RegularExpressions.ValidNumberRegex.IsMatch(dtmfDigits) && dtmfDigits.Length >= base.CallContext.DialPlan.NumberOfDigitsInExtension)
			{
				RecipientLookup recipientLookup = RecipientLookup.Create(base.VoiceObject);
				ADRecipient adrecipient = recipientLookup.LookupByExtension(dtmfDigits, base.VoiceObject, DirectorySearchPurpose.Both, base.Config.ContactScope);
				if (adrecipient == null)
				{
					result = "noResultsMatched";
				}
				else
				{
					base.WriteVariable("directorySearchResult", adrecipient);
				}
			}
			else
			{
				base.WriteVariable("invalidExtension", dtmfDigits);
				result = "invalidOption";
			}
			base.PerfCounters.Increment(DirectoryAccessCountersUtil.DirectoryAccessCounter.DirectoryAccessedByExtension);
			return result;
		}
	}
}
