using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class DialingPermissionsCheck
	{
		internal DialingPermissionsCheck(UMDialPlan originatingDialPlan, bool authenticatedCaller)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, null, "DialingPermissionsCheck::ctor(DialPlan=({0}) authenticatedCaller=({1}))", new object[]
			{
				originatingDialPlan.Name,
				authenticatedCaller
			});
			this.dialPermissions = DialPermissionWrapper.CreateFromDialPlan(originatingDialPlan);
			this.authenticatedCaller = authenticatedCaller;
			this.originatingDialPlan = originatingDialPlan;
			this.operatorEnabled = PhoneNumber.TryParse(this.originatingDialPlan.OperatorExtension, out this.operatorNumber);
		}

		internal DialingPermissionsCheck(UMAutoAttendant autoAttendant, AutoAttendantSettings currentAutoAttendantSettings, UMDialPlan originatingDialPlan)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, null, "DialingPermissionsCheck::ctor(AA=({0}) Originating DP=({1}))", new object[]
			{
				autoAttendant.Name,
				originatingDialPlan.Name
			});
			this.dialPermissions = DialPermissionWrapper.CreateFromAutoAttendant(autoAttendant);
			this.originatingDialPlan = originatingDialPlan;
			this.operatorEnabled = CommonUtil.GetOperatorExtensionForAutoAttendant(autoAttendant, currentAutoAttendantSettings, originatingDialPlan, true, out this.operatorNumber);
		}

		internal DialingPermissionsCheck(ADUser user, UMDialPlan originatingDialPlan)
		{
			this.dialPermissions = DialPermissionWrapper.CreateFromRecipientPolicy(user);
			this.authenticatedCaller = true;
			this.originatingDialPlan = originatingDialPlan;
		}

		public bool OperatorEnabled
		{
			get
			{
				return this.operatorEnabled;
			}
		}

		public PhoneNumber OperatorNumber
		{
			get
			{
				return this.operatorNumber;
			}
		}

		internal DialingPermissionsCheck.DialingPermissionsCheckResult CheckDirectoryUser(ADRecipient recipient, PhoneNumber phoneOrExtension)
		{
			DialingPermissionsCheck.DialingPermissionsCheckResult dialingPermissionsCheckResult = new DialingPermissionsCheck.DialingPermissionsCheckResult(this);
			dialingPermissionsCheckResult.CheckDirectoryUser(recipient, phoneOrExtension);
			return dialingPermissionsCheckResult;
		}

		internal DialingPermissionsCheck.DialingPermissionsCheckResult CheckPhoneNumber(PhoneNumber number)
		{
			DialingPermissionsCheck.DialingPermissionsCheckResult dialingPermissionsCheckResult = new DialingPermissionsCheck.DialingPermissionsCheckResult(this);
			dialingPermissionsCheckResult.CheckPhoneNumber(number);
			return dialingPermissionsCheckResult;
		}

		private DialPermissionWrapper dialPermissions;

		private UMDialPlan originatingDialPlan;

		private bool authenticatedCaller;

		private bool operatorEnabled;

		private PhoneNumber operatorNumber;

		internal class DialingPermissionsCheckResult
		{
			internal DialingPermissionsCheckResult(DialingPermissionsCheck parent)
			{
				this.parent = parent;
			}

			public bool AllowCall
			{
				get
				{
					return this.canCall;
				}
			}

			public bool AllowSendMessage
			{
				get
				{
					return this.canSendMsg;
				}
			}

			public bool HaveCallPermissions
			{
				get
				{
					return this.haveCallPermissions;
				}
			}

			public PhoneNumber NumberToDial
			{
				get
				{
					return this.numberToDial;
				}
			}

			public bool IsProtectedUser
			{
				get
				{
					return this.protectedUser;
				}
			}

			public bool HaveValidPhone { get; private set; }

			internal void CheckDirectoryUser(ADRecipient recipient, PhoneNumber phoneOrExtension)
			{
				this.canCall = false;
				this.canSendMsg = false;
				this.haveCallPermissions = false;
				this.numberToDial = null;
				this.HaveValidPhone = false;
				PIIMessage data = PIIMessage.Create(PIIType._UserDisplayName, recipient.DisplayName);
				CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, this, data, "CheckDirectoryUser(_UserDisplayName,\"{0}\").", new object[]
				{
					phoneOrExtension
				});
				IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromADRecipient(recipient);
				UMDialPlan dialPlanFromRecipient = iadsystemConfigurationLookup.GetDialPlanFromRecipient(recipient);
				if (phoneOrExtension == null)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, this, data, "CheckDirectoryUser(_UserDisplayName) - getting GAL phone or UM extension.", new object[0]);
					this.HaveValidPhone = DialPermissions.GetBestOfficeNumber(recipient, this.parent.originatingDialPlan, out phoneOrExtension);
					CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, this, data, "GetExtensionOrGalPhone(_UserDisplayName) returned Sucess:{0} Phone:{1}", new object[]
					{
						this.HaveValidPhone,
						(phoneOrExtension != null) ? phoneOrExtension.ToString() : "<null>"
					});
					if (this.HaveValidPhone && phoneOrExtension.Kind == PhoneNumberKind.Extension)
					{
						PhoneNumber value;
						if (DialPermissions.CheckExtension(phoneOrExtension, this.parent.dialPermissions, this.parent.originatingDialPlan, dialPlanFromRecipient, out value))
						{
							this.haveCallPermissions = true;
							this.numberToDial = value;
						}
						else
						{
							data = PIIMessage.Create(PIIType._UserDisplayName, (recipient != null) ? recipient.DisplayName : "<null>");
							CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, this, data, "Extension {0} for _UserDisplayName did not pass dialing checks.", new object[]
							{
								phoneOrExtension
							});
						}
					}
				}
				else
				{
					this.HaveValidPhone = true;
				}
				if (this.HaveValidPhone)
				{
					PhoneNumber phoneNumber = DialPermissions.Canonicalize(phoneOrExtension, this.parent.originatingDialPlan, recipient, dialPlanFromRecipient);
					if (phoneNumber != null)
					{
						PIIMessage piimessage = PIIMessage.Create(PIIType._PhoneNumber, phoneNumber);
						CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, this, piimessage, "Doing DialingPermission check for: PhoneNumber _PhoneNumber.", new object[0]);
						PhoneNumber value;
						this.haveCallPermissions = DialPermissions.Check(phoneNumber, this.parent.dialPermissions, this.parent.originatingDialPlan, dialPlanFromRecipient, out value);
						PIIMessage piimessage2 = PIIMessage.Create(PIIType._Callee, value);
						PIIMessage[] data2 = new PIIMessage[]
						{
							piimessage,
							piimessage2
						};
						CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, this, data2, "DialingPermission check for: _PhoneNumber returned result,phone = {0}[_Callee].", new object[]
						{
							this.haveCallPermissions
						});
						if (this.haveCallPermissions)
						{
							this.numberToDial = value;
						}
					}
					else
					{
						CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, this, "PhoneNumber {0} did not pass canonicalization.", new object[]
						{
							phoneOrExtension
						});
					}
				}
				if (!this.parent.dialPermissions.CallSomeoneEnabled)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, this, "DialPermissionWrapper.CallSomeoneEnabled = false. Hence setting haveCallPermissions = false.", new object[0]);
					this.haveCallPermissions = false;
				}
				if (!this.parent.authenticatedCaller)
				{
					if (this.haveCallPermissions)
					{
						this.ProcessAnonymousCaller(recipient);
					}
				}
				else
				{
					this.canCall = this.haveCallPermissions;
				}
				bool flag = recipient.PrimarySmtpAddress != SmtpAddress.Empty;
				if (flag)
				{
					this.canSendMsg = (this.parent.dialPermissions.SendVoiceMessageEnabled || !this.HaveValidPhone || !this.haveCallPermissions);
				}
			}

			internal void CheckPhoneNumber(PhoneNumber number)
			{
				PIIMessage piimessage = PIIMessage.Create(PIIType._PhoneNumber, number);
				CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, this, piimessage, "CheckPhoneNumber(_PhoneNumber).", new object[0]);
				PhoneNumber phoneNumber = DialPermissions.Canonicalize(number, this.parent.originatingDialPlan, null);
				PIIMessage piimessage2 = PIIMessage.Create(PIIType._PhoneNumber, phoneNumber);
				PIIMessage[] data = new PIIMessage[]
				{
					piimessage,
					piimessage2
				};
				CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, this, data, "Canonicalize(_PhoneNumber(1)) returned {_PhoneNumber(2)}.", new object[0]);
				if (phoneNumber == null)
				{
					return;
				}
				PhoneNumber phoneNumber2 = null;
				this.haveCallPermissions = DialPermissions.Check(phoneNumber, this.parent.dialPermissions, this.parent.originatingDialPlan, null, out phoneNumber2);
				CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, this, piimessage2, "CheckPermissions(_PhoneNumber) returned {0}.", new object[]
				{
					this.haveCallPermissions
				});
				if (this.haveCallPermissions)
				{
					this.canCall = true;
					this.numberToDial = phoneNumber2;
				}
			}

			internal void ProcessAnonymousCaller(ADRecipient recipient)
			{
				if ((recipient.AllowUMCallsFromNonUsers & AllowUMCallsFromNonUsersFlags.SearchEnabled) == AllowUMCallsFromNonUsersFlags.SearchEnabled)
				{
					this.canCall = true;
					return;
				}
				PIIMessage data = PIIMessage.Create(PIIType._UserDisplayName, recipient.DisplayName);
				CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, this, data, "Recipient _UserDisplayName does not allow calls from nonUMusers.", new object[0]);
				this.protectedUser = true;
				if (this.parent.operatorEnabled)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, this, data, "ProcessAnonymousCaller(_UserDisplayName) returned operator number {0}.", new object[]
					{
						this.parent.operatorNumber
					});
					this.numberToDial = this.parent.operatorNumber;
					this.canCall = true;
					return;
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, this, "Operator is not configured for AA, and recipient does not allow calls from NonUMEnabled users.", new object[0]);
				this.canCall = false;
			}

			private DialingPermissionsCheck parent;

			private bool canCall;

			private bool canSendMsg;

			private bool haveCallPermissions;

			private PhoneNumber numberToDial;

			private bool protectedUser;
		}
	}
}
