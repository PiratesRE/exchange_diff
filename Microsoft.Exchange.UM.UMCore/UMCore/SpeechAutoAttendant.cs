using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class SpeechAutoAttendant : AutoAttendantCore
	{
		internal SpeechAutoAttendant(IAutoAttendantUI autoAttendantManager, BaseUMCallSession voiceObject) : base(autoAttendantManager, voiceObject)
		{
		}

		internal override void Configure()
		{
			base.Configure();
			this.dtmfFallbackEnabled = false;
			if (base.Config.DTMFFallbackAutoAttendant != null)
			{
				IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromOrganizationId(base.Config.OrganizationId);
				UMAutoAttendant autoAttendantFromId = iadsystemConfigurationLookup.GetAutoAttendantFromId(base.Config.DTMFFallbackAutoAttendant);
				LocalizedString localizedString;
				if (AutoAttendantCore.IsRunnableAutoAttendant(autoAttendantFromId, out localizedString))
				{
					this.dtmfFallbackEnabled = true;
				}
			}
			base.WriteVariable("aa_dtmfFallbackEnabled", this.dtmfFallbackEnabled);
			this.ConfigureDtmfFallbackAction();
			bool flag = base.NameLookupConfigured && (base.Config.CallSomeoneEnabled || base.Config.SendVoiceMsgEnabled);
			base.WriteVariable("contacts_nameLookupEnabled", flag);
			this.ConfigureDepartmentPrompt();
		}

		internal override void Initialize()
		{
			this.asrNameSpoken = false;
			this.dtmfInputForDeptMenu = false;
		}

		internal override bool ExecuteAction(string action, BaseUMCallSession voiceObject, ref string autoEvent)
		{
			bool result;
			if (string.Compare(action, "processResult", true, CultureInfo.InvariantCulture) == 0)
			{
				result = true;
				autoEvent = this.Action_ProcessResult();
			}
			else if (string.Compare(action, "setCustomExtensionNumber", true, CultureInfo.InvariantCulture) == 0)
			{
				result = true;
				autoEvent = this.Action_SetCustomExtensionNumber();
			}
			else if (string.Compare(action, "prepareForANROperatorTransfer", true, CultureInfo.InvariantCulture) == 0)
			{
				result = true;
				autoEvent = this.Action_PrepareForANROperatorTransfer();
			}
			else
			{
				if (string.Compare(action, "setFallbackAutoAttendant", true, CultureInfo.InvariantCulture) != 0)
				{
					return base.ExecuteAction(action, base.VoiceObject, ref autoEvent);
				}
				result = true;
				if (!base.VoiceObject.CurrentCallContext.SwitchToFallbackAutoAttendant())
				{
					autoEvent = "fallbackAutoAttendantFailure";
				}
			}
			return result;
		}

		internal override void OnSpeech()
		{
			base.OnSpeech();
			if (this.incrementedSpeechCallCounter)
			{
				return;
			}
			this.incrementedSpeechCallCounter = true;
			base.PerfCounters.IncrementSpeechCallsCounter();
		}

		internal override void OnNameSpoken()
		{
			base.OnNameSpoken();
			this.asrNameSpoken = true;
		}

		internal override string Action_ProcessCustomMenuExtension()
		{
			string dtmfDigits = ((ActivityManager)base.AutoAttendantManager).DtmfDigits;
			string result = null;
			if (base.TimeoutPending)
			{
				base.TimeoutPending = false;
				base.SelectedMenu = base.CustomMenuTimeoutOption;
				this.dtmfInputForDeptMenu = true;
			}
			else if (Constants.RegularExpressions.ValidDigitRegex.IsMatch(dtmfDigits))
			{
				int selectedMenuOption = int.Parse(dtmfDigits, CultureInfo.InvariantCulture);
				if (this.SetSelectedMenuOption(selectedMenuOption))
				{
					this.dtmfInputForDeptMenu = true;
				}
				else
				{
					result = "invalidOption";
					base.WriteVariable("invalidExtension", dtmfDigits);
				}
			}
			else
			{
				result = "invalidOption";
				base.WriteVariable("invalidExtension", dtmfDigits);
			}
			AsrSearchResult value = AsrSearchResult.Create(base.SelectedMenu);
			base.AutoAttendantManager.WriteProperty("searchResult", value);
			return result;
		}

		internal override string GetCustomMenuAutoAttendant()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, this, "SpeechAutoAttendant::GetCustomMenuAutoAttendant()", new object[0]);
			return this.GetTarget();
		}

		internal override string GetCustomMenuVoicemailTarget()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, this, "SpeechAutoAttendant::GetCustomMenuVoicemailTarget()", new object[0]);
			return this.GetTarget();
		}

		internal override string GetCustomMenuTargetPAA()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, this, "SpeechAutoAttendant::GetCustomMenuTargetPAA()", new object[0]);
			return this.GetTarget();
		}

		internal string Action_ProcessResult()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, this, "SpeechAutoAttendant::Action_ProcessResult().", new object[0]);
			AsrSearchResult asrSearchResult = (AsrSearchResult)base.AutoAttendantManager.ReadProperty("searchResult");
			string text;
			if (asrSearchResult is AsrDepartmentSearchResult)
			{
				text = this.Action_ProcessResult((AsrDepartmentSearchResult)asrSearchResult);
			}
			else if (asrSearchResult is AsrDirectorySearchResult)
			{
				text = this.Action_ProcessResult((AsrDirectorySearchResult)asrSearchResult);
			}
			else
			{
				if (!(asrSearchResult is AsrExtensionSearchResult))
				{
					throw new InvalidOperationException("Invalid searchResult: " + asrSearchResult.GetType().ToString());
				}
				text = this.Action_ProcessResult((AsrExtensionSearchResult)asrSearchResult);
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, this, "SpeechAutoAttendant::Action_ProcessResult() returning autoevent: {0}", new object[]
			{
				text ?? "<null>"
			});
			return text;
		}

		internal string Action_ProcessResult(AsrDepartmentSearchResult departmentResult)
		{
			string result = null;
			CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, this, "SpeechAutoAttendant::Action_ProcessResult(<Department>).", new object[0]);
			base.IncrementKeyMappingPerfCounters(departmentResult.KeyPress);
			if (this.asrNameSpoken)
			{
				base.PerfCounters.Increment(DirectoryAccessCountersUtil.DirectoryAccessCounter.DirectoryAccessedSuccessfullyBySpokenName);
			}
			else if (this.dtmfInputForDeptMenu)
			{
				base.PerfCounters.IncrementSingleCounter(DirectoryAccessCountersUtil.DirectoryAccessCounter.DirectoryAccess);
			}
			this.SetSelectedMenuOption((int)departmentResult.KeyPress);
			departmentResult.SetManagerVariables((ActivityManager)base.AutoAttendantManager, base.VoiceObject);
			return result;
		}

		internal string Action_ProcessResult(AsrExtensionSearchResult extensionResult)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, this, "SpeechAutoAttendant::Action_ProcessResult(<Extension>).", new object[0]);
			RecipientLookup recipientLookup = RecipientLookup.Create(base.VoiceObject);
			DialPermissionWrapper dialPermissionWrapper = DialPermissionWrapperFactory.Create(base.VoiceObject);
			extensionResult.SetManagerVariables((ActivityManager)base.AutoAttendantManager, base.VoiceObject);
			ADRecipient adrecipient = recipientLookup.LookupByExtension(extensionResult.Extension.Number, base.VoiceObject, DirectorySearchPurpose.Both, dialPermissionWrapper.ContactScope);
			string result;
			if (adrecipient == null)
			{
				result = "unreachableUser";
				if (dialPermissionWrapper.CallingNonUmExtensionsAllowed && extensionResult.Extension.IsValid(base.VoiceObject.CurrentCallContext.DialPlan) && !extensionResult.Extension.StartsWithTrunkAccessCode(base.VoiceObject.CurrentCallContext.DialPlan))
				{
					ActivityManager manager = (ActivityManager)base.AutoAttendantManager;
					PhoneUtil.SetTransferTargetPhone(manager, TransferExtension.UserExtension, extensionResult.Extension);
					result = "allowCallOnly";
				}
			}
			else
			{
				result = this.ProcessDialingPermissions(adrecipient);
			}
			return result;
		}

		internal string Action_ProcessResult(AsrDirectorySearchResult userResult)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, this, "SpeechAutoAttendant::Action_ProcessResult(<DirectoryResult>).", new object[0]);
			base.PerfCounters.Increment(DirectoryAccessCountersUtil.DirectoryAccessCounter.DirectoryAccessedSuccessfullyBySpokenName);
			userResult.SetManagerVariables((ActivityManager)base.AutoAttendantManager, base.VoiceObject);
			ADRecipient recipient = userResult.Recipient;
			return this.ProcessDialingPermissions(recipient);
		}

		internal string Action_PrepareForANROperatorTransfer()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, this, "Action_PrepareForANROperatorTransfer().", new object[0]);
			base.PerfCounters.IncrementANRTransfersToOperatorCounter();
			return null;
		}

		internal string ProcessDialingPermissions(ADRecipient recipient)
		{
			DialingPermissionsCheck dialingPermissionsCheck = DialingPermissionsCheckFactory.Create(base.VoiceObject);
			DialingPermissionsCheck.DialingPermissionsCheckResult perms = dialingPermissionsCheck.CheckDirectoryUser(recipient, null);
			AnonCallerUtils.SetVariables(recipient, perms, (ActivityManager)base.AutoAttendantManager);
			ActivityManager activityManager = (ActivityManager)base.AutoAttendantManager;
			bool flag = false;
			ActivityManager activityManager2 = activityManager;
			string variableName = "userName";
			string recipientName;
			if ((recipientName = recipient.DisplayName) == null)
			{
				recipientName = (recipient.Alias ?? string.Empty);
			}
			activityManager2.SetRecordedName(variableName, recipientName, recipient, true, Util.GetDisambiguationField(base.CallContext), ref flag);
			object varValue = activityManager.ReadVariable("userName");
			activityManager.GlobalManager.WriteVariable("userName", varValue);
			return AnonCallerUtils.GetAutoEvent(perms);
		}

		internal string ConfigureDtmfFallbackAction()
		{
			bool flag = false;
			bool flag2 = false;
			if (base.BusinessHoursCall)
			{
				if (this.dtmfFallbackEnabled)
				{
					flag = true;
				}
				else if (base.OperatorEnabled)
				{
					flag2 = true;
				}
			}
			else if (this.dtmfFallbackEnabled)
			{
				flag = true;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, this, "DTMFFallbackAction: ToOperator:{0} ToDtmfAA:{1}.", new object[]
			{
				flag2,
				flag
			});
			base.WriteVariable("aa_goto_dtmf_autoattendant", flag);
			base.WriteVariable("aa_goto_operator", flag2);
			return null;
		}

		internal void SetVariables(AsrSearchManager autoAttendantManager)
		{
			if (base.CustomizedMenuConfigured)
			{
				for (int i = 0; i < base.CustomMenu.Length; i++)
				{
					CustomMenuKeyMapping customMenuKeyMapping = base.CustomMenu[i];
					if (customMenuKeyMapping.MappedKey == CustomMenuKey.Timeout)
					{
						autoAttendantManager.WriteVariable("TimeoutOption", true);
					}
					else
					{
						int mappedKey = (int)customMenuKeyMapping.MappedKey;
						if (mappedKey >= 1 && mappedKey <= 9)
						{
							autoAttendantManager.WriteVariable(string.Format(CultureInfo.InvariantCulture, "DtmfKey{0}", new object[]
							{
								mappedKey
							}), true);
						}
					}
				}
			}
		}

		private string GetTarget()
		{
			AsrDepartmentSearchResult asrDepartmentSearchResult = (AsrDepartmentSearchResult)base.AutoAttendantManager.ReadProperty("searchResult");
			return asrDepartmentSearchResult.TransferTarget;
		}

		private string Action_SetCustomExtensionNumber()
		{
			AsrSearchResult asrSearchResult = (AsrSearchResult)base.AutoAttendantManager.ReadProperty("searchResult");
			AsrDepartmentSearchResult asrDepartmentSearchResult = asrSearchResult as AsrDepartmentSearchResult;
			string result;
			if (asrDepartmentSearchResult != null)
			{
				PhoneNumber phoneNumber = asrDepartmentSearchResult.PhoneNumber;
				if (phoneNumber == null)
				{
					return "cannotTransferToCustomExtension";
				}
				PhoneUtil.SetTransferTargetPhone((ActivityManager)base.AutoAttendantManager, TransferExtension.CustomMenuExtension, phoneNumber);
				result = null;
			}
			else
			{
				result = null;
			}
			return result;
		}

		private void ConfigureDepartmentPrompt()
		{
			if (!base.CustomizedMenuConfigured)
			{
				return;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, this, "Customized menu: #options = {0}.", new object[]
			{
				base.NumCustomizedMenuOptions
			});
			List<string> list = new List<string>();
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
					list.Add(customMenuKeyMapping.Description);
				}
			}
			if (list.Count > 0)
			{
				base.WriteVariable("selectableDepartments", list);
			}
			string description = base.CustomMenu[0].Description;
			CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, this, "Generating first department prompt.", new object[0]);
			CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, this, "First Department: {0}.", new object[]
			{
				description
			});
			base.AutoAttendantManager.SetTextPrompt("firstDepartment", description);
			CustomGrammarFile value = new CustomGrammarFile(base.CustomMenu, "Department", base.CallContext.Culture, base.Config.Name);
			base.WriteVariable("customizedMenuGrammar", value);
		}

		private bool SetSelectedMenuOption(int key)
		{
			CustomMenuKeyMapping[] keyMapping = base.Settings.KeyMapping;
			CustomMenuKeyMapping selectedMenu;
			if (AutoAttendantCore.GetExtensionForKey(key, keyMapping, out selectedMenu))
			{
				base.SelectedMenu = selectedMenu;
				return true;
			}
			return false;
		}

		private bool incrementedSpeechCallCounter;

		private bool asrNameSpoken;

		private bool dtmfFallbackEnabled;

		private bool dtmfInputForDeptMenu;
	}
}
