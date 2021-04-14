using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.UM.PersonalAutoAttendant;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UMCallAnsweringRuleDataProvider : XsoMailboxDataProviderBase
	{
		public UMCallAnsweringRuleDataProvider(ADSessionSettings adSessionSettings, ADUser mailboxOwner, string action) : base(adSessionSettings, mailboxOwner, action)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.umSubscriber = new UMSubscriber(mailboxOwner, base.MailboxSession);
				this.paaStore = PAAStore.Create(this.umSubscriber);
				disposeGuard.Success();
			}
		}

		protected override IEnumerable<T> InternalFindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize)
		{
			UMCallAnsweringRuleId caRuleId = rootId as UMCallAnsweringRuleId;
			if (sortBy != null)
			{
				throw new NotSupportedException("sortBy");
			}
			if (rootId != null && caRuleId == null)
			{
				throw new NotSupportedException("rootId");
			}
			IList<PersonalAutoAttendant> list = this.paaStore.GetAutoAttendants(PAAValidationMode.None);
			if (caRuleId == null || caRuleId.RuleIdGuid == Guid.Empty)
			{
				for (int i = 0; i < list.Count; i++)
				{
					yield return (T)((object)this.ConvertPersonalAutoAttendantToCallAnsweringRule(list[i], i));
				}
			}
			else
			{
				bool found = false;
				for (int j = 0; j < list.Count; j++)
				{
					if (list[j].Identity == caRuleId.RuleIdGuid)
					{
						found = true;
						yield return (T)((object)this.ConvertPersonalAutoAttendantToCallAnsweringRule(list[j], j));
						break;
					}
				}
				if (!found)
				{
					throw new DataSourceOperationException(Strings.RuleNotFound(caRuleId.ToString()));
				}
			}
			yield break;
		}

		protected override void InternalSave(ConfigurableObject instance)
		{
			UMCallAnsweringRule umcallAnsweringRule = instance as UMCallAnsweringRule;
			if (umcallAnsweringRule == null)
			{
				throw new NotSupportedException("Save: " + instance.GetType().FullName);
			}
			PersonalAutoAttendant personalAutoAttendant = null;
			switch (umcallAnsweringRule.ObjectState)
			{
			case ObjectState.New:
				personalAutoAttendant = this.CreateNewPAAFromCallAnsweringRule(umcallAnsweringRule);
				this.ConvertCallAnsweringRuleToPersonalAutoAttendant(umcallAnsweringRule, personalAutoAttendant);
				break;
			case ObjectState.Unchanged:
				return;
			case ObjectState.Changed:
				if (!this.paaStore.TryGetAutoAttendant(((UMCallAnsweringRuleId)umcallAnsweringRule.Identity).RuleIdGuid, PAAValidationMode.None, out personalAutoAttendant))
				{
					throw new DataSourceOperationException(Strings.RuleNotFound(umcallAnsweringRule.Identity.ToString()));
				}
				this.ConvertCallAnsweringRuleToPersonalAutoAttendant(umcallAnsweringRule, personalAutoAttendant);
				break;
			case ObjectState.Deleted:
				throw new InvalidOperationException(ServerStrings.ExceptionObjectHasBeenDeleted);
			}
			if (personalAutoAttendant != null)
			{
				IList<PersonalAutoAttendant> autoAttendants = this.paaStore.GetAutoAttendants(PAAValidationMode.None);
				if (autoAttendants.Contains(personalAutoAttendant))
				{
					autoAttendants.Remove(personalAutoAttendant);
				}
				int num = umcallAnsweringRule.Priority - 1;
				if (autoAttendants.Count > 0 && num <= autoAttendants.Count - 1)
				{
					autoAttendants.Insert(num, personalAutoAttendant);
					umcallAnsweringRule.Priority = num + 1;
				}
				else
				{
					autoAttendants.Add(personalAutoAttendant);
					umcallAnsweringRule.Priority = autoAttendants.Count;
				}
				this.paaStore.Save(autoAttendants);
			}
		}

		protected override void InternalDelete(ConfigurableObject instance)
		{
			UMCallAnsweringRule umcallAnsweringRule = instance as UMCallAnsweringRule;
			if (umcallAnsweringRule == null)
			{
				throw new NotSupportedException("Delete: " + instance.GetType().FullName);
			}
			UMCallAnsweringRuleId umcallAnsweringRuleId = (UMCallAnsweringRuleId)umcallAnsweringRule.Identity;
			this.paaStore.DeleteAutoAttendant(umcallAnsweringRuleId.RuleIdGuid);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<UMCallAnsweringRuleDataProvider>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.umSubscriber != null)
				{
					this.umSubscriber.Dispose();
					this.umSubscriber = null;
				}
				if (this.paaStore != null)
				{
					this.paaStore.Dispose();
					this.paaStore = null;
				}
			}
			base.InternalDispose(disposing);
		}

		private PersonalAutoAttendant CreateNewPAAFromCallAnsweringRule(UMCallAnsweringRule caRule)
		{
			PersonalAutoAttendant personalAutoAttendant = PersonalAutoAttendant.CreateUninitialized();
			personalAutoAttendant.Identity = Guid.NewGuid();
			personalAutoAttendant.Valid = true;
			personalAutoAttendant.Version = PAAConstants.CurrentVersion;
			personalAutoAttendant.Enabled = caRule.Enabled;
			personalAutoAttendant.EnableBargeIn = caRule.CallersCanInterruptGreeting;
			caRule.propertyBag.SetField(SimpleProviderObjectSchema.Identity, new UMCallAnsweringRuleId(this.umSubscriber.ADRecipient.Id, personalAutoAttendant.Identity));
			return personalAutoAttendant;
		}

		private UMCallAnsweringRule ConvertPersonalAutoAttendantToCallAnsweringRule(PersonalAutoAttendant paa, int priority)
		{
			UMCallAnsweringRule umcallAnsweringRule = new UMCallAnsweringRule(new UMCallAnsweringRuleId(this.umSubscriber.ADUser.Id, paa.Identity));
			umcallAnsweringRule.Enabled = paa.Enabled;
			umcallAnsweringRule.Name = paa.Name;
			umcallAnsweringRule.Priority = priority + 1;
			umcallAnsweringRule.CallersCanInterruptGreeting = paa.EnableBargeIn;
			foreach (UMCallAnsweringRuleDataProvider.MappingEntry mappingEntry in UMCallAnsweringRuleDataProvider.MappingEntry.MappingEntries)
			{
				mappingEntry.AddPAAPropertyToCallAnsweringRule(umcallAnsweringRule, paa);
			}
			this.BuildRuleDescription(umcallAnsweringRule);
			return umcallAnsweringRule;
		}

		private void ConvertCallAnsweringRuleToPersonalAutoAttendant(UMCallAnsweringRule caRule, PersonalAutoAttendant paa)
		{
			this.CopyPropertiesToPAA(caRule, paa);
			this.CopyActionsAndConditionsToPAA(caRule, paa);
		}

		private void CopyPropertiesToPAA(UMCallAnsweringRule caRule, PersonalAutoAttendant paa)
		{
			if (caRule.propertyBag.IsChanged(UMCallAnsweringRuleSchema.CallersCanInterruptGreeting))
			{
				paa.EnableBargeIn = caRule.CallersCanInterruptGreeting;
			}
			if (caRule.propertyBag.IsChanged(UMCallAnsweringRuleSchema.Enabled))
			{
				paa.Enabled = caRule.Enabled;
			}
			if (caRule.propertyBag.IsChanged(UMCallAnsweringRuleSchema.Name))
			{
				paa.Name = caRule.Name;
			}
		}

		private void CopyActionsAndConditionsToPAA(UMCallAnsweringRule caRule, PersonalAutoAttendant paa)
		{
			foreach (UMCallAnsweringRuleDataProvider.MappingEntry mappingEntry in UMCallAnsweringRuleDataProvider.MappingEntry.MappingEntries)
			{
				if (caRule.propertyBag.IsChanged(mappingEntry.PropertyDefinition))
				{
					mappingEntry.AddCallAnsweringRulePropertyToPAA(caRule, paa);
				}
			}
		}

		private void BuildRuleDescription(UMCallAnsweringRule caRule)
		{
			RuleDescription ruleDescription = new UMCallAnsweringRuleDescription();
			foreach (UMCallAnsweringRuleDataProvider.MappingEntry mappingEntry in UMCallAnsweringRuleDataProvider.MappingEntry.MappingEntries)
			{
				switch (mappingEntry.MappingEntryType)
				{
				case UMCallAnsweringRuleDataProvider.MappingEntryType.Condition:
				{
					string text = mappingEntry.GetDescriptionString(caRule, this.paaStore);
					if (!string.IsNullOrEmpty(text))
					{
						ruleDescription.ConditionDescriptions.Add(text);
					}
					break;
				}
				case UMCallAnsweringRuleDataProvider.MappingEntryType.Action:
				{
					string text = mappingEntry.GetDescriptionString(caRule, this.paaStore);
					if (!string.IsNullOrEmpty(text))
					{
						ruleDescription.ActionDescriptions.Add(text);
					}
					break;
				}
				}
			}
			caRule.Description = ruleDescription;
		}

		public void ValidateUMCallAnsweringRuleProperties(UMCallAnsweringRule callAnsweringRule, Task.TaskErrorLoggingDelegate errorLogger)
		{
			PAAValidationResult result;
			string data;
			if (callAnsweringRule.IsModified(UMCallAnsweringRuleSchema.ExtensionsDialed) && callAnsweringRule.ExtensionsDialed != null && !this.paaStore.ValidateExtensions(callAnsweringRule.ExtensionsDialed, out result, out data))
			{
				errorLogger(new LocalizedException(this.GetStringForValidationModeError(result, data)), ErrorCategory.InvalidArgument, callAnsweringRule);
			}
			if (callAnsweringRule.IsModified(UMCallAnsweringRuleSchema.CallerIds) && callAnsweringRule.CallerIds != null)
			{
				this.ValidateCallerIds(callAnsweringRule.CallerIds, errorLogger);
			}
			if (callAnsweringRule.IsModified(UMCallAnsweringRuleSchema.KeyMappings) && callAnsweringRule.KeyMappings != null)
			{
				this.ValidateKeyMappings(callAnsweringRule.KeyMappings, errorLogger);
			}
			if (callAnsweringRule.IsModified(UMCallAnsweringRuleSchema.TimeOfDay) && callAnsweringRule.TimeOfDay != null)
			{
				this.ValidateTimeOfDay(callAnsweringRule.TimeOfDay, errorLogger);
			}
		}

		private void ValidateTimeOfDay(TimeOfDay timeOfDay, Task.TaskErrorLoggingDelegate errorLogger)
		{
			try
			{
				timeOfDay.Validate();
			}
			catch (FormatException exception)
			{
				errorLogger(exception, ErrorCategory.InvalidArgument, null);
			}
		}

		private void ValidateCallerIds(IList<CallerIdItem> callerIds, Task.TaskErrorLoggingDelegate errorLogger)
		{
			try
			{
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				foreach (CallerIdItem callerIdItem in callerIds)
				{
					callerIdItem.Validate();
					switch (callerIdItem.CallerIdType)
					{
					case CallerIdItemType.PhoneNumber:
					{
						num++;
						IDataValidationResult dataValidationResult;
						if (!this.paaStore.ValidatePhoneNumberCallerId(callerIdItem.PhoneNumber, out dataValidationResult))
						{
							errorLogger(new LocalizedException(this.GetStringForValidationModeError(dataValidationResult.PAAValidationResult, callerIdItem.PhoneNumber)), ErrorCategory.InvalidArgument, null);
						}
						break;
					}
					case CallerIdItemType.GALContact:
					{
						num3++;
						IDataValidationResult dataValidationResult;
						if (!this.paaStore.ValidateADContactCallerId(callerIdItem.GALContactLegacyDN, out dataValidationResult))
						{
							errorLogger(new LocalizedException(this.GetStringForValidationModeError(dataValidationResult.PAAValidationResult, callerIdItem.GALContactLegacyDN)), ErrorCategory.InvalidArgument, null);
						}
						break;
					}
					case CallerIdItemType.PersonalContact:
					{
						num3++;
						StoreObjectId storeId = StoreObjectId.Deserialize(callerIdItem.PersonalContactStoreId);
						IDataValidationResult dataValidationResult;
						if (!this.paaStore.ValidateContactItemCallerId(storeId, out dataValidationResult))
						{
							errorLogger(new LocalizedException(this.GetStringForValidationModeError(dataValidationResult.PAAValidationResult, callerIdItem.PersonalContactStoreId)), ErrorCategory.InvalidArgument, null);
						}
						break;
					}
					case CallerIdItemType.DefaultContactsFolder:
					{
						num2++;
						IDataValidationResult dataValidationResult;
						if (!this.paaStore.ValidateContactFolderCallerId(out dataValidationResult))
						{
							errorLogger(new LocalizedException(this.GetStringForValidationModeError(dataValidationResult.PAAValidationResult, null)), ErrorCategory.InvalidArgument, null);
						}
						break;
					}
					case CallerIdItemType.PersonaContact:
					{
						num3++;
						IDataValidationResult dataValidationResult;
						if (!this.paaStore.ValidatePersonaContactCallerId(callerIdItem.PersonaEmailAddress, out dataValidationResult))
						{
							errorLogger(new LocalizedException(this.GetStringForValidationModeError(dataValidationResult.PAAValidationResult, callerIdItem.PersonaEmailAddress)), ErrorCategory.InvalidArgument, null);
						}
						break;
					}
					default:
						throw new InvalidOperationException("Unknown enum type");
					}
					if (num3 > 50 || num2 > 1 || num > 50)
					{
						errorLogger(new LocalizedException(Strings.ErrorMaxNumberOfCallerIds(50, 50, 1)), ErrorCategory.InvalidArgument, null);
					}
				}
			}
			catch (FormatException exception)
			{
				errorLogger(exception, ErrorCategory.InvalidOperation, null);
			}
			catch (LocalizedException exception2)
			{
				errorLogger(exception2, ErrorCategory.InvalidOperation, null);
			}
		}

		private void ValidateKeyMappings(IList<KeyMapping> keyMappings, Task.TaskErrorLoggingDelegate errorLogger)
		{
			if (keyMappings.Count > 10)
			{
				errorLogger(new LocalizedException(Strings.ErrorMaxKeyMappings(10)), ErrorCategory.InvalidOperation, null);
			}
			try
			{
				Dictionary<int, bool> dictionary = new Dictionary<int, bool>();
				bool flag = false;
				foreach (KeyMapping keyMapping in keyMappings)
				{
					keyMapping.Validate();
					switch (keyMapping.KeyMappingType)
					{
					case KeyMappingType.TransferToNumber:
					{
						IDataValidationResult dataValidationResult;
						if (!this.paaStore.ValidatePhoneNumberForOutdialing(keyMapping.TransferToNumber, out dataValidationResult))
						{
							errorLogger(new LocalizedException(this.GetStringForValidationModeError(dataValidationResult.PAAValidationResult, keyMapping.TransferToNumber)), ErrorCategory.InvalidArgument, null);
						}
						break;
					}
					case KeyMappingType.TransferToGALContact:
					{
						IDataValidationResult dataValidationResult;
						if (!this.paaStore.ValidateADContactForOutdialing(keyMapping.TransferToGALContactLegacyDN, out dataValidationResult))
						{
							errorLogger(new LocalizedException(this.GetStringForValidationModeError(dataValidationResult.PAAValidationResult, keyMapping.TransferToGALContactLegacyDN)), ErrorCategory.InvalidArgument, null);
						}
						break;
					}
					case KeyMappingType.TransferToGALContactVoiceMail:
					{
						IDataValidationResult dataValidationResult;
						if (!this.paaStore.ValidateADContactForTransferToMailbox(keyMapping.TransferToGALContactLegacyDN, out dataValidationResult))
						{
							errorLogger(new LocalizedException(this.GetStringForValidationModeError(dataValidationResult.PAAValidationResult, keyMapping.TransferToGALContactLegacyDN)), ErrorCategory.InvalidArgument, null);
						}
						break;
					}
					case KeyMappingType.VoiceMail:
						break;
					case KeyMappingType.FindMe:
					{
						IDataValidationResult dataValidationResult;
						if (!this.paaStore.ValidatePhoneNumberForOutdialing(keyMapping.FindMeFirstNumber, out dataValidationResult))
						{
							errorLogger(new LocalizedException(this.GetStringForValidationModeError(dataValidationResult.PAAValidationResult, keyMapping.FindMeFirstNumber)), ErrorCategory.InvalidArgument, null);
						}
						if (!string.IsNullOrEmpty(keyMapping.FindMeSecondNumber) && !this.paaStore.ValidatePhoneNumberForOutdialing(keyMapping.FindMeSecondNumber, out dataValidationResult))
						{
							errorLogger(new LocalizedException(this.GetStringForValidationModeError(dataValidationResult.PAAValidationResult, keyMapping.FindMeSecondNumber)), ErrorCategory.InvalidArgument, null);
						}
						break;
					}
					default:
						throw new InvalidOperationException("Unknown enum value.");
					}
					if (!dictionary.TryGetValue(keyMapping.Key, out flag))
					{
						dictionary.Add(keyMapping.Key, true);
					}
					else
					{
						errorLogger(new LocalizedException(Strings.ErrorDuplicateKey(keyMapping.Key)), ErrorCategory.InvalidOperation, null);
					}
				}
			}
			catch (FormatException exception)
			{
				errorLogger(exception, ErrorCategory.InvalidOperation, null);
			}
		}

		private LocalizedString GetStringForValidationModeError(PAAValidationResult result, string data)
		{
			LocalizedString result2;
			switch (result)
			{
			case PAAValidationResult.ParseError:
				result2 = Strings.ErrorPhoneNumberIsNotValid(data);
				break;
			case PAAValidationResult.SipUriInNonSipDialPlan:
				result2 = Strings.ErrorSipUriInNonSipDialPlan(data);
				break;
			case PAAValidationResult.PermissionCheckFailure:
				result2 = Strings.ErrorPermissionCheckFailure(data);
				break;
			case PAAValidationResult.NonExistentContact:
				result2 = Strings.ErrorNonExistentContact(data);
				break;
			case PAAValidationResult.NoValidPhones:
				result2 = Strings.ErrorNoValidPhones(data);
				break;
			case PAAValidationResult.NonExistentDefaultContactsFolder:
				result2 = Strings.ErrorNonExistentDefaultContactsFolder;
				break;
			case PAAValidationResult.NonExistentDirectoryUser:
				result2 = Strings.ErrorNonExistentDirectoryUser(data);
				break;
			case PAAValidationResult.NonMailboxDirectoryUser:
				result2 = Strings.ErrorNonMailboxDirectoryUser(data);
				break;
			case PAAValidationResult.NonExistentPersona:
				result2 = Strings.ErrorNonExistentDirectoryUser(data);
				break;
			case PAAValidationResult.InvalidExtension:
				result2 = Strings.ErrorInvalidExtension(data);
				break;
			default:
				result2 = LocalizedString.Empty;
				break;
			}
			return result2;
		}

		private UMSubscriber umSubscriber;

		private IPAAStore paaStore;

		public enum MappingEntryType
		{
			Condition,
			Action
		}

		public delegate string GetDescriptionString(UMCallAnsweringRule callAnsweringRule, IPAAStore store);

		public delegate void AddCallAnsweringRulePropertyToPAA(UMCallAnsweringRule callAnsweringRule, PersonalAutoAttendant paa);

		public delegate void AddPAAPropertyToCallAnsweringRule(UMCallAnsweringRule callAnsweringRule, PersonalAutoAttendant paa);

		internal class MappingEntry
		{
			public ProviderPropertyDefinition PropertyDefinition { get; private set; }

			public UMCallAnsweringRuleDataProvider.AddCallAnsweringRulePropertyToPAA AddCallAnsweringRulePropertyToPAA { get; private set; }

			public UMCallAnsweringRuleDataProvider.AddPAAPropertyToCallAnsweringRule AddPAAPropertyToCallAnsweringRule { get; private set; }

			public UMCallAnsweringRuleDataProvider.GetDescriptionString GetDescriptionString { get; private set; }

			public UMCallAnsweringRuleDataProvider.MappingEntryType MappingEntryType { get; private set; }

			public MappingEntry(UMCallAnsweringRuleDataProvider.MappingEntryType mappingEntryType, ProviderPropertyDefinition propertyDefinition, UMCallAnsweringRuleDataProvider.AddCallAnsweringRulePropertyToPAA addCallAnsweringRulePropertyToPAA, UMCallAnsweringRuleDataProvider.AddPAAPropertyToCallAnsweringRule addPAAPropertyToCallAnsweringRule, UMCallAnsweringRuleDataProvider.GetDescriptionString getDescriptionString)
			{
				ValidateArgument.NotNull(propertyDefinition, "propertyDefinition");
				ValidateArgument.NotNull(addCallAnsweringRulePropertyToPAA, "addCallAnsweringRulePropertyToPAA");
				ValidateArgument.NotNull(addPAAPropertyToCallAnsweringRule, "addPAAPropertyToCallAnsweringRule");
				ValidateArgument.NotNull(getDescriptionString, "GetDescriptionString");
				this.MappingEntryType = mappingEntryType;
				this.PropertyDefinition = propertyDefinition;
				this.AddCallAnsweringRulePropertyToPAA = addCallAnsweringRulePropertyToPAA;
				this.AddPAAPropertyToCallAnsweringRule = addPAAPropertyToCallAnsweringRule;
				this.GetDescriptionString = getDescriptionString;
			}

			private static string GetStringFromSetBits(uint unsignedVal, int maxBitsToScan, string[] bitStringValues)
			{
				List<string> list = new List<string>();
				for (int i = 0; i < maxBitsToScan; i++)
				{
					uint num = unsignedVal & 1U;
					unsignedVal >>= 1;
					if (num != 0U)
					{
						list.Add(bitStringValues[i]);
					}
				}
				return RuleDescription.BuildDescriptionStringFromStringArray(list, Strings.InboxRuleDescriptionOr, 200);
			}

			private static void MapCallerIdsCallAnsweringRuleToPAA(UMCallAnsweringRule callAnsweringRule, PersonalAutoAttendant paa)
			{
				paa.CallerIdList.Clear();
				if (callAnsweringRule.CallerIds != null)
				{
					foreach (CallerIdItem callerIdItem in callAnsweringRule.CallerIds)
					{
						switch (callerIdItem.CallerIdType)
						{
						case CallerIdItemType.PhoneNumber:
							paa.AddPhoneNumberCallerId(callerIdItem.PhoneNumber);
							break;
						case CallerIdItemType.GALContact:
							paa.AddADContactCallerId(callerIdItem.GALContactLegacyDN);
							break;
						case CallerIdItemType.PersonalContact:
							paa.AddContactItemCallerId(callerIdItem.PersonalContactStoreId);
							break;
						case CallerIdItemType.DefaultContactsFolder:
							paa.AddDefaultContactFolderCallerId();
							break;
						case CallerIdItemType.PersonaContact:
							paa.AddPersonaContactCallerId(new EmailAddress
							{
								Name = callerIdItem.DisplayName,
								Address = callerIdItem.PersonaEmailAddress
							});
							break;
						}
					}
				}
			}

			private static void MapCallerIdsPAAToCallAnsweringRule(UMCallAnsweringRule callAnsweringRule, PersonalAutoAttendant paa)
			{
				if (paa.CallerIdList != null && paa.CallerIdList.Count > 0)
				{
					callAnsweringRule.CallerIds = new MultiValuedProperty<CallerIdItem>();
					foreach (CallerIdBase callerIdBase in paa.CallerIdList)
					{
						switch (callerIdBase.CallerIdType)
						{
						case CallerIdTypeEnum.Number:
						{
							string phoneNumberString = ((PhoneNumberCallerId)callerIdBase).PhoneNumberString;
							callAnsweringRule.CallerIds.Add(new CallerIdItem(CallerIdItemType.PhoneNumber, phoneNumberString, null, null, null, null));
							break;
						}
						case CallerIdTypeEnum.ContactItem:
						{
							ContactItemCallerId contactItemCallerId = callerIdBase as ContactItemCallerId;
							callAnsweringRule.CallerIds.Add(new CallerIdItem(CallerIdItemType.PersonalContact, null, null, contactItemCallerId.StoreObjectId.ToBase64String(), null, null));
							break;
						}
						case CallerIdTypeEnum.DefaultContactFolder:
							callAnsweringRule.CallerIds.Add(new CallerIdItem(CallerIdItemType.DefaultContactsFolder, null, null, null, null, null));
							break;
						case CallerIdTypeEnum.ADContact:
						{
							string legacyExchangeDN = ((ADContactCallerId)callerIdBase).LegacyExchangeDN;
							callAnsweringRule.CallerIds.Add(new CallerIdItem(CallerIdItemType.GALContact, null, legacyExchangeDN, null, null, null));
							break;
						}
						case CallerIdTypeEnum.PersonaContact:
						{
							PersonaContactCallerId personaContactCallerId = (PersonaContactCallerId)callerIdBase;
							callAnsweringRule.CallerIds.Add(new CallerIdItem(CallerIdItemType.PersonaContact, null, null, null, personaContactCallerId.EmailAddress.Address, personaContactCallerId.EmailAddress.Name));
							break;
						}
						}
					}
				}
			}

			private static string GetDescriptionStringForCallerIds(UMCallAnsweringRule callAnsweringRule, IPAAStore store)
			{
				string result = string.Empty;
				if (callAnsweringRule.CallerIds != null && callAnsweringRule.CallerIds.Count > 0)
				{
					List<string> list = new List<string>();
					foreach (CallerIdItem callerIdItem in callAnsweringRule.CallerIds)
					{
						switch (callerIdItem.CallerIdType)
						{
						case CallerIdItemType.PhoneNumber:
						{
							IDataValidationResult dataValidationResult;
							if (!store.ValidatePhoneNumberCallerId(callerIdItem.PhoneNumber, out dataValidationResult))
							{
								list.Add(Strings.UMCallAnsweringRuleInvalidValue);
								callAnsweringRule.InError = true;
							}
							else
							{
								list.Add(callerIdItem.PhoneNumber);
							}
							break;
						}
						case CallerIdItemType.GALContact:
						{
							callAnsweringRule.InError = true;
							IDataValidationResult dataValidationResult;
							if (!store.ValidateADContactCallerId(callerIdItem.GALContactLegacyDN, out dataValidationResult))
							{
								list.Add(Strings.UMCallAnsweringRuleInvalidValue);
							}
							else
							{
								list.Add(string.IsNullOrEmpty(dataValidationResult.ADRecipient.DisplayName) ? Strings.UMCallAnsweringRuleUnknownContact : dataValidationResult.ADRecipient.DisplayName);
								callerIdItem.DisplayName = dataValidationResult.ADRecipient.DisplayName;
							}
							break;
						}
						case CallerIdItemType.PersonalContact:
						{
							callAnsweringRule.InError = true;
							IDataValidationResult dataValidationResult;
							if (!store.ValidateContactItemCallerId(StoreObjectId.Deserialize(callerIdItem.PersonalContactStoreId), out dataValidationResult))
							{
								list.Add(Strings.UMCallAnsweringRuleInvalidValue);
							}
							else
							{
								PersonalContactInfo personalContactInfo = dataValidationResult.PersonalContactInfo;
								list.Add(string.IsNullOrEmpty(personalContactInfo.DisplayName) ? Strings.UMCallAnsweringRuleUnknownContact : personalContactInfo.DisplayName);
								callerIdItem.DisplayName = personalContactInfo.DisplayName;
							}
							break;
						}
						case CallerIdItemType.DefaultContactsFolder:
							list.Add(Strings.CallerIsInMyDefaultContactsFolder);
							break;
						case CallerIdItemType.PersonaContact:
						{
							IDataValidationResult dataValidationResult;
							if (!store.ValidatePersonaContactCallerId(callerIdItem.PersonaEmailAddress, out dataValidationResult))
							{
								list.Add(Strings.UMCallAnsweringRuleInvalidValue);
								callAnsweringRule.InError = true;
							}
							else
							{
								callerIdItem.DisplayName = dataValidationResult.PersonaContactInfo.DisplayName;
								list.Add(string.IsNullOrEmpty(callerIdItem.DisplayName) ? Strings.UMCallAnsweringRuleUnknownContact : callerIdItem.DisplayName);
							}
							break;
						}
						}
					}
					result = Strings.UMCallAnsweringRuleCallerIdCondition(RuleDescription.BuildDescriptionStringFromStringArray(list, Strings.InboxRuleDescriptionOr, 200));
				}
				return result;
			}

			private static void MapCheckAutomaticRepliesCallAnsweringRuleToPAA(UMCallAnsweringRule callAnsweringRule, PersonalAutoAttendant paa)
			{
				if (callAnsweringRule.CheckAutomaticReplies)
				{
					paa.OutOfOffice = OutOfOfficeStatusEnum.Oof;
					return;
				}
				paa.OutOfOffice = OutOfOfficeStatusEnum.NotOof;
			}

			private static void MapCheckAutomaticRepliesPAAToCallAnsweringRule(UMCallAnsweringRule callAnsweringRule, PersonalAutoAttendant paa)
			{
				if (paa.OutOfOffice == OutOfOfficeStatusEnum.Oof)
				{
					callAnsweringRule.CheckAutomaticReplies = true;
					return;
				}
				callAnsweringRule.CheckAutomaticReplies = false;
			}

			private static string GetDescriptionStringForCheckAutomaticReplies(UMCallAnsweringRule callAnsweringRule, IPAAStore store)
			{
				string result = string.Empty;
				if (callAnsweringRule.CheckAutomaticReplies)
				{
					result = Strings.UMCallAnsweringRuleCheckAutomaticReplies;
				}
				return result;
			}

			private static void MapExtensionsDialedCallAnsweringRuleToPAA(UMCallAnsweringRule callAnsweringRule, PersonalAutoAttendant paa)
			{
				ExtensionList extensionList = new ExtensionList();
				if (callAnsweringRule.ExtensionsDialed != null)
				{
					foreach (string item in callAnsweringRule.ExtensionsDialed)
					{
						extensionList.Add(item);
					}
				}
				paa.ExtensionList = extensionList;
			}

			private static void MapExtensionsDialedPAAToCallAnsweringRule(UMCallAnsweringRule callAnsweringRule, PersonalAutoAttendant paa)
			{
				if (paa.ExtensionList != null && paa.ExtensionList.Count > 0)
				{
					callAnsweringRule.ExtensionsDialed = new MultiValuedProperty<string>(paa.ExtensionList);
				}
			}

			private static string GetDescriptionStringForExtensionsDialed(UMCallAnsweringRule callAnsweringRule, IPAAStore store)
			{
				string result = string.Empty;
				if (callAnsweringRule.ExtensionsDialed != null && callAnsweringRule.ExtensionsDialed.Count > 0)
				{
					PAAValidationResult paavalidationResult;
					string text;
					store.ValidateExtensions(callAnsweringRule.ExtensionsDialed, out paavalidationResult, out text);
					List<string> list = new List<string>();
					foreach (string text2 in callAnsweringRule.ExtensionsDialed)
					{
						if (!string.IsNullOrEmpty(text) && string.Equals(text, text2))
						{
							list.Add(Strings.UMCallAnsweringRuleInvalidValue);
							callAnsweringRule.InError = true;
						}
						else
						{
							list.Add(text2);
						}
					}
					result = Strings.UMCallAnsweringRuleExtension(RuleDescription.BuildDescriptionStringFromStringArray(list, Strings.InboxRuleDescriptionOr, 200));
				}
				return result;
			}

			private static void MapKeyMappingsCallAnsweringRuleToPAA(UMCallAnsweringRule callAnsweringRule, PersonalAutoAttendant paa)
			{
				KeyMappings keyMappings = new KeyMappings();
				if (callAnsweringRule.KeyMappings != null)
				{
					foreach (KeyMapping keyMapping in callAnsweringRule.KeyMappings)
					{
						switch (keyMapping.KeyMappingType)
						{
						case KeyMappingType.TransferToNumber:
							keyMappings.AddTransferToNumber(keyMapping.Key, keyMapping.Context, keyMapping.TransferToNumber);
							break;
						case KeyMappingType.TransferToGALContact:
							keyMappings.AddTransferToADContactPhone(keyMapping.Key, keyMapping.Context, keyMapping.TransferToGALContactLegacyDN);
							break;
						case KeyMappingType.TransferToGALContactVoiceMail:
							keyMappings.AddTransferToADContactMailbox(keyMapping.Key, keyMapping.Context, keyMapping.TransferToGALContactLegacyDN);
							break;
						case KeyMappingType.VoiceMail:
							keyMappings.AddTransferToVoicemail(keyMapping.Context);
							break;
						case KeyMappingType.FindMe:
							keyMappings.AddFindMe(keyMapping.Key, keyMapping.Context, keyMapping.FindMeFirstNumber, keyMapping.FindMeFirstNumberDuration);
							if (!string.IsNullOrEmpty(keyMapping.FindMeSecondNumber))
							{
								keyMappings.AddFindMe(keyMapping.Key, keyMapping.Context, keyMapping.FindMeSecondNumber, keyMapping.FindMeSecondNumberDuration);
							}
							break;
						}
					}
				}
				paa.KeyMappingList = keyMappings;
			}

			private static void MapKeyMappingsPAAToCallAnsweringRule(UMCallAnsweringRule callAnsweringRule, PersonalAutoAttendant paa)
			{
				if (paa.KeyMappingList != null && paa.KeyMappingList.Count > 0)
				{
					callAnsweringRule.KeyMappings = new MultiValuedProperty<KeyMapping>();
					foreach (KeyMappingBase keyMappingBase in paa.KeyMappingList.Menu)
					{
						switch (keyMappingBase.KeyMappingType)
						{
						case KeyMappingTypeEnum.TransferToNumber:
						{
							string phoneNumberString = ((TransferToNumber)keyMappingBase).PhoneNumberString;
							callAnsweringRule.KeyMappings.Add(new KeyMapping(KeyMappingType.TransferToNumber, keyMappingBase.Key, keyMappingBase.Context, null, 0, null, 0, phoneNumberString, null));
							break;
						}
						case KeyMappingTypeEnum.TransferToADContactMailbox:
						{
							string legacyExchangeDN = ((TransferToADContactMailbox)keyMappingBase).LegacyExchangeDN;
							callAnsweringRule.KeyMappings.Add(new KeyMapping(KeyMappingType.TransferToGALContactVoiceMail, keyMappingBase.Key, keyMappingBase.Context, null, 0, null, 0, null, legacyExchangeDN));
							break;
						}
						case KeyMappingTypeEnum.TransferToADContactPhone:
						{
							string legacyExchangeDN = ((TransferToADContact)keyMappingBase).LegacyExchangeDN;
							callAnsweringRule.KeyMappings.Add(new KeyMapping(KeyMappingType.TransferToGALContact, keyMappingBase.Key, keyMappingBase.Context, null, 0, null, 0, null, legacyExchangeDN));
							break;
						}
						case KeyMappingTypeEnum.TransferToVoicemail:
							callAnsweringRule.KeyMappings.Add(new KeyMapping(KeyMappingType.VoiceMail, keyMappingBase.Key, keyMappingBase.Context, null, 0, null, 0, null, null));
							break;
						case KeyMappingTypeEnum.FindMe:
						{
							FindMeNumbers numbers = ((TransferToFindMe)keyMappingBase).Numbers;
							KeyMapping keyMapping = new KeyMapping(KeyMappingType.FindMe, keyMappingBase.Key, keyMappingBase.Context, numbers[0].Number, numbers[0].Timeout, null, 0, null, null);
							if (numbers.Count > 1)
							{
								keyMapping.FindMeSecondNumber = numbers[1].Number;
								keyMapping.FindMeSecondNumberDuration = numbers[1].Timeout;
							}
							callAnsweringRule.KeyMappings.Add(keyMapping);
							break;
						}
						}
					}
				}
			}

			private static string GetDescriptionStringForKeyMappings(UMCallAnsweringRule callAnsweringRule, IPAAStore store)
			{
				string result = string.Empty;
				if (callAnsweringRule.KeyMappings != null && callAnsweringRule.KeyMappings.Count > 0)
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append(Strings.UMCallAnsweringRuleProvideTheCallerWithTheseOptions);
					stringBuilder.Append("\r\n\t");
					int num = callAnsweringRule.KeyMappings.Count - 1;
					for (int i = 0; i < callAnsweringRule.KeyMappings.Count; i++)
					{
						KeyMapping keyMapping = callAnsweringRule.KeyMappings[i];
						switch (keyMapping.KeyMappingType)
						{
						case KeyMappingType.TransferToNumber:
						{
							string number = UMCallAnsweringRuleDataProvider.MappingEntry.ValidatePhoneNumberForOutDialing(store, keyMapping.TransferToNumber, callAnsweringRule);
							if (!string.IsNullOrEmpty(keyMapping.Context))
							{
								stringBuilder.Append(Strings.UMCallAnsweringRuleTransferToWithContext(keyMapping.Context, keyMapping.Key, number));
							}
							else
							{
								stringBuilder.Append(Strings.UMCallAnsweringRuleTransferToWithoutContext(keyMapping.Key, number));
							}
							break;
						}
						case KeyMappingType.TransferToGALContact:
						{
							IDataValidationResult dataValidationResult;
							string text;
							if (!store.ValidateADContactForOutdialing(keyMapping.TransferToGALContactLegacyDN, out dataValidationResult))
							{
								text = Strings.UMCallAnsweringRuleInvalidValue;
								callAnsweringRule.InError = true;
							}
							else
							{
								text = (string.IsNullOrEmpty(dataValidationResult.ADRecipient.DisplayName) ? Strings.UMCallAnsweringRuleUnknownContact : dataValidationResult.ADRecipient.DisplayName);
							}
							if (!string.IsNullOrEmpty(keyMapping.Context))
							{
								stringBuilder.Append(Strings.UMCallAnsweringRuleTransferToWithContext(keyMapping.Context, keyMapping.Key, text));
							}
							else
							{
								stringBuilder.Append(Strings.UMCallAnsweringRuleTransferToWithoutContext(keyMapping.Key, text));
							}
							break;
						}
						case KeyMappingType.TransferToGALContactVoiceMail:
						{
							IDataValidationResult dataValidationResult;
							string text;
							if (!store.ValidateADContactForTransferToMailbox(keyMapping.TransferToGALContactLegacyDN, out dataValidationResult))
							{
								text = Strings.UMCallAnsweringRuleInvalidValue;
								callAnsweringRule.InError = true;
							}
							else
							{
								text = (string.IsNullOrEmpty(dataValidationResult.ADRecipient.DisplayName) ? Strings.UMCallAnsweringRuleUnknownContact : dataValidationResult.ADRecipient.DisplayName);
							}
							if (!string.IsNullOrEmpty(keyMapping.Context))
							{
								stringBuilder.Append(Strings.UMCallAnsweringRuleTransferToVoiceMailWithContext(keyMapping.Context, keyMapping.Key, text));
							}
							else
							{
								stringBuilder.Append(Strings.UMCallAnsweringRuleTransferToVoiceMailWithoutContext(keyMapping.Key, text));
							}
							break;
						}
						case KeyMappingType.VoiceMail:
							stringBuilder.Append(Strings.UMCallAnsweringRuleLeaveVoiceMessage);
							break;
						case KeyMappingType.FindMe:
						{
							string firstNumber = UMCallAnsweringRuleDataProvider.MappingEntry.ValidatePhoneNumberForOutDialing(store, keyMapping.FindMeFirstNumber, callAnsweringRule);
							if (!string.IsNullOrEmpty(keyMapping.FindMeSecondNumber))
							{
								string secondNumber = UMCallAnsweringRuleDataProvider.MappingEntry.ValidatePhoneNumberForOutDialing(store, keyMapping.FindMeSecondNumber, callAnsweringRule);
								if (!string.IsNullOrEmpty(keyMapping.Context))
								{
									stringBuilder.Append(Strings.UMCallAnsweringRuleFindMeWithContextAndWithSecondNumber(keyMapping.Context, keyMapping.Key, firstNumber, secondNumber));
								}
								else
								{
									stringBuilder.Append(Strings.UMCallAnsweringRuleFindMeWithoutContextWithSecondNumber(keyMapping.Key, firstNumber, secondNumber));
								}
							}
							else if (!string.IsNullOrEmpty(keyMapping.Context))
							{
								stringBuilder.Append(Strings.UMCallAnsweringRuleFindMeWithContext(keyMapping.Context, keyMapping.Key, firstNumber));
							}
							else
							{
								stringBuilder.Append(Strings.UMCallAnsweringRuleFindMeWithoutContext(keyMapping.Key, firstNumber));
							}
							break;
						}
						}
						if (i < num)
						{
							stringBuilder.Append("\r\n\t");
						}
					}
					result = stringBuilder.ToString();
				}
				else
				{
					result = Strings.UMCallAnsweringRuleCallbackLater;
				}
				return result;
			}

			private static void MapScheduleStatusCallAnsweringRuleToPAA(UMCallAnsweringRule callAnsweringRule, PersonalAutoAttendant paa)
			{
				paa.FreeBusy = (FreeBusyStatusEnum)callAnsweringRule.ScheduleStatus;
			}

			private static void MapScheduleStatusPAAToCallAnsweringRule(UMCallAnsweringRule callAnsweringRule, PersonalAutoAttendant paa)
			{
				callAnsweringRule.ScheduleStatus = (int)paa.FreeBusy;
			}

			private static string GetDescriptionStringForScheduleStatus(UMCallAnsweringRule callAnsweringRule, IPAAStore store)
			{
				string[] array = new string[]
				{
					Strings.Free,
					Strings.Tentative,
					Strings.Busy,
					Strings.Away
				};
				string stringFromSetBits = UMCallAnsweringRuleDataProvider.MappingEntry.GetStringFromSetBits((uint)callAnsweringRule.ScheduleStatus, array.Length, array);
				if (!string.IsNullOrEmpty(stringFromSetBits))
				{
					return Strings.UMCallAnsweringRuleScheduleStatus(stringFromSetBits);
				}
				return stringFromSetBits;
			}

			private static void MapTimeOfDayCallAnsweringRuleToPAA(UMCallAnsweringRule callAnsweringRule, PersonalAutoAttendant paa)
			{
				if (callAnsweringRule.TimeOfDay == null)
				{
					paa.TimeOfDay = TimeOfDayEnum.None;
					paa.WorkingPeriod = null;
					return;
				}
				switch (callAnsweringRule.TimeOfDay.TimeOfDayType)
				{
				case TimeOfDayType.WorkingHours:
					paa.TimeOfDay = TimeOfDayEnum.WorkingHours;
					return;
				case TimeOfDayType.NonWorkingHours:
					paa.TimeOfDay = TimeOfDayEnum.NonWorkingHours;
					return;
				case TimeOfDayType.CustomPeriod:
					paa.TimeOfDay = TimeOfDayEnum.Custom;
					paa.WorkingPeriod = new WorkingPeriod((Microsoft.Exchange.InfoWorker.Common.Availability.DaysOfWeek)callAnsweringRule.TimeOfDay.CustomPeriodDays, (int)callAnsweringRule.TimeOfDay.CustomPeriodStartTime.Value.TotalMinutes, (int)callAnsweringRule.TimeOfDay.CustomPeriodEndTime.Value.TotalMinutes);
					return;
				default:
					return;
				}
			}

			private static void MapTimeOfDayPAAToCallAnsweringRule(UMCallAnsweringRule callAnsweringRule, PersonalAutoAttendant paa)
			{
				if (paa.TimeOfDay != TimeOfDayEnum.None)
				{
					switch (paa.TimeOfDay)
					{
					case TimeOfDayEnum.WorkingHours:
						callAnsweringRule.TimeOfDay = new TimeOfDay(TimeOfDayType.WorkingHours, Microsoft.Exchange.Data.DaysOfWeek.None, null, null);
						return;
					case TimeOfDayEnum.NonWorkingHours:
						callAnsweringRule.TimeOfDay = new TimeOfDay(TimeOfDayType.NonWorkingHours, Microsoft.Exchange.Data.DaysOfWeek.None, null, null);
						return;
					case TimeOfDayEnum.Custom:
						callAnsweringRule.TimeOfDay = new TimeOfDay(TimeOfDayType.CustomPeriod, (Microsoft.Exchange.Data.DaysOfWeek)paa.WorkingPeriod.DayOfWeek, new TimeSpan?(new TimeSpan(0, paa.WorkingPeriod.StartTimeInMinutes, 0)), new TimeSpan?(new TimeSpan(0, paa.WorkingPeriod.EndTimeInMinutes, 0)));
						break;
					default:
						return;
					}
				}
			}

			private static string GetDescriptionStringForTimeOfDay(UMCallAnsweringRule callAnsweringRule, IPAAStore store)
			{
				string result = string.Empty;
				if (callAnsweringRule.TimeOfDay != null)
				{
					switch (callAnsweringRule.TimeOfDay.TimeOfDayType)
					{
					case TimeOfDayType.WorkingHours:
					case TimeOfDayType.NonWorkingHours:
					{
						string workinghours = LocalizedDescriptionAttribute.FromEnum(typeof(TimeOfDayType), callAnsweringRule.TimeOfDay.TimeOfDayType);
						result = Strings.UMCallAnsweringRuleTimeOfDaySimple(workinghours);
						break;
					}
					case TimeOfDayType.CustomPeriod:
					{
						DateTime dateTime = DateTime.UtcNow.Date + callAnsweringRule.TimeOfDay.CustomPeriodStartTime.Value;
						DateTime dateTime2 = DateTime.UtcNow.Date + callAnsweringRule.TimeOfDay.CustomPeriodEndTime.Value;
						DateTimeFormatInfo dateTimeFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat;
						result = Strings.UMCallAnsweringRuleTimeOfDayComplex(dateTime.ToShortTimeString(), dateTime2.ToShortTimeString(), UMCallAnsweringRuleDataProvider.MappingEntry.GetStringFromSetBits((uint)callAnsweringRule.TimeOfDay.CustomPeriodDays, 7, dateTimeFormat.DayNames));
						break;
					}
					}
				}
				return result;
			}

			private static string ValidatePhoneNumberForOutDialing(IPAAStore store, string phoneNumber, UMCallAnsweringRule callAnsweringRule)
			{
				IDataValidationResult dataValidationResult;
				string result;
				if (!store.ValidatePhoneNumberForOutdialing(phoneNumber, out dataValidationResult))
				{
					result = Strings.UMCallAnsweringRuleInvalidValue;
					callAnsweringRule.InError = true;
				}
				else
				{
					result = phoneNumber;
				}
				return result;
			}

			public static readonly UMCallAnsweringRuleDataProvider.MappingEntry[] MappingEntries = new UMCallAnsweringRuleDataProvider.MappingEntry[]
			{
				new UMCallAnsweringRuleDataProvider.MappingEntry(UMCallAnsweringRuleDataProvider.MappingEntryType.Condition, UMCallAnsweringRuleSchema.CallerIds, new UMCallAnsweringRuleDataProvider.AddCallAnsweringRulePropertyToPAA(UMCallAnsweringRuleDataProvider.MappingEntry.MapCallerIdsCallAnsweringRuleToPAA), new UMCallAnsweringRuleDataProvider.AddPAAPropertyToCallAnsweringRule(UMCallAnsweringRuleDataProvider.MappingEntry.MapCallerIdsPAAToCallAnsweringRule), new UMCallAnsweringRuleDataProvider.GetDescriptionString(UMCallAnsweringRuleDataProvider.MappingEntry.GetDescriptionStringForCallerIds)),
				new UMCallAnsweringRuleDataProvider.MappingEntry(UMCallAnsweringRuleDataProvider.MappingEntryType.Condition, UMCallAnsweringRuleSchema.CheckAutomaticReplies, new UMCallAnsweringRuleDataProvider.AddCallAnsweringRulePropertyToPAA(UMCallAnsweringRuleDataProvider.MappingEntry.MapCheckAutomaticRepliesCallAnsweringRuleToPAA), new UMCallAnsweringRuleDataProvider.AddPAAPropertyToCallAnsweringRule(UMCallAnsweringRuleDataProvider.MappingEntry.MapCheckAutomaticRepliesPAAToCallAnsweringRule), new UMCallAnsweringRuleDataProvider.GetDescriptionString(UMCallAnsweringRuleDataProvider.MappingEntry.GetDescriptionStringForCheckAutomaticReplies)),
				new UMCallAnsweringRuleDataProvider.MappingEntry(UMCallAnsweringRuleDataProvider.MappingEntryType.Condition, UMCallAnsweringRuleSchema.ExtensionsDialed, new UMCallAnsweringRuleDataProvider.AddCallAnsweringRulePropertyToPAA(UMCallAnsweringRuleDataProvider.MappingEntry.MapExtensionsDialedCallAnsweringRuleToPAA), new UMCallAnsweringRuleDataProvider.AddPAAPropertyToCallAnsweringRule(UMCallAnsweringRuleDataProvider.MappingEntry.MapExtensionsDialedPAAToCallAnsweringRule), new UMCallAnsweringRuleDataProvider.GetDescriptionString(UMCallAnsweringRuleDataProvider.MappingEntry.GetDescriptionStringForExtensionsDialed)),
				new UMCallAnsweringRuleDataProvider.MappingEntry(UMCallAnsweringRuleDataProvider.MappingEntryType.Action, UMCallAnsweringRuleSchema.KeyMappings, new UMCallAnsweringRuleDataProvider.AddCallAnsweringRulePropertyToPAA(UMCallAnsweringRuleDataProvider.MappingEntry.MapKeyMappingsCallAnsweringRuleToPAA), new UMCallAnsweringRuleDataProvider.AddPAAPropertyToCallAnsweringRule(UMCallAnsweringRuleDataProvider.MappingEntry.MapKeyMappingsPAAToCallAnsweringRule), new UMCallAnsweringRuleDataProvider.GetDescriptionString(UMCallAnsweringRuleDataProvider.MappingEntry.GetDescriptionStringForKeyMappings)),
				new UMCallAnsweringRuleDataProvider.MappingEntry(UMCallAnsweringRuleDataProvider.MappingEntryType.Condition, UMCallAnsweringRuleSchema.ScheduleStatus, new UMCallAnsweringRuleDataProvider.AddCallAnsweringRulePropertyToPAA(UMCallAnsweringRuleDataProvider.MappingEntry.MapScheduleStatusCallAnsweringRuleToPAA), new UMCallAnsweringRuleDataProvider.AddPAAPropertyToCallAnsweringRule(UMCallAnsweringRuleDataProvider.MappingEntry.MapScheduleStatusPAAToCallAnsweringRule), new UMCallAnsweringRuleDataProvider.GetDescriptionString(UMCallAnsweringRuleDataProvider.MappingEntry.GetDescriptionStringForScheduleStatus)),
				new UMCallAnsweringRuleDataProvider.MappingEntry(UMCallAnsweringRuleDataProvider.MappingEntryType.Condition, UMCallAnsweringRuleSchema.TimeOfDay, new UMCallAnsweringRuleDataProvider.AddCallAnsweringRulePropertyToPAA(UMCallAnsweringRuleDataProvider.MappingEntry.MapTimeOfDayCallAnsweringRuleToPAA), new UMCallAnsweringRuleDataProvider.AddPAAPropertyToCallAnsweringRule(UMCallAnsweringRuleDataProvider.MappingEntry.MapTimeOfDayPAAToCallAnsweringRule), new UMCallAnsweringRuleDataProvider.GetDescriptionString(UMCallAnsweringRuleDataProvider.MappingEntry.GetDescriptionStringForTimeOfDay))
			};
		}
	}
}
