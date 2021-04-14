using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.MessageContent;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class AsrContactsManager : ActivityManager
	{
		internal AsrContactsManager(ActivityManager manager, AsrContactsManager.ConfigClass config) : base(manager, config)
		{
		}

		public bool CanSendEmail
		{
			get
			{
				if (this.selectedSearchItem.IsGroup)
				{
					return this.selectedSearchItem.GroupHasEmail;
				}
				return !string.IsNullOrEmpty(this.selectedSearchItem.PrimarySmtpAddress);
			}
		}

		internal bool AuthenticatedCaller
		{
			get
			{
				return this.callerIsAuthenthicated;
			}
		}

		internal SearchTarget CurrentSearchTarget
		{
			get
			{
				return this.currentSearchTarget;
			}
		}

		internal DisambiguationFieldEnum MatchedNameSelectionMethod
		{
			get
			{
				return this.disambiguationField;
			}
		}

		internal ContactSearchItem SelectedSearchItem
		{
			get
			{
				return this.selectedSearchItem;
			}
			set
			{
				this.selectedSearchItem = value;
			}
		}

		protected UMDialPlan TargetDialPlan
		{
			get
			{
				return this.targetDialPlan;
			}
			set
			{
				this.targetDialPlan = value;
			}
		}

		protected UMDialPlan OriginatingDialPlan
		{
			get
			{
				return this.originatingDialPlan;
			}
			set
			{
				this.originatingDialPlan = value;
			}
		}

		protected PhoneNumber CanonicalizedNumber
		{
			get
			{
				return this.canonicalizedNumber;
			}
			set
			{
				this.canonicalizedNumber = value;
			}
		}

		protected ResultType SelectedResultType
		{
			get
			{
				return this.selectedResultType;
			}
			set
			{
				this.selectedResultType = value;
			}
		}

		internal override void Start(BaseUMCallSession vo, string refInfo)
		{
			this.OriginatingDialPlan = vo.CurrentCallContext.DialPlan;
			this.ConfigureForCall(vo);
			base.WriteVariable("searchContext", new AsrContactsSearchContext(this));
			this.disambiguationField = Util.GetDisambiguationField(vo.CurrentCallContext);
			this.callerIsAuthenthicated = (vo.CurrentCallContext.CallType == 3);
			this.currentCallCulture = vo.CurrentCallContext.Culture;
			if (SearchTarget.GlobalAddressList == this.currentSearchTarget)
			{
				vo.IncrementCounter(SubscriberAccessCounters.DirectoryAccessed);
			}
			base.Start(vo, refInfo);
		}

		internal override TransitionBase ExecuteAction(string action, BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.AsrContactsTracer, this, "ASRContactsManager::ExecuteAction({0}).", new object[]
			{
				action
			});
			string text = null;
			if (string.Equals(action, "initializeNamesGrammar", StringComparison.OrdinalIgnoreCase))
			{
				text = this.InitializeNamesGrammar(vo);
				base.WriteVariable("retryAsrSearch", false);
			}
			else if (string.Equals(action, "processResult", StringComparison.OrdinalIgnoreCase))
			{
				AsrSearchResult asrSearchResult = (AsrSearchResult)this.ReadVariable("searchResult");
				asrSearchResult.SetManagerVariables(this, vo);
				if (base.Manager != null)
				{
					asrSearchResult.SetManagerVariables(base.Manager, vo);
				}
				this.SelectedResultType = (ResultType)this.ReadVariable("resultType");
				this.selectedSearchItem = (ContactSearchItem)this.ReadVariable("selectedUser");
				this.phoneNumberToDial = (PhoneNumber)this.ReadVariable("selectedPhoneNumber");
				if (this.selectedSearchItem == null)
				{
					if (this.phoneNumberToDial == null)
					{
						text = "invalidResult";
					}
					else if (this.phoneNumberToDial.Number.Length < vo.CurrentCallContext.DialPlan.NumberOfDigitsInExtension)
					{
						base.WriteVariable("invalidExtension", base.DtmfDigits);
						text = "invalidOption";
					}
				}
				else if (this.selectedSearchItem.Recipient != null)
				{
					this.selectedSearchItem.SetBusinessPhoneForDialPlan(vo.CurrentCallContext.DialPlan);
				}
				if (SearchTarget.GlobalAddressList == this.currentSearchTarget)
				{
					vo.IncrementCounter(SubscriberAccessCounters.DirectoryAccessedSuccessfullyBySpokenName);
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.AsrContactsTracer, this, "ProcessResult setting autoEvent: {0}.", new object[]
				{
					text ?? "<null>"
				});
			}
			else if (string.Equals(action, "setName", StringComparison.OrdinalIgnoreCase))
			{
				PIIMessage data = PIIMessage.Create(PIIType._UserDisplayName, this.selectedSearchItem.FullName);
				CallIdTracer.TraceDebug(ExTraceGlobals.AsrContactsTracer, this, data, "Setting TTS/recorded name for user = _UserDisplayName.", new object[0]);
				if (this.selectedSearchItem.Recipient != null)
				{
					base.SetRecordedName("userName", this.selectedSearchItem.Recipient);
				}
				else
				{
					base.WriteVariable("userName", this.selectedSearchItem.FullName);
				}
			}
			else if (string.Equals(action, "setContactInfoVariables", StringComparison.OrdinalIgnoreCase))
			{
				this.SetContactInfoVariables(vo);
			}
			else if (string.Equals(action, "setEmailAddress", StringComparison.OrdinalIgnoreCase))
			{
				text = this.SetEmailAddress();
			}
			else if (string.Equals(action, "selectEmailAddress", StringComparison.OrdinalIgnoreCase))
			{
				text = this.SelectEmailAddress();
			}
			else if (string.Equals(action, "setContactInfo", StringComparison.OrdinalIgnoreCase))
			{
				this.selectedSearchItem.SetVariablesForTts(this, vo);
			}
			else if (string.Equals(action, "canonicalizeNumber", StringComparison.OrdinalIgnoreCase))
			{
				text = this.CanonicalizeNumber(vo);
			}
			else if (string.Equals(action, "checkDialPermissions", StringComparison.OrdinalIgnoreCase))
			{
				PhoneNumber phone = null;
				if (this.CheckDialPermissions(vo, out phone))
				{
					ContactInfo selectedUserAsContactInfo = this.GetSelectedUserAsContactInfo();
					PhoneUtil.SetTransferTargetPhone(this, TransferExtension.UserExtension, phone, selectedUserAsContactInfo);
					if (base.Manager is AutoAttendantManager)
					{
						PhoneUtil.SetTransferTargetPhone(base.Manager, TransferExtension.UserExtension, phone, selectedUserAsContactInfo);
					}
					text = null;
				}
				else
				{
					text = "dialingPermissionCheckFailed";
				}
			}
			else if (string.Equals(action, "prepareForTransferToCell", StringComparison.OrdinalIgnoreCase))
			{
				this.phoneNumberToDial = AsrContactsManager.ParsePhoneNumberWithDialplanInfo(vo.CurrentCallContext.DialPlan, this.selectedSearchItem.MobilePhone);
				base.WriteVariable("callingType", AsrContactsManager.CallingType.Cell.ToString());
			}
			else if (string.Equals(action, "prepareForTransferToOffice", StringComparison.OrdinalIgnoreCase))
			{
				this.phoneNumberToDial = AsrContactsManager.ParsePhoneNumberWithDialplanInfo(vo.CurrentCallContext.DialPlan, this.selectedSearchItem.BusinessPhone);
				base.WriteVariable("callingType", AsrContactsManager.CallingType.Office.ToString());
			}
			else if (string.Equals(action, "prepareForTransferToHome", StringComparison.OrdinalIgnoreCase))
			{
				this.phoneNumberToDial = AsrContactsManager.ParsePhoneNumberWithDialplanInfo(vo.CurrentCallContext.DialPlan, this.selectedSearchItem.HomePhone);
				base.WriteVariable("callingType", AsrContactsManager.CallingType.Home.ToString());
			}
			else if (string.Equals(action, "setInitialSearchTargetContacts", StringComparison.OrdinalIgnoreCase))
			{
				this.SetInitialSearchTargetContacts();
				base.WriteVariable("contacts_nameLookupEnabled", true);
			}
			else if (string.Equals(action, "setInitialSearchTargetGAL", StringComparison.OrdinalIgnoreCase))
			{
				this.SetInitialSearchTargetGal();
				base.WriteVariable("contacts_nameLookupEnabled", true);
			}
			else if (string.Equals(action, "retryAsrSearch", StringComparison.OrdinalIgnoreCase))
			{
				base.WriteVariable("retryAsrSearch", true);
			}
			else
			{
				if (!string.Equals(action, "handlePlatformFailure", StringComparison.OrdinalIgnoreCase))
				{
					return base.ExecuteAction(action, vo);
				}
				if (this.currentSearchTarget == SearchTarget.PersonalContacts)
				{
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_PersonalContactsSearchPlatformFailure, null, new object[]
					{
						vo.CurrentCallContext.CallerInfo.ToString()
					});
				}
				else
				{
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_GALSearchPlatformFailure, null, new object[0]);
				}
			}
			return base.CurrentActivity.GetTransition(text);
		}

		internal virtual void PrepareForPromptForAliasQA(List<IUMRecognitionPhrase> alternates)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.AsrContactsTracer, this, "AsrContactsManager::PrepareForPromptForAliasQA()", new object[0]);
			PromptForAliasGrammarFile promptForAliasGrammarFile = new PromptForAliasGrammarFile(alternates, this.currentCallCulture);
			CallIdTracer.TraceDebug(ExTraceGlobals.AsrContactsTracer, this, "Grammar File Path: {0}.HasEntries={1}", new object[]
			{
				promptForAliasGrammarFile.FilePath,
				promptForAliasGrammarFile.HasEntries
			});
			if (promptForAliasGrammarFile.HasEntries)
			{
				base.WriteVariable("emailAliasGrammar", promptForAliasGrammarFile);
				return;
			}
			base.WriteVariable("emailAliasGrammar", null);
			CallIdTracer.TraceDebug(ExTraceGlobals.AsrContactsTracer, this, "PromptForAlias Grammar File at Path: {0} does not have any entries. PromptForAlias will not be entered", new object[]
			{
				promptForAliasGrammarFile.FilePath
			});
		}

		internal virtual void PrepareForCollisionQA(List<List<IUMRecognitionPhrase>> alternates)
		{
			this.PrepareToPlayNames(alternates, true, false);
		}

		internal virtual void PrepareForConfirmViaListQA(List<List<IUMRecognitionPhrase>> alternates)
		{
			this.PrepareToPlayNames(alternates, true, true);
		}

		internal virtual void PrepareForNBestPhase2()
		{
		}

		internal virtual bool PrepareForConfirmQA(List<List<IUMRecognitionPhrase>> alternates)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.AsrContactsTracer, this, "Enter: SetVariablesForConfirmNameDepartmentQA.", new object[0]);
			int count = alternates[0].Count;
			IUMRecognitionPhrase iumrecognitionPhrase = alternates[0][0];
			string text = iumrecognitionPhrase["ResultType"] as string;
			string recordedName = iumrecognitionPhrase["ContactName"] as string;
			base.WriteVariable("resultType", text);
			string a;
			if ((a = text) != null)
			{
				if (!(a == "DirectoryContact"))
				{
					if (!(a == "PersonalContact"))
					{
						if (a == "Department")
						{
							string recordedName2 = iumrecognitionPhrase["DepartmentName"] as string;
							base.SetTextPartVariable("departmentName", recordedName2);
						}
					}
					else
					{
						string varValue = iumrecognitionPhrase["ContactName"] as string;
						base.WriteVariable("userName", varValue);
					}
				}
				else if (count == 1)
				{
					string text2 = (string)iumrecognitionPhrase["ObjectGuid"];
					ADRecipient r = null;
					if (!this.recipientCache.TryGetRecipient(new Guid(text2), out r))
					{
						CallIdTracer.TraceDebug(ExTraceGlobals.AsrContactsTracer, this, "Recipient for GUID: {0} was not found.", new object[]
						{
							text2
						});
						return false;
					}
					bool flag = false;
					base.SetRecordedName("userName", r, ref flag);
					base.WriteVariable("haveNameRecording", flag);
				}
				else
				{
					base.SetTextPartVariable("userName", recordedName);
					base.WriteVariable("haveNameRecording", false);
				}
			}
			return true;
		}

		internal virtual bool PrepareForConfirmAgainQA(List<List<IUMRecognitionPhrase>> alternates)
		{
			return this.PrepareForConfirmQA(alternates);
		}

		internal virtual List<List<IUMRecognitionPhrase>> ProcessMultipleResults(List<List<IUMRecognitionPhrase>> results)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.AsrContactsTracer, this, "AsrContactsSearchContext::ProcessMultipleResults(#results = {0}).", new object[]
			{
				results.Count
			});
			this.Dump(results);
			this.recipientCache.BuildCache(results, base.CallSession.CurrentCallContext.TenantGuid);
			List<List<IUMRecognitionPhrase>> list = this.RemoveInvalidResults(results, true);
			CallIdTracer.TraceDebug(ExTraceGlobals.AsrContactsTracer, this, "AsrContactsSearchContext::ProcessMultipleResults(#ValidResults = {0}).", new object[]
			{
				list.Count
			});
			this.Dump(list);
			List<List<IUMRecognitionPhrase>> list2 = this.RemoveDuplicateResults(list);
			CallIdTracer.TraceDebug(ExTraceGlobals.AsrContactsTracer, this, "AsrContactsSearchContext::ProcessMultipleResults(#UniqueResults = {0}).", new object[]
			{
				list2.Count
			});
			this.Dump(list2);
			return list2;
		}

		internal virtual void OnNameSpoken()
		{
			if (SearchTarget.GlobalAddressList == this.currentSearchTarget)
			{
				base.CallSession.IncrementCounter(SubscriberAccessCounters.DirectoryAccessedBySpokenName);
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.AsrContactsTracer, this, "AsrContactsManager::OnNameSpoken().", new object[0]);
		}

		protected virtual void ConfigureForCall(BaseUMCallSession vo)
		{
			base.WriteVariable("contacts_nameLookupEnabled", true);
			string varName = "initialSearchTarget";
			string value = (string)this.GlobalManager.ReadVariable(varName);
			base.WriteVariable(varName, this.GlobalManager.ReadVariable(varName));
			if (!string.IsNullOrEmpty(value))
			{
				this.currentSearchTarget = (SearchTarget)Enum.Parse(typeof(SearchTarget), value);
				return;
			}
			this.currentSearchTarget = SearchTarget.GlobalAddressList;
		}

		protected virtual SearchGrammarFile CreateNamesGrammar(BaseUMCallSession vo)
		{
			SearchGrammarFile result = null;
			string text = (string)this.ReadVariable("initialSearchTarget");
			this.currentSearchTarget = (SearchTarget)Enum.Parse(typeof(SearchTarget), text);
			if (string.Equals(text, SearchTarget.GlobalAddressList.ToString()))
			{
				result = null;
			}
			else if (string.Equals(text, SearchTarget.PersonalContacts.ToString()))
			{
				PersonalContactsGrammarFile personalContactsGrammarFile = this.GlobalManager.PersonalContactsGrammarFile;
				if (personalContactsGrammarFile == null)
				{
					personalContactsGrammarFile = new OVAPersonalContactsGrammarFile(vo.CurrentCallContext.CallerInfo, vo.CurrentCallContext.Culture);
					this.GlobalManager.PersonalContactsGrammarFile = personalContactsGrammarFile;
				}
				result = personalContactsGrammarFile;
			}
			return result;
		}

		protected virtual bool CheckDialPermissions(BaseUMCallSession vo, out PhoneNumber numberToDial)
		{
			return DialPermissions.Check(this.CanonicalizedNumber, (ADUser)vo.CurrentCallContext.CallerInfo.ADRecipient, vo.CurrentCallContext.DialPlan, this.TargetDialPlan, out numberToDial);
		}

		protected void SetInitialSearchTargetContacts()
		{
			this.currentSearchTarget = SearchTarget.PersonalContacts;
			base.WriteVariable("initialSearchTarget", SearchTarget.PersonalContacts.ToString());
		}

		protected void SetInitialSearchTargetGal()
		{
			this.currentSearchTarget = SearchTarget.GlobalAddressList;
			base.WriteVariable("initialSearchTarget", SearchTarget.GlobalAddressList.ToString());
		}

		protected virtual void LookupRecipientAndDialPlan(BaseUMCallSession vo, PhoneNumber numberToDial)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.AsrContactsTracer, this, "Result is a user extension, looking up extension in current dialplan.", new object[0]);
			RecipientLookup recipientLookup = RecipientLookup.Create(vo);
			DialPermissionWrapper dialPermissionWrapper = DialPermissionWrapperFactory.Create(vo);
			ADRecipient adrecipient = recipientLookup.LookupByExtension(numberToDial.ToDial, vo, DirectorySearchPurpose.Call, dialPermissionWrapper.ContactScope);
			if (adrecipient != null)
			{
				this.selectedSearchItem = ContactSearchItem.CreateFromRecipient(adrecipient);
				IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromADRecipient(this.selectedSearchItem.Recipient);
				this.TargetDialPlan = iadsystemConfigurationLookup.GetDialPlanFromRecipient(this.selectedSearchItem.Recipient);
			}
			PIIMessage data = PIIMessage.Create(PIIType._UserDisplayName, (this.selectedSearchItem != null) ? this.selectedSearchItem.FullName : "<null>");
			CallIdTracer.TraceDebug(ExTraceGlobals.AsrContactsTracer, this, data, "Found recipient _UserDisplayName for extension {0} in current dialplan.", new object[]
			{
				numberToDial
			});
			if (this.TargetDialPlan == null)
			{
				PIIMessage data2 = PIIMessage.Create(PIIType._PhoneNumber, numberToDial);
				CallIdTracer.TraceDebug(ExTraceGlobals.AsrContactsTracer, this, data2, "Did not find targetdialplan for a UserExtension: _PhoneNumber. Using originatingdialplan as targetdialplan.", new object[0]);
				this.TargetDialPlan = this.OriginatingDialPlan;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<AsrContactsManager>(this);
		}

		private static PhoneNumber ParsePhoneNumberWithDialplanInfo(UMDialPlan originatingDP, PhoneNumber phone)
		{
			PhoneNumber phoneNumber = (phone != null) ? phone.Clone(originatingDP) : null;
			PIIMessage[] data = new PIIMessage[]
			{
				PIIMessage.Create(PIIType._PhoneNumber, phone),
				PIIMessage.Create(PIIType._PhoneNumber, (phoneNumber != null) ? phoneNumber.ToString() : "<null>")
			};
			CallIdTracer.TraceDebug(ExTraceGlobals.AsrContactsTracer, null, data, "ParsePhoneNumberWithDialplanInfo(Number=(_PhoneNumber1)  returning ParsedNumber=(_PhoneNumber2))", new object[0]);
			return phoneNumber;
		}

		private static string ListToString(List<IUMRecognitionPhrase> results)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (IUMRecognitionPhrase phrase in results)
			{
				stringBuilder.Append(ResultWrapper.GetIdFromPhrase(phrase));
				stringBuilder.Append(",");
			}
			return stringBuilder.ToString();
		}

		private ContactInfo GetSelectedUserAsContactInfo()
		{
			FoundByType selectedPhoneType = FoundByType.NotSpecified;
			string value = this.ReadVariable("callingType") as string;
			AsrContactsManager.CallingType callingType;
			if (!string.IsNullOrEmpty(value) && EnumValidator<AsrContactsManager.CallingType>.TryParse(value, EnumParseOptions.Default, out callingType))
			{
				switch (callingType)
				{
				case AsrContactsManager.CallingType.Cell:
					selectedPhoneType = FoundByType.MobilePhone;
					break;
				case AsrContactsManager.CallingType.Office:
					selectedPhoneType = FoundByType.BusinessPhone;
					break;
				case AsrContactsManager.CallingType.Home:
					selectedPhoneType = FoundByType.HomePhone;
					break;
				}
			}
			if (this.selectedSearchItem == null)
			{
				return null;
			}
			return this.selectedSearchItem.ToContactInfo(selectedPhoneType);
		}

		private void SetContactInfoVariables(BaseUMCallSession vo)
		{
			base.WriteVariable("hasCell", Util.IsDialableNumber(this.selectedSearchItem.MobilePhone, vo, this.selectedSearchItem.Recipient));
			base.WriteVariable("hasHome", Util.IsDialableNumber(this.selectedSearchItem.HomePhone, vo, this.selectedSearchItem.Recipient));
			base.WriteVariable("hasOffice", Util.IsDialableNumber(this.selectedSearchItem.BusinessPhone, vo, this.selectedSearchItem.Recipient));
		}

		private string SetEmailAddress()
		{
			if (this.selectedSearchItem.Recipient == null && this.selectedSearchItem.ContactEmailAddresses.Count > 1)
			{
				this.selectedSearchItem.SetEmailAddresses(this);
				return "moreThanOneAddress";
			}
			base.Manager.WriteVariable("directorySearchResult", this.selectedSearchItem);
			return null;
		}

		private string SelectEmailAddress()
		{
			string s = base.RecoResult["Choice"] as string;
			int num = int.Parse(s, CultureInfo.InvariantCulture);
			base.WriteVariable("emailAddressSelection", num);
			num--;
			if (num >= this.selectedSearchItem.ContactEmailAddresses.Count)
			{
				return "invalidOption";
			}
			this.selectedSearchItem.PrimarySmtpAddress = this.selectedSearchItem.ContactEmailAddresses[num];
			base.Manager.WriteVariable("directorySearchResult", this.selectedSearchItem);
			return null;
		}

		private string CanonicalizeNumber(BaseUMCallSession vo)
		{
			ADRecipient adrecipient = (this.selectedSearchItem != null) ? this.selectedSearchItem.Recipient : null;
			if (adrecipient != null)
			{
				IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromADRecipient(adrecipient);
				this.TargetDialPlan = iadsystemConfigurationLookup.GetDialPlanFromRecipient(adrecipient);
			}
			else if (this.selectedSearchItem == null)
			{
				this.LookupRecipientAndDialPlan(vo, this.phoneNumberToDial);
			}
			this.CanonicalizedNumber = DialPermissions.Canonicalize(this.phoneNumberToDial, vo.CurrentCallContext.DialPlan, adrecipient, this.TargetDialPlan);
			if (this.CanonicalizedNumber != null)
			{
				return "validCanonicalNumber";
			}
			return "numberCanonicalizationFailed";
		}

		private void PrepareToPlayNames(List<List<IUMRecognitionPhrase>> alternates, bool disambiguate, bool useRecoTextForMultipleResults)
		{
			bool flag = true;
			bool flag2 = true;
			int num = 1;
			StringBuilder stringBuilder = new StringBuilder();
			string arg = "haveNameRecording";
			foreach (List<IUMRecognitionPhrase> list in alternates)
			{
				int count = list.Count;
				bool useRecoText = useRecoTextForMultipleResults && count > 1;
				IUMRecognitionPhrase result = list[0];
				string variableName = string.Format(CultureInfo.InvariantCulture, "user{0}", new object[]
				{
					num
				});
				string name;
				bool flag3;
				bool flag4;
				bool flag5;
				this.SetRecordingForRecognitionPhrase(variableName, result, useRecoText, disambiguate, out name, out flag3, out flag4, out flag5);
				if (!flag5)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.AsrContactsTracer, this, "Skipping nonexistent recipient from display for ConfirmViaListQA.", new object[0]);
				}
				else
				{
					base.WriteVariable(arg + num, flag3);
					num++;
					flag2 = (flag2 && flag3);
					flag = (flag && flag4);
					stringBuilder.Append(Strings.SayNumberAndName(num, name).ToString(this.currentCallCulture));
				}
			}
			base.WriteVariable("namesOnly", flag);
			base.WriteVariable("recordedNamesOnly", flag2);
			base.WriteVariable("resultList", stringBuilder.ToString());
			base.WriteVariable("numUsers", num - 1);
		}

		private string InitializeNamesGrammar(BaseUMCallSession vo)
		{
			string result = null;
			bool flag = (bool)this.ReadVariable("contacts_nameLookupEnabled");
			if (flag)
			{
				SearchGrammarFile searchGrammarFile = this.CreateNamesGrammar(vo);
				if (searchGrammarFile != null && File.Exists(searchGrammarFile.FilePath))
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.AsrContactsTracer, this, "Grammar File Path: {0}.", new object[]
					{
						searchGrammarFile.FilePath
					});
					base.WriteVariable("namesGrammar", searchGrammarFile);
				}
				else
				{
					base.WriteVariable("contacts_nameLookupEnabled", false);
					result = "noGrammarFile";
				}
				base.WriteVariable("distributionListGrammar", null);
			}
			return result;
		}

		private void SetRecordingForRecognitionPhrase(string variableName, IUMRecognitionPhrase result, bool useRecoText, bool disambiguate, out string name, out bool haveRecordedName, out bool directoryOrPersonalContact, out bool exists)
		{
			haveRecordedName = false;
			directoryOrPersonalContact = false;
			name = null;
			exists = true;
			string text = (string)result["ResultType"];
			string a;
			if ((a = text) != null)
			{
				if (!(a == "DirectoryContact"))
				{
					if (a == "PersonalContact")
					{
						directoryOrPersonalContact = true;
						if (useRecoText)
						{
							name = (result["ContactName"] as string);
						}
						else
						{
							name = (result["DisambiguationField"] as string);
						}
						base.WriteVariable(variableName, name);
						return;
					}
					if (!(a == "Department"))
					{
						return;
					}
					directoryOrPersonalContact = false;
					name = (result["DepartmentName"] as string);
					base.SetTextPartVariable(variableName, name);
				}
				else
				{
					directoryOrPersonalContact = true;
					string text2 = result["ContactName"] as string;
					string text3 = (string)result["ObjectGuid"];
					ADRecipient adrecipient = null;
					if (!this.recipientCache.TryGetRecipient(new Guid(text3), out adrecipient))
					{
						exists = false;
						CallIdTracer.TraceDebug(ExTraceGlobals.AsrContactsTracer, this, "Recipient for guid: {0} does not exist.", new object[]
						{
							text3
						});
						return;
					}
					if (useRecoText)
					{
						PIIMessage data = PIIMessage.Create(PIIType._User, text2);
						CallIdTracer.TraceDebug(ExTraceGlobals.AsrContactsTracer, this, data, "Using RecoText _user as prompt.", new object[0]);
						base.SetTextPartVariable(variableName, text2);
						name = text2;
						haveRecordedName = false;
						return;
					}
					base.SetRecordedName(variableName, text2, adrecipient, disambiguate, this.disambiguationField, ref haveRecordedName);
					name = adrecipient.DisplayName;
					return;
				}
			}
		}

		private List<List<IUMRecognitionPhrase>> RemoveInvalidResults(List<List<IUMRecognitionPhrase>> alternates, bool removeNonExistingRecipients)
		{
			List<List<IUMRecognitionPhrase>> list = new List<List<IUMRecognitionPhrase>>();
			foreach (List<IUMRecognitionPhrase> list2 in alternates)
			{
				List<IUMRecognitionPhrase> list3 = null;
				foreach (IUMRecognitionPhrase iumrecognitionPhrase in list2)
				{
					string text = (string)iumrecognitionPhrase["ResultType"];
					float confidence = iumrecognitionPhrase.Confidence;
					string value = null;
					if (!string.IsNullOrEmpty(text) && confidence >= 0.2f)
					{
						string a;
						if ((a = text) == null)
						{
							goto IL_120;
						}
						bool flag;
						if (!(a == "DirectoryContact"))
						{
							if (!(a == "PersonalContact"))
							{
								if (!(a == "Department"))
								{
									goto IL_120;
								}
								value = (iumrecognitionPhrase["DepartmentName"] as string);
								flag = true;
							}
							else
							{
								value = (iumrecognitionPhrase["ContactName"] as string);
								flag = !string.IsNullOrEmpty(value);
							}
						}
						else
						{
							value = (iumrecognitionPhrase["ContactName"] as string);
							string g = (string)iumrecognitionPhrase["ObjectGuid"];
							Guid objectGuid = new Guid(g);
							flag = (!removeNonExistingRecipients || this.recipientCache.RecipientExists(objectGuid));
						}
						IL_142:
						if (flag)
						{
							PIIMessage data = PIIMessage.Create(PIIType._PII, value);
							CallIdTracer.TraceDebug(ExTraceGlobals.AsrContactsTracer, this, data, "Valid Result: _PII.", new object[0]);
							if (list3 == null)
							{
								list3 = new List<IUMRecognitionPhrase>();
							}
							list3.Add(iumrecognitionPhrase);
							continue;
						}
						continue;
						IL_120:
						CallIdTracer.TraceDebug(ExTraceGlobals.AsrContactsTracer, this, "Invalid resulttype: [{0}].", new object[]
						{
							text
						});
						flag = false;
						goto IL_142;
					}
					CallIdTracer.TraceDebug(ExTraceGlobals.AsrContactsTracer, this, "Invalid Result: {0}(confidence = {1}, min Confidence = {2}).", new object[]
					{
						iumrecognitionPhrase.Text,
						iumrecognitionPhrase.Confidence,
						0.2f
					});
				}
				if (list3 != null && list3.Count > 0)
				{
					list.Add(list3);
				}
			}
			return list;
		}

		private List<List<IUMRecognitionPhrase>> RemoveDuplicateResults(List<List<IUMRecognitionPhrase>> speechResults)
		{
			DuplicateResultCheck duplicateResultCheck = new DuplicateResultCheck();
			List<List<IUMRecognitionPhrase>> list = new List<List<IUMRecognitionPhrase>>();
			foreach (List<IUMRecognitionPhrase> list2 in speechResults)
			{
				if (!duplicateResultCheck.Contains(list2))
				{
					list.Add(list2);
					CallIdTracer.TraceDebug(ExTraceGlobals.AsrContactsTracer, this, "ADDING RESULT LIST: {0}.", new object[]
					{
						AsrContactsManager.ListToString(list2)
					});
				}
				else
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.AsrContactsTracer, this, "DUPLICATE RESULT LIST: {0}.", new object[]
					{
						AsrContactsManager.ListToString(list2)
					});
				}
			}
			return list;
		}

		private void Dump(List<List<IUMRecognitionPhrase>> results)
		{
			int num = 0;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (List<IUMRecognitionPhrase> results2 in results)
			{
				num++;
				stringBuilder.AppendFormat("Alternate({0})::", num);
				stringBuilder.Append(AsrContactsManager.ListToString(results2));
				stringBuilder.AppendLine();
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.AsrContactsTracer, this, stringBuilder.ToString(), new object[0]);
		}

		private AsrContactsManager.RecipientCache recipientCache = new AsrContactsManager.RecipientCache();

		private ResultType selectedResultType;

		private PhoneNumber phoneNumberToDial;

		private PhoneNumber canonicalizedNumber;

		private UMDialPlan originatingDialPlan;

		private UMDialPlan targetDialPlan;

		private DisambiguationFieldEnum disambiguationField;

		private CultureInfo currentCallCulture;

		private ContactSearchItem selectedSearchItem;

		private SearchTarget currentSearchTarget;

		private bool callerIsAuthenthicated;

		private enum CallingType
		{
			Cell,
			Office,
			Home
		}

		internal class ConfigClass : ActivityManagerConfig
		{
			internal ConfigClass(ActivityManagerConfig manager) : base(manager)
			{
			}

			internal override ActivityManager CreateActivityManager(ActivityManager manager)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Constructing ASR contacts activity manager.", new object[0]);
				return new AsrContactsManager(manager, this);
			}
		}

		private class RecipientCache
		{
			internal RecipientCache()
			{
				this.recipientCache = new Dictionary<Guid, AsrContactsManager.RecipientCache>();
			}

			private RecipientCache(ADRecipient recipient)
			{
				this.recipient = recipient;
			}

			internal ADRecipient Recipient
			{
				get
				{
					return this.recipient;
				}
			}

			internal void BuildCache(List<List<IUMRecognitionPhrase>> alternates, Guid tenantGuid)
			{
				foreach (List<IUMRecognitionPhrase> list in alternates)
				{
					foreach (IUMRecognitionPhrase phrase in list)
					{
						string text = null;
						ResultType resultTypeAndIdFromPhrase = ResultWrapper.GetResultTypeAndIdFromPhrase(phrase, out text);
						if (resultTypeAndIdFromPhrase == ResultType.DirectoryContact)
						{
							Guid guid = new Guid(text);
							if (!this.recipientCache.ContainsKey(guid))
							{
								IADRecipientLookup iadrecipientLookup = ADRecipientLookupFactory.CreateFromTenantGuid(tenantGuid);
								ADRecipient adrecipient = iadrecipientLookup.LookupByObjectId(new ADObjectId(guid));
								if (adrecipient != null)
								{
									PIIMessage data = PIIMessage.Create(PIIType._PII, text);
									CallIdTracer.TraceDebug(ExTraceGlobals.AsrContactsTracer, this, data, "Adding _PII to recipient cache.", new object[0]);
								}
								this.recipientCache.Add(guid, new AsrContactsManager.RecipientCache(adrecipient));
							}
						}
					}
				}
			}

			internal bool TryGetRecipient(Guid objectGuid, out ADRecipient recipient)
			{
				recipient = null;
				AsrContactsManager.RecipientCache recipientCache = null;
				if (this.recipientCache.TryGetValue(objectGuid, out recipientCache))
				{
					recipient = recipientCache.Recipient;
				}
				return recipient != null;
			}

			internal bool RecipientExists(Guid objectGuid)
			{
				ADRecipient adrecipient = null;
				return this.TryGetRecipient(objectGuid, out adrecipient);
			}

			private Dictionary<Guid, AsrContactsManager.RecipientCache> recipientCache;

			private ADRecipient recipient;
		}
	}
}
