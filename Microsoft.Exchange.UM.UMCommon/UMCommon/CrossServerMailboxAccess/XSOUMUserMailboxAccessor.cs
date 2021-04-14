using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.SoapWebClient.EWS;
using Microsoft.Exchange.UM.Rpc;

namespace Microsoft.Exchange.UM.UMCommon.CrossServerMailboxAccess
{
	internal class XSOUMUserMailboxAccessor : DisposableBase, IUMUserMailboxStorage, IDisposeTrackable, IDisposable
	{
		public XSOUMUserMailboxAccessor(ExchangePrincipal mailboxPrincipal, ADUser user)
		{
			ValidateArgument.NotNull(user, "user");
			ValidateArgument.NotNull(mailboxPrincipal, "mailboxPrincipal");
			this.user = user;
			this.mailboxPrincipal = mailboxPrincipal;
			this.tracer = new DiagnosticHelper(this, ExTraceGlobals.XsoTracer);
			this.ExecuteXSOOperation(delegate
			{
				this.Initialize(this.CreateMailboxSession("Client=UM;Action=Manage-UMMailbox"));
			});
			this.disposeMailboxSession = true;
		}

		public XSOUMUserMailboxAccessor(ADUser user, MailboxSession mailboxSession)
		{
			ValidateArgument.NotNull(mailboxSession, "mailboxSession");
			ValidateArgument.NotNull(user, "user");
			this.tracer = new DiagnosticHelper(this, ExTraceGlobals.XsoTracer);
			this.user = user;
			this.Initialize(mailboxSession);
		}

		public void InitUMMailbox()
		{
			PIIMessage piiUser = PIIMessage.Create(PIIType._User, this.user);
			this.tracer.Trace(piiUser, "XSOUMUserMailboxAccessor : Initialize UM mailbox for user _User", new object[0]);
			this.ExecuteXSOOperation(delegate
			{
				Utils.InitUMMailbox(this.mailboxSession, this.user);
				this.tracer.Trace(piiUser, "XSOUMUserMailboxAccessor : InitUMMailbox, Successfully initialized mailbox for user _user", new object[0]);
			});
		}

		public void ResetUMMailbox(bool keepProperties)
		{
			PIIMessage piiUser = PIIMessage.Create(PIIType._User, this.user);
			this.tracer.Trace(piiUser, "XSOUMUserMailboxAccessor : Reset UM mailbox for user _User", new object[0]);
			this.ExecuteXSOOperation(delegate
			{
				Utils.ResetUMMailbox(this.user, keepProperties, this.mailboxSession);
				this.tracer.Trace(piiUser, "XSOUMUserMailboxAccessor : ResetUMailbox, Successfully reset mailbox for user _User", new object[0]);
			});
		}

		public PINInfo ValidateUMPin(string pin, Guid userUMMailboxPolicyGuid)
		{
			PIIMessage piiUser = PIIMessage.Create(PIIType._User, this.user);
			this.tracer.Trace(piiUser, "XSOUMUserMailboxAccessor : Validate Pin for user _User", new object[0]);
			PINInfo resultPinInfo = null;
			this.ExecuteXSOOperation(delegate
			{
				if (userUMMailboxPolicyGuid != Guid.Empty)
				{
					resultPinInfo = Utils.ValidateOrGeneratePIN(this.user, pin, this.mailboxSession, this.GetUMMailboxPolicy(userUMMailboxPolicyGuid));
				}
				else
				{
					resultPinInfo = Utils.ValidateOrGeneratePIN(this.user, pin, this.mailboxSession);
				}
				this.tracer.Trace(piiUser, "XSOUMUserMailboxAccessor : Validate Pin, Successfully validated or generated Pin for user _User", new object[0]);
			});
			return resultPinInfo;
		}

		public void SaveUMPin(PINInfo pin, Guid userUMMailboxPolicyGuid)
		{
			ValidateArgument.NotNull(pin, "Pin");
			PIIMessage piiUser = PIIMessage.Create(PIIType._User, this.user);
			this.tracer.Trace(piiUser, "XSOUMUserMailboxAccessor : Save Pin for user _User", new object[0]);
			this.ExecuteXSOOperation(delegate
			{
				if (userUMMailboxPolicyGuid != Guid.Empty)
				{
					Utils.SetUserPassword(this.mailboxSession, this.GetUMMailboxPolicy(userUMMailboxPolicyGuid), this.user, pin.PIN, pin.PinExpired, pin.LockedOut);
				}
				else
				{
					Utils.SetUserPassword(this.mailboxSession, this.user, pin.PIN, pin.PinExpired, pin.LockedOut);
				}
				this.tracer.Trace(piiUser, "XSOUMUserMailboxAccessor : SaveUMPin, Successfully saved UM Pin for user _User", new object[0]);
			});
		}

