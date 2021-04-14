using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	public class ValidationHelper
	{
		public static LocalizedException ValidateE164Entries(ADConfigurationObject dataObject, MultiValuedProperty<string> e164NumberList)
		{
			LocalizedException result = null;
			if (e164NumberList == null || e164NumberList.Count == 0)
			{
				return null;
			}
			Dictionary<string, int> dictionary = new Dictionary<string, int>(e164NumberList.Count);
			for (int i = 0; i < e164NumberList.Count; i++)
			{
				string text = e164NumberList[i];
				if (!Utils.IsUriValid(text, UMUriType.E164))
				{
					result = new InvalidUMPilotIdentifierListEntry(text);
					break;
				}
				if (dictionary.ContainsKey(text))
				{
					result = new DuplicateE164PilotIdentifierListEntryException(dataObject.Name);
					break;
				}
				dictionary[text] = 1;
			}
			return result;
		}

		public static LocalizedException ValidateDialedNumbers(MultiValuedProperty<string> dialedNumbers, UMDialPlan dialPlan)
		{
			int numberOfDigitsInExtension = dialPlan.NumberOfDigitsInExtension;
			UMUriType uritype = dialPlan.URIType;
			LocalizedException ex = null;
			foreach (string text in dialedNumbers)
			{
				switch (uritype)
				{
				case UMUriType.TelExtn:
					try
					{
						ValidationHelper.ValidateExtension(UMAutoAttendantSchema.PilotIdentifierList.ToString(), text, numberOfDigitsInExtension);
					}
					catch (InvalidPilotIdentiferException ex2)
					{
						ex = ex2;
					}
					catch (NumericArgumentLengthInvalidException ex3)
					{
						ex = ex3;
					}
					break;
				case UMUriType.E164:
					if (!Utils.IsUriValid(text, UMUriType.E164))
					{
						ex = new InvalidPilotIdentiferException(Strings.ErrorUMInvalidE164AddressFormat(text));
					}
					break;
				case UMUriType.SipName:
					ex = ValidationHelper.ValidateE164Entries(dialPlan, dialedNumbers);
					break;
				}
				if (ex != null)
				{
					break;
				}
			}
			return ex;
		}

		public static void ValidateExtension(string property, string extension, int numberOfDigitsInExtension)
		{
			if (!ValidationHelper.numberRegex.IsMatch(extension))
			{
				throw new InvalidPilotIdentiferException(Strings.ErrorUMInvalidExtensionFormat(extension));
			}
			if (extension.Length < numberOfDigitsInExtension)
			{
				throw new NumericArgumentLengthInvalidException(extension, property, numberOfDigitsInExtension);
			}
		}

		public static void ValidateTimeZone(string timeZoneKeyName)
		{
			ExTimeZone exTimeZone = null;
			if (!ExTimeZoneEnumerator.Instance.TryGetTimeZoneByName(timeZoneKeyName, out exTimeZone))
			{
				throw new InvalidTimeZoneException(timeZoneKeyName);
			}
		}

		internal static void ValidateCustomMenu(LocalizedString setting, IConfigurationSession session, string property, MultiValuedProperty<CustomMenuKeyMapping> customMenu, int numberOfDigitsInExtension, UMAutoAttendant containingAutoAttendant, DataAccessHelper.GetDataObjectDelegate getUniqueObject, out bool serializeAgain)
		{
			serializeAgain = false;
			new List<string>();
			IRecipientSession recipientSessionScopedToOrganization = Utility.GetRecipientSessionScopedToOrganization(containingAutoAttendant.OrganizationId, true);
			foreach (CustomMenuKeyMapping customMenuKeyMapping in customMenu)
			{
				if (!string.IsNullOrEmpty(customMenuKeyMapping.AutoAttendantName))
				{
					ValidationHelper.ValidateLinkedAutoAttendant(session, customMenuKeyMapping.AutoAttendantName, containingAutoAttendant.Status == StatusEnum.Enabled, containingAutoAttendant);
				}
				string text = Utils.TrimSpaces(customMenuKeyMapping.PromptFileName);
				if (text != null)
				{
					ValidationHelper.ValidateWavFile(text);
				}
				if (!string.IsNullOrEmpty(customMenuKeyMapping.LeaveVoicemailFor))
				{
					string legacyDNToUseForLeaveVoicemailFor;
					ValidationHelper.ValidateMailbox(setting, customMenuKeyMapping.LeaveVoicemailFor, containingAutoAttendant.UMDialPlan, recipientSessionScopedToOrganization, getUniqueObject, out legacyDNToUseForLeaveVoicemailFor);
					customMenuKeyMapping.LegacyDNToUseForLeaveVoicemailFor = legacyDNToUseForLeaveVoicemailFor;
					serializeAgain = true;
				}
				if (!string.IsNullOrEmpty(customMenuKeyMapping.TransferToMailbox))
				{
					string legacyDNToUseForTransferToMailbox;
					ValidationHelper.ValidateMailbox(setting, customMenuKeyMapping.TransferToMailbox, containingAutoAttendant.UMDialPlan, recipientSessionScopedToOrganization, getUniqueObject, out legacyDNToUseForTransferToMailbox);
					customMenuKeyMapping.LegacyDNToUseForTransferToMailbox = legacyDNToUseForTransferToMailbox;
					serializeAgain = true;
				}
			}
			CustomMenuKeyMapping[] array = customMenu.ToArray();
			Dictionary<string, int> dictionary = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
			Dictionary<CustomMenuKey, bool> dictionary2 = new Dictionary<CustomMenuKey, bool>();
			for (int i = 0; i < array.Length; i++)
			{
				CustomMenuKeyMapping customMenuKeyMapping2 = array[i];
				try
				{
					dictionary.Add(customMenuKeyMapping2.Description, i);
				}
				catch (ArgumentException)
				{
					throw new InvalidCustomMenuException(Strings.DuplicateMenuName(customMenuKeyMapping2.Description));
				}
				if (!string.IsNullOrEmpty(customMenuKeyMapping2.AsrPhrases) && customMenuKeyMapping2.AsrPhrases.Length > 256)
				{
					throw new InvalidCustomMenuException(Strings.MaxAsrPhraseLengthExceeded(customMenuKeyMapping2.Description));
				}
				string[] asrPhraseList = customMenuKeyMapping2.AsrPhraseList;
				if (asrPhraseList != null)
				{
					if (asrPhraseList.Length > 9)
					{
						throw new InvalidCustomMenuException(Strings.MaxAsrPhraseCountExceeded(customMenuKeyMapping2.Description));
					}
					for (int j = 0; j < asrPhraseList.Length; j++)
					{
						if (string.IsNullOrEmpty(asrPhraseList[j]))
						{
							throw new InvalidCustomMenuException(Strings.EmptyASRPhrase(customMenuKeyMapping2.Description));
						}
						try
						{
							dictionary.Add(asrPhraseList[j], -1);
						}
						catch (ArgumentException)
						{
							if (dictionary[asrPhraseList[j]] != i)
							{
								throw new InvalidCustomMenuException(Strings.DuplicateASRPhrase(asrPhraseList[j]));
							}
						}
					}
				}
				try
				{
					if (customMenuKeyMapping2.MappedKey != CustomMenuKey.NotSpecified)
					{
						dictionary2.Add(customMenuKeyMapping2.MappedKey, true);
					}
				}
				catch (ArgumentException)
				{
					throw new InvalidCustomMenuException(Strings.DuplicateKeys(customMenuKeyMapping2.Key));
				}
			}
		}

		public static void ValidateWavFile(string file)
		{
			string text = Utils.TrimSpaces(file);
			if (text == null)
			{
				throw new ArgumentNullException("file");
			}
			string extension = Path.GetExtension(text);
			if (!string.Equals(extension, ".wav", StringComparison.OrdinalIgnoreCase) && !string.Equals(extension, ".wma", StringComparison.OrdinalIgnoreCase))
			{
				throw new InvalidParameterException(Strings.InvalidAAFileExtension(text));
			}
		}

		public static void ValidateLinkedAutoAttendant(IConfigDataProvider session, string autoAttendantName, bool checkEnabled, UMAutoAttendant referringAutoAttendant)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (autoAttendantName == null)
			{
				throw new ArgumentNullException("autoAttendantName");
			}
			if (referringAutoAttendant == null)
			{
				throw new ArgumentNullException("referringAutoAttendant");
			}
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, autoAttendantName),
				new ComparisonFilter(ComparisonOperator.Equal, UMAutoAttendantSchema.UMDialPlan, referringAutoAttendant.UMDialPlan)
			});
			UMAutoAttendant umautoAttendant = null;
			UMAutoAttendant[] array = (UMAutoAttendant[])session.Find<UMAutoAttendant>(filter, referringAutoAttendant.Id.Parent, false, null);
			if (array != null && array.Length == 1)
			{
				umautoAttendant = array[0];
			}
			if (umautoAttendant == null)
			{
				throw new InvalidAutoAttendantException(Strings.InvalidAutoAttendantInDialPlan(autoAttendantName, referringAutoAttendant.UMDialPlan.Name));
			}
			if (checkEnabled && umautoAttendant.Status != StatusEnum.Enabled)
			{
				throw new InvalidAutoAttendantException(Strings.DisabledLinkedAutoAttendant(autoAttendantName, referringAutoAttendant.Id.ToString()));
			}
		}

		public static bool IsFallbackAAInDialPlan(IConfigDataProvider dataSession, UMAutoAttendant dataObject, out ADObjectId targetAA)
		{
			bool result = false;
			targetAA = null;
			IEnumerable<UMAutoAttendant> autoAttendantsInSameDialPlan = ValidationHelper.GetAutoAttendantsInSameDialPlan(dataSession, dataObject, dataObject.UMDialPlan);
			if (autoAttendantsInSameDialPlan == null)
			{
				return false;
			}
			foreach (UMAutoAttendant umautoAttendant in autoAttendantsInSameDialPlan)
			{
				if (!umautoAttendant.Id.ObjectGuid.Equals(dataObject.Id.ObjectGuid) && umautoAttendant.DTMFFallbackAutoAttendant != null && umautoAttendant.DTMFFallbackAutoAttendant.ObjectGuid.Equals(dataObject.Guid))
				{
					result = true;
					targetAA = umautoAttendant.Id;
					break;
				}
			}
			return result;
		}

		public static void ValidateDtmfFallbackAA(UMAutoAttendant dataObject, UMDialPlan aaDialPlan, UMAutoAttendant dtmfFallbackAA)
		{
			if (dtmfFallbackAA.Guid.Equals(dataObject.Guid))
			{
				throw new InvalidDtmfFallbackAutoAttendantException(Strings.InvalidDtmfFallbackAutoAttendantSelf(dtmfFallbackAA.Identity.ToString()));
			}
			if (!dtmfFallbackAA.UMDialPlan.ObjectGuid.Equals(aaDialPlan.Guid))
			{
				throw new InvalidDtmfFallbackAutoAttendantException(Strings.InvalidDtmfFallbackAutoAttendantDialPlan(dtmfFallbackAA.Identity.ToString()));
			}
			if (dtmfFallbackAA.SpeechEnabled)
			{
				throw new InvalidDtmfFallbackAutoAttendantException(Strings.InvalidDtmfFallbackAutoAttendant(dtmfFallbackAA.Identity.ToString()));
			}
			if (dataObject.SpeechEnabled && dataObject.Status == StatusEnum.Enabled && dtmfFallbackAA.Status == StatusEnum.Disabled)
			{
				throw new InvalidDtmfFallbackAutoAttendantException(Strings.InvalidDtmfFallbackAutoAttendant_Disabled(dtmfFallbackAA.Identity.ToString()));
			}
		}

		public static bool IsKnownException(Exception exception)
		{
			if (exception == null)
			{
				throw new ArgumentNullException("exception");
			}
			return exception is NumericArgumentLengthInvalidException || exception is InvalidPilotIdentiferException || exception is InvalidTimeZoneException || exception is InvalidCustomMenuException || exception is InvalidParameterException || exception is InvalidAutoAttendantException || exception is InvalidDtmfFallbackAutoAttendantException || exception is DefaultAutoAttendantInDialPlanException;
		}

		internal static void ValidateDisabledAA(IConfigDataProvider dataSession, UMDialPlan dialPlanConfig, UMAutoAttendant disabledAutoAttendant)
		{
			if (dialPlanConfig.ContactScope == CallSomeoneScopeEnum.AutoAttendantLink)
			{
				ADObjectId umautoAttendant = dialPlanConfig.UMAutoAttendant;
				if (umautoAttendant != null && umautoAttendant.ObjectGuid.Equals(disabledAutoAttendant.Guid))
				{
					throw new DefaultAutoAttendantInDialPlanException(dialPlanConfig.Id.ToString());
				}
			}
			ValidationHelper.CheckLinkWithOtherAAsInDialPlan(dataSession, disabledAutoAttendant);
		}

		private static void CheckLinkWithOtherAAsInDialPlan(IConfigDataProvider dataSession, UMAutoAttendant dataObject)
		{
			IEnumerable<UMAutoAttendant> autoAttendantsInSameDialPlan = ValidationHelper.GetAutoAttendantsInSameDialPlan(dataSession, dataObject, dataObject.UMDialPlan);
			if (autoAttendantsInSameDialPlan == null)
			{
				return;
			}
			foreach (UMAutoAttendant umautoAttendant in autoAttendantsInSameDialPlan)
			{
				if (umautoAttendant != null && !umautoAttendant.Guid.Equals(dataObject.Guid))
				{
					if (umautoAttendant.DTMFFallbackAutoAttendant != null && umautoAttendant.DTMFFallbackAutoAttendant.ObjectGuid.Equals(dataObject.Guid))
					{
						throw new InvalidDtmfFallbackAutoAttendantException(Strings.CannotDisableAutoAttendant(umautoAttendant.Id.ToString()));
					}
					if (umautoAttendant.BusinessHoursKeyMapping != null && umautoAttendant.BusinessHoursKeyMapping.Count > 0)
					{
						foreach (CustomMenuKeyMapping customMenuKeyMapping in umautoAttendant.BusinessHoursKeyMapping)
						{
							if (!string.IsNullOrEmpty(customMenuKeyMapping.AutoAttendantName) && string.Equals(dataObject.Name, customMenuKeyMapping.AutoAttendantName, StringComparison.OrdinalIgnoreCase))
							{
								throw new InvalidDtmfFallbackAutoAttendantException(Strings.CannotDisableAutoAttendant_KeyMapping(umautoAttendant.Id.ToString()));
							}
						}
					}
					if (umautoAttendant.AfterHoursKeyMapping != null && umautoAttendant.AfterHoursKeyMapping.Count > 0)
					{
						foreach (CustomMenuKeyMapping customMenuKeyMapping2 in umautoAttendant.AfterHoursKeyMapping)
						{
							if (!string.IsNullOrEmpty(customMenuKeyMapping2.AutoAttendantName) && string.Equals(dataObject.Name, customMenuKeyMapping2.AutoAttendantName, StringComparison.OrdinalIgnoreCase))
							{
								throw new InvalidDtmfFallbackAutoAttendantException(Strings.CannotDisableAutoAttendant_KeyMapping(umautoAttendant.Id.ToString()));
							}
						}
					}
				}
			}
		}

		private static IEnumerable<UMAutoAttendant> GetAutoAttendantsInSameDialPlan(IConfigDataProvider session, UMAutoAttendant autoAttendant, ADObjectId dialPlanId)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (autoAttendant == null)
			{
				throw new ArgumentNullException("autoAttendant");
			}
			if (dialPlanId == null)
			{
				throw new ArgumentNullException("dialPlanId");
			}
			ComparisonFilter filter = new ComparisonFilter(ComparisonOperator.Equal, UMAutoAttendantSchema.UMDialPlan, dialPlanId);
			ADObjectId parent = autoAttendant.Id.Parent;
			return session.FindPaged<UMAutoAttendant>(filter, parent, false, null, 0);
		}

		private static void ValidateMailbox(LocalizedString setting, string mailbox, ADObjectId dialPlanId, IRecipientSession recipSession, DataAccessHelper.GetDataObjectDelegate getUniqueObject, out string legacyDN)
		{
			legacyDN = null;
			ADRecipient adrecipient = (ADRecipient)getUniqueObject(new MailboxIdParameter(mailbox), recipSession, null, null, new LocalizedString?(Strings.InvalidMailbox(mailbox, setting)), new LocalizedString?(Strings.InvalidMailbox(mailbox, setting)));
			ADUser aduser = adrecipient as ADUser;
			if (!Utils.IsUserUMEnabledInGivenDialplan(aduser, dialPlanId))
			{
				throw new InvalidCustomMenuException(Strings.InvalidMailbox(mailbox, setting));
			}
			legacyDN = aduser.LegacyExchangeDN;
		}

		private static Regex numberRegex = new Regex("^\\d+$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
	}
}
