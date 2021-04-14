using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class DirectorySearchManager : ActivityManager
	{
		internal DirectorySearchManager(ActivityManager manager, DirectorySearchManager.ConfigClass config) : base(manager, config)
		{
			DirectorySearchManager.reverseDtmfMap = new string[][]
			{
				new string[]
				{
					"0"
				},
				new string[]
				{
					"1"
				},
				new string[]
				{
					"a",
					"b",
					"c",
					"2"
				},
				new string[]
				{
					"d",
					"e",
					"f",
					"3"
				},
				new string[]
				{
					"g",
					"h",
					"i",
					"4"
				},
				new string[]
				{
					"j",
					"k",
					"l",
					"5"
				},
				new string[]
				{
					"m",
					"n",
					"o",
					"6"
				},
				new string[]
				{
					"p",
					"q",
					"r",
					"s",
					"7"
				},
				new string[]
				{
					"t",
					"u",
					"v",
					"8"
				},
				new string[]
				{
					"w",
					"x",
					"y",
					"z",
					"9"
				}
			};
		}

		public bool CanSendEmail
		{
			get
			{
				if (this.selectedResult.IsGroup)
				{
					return this.selectedResult.GroupHasEmail;
				}
				return !string.IsNullOrEmpty(this.selectedResult.PrimarySmtpAddress);
			}
		}

		public ContactSearchItem SelectedSearchItem
		{
			get
			{
				return this.selectedResult;
			}
		}

		public static DialScopeEnum GetScopeForPurpose(DialPermissionWrapper dp, DirectorySearchPurpose purpose)
		{
			switch (purpose)
			{
			case DirectorySearchPurpose.Call:
			case DirectorySearchPurpose.SendMessage:
			case DirectorySearchPurpose.Both:
				return dp.ContactScope;
			default:
				CallIdTracer.TraceError(ExTraceGlobals.DirectorySearchTracer, null, "Unexpected purpose value: {0}.", new object[]
				{
					purpose
				});
				throw new UnexpectedSwitchValueException(purpose.ToString());
			}
		}

		internal static SearchMethod ConvertMode(DialByNamePrimaryEnum dbn)
		{
			SearchMethod result;
			switch (dbn)
			{
			case DialByNamePrimaryEnum.LastFirst:
				result = SearchMethod.LastNameFirstName;
				break;
			case DialByNamePrimaryEnum.FirstLast:
				result = SearchMethod.FirstNameLastName;
				break;
			case DialByNamePrimaryEnum.SMTPAddress:
				result = SearchMethod.EmailAlias;
				break;
			default:
				CallIdTracer.TraceError(ExTraceGlobals.DirectorySearchTracer, null, "Unexpected dbn value: {0}.", new object[]
				{
					dbn
				});
				throw new UnexpectedSwitchValueException(dbn.ToString());
			}
			return result;
		}

		internal static SearchMethod ConvertMode(DialByNameSecondaryEnum dbn)
		{
			SearchMethod result;
			switch (dbn)
			{
			case DialByNameSecondaryEnum.LastFirst:
				result = SearchMethod.LastNameFirstName;
				break;
			case DialByNameSecondaryEnum.FirstLast:
				result = SearchMethod.FirstNameLastName;
				break;
			case DialByNameSecondaryEnum.SMTPAddress:
				result = SearchMethod.EmailAlias;
				break;
			case DialByNameSecondaryEnum.None:
				result = SearchMethod.None;
				break;
			default:
				CallIdTracer.TraceError(ExTraceGlobals.DirectorySearchTracer, null, "Unexpected dbn value: {0}.", new object[]
				{
					dbn.ToString()
				});
				throw new UnexpectedSwitchValueException(dbn.ToString());
			}
			return result;
		}

		internal static bool DialableNonUmExtension(string dtmfDigits, UMDialPlan dialPlan, out PhoneNumber phoneNumber)
		{
			bool result = false;
			phoneNumber = null;
			PhoneNumber phoneNumber2 = null;
			if (PhoneNumber.TryParse(dialPlan, dtmfDigits, out phoneNumber2) && ((phoneNumber2.Kind == PhoneNumberKind.Extension && !phoneNumber2.StartsWithTrunkAccessCode(dialPlan)) || (phoneNumber2.UriType == UMUriType.TelExtn && dialPlan.URIType == UMUriType.SipName)))
			{
				result = true;
				phoneNumber = phoneNumber2;
			}
			return result;
		}

		internal override void Start(BaseUMCallSession vo, string refInfo)
		{
			if (vo.CurrentCallContext.CallType == 2)
			{
				this.directoryAccessCounters = new AutoAttendantCountersUtil(vo);
			}
			else
			{
				this.directoryAccessCounters = new DirectorySearchCountersUtil(vo);
			}
			this.testCall = vo.CurrentCallContext.IsTestCall;
			this.originatingDialPlan = vo.CurrentCallContext.DialPlan;
			this.autoAttendantCall = (vo.CurrentCallContext.CallType == 2);
			this.numOfAttempts = 0;
			this.GetSearchModeFromConfig(vo);
			this.ReadVariablesFromManager();
			this.SetSearchTargetToGlobalAddressList();
			UMSubscriber callerInfo = vo.CurrentCallContext.CallerInfo;
			this.authenticatedUser = (callerInfo != null && callerInfo.IsAuthenticated);
			base.WriteVariable("none", SearchMethod.None.ToString());
			base.WriteVariable("firstNameLastName", SearchMethod.FirstNameLastName.ToString());
			base.WriteVariable("lastNameFirstName", SearchMethod.LastNameFirstName.ToString());
			base.WriteVariable("emailAlias", SearchMethod.EmailAlias.ToString());
			base.WriteVariable("companyName", SearchMethod.CompanyName.ToString());
			base.WriteVariable("globalAddressList", SearchTarget.GlobalAddressList.ToString());
			base.WriteVariable("personalContacts", SearchTarget.PersonalContacts.ToString());
			base.WriteVariable("primarySearchMode", this.primarySearchMode.ToString());
			base.WriteVariable("secondarySearchMode", this.secondarySearchMode.ToString());
			base.WriteVariable("currentSearchMode", this.currentSearchMode.ToString());
			base.WriteVariable("searchTarget", this.currentSearchTarget.ToString());
			base.WriteVariable("authenticatedUser", this.authenticatedUser);
			this.dialPermissions = DialPermissionWrapperFactory.Create(vo);
			if (vo.CurrentCallContext.CallType == 1)
			{
				bool flag = !string.IsNullOrEmpty(vo.CurrentCallContext.DialPlan.OperatorExtension);
				base.WriteVariable("pilotNumberTransferToOperatorEnabled", flag);
			}
			this.disambiguationField = Util.GetDisambiguationField(vo.CurrentCallContext);
			this.lookup = RecipientLookup.Create(vo);
			if (vo.CurrentCallContext.CallType == 3)
			{
				this.resultHandler = new DirectorySearchManager.HandleResultsDelegate(this.HandleResults_SubscriberAccess);
			}
			else
			{
				this.resultHandler = new DirectorySearchManager.HandleResultsDelegate(this.HandleResults_AnonymousCaller);
			}
			this.StartNewSearch();
			base.Start(vo, refInfo);
		}

		internal override void CheckAuthorization(UMSubscriber u)
		{
		}

		internal override TransitionBase ExecuteAction(string action, BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, "DirectorySearch Manager asked to do action: {0}.", new object[]
			{
				action
			});
			string input = null;
			if (string.Equals(action, "changeSearchMode", StringComparison.OrdinalIgnoreCase))
			{
				this.ToggleSearchMode();
			}
			else if (string.Equals(action, "continueSearch", StringComparison.OrdinalIgnoreCase))
			{
				this.ContinueSearch();
			}
			else if (string.Equals(action, "setSearchTargetToGlobalAddressList", StringComparison.OrdinalIgnoreCase))
			{
				this.SetSearchTargetToGlobalAddressList();
			}
			else if (string.Equals(action, "setSearchTargetToContacts", StringComparison.OrdinalIgnoreCase))
			{
				this.SetSearchTargetToContacts();
			}
			else if (string.Equals(action, "changeSearchTarget", StringComparison.OrdinalIgnoreCase))
			{
				this.ToggleSearchTarget();
			}
			else if (string.Equals(action, "startNewSearch", StringComparison.OrdinalIgnoreCase))
			{
				this.StartNewSearch();
			}
			else if (string.Equals(action, "searchDirectoryByExtension", StringComparison.OrdinalIgnoreCase))
			{
				input = this.SearchDirectoryByExtension(vo);
			}
			else if (string.Equals(action, "checkNonUmExtension", StringComparison.OrdinalIgnoreCase))
			{
				input = this.CheckNonUmExtension(vo);
			}
			else if (string.Equals(action, "searchDirectory", StringComparison.OrdinalIgnoreCase))
			{
				input = this.SearchDirectory(vo);
			}
			else if (string.Equals(action, "handleInvalidSearchKey", StringComparison.OrdinalIgnoreCase))
			{
				input = null;
				string value = (string)this.ReadVariable("lastDtmfSearchInput");
				if (!string.IsNullOrEmpty(value))
				{
					this.ContinueSearch();
				}
				else
				{
					this.StartNewSearch();
				}
			}
			else if (string.Equals(action, "anyMoreResultsToPlay", StringComparison.OrdinalIgnoreCase))
			{
				input = this.GetNextResult();
			}
			else if (string.Equals(action, "replayResults", StringComparison.OrdinalIgnoreCase))
			{
				this.InitializeEnumeration();
			}
			else if (string.Equals(action, "validateSearchSelection", StringComparison.OrdinalIgnoreCase))
			{
				input = this.ValidateSearchSelection(vo);
			}
			else if (string.Equals(action, "processResult", StringComparison.OrdinalIgnoreCase))
			{
				input = this.ProcessResult(vo);
			}
			else if (string.Compare(action, "setOperatorNumber", true, CultureInfo.InvariantCulture) == 0)
			{
				input = this.SetOperatorNumber(vo);
			}
			else if (string.Equals(action, "canonicalizeNumber", StringComparison.OrdinalIgnoreCase))
			{
				input = this.CanonicalizeNumber(vo);
			}
			else if (string.Equals(action, "checkDialPermissions", StringComparison.OrdinalIgnoreCase))
			{
				input = this.CheckDialPermissions(vo);
			}
			else if (string.Equals(action, "setMobileNumber", StringComparison.OrdinalIgnoreCase))
			{
				this.numberToDial = this.selectedResult.MobilePhone;
				this.selectedPhoneType = FoundByType.MobilePhone;
			}
			else if (string.Equals(action, "setBusinessNumber", StringComparison.OrdinalIgnoreCase))
			{
				this.numberToDial = this.selectedResult.BusinessPhone;
				this.selectedPhoneType = FoundByType.BusinessPhone;
			}
			else if (string.Equals(action, "setHomeNumber", StringComparison.OrdinalIgnoreCase))
			{
				this.numberToDial = this.selectedResult.HomePhone;
				this.selectedPhoneType = FoundByType.HomePhone;
			}
			else
			{
				if (!string.Equals(action, "playContactDetails", StringComparison.OrdinalIgnoreCase))
				{
					return base.ExecuteAction(action, vo);
				}
				this.Action_PlayContactDetails();
			}
			return base.CurrentActivity.GetTransition(input);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<DirectorySearchManager>(this);
		}

		private void GetSearchModeFromConfig(BaseUMCallSession vo)
		{
			this.galPrimarySearchMode = DirectorySearchManager.ConvertMode(vo.CurrentCallContext.DialPlan.DialByNamePrimary);
			this.galSecondarySearchMode = DirectorySearchManager.ConvertMode(vo.CurrentCallContext.DialPlan.DialByNameSecondary);
			this.contactsPrimarySearchMode = SearchMethod.LastNameFirstName;
		}

		private void Action_PlayContactDetails()
		{
			this.selectedResult.SetVariablesForTts(this, base.CallSession);
			base.CallSession.IncrementCounter(SubscriberAccessCounters.ContactItemsHeard);
		}

		private void IncrementDirectoryCounter(DirectoryAccessCountersUtil.DirectoryAccessCounter counterName)
		{
			if (this.currentSearchTarget == SearchTarget.GlobalAddressList)
			{
				this.directoryAccessCounters.Increment(counterName);
			}
		}

		private string ValidateSearchSelection(ContactSearchItem selectedResult, BaseUMCallSession vo)
		{
			string text;
			switch (vo.CurrentCallContext.CallType)
			{
			case 1:
				text = this.ValidateSearchSelection_PilotNumber(selectedResult, vo);
				break;
			case 2:
				text = this.ValidateSearchSelection_AA(selectedResult, vo);
				break;
			case 3:
				text = this.ValidateSearchSelection_SA(selectedResult, vo);
				break;
			default:
				CallIdTracer.TraceError(ExTraceGlobals.DirectorySearchTracer, this, "Unexpected vo.CurrentCallContext.CallType value: {0}.", new object[]
				{
					vo.CurrentCallContext.CallType.ToString()
				});
				throw new UnexpectedSwitchValueException(vo.CurrentCallContext.CallType.ToString());
			}
			this.WriteSearchResultInContextBag();
			this.SetNextResultForTts(selectedResult);
			CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, "ValidateSearchSelection returning autoEvent: {0}.", new object[]
			{
				text
			});
			return text;
		}

		private void WriteSearchResultInContextBag()
		{
			if (this.selectedResult == null)
			{
				throw new ArgumentNullException("selectedResult");
			}
			base.Manager.WriteVariable("directorySearchResult", this.selectedResult);
			base.WriteVariable("directorySearchResult", this.selectedResult);
		}

		private string ValidateSearchSelection_AA(ContactSearchItem selectedResult, BaseUMCallSession vo)
		{
			string text = "validSelection";
			base.WriteVariable("phoneNumberToDial", this.selectedResult.Phone);
			CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, "ValidateSearchSelection_AA returning autoEvent: {0}.", new object[]
			{
				text
			});
			return text;
		}

		private string ValidateSearchSelection_SA(ContactSearchItem selectedResult, BaseUMCallSession vo)
		{
			string text = "validSelection";
			CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, "ValidateSearchSelection_SA returning autoEvent: {0}.", new object[]
			{
				text
			});
			return text;
		}

		private string ValidateSearchSelection_PilotNumber(ContactSearchItem selectedResult, BaseUMCallSession vo)
		{
			string text = "validSelection";
			base.WriteVariable("phoneNumberToDial", this.selectedResult.Phone);
			CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, "ValidateSearchSelection_PilotNumber returning autoEvent: {0}.", new object[]
			{
				text
			});
			return text;
		}

		private string ProcessResult(BaseUMCallSession vo)
		{
			if (vo.CurrentCallContext.CallType != 3)
			{
				return this.ProcessResultUnauthenticatedCaller(vo);
			}
			return null;
		}

		private string ProcessResultUnauthenticatedCaller(BaseUMCallSession vo)
		{
			ADRecipient recipient = this.selectedResult.Recipient;
			DialingPermissionsCheck dialingPermissionsCheck = DialingPermissionsCheckFactory.Create(vo);
			DialingPermissionsCheck.DialingPermissionsCheckResult perms = dialingPermissionsCheck.CheckDirectoryUser(recipient, null);
			AnonCallerUtils.SetVariables(recipient, perms, this);
			string autoEvent = AnonCallerUtils.GetAutoEvent(perms);
			PIIMessage data = PIIMessage.Create(PIIType._UserDisplayName, recipient.DisplayName);
			CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, data, "ProcessResultUnauthenticatedCaller:: Recipient: _UserDisplayName returning autoevent: {0}.", new object[]
			{
				autoEvent
			});
			return autoEvent;
		}

		private PhoneNumber CanonicalizeNumber(PhoneNumber phone, ContactSearchItem selectedResult, UMDialPlan originatingDialPlan)
		{
			ADRecipient recipient = selectedResult.Recipient;
			if (recipient != null)
			{
				IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromADRecipient(recipient);
				selectedResult.TargetDialPlan = iadsystemConfigurationLookup.GetDialPlanFromRecipient(recipient);
			}
			PhoneNumber phoneNumber = null;
			try
			{
				phoneNumber = DialPermissions.Canonicalize(phone, originatingDialPlan, selectedResult.Recipient, selectedResult.TargetDialPlan);
			}
			catch (ArgumentException ex)
			{
				PIIMessage data = PIIMessage.Create(PIIType._PhoneNumber, phone);
				CallIdTracer.TraceWarning(ExTraceGlobals.DirectorySearchTracer, this, data, "CanonicalizeNumber(_PhoneNumber) got exception \"{0}\".", new object[]
				{
					ex
				});
			}
			PIIMessage[] data2 = new PIIMessage[]
			{
				PIIMessage.Create(PIIType._PhoneNumber, phone),
				PIIMessage.Create(PIIType._PhoneNumber, (phoneNumber == null) ? "<null>" : phoneNumber.ToString())
			};
			CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, data2, "CanonicalizeNumber(_PhoneNumber1) returned \"_PhoneNumber2\".", new object[0]);
			return phoneNumber;
		}

		private bool CheckDialPermissions_SubscriberAccess(PhoneNumber canonicalizedNumber, ADUser subscriber, UMDialPlan originatingDialPlan, UMDialPlan targetDialPlan, out PhoneNumber numberToDial)
		{
			numberToDial = null;
			CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, "ENTER: CheckDialPermissions_SubscriberAccess.", new object[0]);
			bool flag = DialPermissions.Check(canonicalizedNumber, subscriber, originatingDialPlan, targetDialPlan, out numberToDial);
			PIIMessage[] data = new PIIMessage[]
			{
				PIIMessage.Create(PIIType._PhoneNumber, canonicalizedNumber),
				PIIMessage.Create(PIIType._PhoneNumber, numberToDial)
			};
			CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, data, "AccessCheck(Phone: _PhoneNumber1, Originating DP: {0} TargetDP: {1}\treturned Passed={2} Phone: \"_PhoneNumber2\".", new object[]
			{
				originatingDialPlan.Name,
				(targetDialPlan == null) ? "<null>" : targetDialPlan.Name,
				flag
			});
			return flag;
		}

		private bool CheckDialPermissions_PilotNumber(PhoneNumber canonicalizedNumber, UMDialPlan originatingDialPlan, UMDialPlan targetDialPlan, out PhoneNumber numberToDial)
		{
			numberToDial = null;
			CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, "ENTER: CheckDialPermissions_PilotNumber.", new object[0]);
			bool flag = DialPermissions.Check(canonicalizedNumber, originatingDialPlan, targetDialPlan, out numberToDial);
			PIIMessage[] data = new PIIMessage[]
			{
				PIIMessage.Create(PIIType._PhoneNumber, canonicalizedNumber),
				PIIMessage.Create(PIIType._PhoneNumber, numberToDial)
			};
			CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, data, "AccessCheck_PilotNumber(Phone: _PhoneNumber1, Originating DP: {0} TargetDP: {1}\treturned Passed={2} Phone: \"_PhoneNumber2\".", new object[]
			{
				originatingDialPlan.Name,
				(targetDialPlan == null) ? "<null>" : targetDialPlan.Name,
				flag
			});
			return flag;
		}

		private bool CheckDialPermissions_AutoAttendant(PhoneNumber canonicalizedNumber, UMAutoAttendant autoAttendant, UMDialPlan originatingDialPlan, UMDialPlan targetDialPlan, out PhoneNumber numberToDial)
		{
			numberToDial = null;
			CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, "ENTER: CheckDialPermissions_AutoAttendant.", new object[0]);
			bool flag = DialPermissions.Check(canonicalizedNumber, autoAttendant, originatingDialPlan, targetDialPlan, out numberToDial);
			PIIMessage[] data = new PIIMessage[]
			{
				PIIMessage.Create(PIIType._PhoneNumber, canonicalizedNumber),
				PIIMessage.Create(PIIType._PhoneNumber, numberToDial)
			};
			CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, data, "AccessCheck_AutoAttendant(Phone: _PhoneNumber1, Originating DP: {0} TargetDP: {1}\treturned Passed={2} Phone: \"_PhoneNumber2\".", new object[]
			{
				originatingDialPlan.Name,
				(targetDialPlan == null) ? "<null>" : targetDialPlan.Name,
				flag
			});
			return flag;
		}

		private void ReadVariablesFromManager()
		{
			if (base.Manager is AutoAttendantManager)
			{
				this.GetAndSetVariableFromManager("aa_transferToOperatorEnabled");
				this.GetAndSetVariableFromManager("aa_customizedMenuEnabled");
			}
			string varName = "initialSearchTarget";
			base.WriteVariable(varName, this.GlobalManager.ReadVariable(varName));
		}

		private void GetAndSetVariableFromManager(string varName)
		{
			object obj = base.Manager.ReadVariable(varName);
			CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, "DirectorySearch read from parent AutoAttendantManager variable: name = {0} value = {1}.", new object[]
			{
				varName,
				obj
			});
			base.WriteVariable(varName, obj);
		}

		private void SetSearchTargetToContacts()
		{
			this.currentSearchTarget = SearchTarget.PersonalContacts;
			base.WriteVariable("searchTarget", this.currentSearchTarget.ToString());
			CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, "Setting search target = {0}", new object[]
			{
				this.currentSearchTarget
			});
			this.SetSearchMode(this.contactsPrimarySearchMode, this.contactsPrimarySearchMode);
			this.SetCurrentSearchMode(this.contactsPrimarySearchMode);
			this.StartNewSearch();
		}

		private void SetSearchTargetToGlobalAddressList()
		{
			this.currentSearchTarget = SearchTarget.GlobalAddressList;
			base.WriteVariable("searchTarget", this.currentSearchTarget.ToString());
			CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, "Setting search target = {0}.", new object[]
			{
				this.currentSearchTarget
			});
			this.SetSearchMode(this.galPrimarySearchMode, this.galSecondarySearchMode);
			this.SetCurrentSearchMode(this.galPrimarySearchMode);
			this.StartNewSearch();
		}

		private void SetSearchMode(SearchMethod primary, SearchMethod secondary)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, "Setting primary = {0} secondary = {1}.", new object[]
			{
				primary,
				secondary
			});
			this.primarySearchMode = primary;
			this.secondarySearchMode = secondary;
			base.WriteVariable("primarySearchMode", this.primarySearchMode.ToString());
			base.WriteVariable("secondarySearchMode", this.secondarySearchMode.ToString());
		}

		private void ToggleSearchMode()
		{
			if (this.currentSearchMode == this.primarySearchMode)
			{
				this.SetCurrentSearchMode(this.secondarySearchMode);
			}
			else
			{
				this.SetCurrentSearchMode(this.primarySearchMode);
			}
			this.StartNewSearch();
		}

		private void ToggleSearchTarget()
		{
			if (this.currentSearchTarget == SearchTarget.GlobalAddressList)
			{
				this.SetSearchTargetToContacts();
				return;
			}
			this.SetSearchTargetToGlobalAddressList();
		}

		private void SetCurrentSearchMode(SearchMethod mode)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, "Setting current search mode = {0}.", new object[]
			{
				mode
			});
			this.currentSearchMode = mode;
			base.WriteVariable("currentSearchMode", this.currentSearchMode.ToString());
		}

		private void StartNewSearch()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, "Starting new search", new object[0]);
			base.WriteVariable("newSearch", true);
			base.WriteVariable("lastDtmfSearchInput", string.Empty);
			base.Manager.WriteVariable("directorySearchResult", null);
			this.numResults = 0;
			this.currentResult = -1;
			this.anrOperatorTransfer = false;
		}

		private void ContinueSearch()
		{
			base.WriteVariable("newSearch", false);
		}

		private string CheckNonUmExtension(BaseUMCallSession vo)
		{
			string result = null;
			DialPermissionWrapper dialPermissionWrapper = DialPermissionWrapperFactory.Create(vo);
			if (dialPermissionWrapper.CallingNonUmExtensionsAllowed)
			{
				result = "denyCallNonUmExtension";
				PhoneNumber phoneNumber;
				if (DirectorySearchManager.DialableNonUmExtension(base.DtmfDigits, this.originatingDialPlan, out phoneNumber))
				{
					PIIMessage data = PIIMessage.Create(PIIType._PhoneNumber, phoneNumber);
					CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, data, "CheckNonUmExtension success. Tranferring to extension _PhoneNumber", new object[0]);
					PhoneUtil.SetTransferTargetPhone(this, TransferExtension.UserExtension, phoneNumber);
					if (base.Manager is AutoAttendantManager)
					{
						PhoneUtil.SetTransferTargetPhone(base.Manager, TransferExtension.UserExtension, phoneNumber);
					}
					result = "allowCallNonUmExtension";
				}
			}
			return result;
		}

		private string SearchDirectoryByExtension(BaseUMCallSession vo)
		{
			this.IncrementDirectoryCounter(DirectoryAccessCountersUtil.DirectoryAccessCounter.DirectoryAccessedByExtension);
			this.searchByExtension = true;
			base.WriteVariable("searchByExtension", true);
			string text = this.CheckNumberofAttempts(vo);
			if (text == null)
			{
				string dtmfDigits = base.DtmfDigits;
				DirectorySearchManager.ConfigClass configClass = (DirectorySearchManager.ConfigClass)base.Config;
				DialPermissionWrapper dp = DialPermissionWrapperFactory.Create(vo);
				DialScopeEnum scopeForPurpose = DirectorySearchManager.GetScopeForPurpose(dp, configClass.SearchPurpose);
				ADRecipient adrecipient = this.lookup.LookupByExtension(dtmfDigits, vo, configClass.SearchPurpose, scopeForPurpose);
				if (adrecipient == null)
				{
					text = "noResultsMatched";
				}
				else
				{
					ContactSearchItem contactSearchItem = ContactSearchItem.CreateFromRecipient(adrecipient);
					contactSearchItem.SetBusinessPhoneForDialPlan(vo.CurrentCallContext.DialPlan);
					this.selectedResult = contactSearchItem;
					this.WriteSearchResultInContextBag();
					this.SetNextResultForTts(this.selectedResult);
					this.InitializeSearch(new List<ContactSearchItem>
					{
						contactSearchItem
					}, this.currentSearchMode, dtmfDigits, vo.CurrentCallContext.IsTestCall);
					text = "resultsLessThanAllowed";
				}
			}
			return text;
		}

		private string CanonicalizeNumber(BaseUMCallSession vo)
		{
			PhoneNumber phoneNumber = this.CanonicalizeNumber(this.numberToDial, this.selectedResult, vo.CurrentCallContext.DialPlan);
			string result;
			if (phoneNumber != null)
			{
				result = "validCanonicalNumber";
				this.canonicalizedNumber = phoneNumber;
			}
			else
			{
				result = "numberCanonicalizationFailed";
			}
			return result;
		}

		private string CheckDialPermissions(BaseUMCallSession vo)
		{
			UMDialPlan dialPlan = vo.CurrentCallContext.DialPlan;
			PhoneNumber phone = null;
			bool flag;
			switch (vo.CurrentCallContext.CallType)
			{
			case 1:
				flag = this.CheckDialPermissions_PilotNumber(this.canonicalizedNumber, dialPlan, this.selectedResult.TargetDialPlan, out phone);
				break;
			case 2:
				flag = this.CheckDialPermissions_AutoAttendant(this.canonicalizedNumber, vo.CurrentCallContext.AutoAttendantInfo, dialPlan, this.selectedResult.TargetDialPlan, out phone);
				break;
			case 3:
			{
				ADUser subscriber = vo.CurrentCallContext.CallerInfo.ADRecipient as ADUser;
				flag = this.CheckDialPermissions_SubscriberAccess(this.canonicalizedNumber, subscriber, dialPlan, this.selectedResult.TargetDialPlan, out phone);
				break;
			}
			default:
				CallIdTracer.TraceError(ExTraceGlobals.DirectorySearchTracer, this, "Unexpected vo.CurrentCallContext.CallType value: {0}.", new object[]
				{
					vo.CurrentCallContext.CallType
				});
				throw new UnexpectedSwitchValueException(vo.CurrentCallContext.CallType.ToString());
			}
			string result;
			if (flag)
			{
				ContactInfo targetContactInfo = this.selectedResult.ToContactInfo(this.selectedPhoneType);
				PhoneUtil.SetTransferTargetPhone(this, TransferExtension.UserExtension, phone, targetContactInfo);
				if (base.Manager is AutoAttendantManager)
				{
					PhoneUtil.SetTransferTargetPhone(base.Manager, TransferExtension.UserExtension, phone, targetContactInfo);
				}
				result = null;
			}
			else
			{
				result = "dialingPermissionCheckFailed";
			}
			return result;
		}

		private string SetOperatorNumber(BaseUMCallSession vo)
		{
			string result = null;
			PhoneNumber phoneNumber = null;
			Util.GetOperatorExtension(vo.CurrentCallContext, out phoneNumber);
			CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, "Setting operator extension = {0}.", new object[]
			{
				phoneNumber
			});
			if (this.autoAttendantCall)
			{
				AutoAttendantCountersUtil autoAttendantCountersUtil = new AutoAttendantCountersUtil(vo);
				if (this.anrOperatorTransfer)
				{
					autoAttendantCountersUtil.IncrementANRTransfersToOperatorCounter();
				}
			}
			PhoneUtil.SetTransferTargetPhone(this, TransferExtension.Operator, phoneNumber);
			if (base.Manager is AutoAttendantManager)
			{
				PhoneUtil.SetTransferTargetPhone(base.Manager, TransferExtension.Operator, phoneNumber);
			}
			return result;
		}

		private string ValidateSearchSelection(BaseUMCallSession vo)
		{
			string dtmfDigits = base.DtmfDigits;
			this.selectedResult = null;
			bool flag = DirectorySearchManager.selectionRegex.IsMatch(dtmfDigits);
			string text;
			if (flag)
			{
				int num = int.Parse(dtmfDigits, CultureInfo.InvariantCulture);
				if (num > 0 && num <= this.currentResult)
				{
					if (this.currentSearchTarget == SearchTarget.GlobalAddressList)
					{
						this.selectedResult = this.userArray[num];
						this.selectedResult.SetBusinessPhoneForDialPlan(vo.CurrentCallContext.DialPlan);
					}
					else if (this.currentSearchTarget == SearchTarget.PersonalContacts)
					{
						this.selectedResult = ContactSearchItem.GetSelectedSearchItemFromId(vo.CurrentCallContext.CallerInfo, this.userArray[num].Id);
					}
					text = this.ValidateSearchSelection(this.selectedResult, vo);
					if (!this.searchByExtension)
					{
						this.IncrementDirectoryCounter(DirectoryAccessCountersUtil.DirectoryAccessCounter.DirectoryAccessedSuccessfullyByDialByName);
					}
				}
				else
				{
					text = "invalidSelection";
				}
			}
			else
			{
				text = "invalidSelection";
			}
			if (text == "invalidSelection")
			{
				base.WriteVariable("invalidSearchSelection", dtmfDigits);
			}
			return text;
		}

		private string SearchDirectory(BaseUMCallSession vo)
		{
			this.anrOperatorTransfer = false;
			this.IncrementDirectoryCounter(DirectoryAccessCountersUtil.DirectoryAccessCounter.DirectoryAccessedByDialByName);
			this.searchByExtension = false;
			base.WriteVariable("searchByExtension", false);
			string text = this.CheckNumberofAttempts(vo);
			if (text == null)
			{
				string text2 = base.DtmfDigits;
				text2 = text2.TrimEnd(new char[]
				{
					'#'
				});
				if (string.IsNullOrEmpty(text2))
				{
					text2 = base.DtmfDigits;
				}
				bool flag = DirectorySearchManager.searchKeyRegex.IsMatch(text2);
				this.numResults = 0;
				this.currentResult = -1;
				if (flag)
				{
					if (!(bool)this.ReadVariable("newSearch"))
					{
						string str = (string)this.ReadVariable("lastDtmfSearchInput");
						text2 = str + text2;
					}
					base.WriteVariable("lastDtmfSearchInput", text2);
					text = this.SearchDirectory(vo, vo.CurrentCallContext.CallerInfo, vo.CurrentCallContext.DialPlan, text2, vo.CurrentCallContext.IsTestCall);
				}
				else
				{
					text = "invalidSearchKey";
					base.WriteVariable("searchInput", text2);
				}
			}
			return text;
		}

		private string SearchDirectory(BaseUMCallSession vo, UMSubscriber caller, UMDialPlan originatingDialPlan, string dtmf, bool testCall)
		{
			string text = "resultsMoreThanAllowed";
			try
			{
				if (this.currentSearchTarget == SearchTarget.GlobalAddressList)
				{
					text = this.SearchGlobalDirectory(vo, caller, originatingDialPlan, dtmf, testCall);
				}
			}
			catch (ADSizelimitExceededException ex)
			{
				CallIdTracer.TraceWarning(ExTraceGlobals.DirectorySearchTracer, this, "ADSizelimitExceededException -> returning ResultsMoreThanAllowed: {0}.", new object[]
				{
					ex
				});
				text = "resultsMoreThanAllowed";
			}
			catch (ADTransientException ex2)
			{
				CallIdTracer.TraceError(ExTraceGlobals.DirectorySearchTracer, this, "Exception while searching AD {0}.", new object[]
				{
					ex2
				});
				text = "adTransientError";
			}
			catch (ADOperationException ex3)
			{
				CallIdTracer.TraceError(ExTraceGlobals.DirectorySearchTracer, this, "Exception while searching AD {0}.", new object[]
				{
					ex3
				});
				text = "adTransientError";
			}
			catch (DataSourceOperationException ex4)
			{
				CallIdTracer.TraceError(ExTraceGlobals.DirectorySearchTracer, this, "Exception while searching AD {0}.", new object[]
				{
					ex4
				});
				text = "adTransientError";
			}
			catch (DataValidationException ex5)
			{
				CallIdTracer.TraceError(ExTraceGlobals.DirectorySearchTracer, this, "Exception while searching AD {0}.", new object[]
				{
					ex5
				});
				text = "adTransientError";
			}
			try
			{
				if (this.currentSearchTarget == SearchTarget.PersonalContacts)
				{
					text = this.SearchPersonalContacts(caller, dtmf, testCall);
				}
			}
			catch (StorageTransientException ex6)
			{
				CallIdTracer.TraceError(ExTraceGlobals.DirectorySearchTracer, this, "Exception while searching personal contacts {0}.", new object[]
				{
					ex6
				});
				text = "adTransientError";
			}
			catch (StoragePermanentException ex7)
			{
				CallIdTracer.TraceError(ExTraceGlobals.DirectorySearchTracer, this, "Exception while searching personal contacts {0}.", new object[]
				{
					ex7
				});
				text = "adTransientError";
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, "SearchDirectory() returning autoevent: {0}.", new object[]
			{
				text
			});
			return text;
		}

		private string SearchPersonalContacts(UMSubscriber caller, string dtmf, bool testCall)
		{
			PIIMessage data = PIIMessage.Create(PIIType._UserDisplayName, caller.DisplayName);
			if (!caller.HasContactsFolder)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, data, "Searching Contacts: Caller = _UserDisplayName does not have a contacts folder.", new object[0]);
				return "noResultsMatched";
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, data, "Searching Contacts: Caller = _UserDisplayName DTMF = {0} SearchMode = {1}.", new object[]
			{
				dtmf,
				this.currentSearchMode
			});
			int num = int.Parse(dtmf.Substring(0, 1), CultureInfo.InvariantCulture);
			string[] array = DirectorySearchManager.reverseDtmfMap[num];
			List<ContactSearchItem> list = new List<ContactSearchItem>();
			foreach (string prefix in array)
			{
				this.AddPersonalContactsWithPrefix(list, caller, prefix, this.currentSearchMode, false);
				this.AddPersonalContactsWithPrefix(list, caller, prefix, this.currentSearchMode, true);
				this.AddPersonalGroupsWithPrefix(list, caller, prefix);
			}
			List<ContactSearchItem> list2 = new List<ContactSearchItem>();
			foreach (ContactSearchItem contactSearchItem in list)
			{
				PIIMessage data2 = PIIMessage.Create(PIIType._User, contactSearchItem);
				CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, data2, "Processing Match = _User", new object[0]);
				bool flag = false;
				if (contactSearchItem.ContactLastNameFirstNameDtmfMap.StartsWith(dtmf, StringComparison.InvariantCulture))
				{
					list2.Add(contactSearchItem);
					flag = true;
				}
				if (flag)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, data2, "Added Match = _User", new object[0]);
				}
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, "Searching Contacts: Got {0} Matches.", new object[]
			{
				list2.Count
			});
			string result;
			if (list2.Count <= 0)
			{
				result = "noResultsMatched";
			}
			else if (list2.Count > Constants.DirectorySearch.MaxResultsToDisplay)
			{
				result = "resultsMoreThanAllowed";
			}
			else
			{
				result = "resultsLessThanAllowed";
				this.InitializeSearch(list2, this.currentSearchMode, dtmf, testCall);
			}
			return result;
		}

		private void AddPersonalContactsWithPrefix(List<ContactSearchItem> al, UMSubscriber caller, string prefix, SearchMethod searchMode, bool missingLastName)
		{
			IDictionary<PropertyDefinition, object> dictionary = new SortedDictionary<PropertyDefinition, object>();
			if (searchMode == SearchMethod.LastNameFirstName)
			{
				if (!missingLastName)
				{
					dictionary.Add(ContactSchema.Surname, prefix);
				}
				else
				{
					dictionary.Add(ContactSchema.GivenName, prefix);
				}
			}
			ContactSearchItem.AddSearchItems(caller, dictionary, al, Constants.DirectorySearch.MaxPersonalContacts);
		}

		private void AddPersonalGroupsWithPrefix(List<ContactSearchItem> al, UMSubscriber owner, string prefix)
		{
			ContactSearchItem.AddSearchItems(owner, new SortedDictionary<PropertyDefinition, object>
			{
				{
					StoreObjectSchema.ItemClass,
					"IPM.DistList"
				},
				{
					StoreObjectSchema.DisplayName,
					prefix
				}
			}, al, Constants.DirectorySearch.MaxPersonalContacts);
		}

		private string SearchGlobalDirectory(BaseUMCallSession vo, UMSubscriber caller, UMDialPlan originatingDialPlan, string dtmf, bool testCall)
		{
			DirectorySearchManager.ConfigClass configClass = (DirectorySearchManager.ConfigClass)base.Config;
			DialScopeEnum scopeForPurpose = DirectorySearchManager.GetScopeForPurpose(this.dialPermissions, configClass.SearchPurpose);
			UMDialPlan targetDialPlan = (scopeForPurpose == DialScopeEnum.DialPlan) ? this.originatingDialPlan : null;
			bool anonymousCaller = vo.CurrentCallContext.CallType != 3;
			int num = this.lookup.Lookup(dtmf, this.currentSearchMode, true, true, anonymousCaller, targetDialPlan);
			if (testCall)
			{
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_DirectorySearchResults, null, new object[]
				{
					this.currentSearchMode.ToString() + ":" + dtmf,
					"Count = " + num
				});
			}
			base.WriteVariable("exactMatches", this.lookup.ExactMatches.Count);
			base.WriteVariable("partialMatches", this.lookup.PartialMatches.Count);
			string text;
			if (num == 0)
			{
				if (this.executingPromptForAlias)
				{
					text = this.HandleResults_PromptForAlias(dtmf, 0, 0);
				}
				else
				{
					text = this.resultHandler(dtmf, 0, 0);
				}
			}
			else if (num > Constants.DirectorySearch.MaxResultsToPreprocess)
			{
				text = "resultsMoreThanAllowed";
			}
			else
			{
				this.lookup.PostProcess(dtmf, this.currentSearchMode, configClass.SearchPurpose);
				if (testCall)
				{
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_DirectorySearchResults, null, new object[]
					{
						this.currentSearchMode.ToString() + ":" + dtmf,
						string.Format(CultureInfo.InvariantCulture, "Exact = {0}, Partial = {1}", new object[]
						{
							this.lookup.ExactMatches.Count,
							this.lookup.PartialMatches.Count
						})
					});
				}
				base.WriteVariable("exactMatches", this.lookup.ExactMatches.Count);
				base.WriteVariable("partialMatches", this.lookup.PartialMatches.Count);
				if (this.executingPromptForAlias)
				{
					text = this.HandleResults_PromptForAlias(dtmf, this.lookup.PartialMatches.Count, this.lookup.ExactMatches.Count);
				}
				else
				{
					text = this.resultHandler(dtmf, this.lookup.PartialMatches.Count, this.lookup.ExactMatches.Count);
				}
				if (!this.executingPromptForAlias && string.Compare(text, "promptForAlias", StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.SetModeToPromptForAlias();
				}
			}
			return text;
		}

		private void SetModeToPromptForAlias()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, "SetModeToPromptForAlias().", new object[0]);
			this.StartNewSearch();
			this.executingPromptForAlias = true;
			this.SetCurrentSearchMode(SearchMethod.PromptForAlias);
			this.lookup.SetSearchInExistingResults();
		}

		private void InitializeSearch(List<ContactSearchItem> filtered, SearchMethod searchMode, string dtmf, bool testCall)
		{
			filtered.Sort(DirectorySearchManager.SearchItemComparer.StaticInstance);
			this.numResults = filtered.Count;
			ContactSearchItem[] array = filtered.ToArray();
			this.userArray = new ContactSearchItem[array.Length + 1];
			Array.Copy(array, 0, this.userArray, 1, array.Length);
			this.InitializeEnumeration();
			if (testCall)
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < array.Length; i++)
				{
					stringBuilder.AppendFormat("Name: {0} Email: {1} Phone: {2}", array[i].FullName, array[i].PrimarySmtpAddress, array[i].Phone);
					stringBuilder.Append("\n");
				}
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_DirectorySearchResults, null, new object[]
				{
					searchMode.ToString() + ":" + dtmf,
					stringBuilder.ToString()
				});
			}
		}

		private string GetNextResult()
		{
			string result;
			if (this.currentResult >= this.numResults)
			{
				result = "noMoreResultsToPlay";
			}
			else
			{
				this.currentResult++;
				ContactSearchItem nextResultForTts = this.userArray[this.currentResult];
				this.SetNextResultForTts(nextResultForTts);
				result = "moreResultsToPlay";
			}
			return result;
		}

		private string CheckNumberofAttempts(BaseUMCallSession vo)
		{
			string result = null;
			if (vo.CurrentCallContext.CallType == 3)
			{
				return null;
			}
			if (++this.numOfAttempts > DirectorySearchManager.maxSearchAttempts)
			{
				result = "maxNumberOfTriesExceeded";
			}
			return result;
		}

		private void InitializeEnumeration()
		{
			this.currentResult = 1;
			ContactSearchItem nextResultForTts = this.userArray[this.currentResult];
			this.SetNextResultForTts(nextResultForTts);
		}

		private void SetNextResultForTts(ContactSearchItem u)
		{
			bool flag = false;
			if (this.currentSearchTarget == SearchTarget.PersonalContacts)
			{
				base.SetTextPartVariable("userName", u.FullName ?? u.PrimarySmtpAddress);
			}
			else
			{
				string variableName = "userName";
				string recipientName;
				if ((recipientName = u.Recipient.DisplayName) == null)
				{
					recipientName = (u.Recipient.Alias ?? string.Empty);
				}
				base.SetRecordedName(variableName, recipientName, u.Recipient, u.NeedDisambiguation, this.disambiguationField, ref flag);
			}
			u.SetVariablesForTts(this, base.CallSession);
			base.WriteVariable("promptindex", this.currentResult);
		}

		private string HandleResults_PromptForAlias(string dtmf, int partial, int exact)
		{
			List<ContactSearchItem> list = new List<ContactSearchItem>();
			int num = partial + exact;
			bool flag = false;
			CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, "HandleResults_PromptForAlias(dtmf={0} Partial={1} Exact={2}).", new object[]
			{
				dtmf,
				partial,
				exact
			});
			if (num == 0)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, "Got Zero results, returning NoResultsMatched.", new object[0]);
				return "noResultsMatched";
			}
			if (exact > Constants.DirectorySearch.MaxResultsToDisplay)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, "Too many exact results.", new object[0]);
				return "resultsMoreThanAllowed";
			}
			string result;
			if (num <= Constants.DirectorySearch.MaxResultsToDisplay)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, "Displaying all results, as they are less than maxresultstodisplay.", new object[0]);
				list.AddRange(this.lookup.ExactMatches);
				list.AddRange(this.lookup.PartialMatches);
				this.InitializeSearch(list, SearchMethod.EmailAlias, dtmf, this.testCall);
				result = "resultsLessThanAllowed";
			}
			else
			{
				if (exact <= 0)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, "Too many partial matches, and no exact matches.", new object[0]);
					return "resultsMoreThanAllowed";
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, "Displaying only exact matches.", new object[0]);
				list.AddRange(this.lookup.ExactMatches);
				this.InitializeSearch(list, SearchMethod.EmailAlias, dtmf, this.testCall);
				result = "resultsLessThanAllowed";
				flag = true;
			}
			base.WriteVariable("haveMorePartialMatches", flag);
			return result;
		}

		private string HandleResults_AnonymousCaller(string dtmf, int partial, int exact)
		{
			int num = partial + exact;
			bool flag = DisambiguationFieldEnum.PromptForAlias == this.disambiguationField;
			CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, "HandleResults_AnonymousCaller(PromptForAlias={3} dtmf={0} Partial={1} Exact={2}).", new object[]
			{
				dtmf,
				partial,
				exact,
				flag
			});
			if (num == 0)
			{
				base.WriteVariable("exceedRetryLimit", this.numOfAttempts + 1 > DirectorySearchManager.maxSearchAttempts);
				return "noResultsMatched";
			}
			if (exact <= Constants.DirectorySearch.MaxResultsToDisplay)
			{
				bool flag2 = false;
				string text;
				if (num > Constants.DirectorySearch.MaxResultsToDisplay)
				{
					if (exact == 0)
					{
						text = "resultsMoreThanAllowed";
					}
					else if (exact > 1 && flag)
					{
						text = "promptForAlias";
					}
					else
					{
						text = "resultsLessThanAllowed";
						this.InitializeSearch(this.lookup.ExactMatches, this.currentSearchMode, dtmf, this.testCall);
						flag2 = true;
					}
				}
				else if (exact > 1 && flag)
				{
					text = "promptForAlias";
				}
				else
				{
					List<ContactSearchItem> list = new List<ContactSearchItem>();
					list.AddRange(this.lookup.ExactMatches);
					list.AddRange(this.lookup.PartialMatches);
					text = "resultsLessThanAllowed";
					this.InitializeSearch(list, this.currentSearchMode, dtmf, this.testCall);
				}
				base.WriteVariable("haveMorePartialMatches", flag2);
				CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, "HandleResults_AnonymousCaller() returning autoEvent: {0}", new object[]
				{
					text
				});
				return text;
			}
			if (flag)
			{
				return "promptForAlias";
			}
			this.anrOperatorTransfer = true;
			return "ambiguousMatches";
		}

		private string HandleResults_SubscriberAccess(string dtmf, int partial, int exact)
		{
			int num = partial + exact;
			CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, "HandleResults_SubscriberAccess(dtmf={0} Partial={1} Exact={2}).", new object[]
			{
				dtmf,
				partial,
				exact
			});
			string text;
			if (num == 0)
			{
				base.WriteVariable("exceedRetryLimit", this.numOfAttempts + 1 > DirectorySearchManager.maxSearchAttempts);
				text = "noResultsMatched";
			}
			else if (num > Constants.DirectorySearch.MaxResultsToDisplay)
			{
				text = "resultsMoreThanAllowed";
				if (exact < Constants.DirectorySearch.MaxResultsToDisplay && exact > 0)
				{
					text = "resultsLessThanAllowed";
					this.InitializeSearch(this.lookup.ExactMatches, this.currentSearchMode, dtmf, this.testCall);
				}
			}
			else
			{
				List<ContactSearchItem> list = new List<ContactSearchItem>();
				list.AddRange(this.lookup.ExactMatches);
				list.AddRange(this.lookup.PartialMatches);
				text = "resultsLessThanAllowed";
				this.InitializeSearch(list, this.currentSearchMode, dtmf, this.testCall);
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, "HandleResults_SubscriberAccess() returning autoEvent: {0}", new object[]
			{
				text
			});
			return text;
		}

		private static Regex selectionRegex = new Regex("^\\d$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private static Regex searchKeyRegex = new Regex("^[0-9]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private static string[][] reverseDtmfMap;

		private static int maxSearchAttempts = 3;

		private SearchMethod primarySearchMode;

		private SearchMethod secondarySearchMode;

		private SearchMethod galPrimarySearchMode;

		private SearchMethod galSecondarySearchMode;

		private SearchMethod contactsPrimarySearchMode;

		private SearchMethod currentSearchMode;

		private SearchTarget currentSearchTarget;

		private bool authenticatedUser;

		private bool testCall;

		private int numOfAttempts;

		private DirectoryAccessCountersUtil directoryAccessCounters;

		private int numResults;

		private int currentResult;

		private ContactSearchItem[] userArray;

		private ContactSearchItem selectedResult;

		private FoundByType selectedPhoneType;

		private RecipientLookup lookup;

		private DirectorySearchManager.HandleResultsDelegate resultHandler;

		private PhoneNumber numberToDial;

		private PhoneNumber canonicalizedNumber;

		private DialPermissionWrapper dialPermissions;

		private UMDialPlan originatingDialPlan;

		private bool autoAttendantCall;

		private bool anrOperatorTransfer;

		private DisambiguationFieldEnum disambiguationField;

		private bool executingPromptForAlias;

		private bool searchByExtension;

		internal delegate string HandleResultsDelegate(string dtmfSearchKey, int partial, int exact);

		internal class ConfigClass : ActivityManagerConfig
		{
			internal ConfigClass(ActivityManagerConfig manager) : base(manager)
			{
			}

			internal DirectorySearchPurpose SearchPurpose
			{
				get
				{
					return this.searchPurpose;
				}
			}

			internal override ActivityManager CreateActivityManager(ActivityManager manager)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Constructing DirectorySearch activity manager.", new object[0]);
				return new DirectorySearchManager(manager, this);
			}

			internal override void Load(XmlNode rootNode)
			{
				this.searchPurpose = (DirectorySearchPurpose)Enum.Parse(typeof(DirectorySearchPurpose), rootNode.Attributes["searchPurpose"].Value, true);
				base.Load(rootNode);
			}

			private DirectorySearchPurpose searchPurpose;
		}

		private class SearchItemComparer : IComparer<ContactSearchItem>
		{
			public int Compare(ContactSearchItem u1, ContactSearchItem u2)
			{
				return string.Compare(u1.FullName, u2.FullName, true, CultureInfo.InvariantCulture);
			}

			internal static readonly DirectorySearchManager.SearchItemComparer StaticInstance = new DirectorySearchManager.SearchItemComparer();
		}
	}
}