		public PINInfo GetUMPin()
		{
			PIIMessage piiUser = PIIMessage.Create(PIIType._User, this.user);
			this.tracer.Trace(piiUser, "XSOUMUserMailboxAccessor : GetUMPin for user _User", new object[0]);
			PINInfo resultPinInfo = null;
			this.ExecuteXSOOperation(delegate
			{
				resultPinInfo = Utils.GetPINInfo(this.user, this.mailboxSession);
				this.tracer.Trace(piiUser, "XSOUMUserMailboxAccessor : GetUMPin, Successfully retrieved Pin for user _User", new object[0]);
			});
			return resultPinInfo;
		}

		public void SendEmail(string recipientMailAddress, string messageSubject, string messageBody)
		{
			PIIMessage piiUser = PIIMessage.Create(PIIType._User, this.user);
			this.tracer.Trace(piiUser, "XSOUMUserMailboxAccessor : Send notify email for user _User", new object[0]);
			ValidateArgument.NotNullOrEmpty(recipientMailAddress, "recipientMailAddress");
			if (!SmtpAddress.IsValidSmtpAddress(recipientMailAddress))
			{
				throw new InvalidArgumentException("Recipient Email address is not a valid SMTP address.");
			}
			ValidateArgument.NotNullOrEmpty(messageSubject, "messageSubject");
			ValidateArgument.NotNullOrEmpty(messageBody, "messageBody");
			this.ExecuteXSOOperation(delegate
			{
				using (MessageItem messageItem = this.ConstructNotifyMail(recipientMailAddress, messageSubject, messageBody))
				{
					messageItem.SendWithoutSavingMessage();
					this.tracer.Trace(piiUser, "XSOUMUserMailboxAccessor : Notify email successfully sent for user _User", new object[0]);
				}
			});
		}

		public UMSubscriberCallAnsweringData GetUMSubscriberCallAnsweringData(UMSubscriber subscriber, TimeSpan timeout)
		{
			PIIMessage pii = PIIMessage.Create(PIIType._User, this.user);
			this.tracer.Trace(pii, "XSOUMUserMailboxAccessor : GetUMSubscriberCallAnsweringData for user _User", new object[0]);
			ValidateArgument.NotNull(subscriber, "subscriber");
			ValidateArgument.NotNull(timeout, "timeout");
			Stopwatch elapsedTime = new Stopwatch();
			elapsedTime.Start();
			UMSubscriberCallAnsweringData subscriberData = new UMSubscriberCallAnsweringData();
			this.ExecuteXSOOperation(delegate
			{
				subscriberData.IsOOF = subscriber.IsOOF();
				subscriberData.IsTranscriptionEnabledInMailboxConfig = subscriber.IsTranscriptionEnabledInMailboxConfig(VoiceMailTypeEnum.ReceivedVoiceMails);
				subscriberData.Greeting = subscriber.GetGreeting();
				if (elapsedTime.Elapsed > timeout)
				{
					subscriberData.TaskTimedOut = true;
					this.tracer.Trace("XSOUMUserMailboxAccessor : UMSubscriberCallAnsweringData timed out before checking IsMailboxQuotaExceeded for user _User", new object[0]);
					return;
				}
				subscriberData.IsMailboxQuotaExceeded = subscriber.IsMailboxQuotaExceeded();
			});
			elapsedTime.Stop();
			this.tracer.Trace(pii, "XSOUMUserMailboxAccessor : UMSubscriberCallAnsweringData succeeded for user _User", new object[0]);
			return subscriberData;
		}

		public UMSubscriberCallAnsweringData GetUMSubscriberCallAnsweringData(TimeSpan timeout)
		{
			ValidateArgument.NotNull(timeout, "timeout");
			UMSubscriberCallAnsweringData umsubscriberCallAnsweringData;
			using (UMSubscriber umsubscriber = new UMSubscriber(this.user, this.mailboxSession))
			{
				umsubscriberCallAnsweringData = this.GetUMSubscriberCallAnsweringData(umsubscriber, timeout);
			}
			return umsubscriberCallAnsweringData;
		}

