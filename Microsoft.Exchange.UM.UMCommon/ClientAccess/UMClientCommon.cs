using System;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Configuration.SQM;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.ClientAccess
{
	internal sealed class UMClientCommon : UMClientCommonBase
	{
		public UMClientCommon()
		{
		}

		public UMClientCommon(IExchangePrincipal principal)
		{
			try
			{
				this.mailboxRecipient = UMRecipient.Factory.FromPrincipal<UMMailboxRecipient>(principal);
				if (this.mailboxRecipient == null)
				{
					throw new InvalidPrincipalException();
				}
				base.TracePrefix = string.Format(CultureInfo.InvariantCulture, "{0}({1}): ", new object[]
				{
					base.GetType().Name,
					this.mailboxRecipient.DisplayName
				});
			}
			catch (LocalizedException ex)
			{
				base.DebugTrace("{0}", new object[]
				{
					ex
				});
				throw;
			}
		}

		internal UMMailboxPolicy UMPolicy
		{
			get
			{
				this.ValidateUser();
				return this.Subscriber.UMMailboxPolicy;
			}
		}

		internal UMDialPlan DialPlan
		{
			get
			{
				this.ValidateUser();
				return this.Subscriber.DialPlan;
			}
		}

		private UMSubscriber Subscriber
		{
			get
			{
				return this.mailboxRecipient as UMSubscriber;
			}
		}

		private ADUser UserInstance
		{
			get
			{
				if (this.lazyUserInstance == null)
				{
					this.ValidateUser();
					IADRecipientLookup iadrecipientLookup = ADRecipientLookupFactory.CreateFromADRecipient(this.mailboxRecipient.ADRecipient, false);
					ADRecipient adrecipient = iadrecipientLookup.LookupByObjectId(this.mailboxRecipient.ADRecipient.Id);
					this.lazyUserInstance = (adrecipient as ADUser);
					if (this.lazyUserInstance == null)
					{
						throw new InvalidADRecipientException();
					}
				}
				return this.lazyUserInstance;
			}
		}

		public UMPropertiesEx GetUMProperties()
		{
			UMPropertiesEx result;
			try
			{
				base.DebugTrace("GetUMProperties()", new object[0]);
				this.ValidateUser();
				ADUser aduser = this.mailboxRecipient.ADRecipient as ADUser;
				if (aduser == null)
				{
					throw new InvalidADRecipientException();
				}
				UMPropertiesEx umpropertiesEx = new UMPropertiesEx();
				umpropertiesEx.OofStatus = this.Subscriber.ConfigFolder.IsOof;
				umpropertiesEx.TelephoneAccessNumbers = this.GetTelephoneAccessNumbers();
				umpropertiesEx.PlayOnPhoneDialString = this.Subscriber.ConfigFolder.PlayOnPhoneDialString;
				umpropertiesEx.TelephoneAccessFolderEmail = this.Subscriber.ConfigFolder.TelephoneAccessFolderEmail;
				umpropertiesEx.ReceivedVoiceMailPreviewEnabled = this.Subscriber.ConfigFolder.ReceivedVoiceMailPreviewEnabled;
				umpropertiesEx.SentVoiceMailPreviewEnabled = this.Subscriber.ConfigFolder.SentVoiceMailPreviewEnabled;
				umpropertiesEx.ReadUnreadVoicemailInFIFOOrder = this.Subscriber.ConfigFolder.ReadUnreadVoicemailInFIFOOrder;
				umpropertiesEx.PlayOnPhoneEnabled = this.IsPlayOnPhoneEnabled();
				UMMailbox ummailbox = new UMMailbox(aduser);
				umpropertiesEx.MissedCallNotificationEnabled = ummailbox.MissedCallNotificationEnabled;
				umpropertiesEx.PinlessAccessToVoicemail = ummailbox.PinlessAccessToVoiceMailEnabled;
				umpropertiesEx.SMSNotificationOption = ummailbox.UMSMSNotificationOption;
				base.DebugTrace("\tOOf={0}, TelNumbers={1}, PopDialStr={2}, EmailFolder={3}, ReceivedVoiceMailPreviewEnabled={4}, SentVoiceMailPreviewEnabled {5}, PlayOnPhone={6}, MissedCall={7}, PinlessAccesstoVoicemail={8}, SMSNotificationOption={9}, ReadUnreadVoicemailInFIFOOrder={10}", new object[]
				{
					umpropertiesEx.OofStatus,
					umpropertiesEx.TelephoneAccessNumbers,
					umpropertiesEx.PlayOnPhoneDialString,
					umpropertiesEx.TelephoneAccessFolderEmail,
					umpropertiesEx.ReceivedVoiceMailPreviewEnabled,
					umpropertiesEx.SentVoiceMailPreviewEnabled,
					umpropertiesEx.PlayOnPhoneEnabled,
					umpropertiesEx.MissedCallNotificationEnabled,
					umpropertiesEx.PinlessAccessToVoicemail,
					umpropertiesEx.SMSNotificationOption,
					umpropertiesEx.ReadUnreadVoicemailInFIFOOrder
				});
				result = umpropertiesEx;
			}
			catch (LocalizedException ex)
			{
				base.DebugTrace("{0}", new object[]
				{
					ex
				});
				throw;
			}
			return result;
		}

		public bool IsUMEnabled()
		{
			return null != this.Subscriber;
		}

		public bool IsPlayOnPhoneEnabled()
		{
			return this.IsUMEnabled() && this.Subscriber.IsPlayOnPhoneEnabled;
		}

		public bool IsSmsNotificationsEnabled()
		{
			return this.IsUMEnabled() && this.Subscriber.IsSmsNotificationsEnabled;
		}

		public bool IsRequireProtectedPlayOnPhone()
		{
			return this.IsUMEnabled() && this.Subscriber.IsRequireProtectedPlayOnPhone;
		}

		public string GetPlayOnPhoneDialString()
		{
			if (!this.IsUMEnabled())
			{
				return string.Empty;
			}
			return this.GetUMProperties().PlayOnPhoneDialString;
		}

		public void SetOofStatus(bool status)
		{
			base.DebugTrace("SetOofStatus({0})", new object[]
			{
				status
			});
			this.UpdateSubscriberConfig(delegate
			{
				this.Subscriber.ConfigFolder.IsOof = status;
			});
		}

		public void SetPlayOnPhoneDialString(string dialString)
		{
			if (dialString != null)
			{
				dialString = dialString.Trim();
			}
			base.DebugTrace("SetPlayOnPhoneDialString({0})", new object[]
			{
				dialString
			});
			this.UpdateSubscriberConfig(delegate
			{
				this.Subscriber.ConfigFolder.PlayOnPhoneDialString = dialString;
			});
		}

		public void SetTelephoneAccessFolderEmail(string base64FolderId)
		{
			base.DebugTrace("SetTelephoneAccessFolderEmail({0})", new object[]
			{
				base64FolderId
			});
			this.UpdateSubscriberConfig(delegate
			{
				this.Subscriber.ConfigFolder.TelephoneAccessFolderEmail = base64FolderId;
			});
		}

		public void SetReceivedVoiceMailPreview(bool receivedVoiceMailPreview)
		{
			base.DebugTrace("ReceivedVoiceMailPreviewEnabled({0})", new object[]
			{
				receivedVoiceMailPreview
			});
			this.UpdateSubscriberConfig(delegate
			{
				this.Subscriber.ConfigFolder.ReceivedVoiceMailPreviewEnabled = receivedVoiceMailPreview;
			});
		}

		public void SetSentVoiceMailPreview(bool sentVoiceMailPreview)
		{
			base.DebugTrace("SentVoiceMailPreviewEnabled({0})", new object[]
			{
				sentVoiceMailPreview
			});
			this.UpdateSubscriberConfig(delegate
			{
				this.Subscriber.ConfigFolder.SentVoiceMailPreviewEnabled = sentVoiceMailPreview;
			});
		}

		public void SetUnReadVoiceMailReadingOrder(bool fifoOrder)
		{
			base.DebugTrace("SetUnReadVoiceMailReadingOrder({0})", new object[]
			{
				fifoOrder
			});
			this.UpdateSubscriberConfig(delegate
			{
				this.Subscriber.ConfigFolder.ReadUnreadVoicemailInFIFOOrder = fifoOrder;
			});
		}

		public void SetVoiceNotificationStatus(VoiceNotificationStatus voiceNotificationStatus)
		{
			base.DebugTrace("SetVoiceNotification({0})", new object[]
			{
				voiceNotificationStatus
			});
			if (voiceNotificationStatus == VoiceNotificationStatus.Enabled || voiceNotificationStatus == VoiceNotificationStatus.Disabled)
			{
				this.UpdateSubscriberConfig(delegate
				{
					this.Subscriber.ConfigFolder.VoiceNotificationStatus = voiceNotificationStatus;
				});
				return;
			}
			throw new InvalidArgumentException("voiceNotificationStatus");
		}

		public void SetMissedCallNotificationEnabled(bool status)
		{
			base.DebugTrace("SetMissedCallNotificationEnabled({0})", new object[]
			{
				status
			});
			this.UpdateUMMailbox(delegate(UMMailbox x)
			{
				x.MissedCallNotificationEnabled = status;
			});
		}

		public void SetPinlessAccessToVoicemail(bool pinlessAccess)
		{
			base.DebugTrace("PinlessAccessToVoicemail({0})", new object[]
			{
				pinlessAccess
			});
			this.UpdateUMMailbox(delegate(UMMailbox x)
			{
				x.PinlessAccessToVoiceMailEnabled = pinlessAccess;
			});
		}

		public void SetSMSNotificationOption(UMSMSNotificationOptions option)
		{
			base.DebugTrace("SetSMSNotificationOption({0})", new object[]
			{
				option
			});
			this.UpdateUMMailbox(delegate(UMMailbox x)
			{
				x.UMSMSNotificationOption = option;
			});
			SmsSqmDataPointHelper.AddNotificationConfigDataPoint(SmsSqmSession.Instance, this.mailboxRecipient.ADRecipient.Id, this.mailboxRecipient.ADRecipient.LegacyExchangeDN, SmsSqmDataPointHelper.TranslateEnumForSqm<UMSMSNotificationOptions>(option));
		}

		public void ResetPIN()
		{
			try
			{
				base.DebugTrace("ResetPIN()", new object[0]);
				if (UMClientCommonBase.Counters != null)
				{
					UMClientCommonBase.Counters.TotalPINResetRequests.Increment();
				}
				this.ValidateUser();
				Utils.ResetPassword(this.Subscriber, true, LockOutResetMode.Reset);
			}
			catch (LocalizedException exception)
			{
				base.LogException(exception);
				if (UMClientCommonBase.Counters != null)
				{
					UMClientCommonBase.Counters.TotalPINResetErrors.Increment();
				}
				throw;
			}
		}

		public string PlayOnPhone(string base64ObjectId, string dialString)
		{
			string result;
			try
			{
				base.DebugTrace("PlayOnPhone({0}, {1})", new object[]
				{
					base64ObjectId,
					dialString
				});
				if (UMClientCommonBase.Counters != null)
				{
					UMClientCommonBase.Counters.TotalPlayOnPhoneRequests.Increment();
				}
				this.ValidateUser();
				this.ValidateObjectId(base64ObjectId);
				UMServerProxy serverByDialplan = UMServerManager.GetServerByDialplan((ADObjectId)this.Subscriber.DialPlan.Identity);
				string sessionId = serverByDialplan.PlayOnPhoneMessage("SMTP:" + this.mailboxRecipient.MailAddress, this.mailboxRecipient.ADRecipient.Guid, this.mailboxRecipient.TenantGuid, base64ObjectId, dialString);
				result = base.EncodeCallId(serverByDialplan.Fqdn, sessionId);
			}
			catch (LocalizedException exception)
			{
				base.LogException(exception);
				if (UMClientCommonBase.Counters != null)
				{
					UMClientCommonBase.Counters.TotalPlayOnPhoneErrors.Increment();
				}
				throw;
			}
			return result;
		}

		public string PlayOnPhoneGreeting(UMGreetingType greetingType, string dialString)
		{
			string result;
			try
			{
				base.DebugTrace("PlayOnPhoneGreeting({0}, {1})", new object[]
				{
					greetingType,
					dialString
				});
				if (UMClientCommonBase.Counters != null)
				{
					UMClientCommonBase.Counters.TotalPlayOnPhoneRequests.Increment();
				}
				this.ValidateUser();
				UMServerProxy serverByDialplan = UMServerManager.GetServerByDialplan((ADObjectId)this.Subscriber.DialPlan.Identity);
				string sessionId = serverByDialplan.PlayOnPhoneGreeting("SMTP:" + this.mailboxRecipient.MailAddress, this.mailboxRecipient.ADRecipient.Guid, this.mailboxRecipient.TenantGuid, greetingType, dialString);
				result = base.EncodeCallId(serverByDialplan.Fqdn, sessionId);
			}
			catch (LocalizedException exception)
			{
				base.LogException(exception);
				if (UMClientCommonBase.Counters != null)
				{
					UMClientCommonBase.Counters.TotalPlayOnPhoneErrors.Increment();
				}
				throw;
			}
			return result;
		}

		public string PlayOnPhonePAAGreeting(Guid paaIdentity, string dialString)
		{
			string result;
			try
			{
				base.DebugTrace("PlayOnPhoneGreeting( PAA = {0},DialString = {1})", new object[]
				{
					paaIdentity.ToString(),
					dialString
				});
				this.ValidateUser();
				UMServerProxy serverByDialplan = UMServerManager.GetServerByDialplan((ADObjectId)this.Subscriber.DialPlan.Identity);
				string sessionId = serverByDialplan.PlayOnPhonePAAGreeting("SMTP:" + this.mailboxRecipient.MailAddress, this.mailboxRecipient.ADRecipient.Guid, this.mailboxRecipient.TenantGuid, paaIdentity, dialString);
				result = base.EncodeCallId(serverByDialplan.Fqdn, sessionId);
			}
			catch (LocalizedException exception)
			{
				base.LogException(exception);
				throw;
			}
			return result;
		}

		protected override void DisposeMe()
		{
			if (this.mailboxRecipient != null)
			{
				this.mailboxRecipient.Dispose();
			}
		}

		protected override string GetUserContext()
		{
			if (this.mailboxRecipient != null)
			{
				return this.mailboxRecipient.ExchangePrincipal.MailboxInfo.DisplayName;
			}
			return string.Empty;
		}

		private string GetTelephoneAccessNumbers()
		{
			StringBuilder stringBuilder = new StringBuilder(string.Empty);
			if (this.Subscriber.DialPlan.AccessTelephoneNumbers != null && this.Subscriber.DialPlan.AccessTelephoneNumbers.Count > 0)
			{
				stringBuilder.Append(this.Subscriber.DialPlan.AccessTelephoneNumbers[0]);
				for (int i = 1; i < this.Subscriber.DialPlan.AccessTelephoneNumbers.Count; i++)
				{
					stringBuilder.Append(", ");
					stringBuilder.Append(this.Subscriber.DialPlan.AccessTelephoneNumbers[i]);
				}
			}
			return stringBuilder.ToString();
		}

		private void ValidateObjectId(string base64ObjectId)
		{
			Exception ex = null;
			try
			{
				byte[] entryId = Convert.FromBase64String(base64ObjectId);
				StoreObjectId storeId = StoreObjectId.FromProviderSpecificId(entryId);
				PropertyDefinition[] propsToReturn = new PropertyDefinition[]
				{
					ItemSchema.Id
				};
				using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.mailboxRecipient.CreateSessionLock())
				{
					Item item = Item.Bind(mailboxSessionLock.Session, storeId, propsToReturn);
					item.Dispose();
				}
			}
			catch (ArgumentException ex2)
			{
				ex = ex2;
			}
			catch (FormatException ex3)
			{
				ex = ex3;
			}
			catch (LocalizedException ex4)
			{
				ex = ex4;
			}
			if (ex != null)
			{
				throw new InvalidObjectIdException(ex);
			}
		}

		private void ValidateUser()
		{
			if (!this.IsUMEnabled())
			{
				throw new UserNotUmEnabledException(this.mailboxRecipient.ExchangePrincipal.MailboxInfo.DisplayName);
			}
		}

		private void UpdateUMMailbox(Action<UMMailbox> updater)
		{
			try
			{
				UMMailbox obj = new UMMailbox(this.UserInstance);
				updater(obj);
				this.UserInstance.Session.Save(this.UserInstance);
			}
			catch (LocalizedException ex)
			{
				base.DebugTrace("{0}", new object[]
				{
					ex
				});
				throw;
			}
		}

		private void UpdateSubscriberConfig(Action updater)
		{
			try
			{
				this.ValidateUser();
				updater();
				this.Subscriber.ConfigFolder.Save();
			}
			catch (LocalizedException ex)
			{
				base.DebugTrace("{0}", new object[]
				{
					ex
				});
				throw;
			}
		}

		private UMMailboxRecipient mailboxRecipient;

		private ADUser lazyUserInstance;
	}
}