		public PersonaType GetPersonaFromEmail(string emailAddress)
		{
			PIIMessage pii = PIIMessage.Create(PIIType._EmailAddress, this.user);
			this.tracer.Trace(pii, "XSOUMUserMailboxAccessor : GetPersonaFromEmail, for user _EmailAddress", new object[0]);
			if (InterServerMailboxAccessor.TestXSOHook)
			{
				return new PersonaType
				{
					EmailAddresses = new EmailAddressType[]
					{
						new EmailAddressType
						{
							EmailAddress = emailAddress
						}
					}
				};
			}
			throw new InvalidOperationException();
		}

		internal UMMailboxPolicy GetUMMailboxPolicy(Guid mbxPolicyGuid)
		{
			ADObjectId mbxPolicyId = new ADObjectId(mbxPolicyGuid);
			IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromADRecipient(this.user);
			UMMailboxPolicy ummailboxPolicyFromId = iadsystemConfigurationLookup.GetUMMailboxPolicyFromId(mbxPolicyId);
			if (ummailboxPolicyFromId == null)
			{
				throw new UMMbxPolicyNotFoundException(mbxPolicyGuid.ToString(), this.user.PrimarySmtpAddress.ToString());
			}
			return ummailboxPolicyFromId;
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.tracer.Trace("XSOUMUserMailboxAccessor : InternalDispose", new object[0]);
				if (this.mailboxSession != null && this.disposeMailboxSession)
				{
					this.mailboxSession.Dispose();
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<XSOUMUserMailboxAccessor>(this);
		}

		private void Initialize(MailboxSession session)
		{
			ExAssert.RetailAssert(session != null, "MailboxSession cannot be null");
			this.mailboxSession = session;
			this.tracer.Trace("XSOUMCallDataRecordAccessor called from WebServices : {1}", new object[]
			{
				!this.disposeMailboxSession
			});
		}

		private void ExecuteXSOOperation(Action function)
		{
			try
			{
				function();
			}
			catch (Exception ex)
			{
				CallIdTracer.TraceError(ExTraceGlobals.UtilTracer, this, ex.ToString(), new object[0]);
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_UMMailboxCmdletError, null, new object[]
				{
					this.user.LegacyExchangeDN,
					CommonUtil.ToEventLogString(ex)
				});
				throw;
			}
		}

		private MessageItem ConstructNotifyMail(string recipientMailAddress, string messageSubject, string messageBody)
		{
			IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromOrganizationId(this.user.OrganizationId);
			MicrosoftExchangeRecipient microsoftExchangeRecipient = iadsystemConfigurationLookup.GetMicrosoftExchangeRecipient();
			MessageItem messageItem = MessageItem.Create(this.mailboxSession, XsoUtil.GetDraftsFolderId(this.mailboxSession));
			IADRecipientLookup iadrecipientLookup = ADRecipientLookupFactory.CreateFromOrganizationId(this.user.OrganizationId, null);
			ADRecipient adrecipient = iadrecipientLookup.LookupBySmtpAddress(recipientMailAddress);
			if (adrecipient != null)
			{
				messageItem.Recipients.Add(new Participant(adrecipient.DisplayName, adrecipient.LegacyExchangeDN, "EX"), RecipientItemType.To);
			}
			else
			{
				messageItem.Recipients.Add(new Participant(recipientMailAddress, recipientMailAddress, "SMTP"), RecipientItemType.To);
			}
			messageItem.From = new Participant(microsoftExchangeRecipient);
			messageItem.AutoResponseSuppress = AutoResponseSuppress.All;
			messageItem.Subject = messageSubject;
			using (TextWriter textWriter = messageItem.Body.OpenTextWriter(BodyFormat.TextHtml))
			{
				textWriter.Write(messageBody);
			}
			return messageItem;
		}

		private MailboxSession CreateMailboxSession(string clientString)
		{
			return MailboxSessionEstablisher.OpenAsAdmin(this.mailboxPrincipal, CultureInfo.InvariantCulture, clientString);
		}

		private readonly bool disposeMailboxSession;

		private MailboxSession mailboxSession;

		private DiagnosticHelper tracer;

		private ADUser user;

		private ExchangePrincipal mailboxPrincipal;
	}
}
