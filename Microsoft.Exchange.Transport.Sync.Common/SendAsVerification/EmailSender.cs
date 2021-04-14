using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.Net.ExSmtpClient;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Transport.Sync.Common.ExSmtpClient;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Transport.Sync.Common.SendAsVerification
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class EmailSender : IEmailSender
	{
		internal EmailSender(PimAggregationSubscription subscription, ADUser subscriptionAdUser, ExchangePrincipal subscriptionExchangePrincipal, SyncLogSession syncLogSession)
		{
			SyncUtilities.ThrowIfArgumentNull("subscription", subscription);
			SyncUtilities.ThrowIfArgumentNull("subscriptinAdUser", subscriptionAdUser);
			SyncUtilities.ThrowIfArgumentNull("subscriptionExchangePrincipal", subscriptionExchangePrincipal);
			SyncUtilities.ThrowIfArgumentNull("syncLogSession", syncLogSession);
			if (!subscription.SendAsNeedsVerification)
			{
				throw new ArgumentException("subscription is not SendAs verified.  Type: " + subscription.SubscriptionType.ToString(), "subscription");
			}
			this.subscription = subscription;
			this.subscriptionAdUser = subscriptionAdUser;
			this.subscriptionExchangePrincipal = subscriptionExchangePrincipal;
			this.syncLogSession = syncLogSession;
		}

		public bool SendAttempted
		{
			get
			{
				return this.sendAttempted;
			}
		}

		public bool SendSuccessful
		{
			get
			{
				return this.SendAttempted && this.sendSuccessful;
			}
		}

		public string MessageId
		{
			get
			{
				return this.messageId;
			}
		}

		internal static IEmailSender NullEmailSender
		{
			get
			{
				return EmailSender.nullEmailSender;
			}
		}

		public void SendWith(Guid sharedSecret)
		{
			SyncUtilities.ThrowIfGuidEmpty("sharedSecret", sharedSecret);
			this.sendAttempted = true;
			this.RegisterSmtpServicePrincipalName();
			this.messageId = string.Empty;
			string host;
			if (!this.TryPickHub(out host))
			{
				this.sendSuccessful = false;
				return;
			}
			Exception ex = null;
			using (SmtpClient smtpClient = new SmtpClient(host, 25, new SmtpClientTransportSyncDebugOutput(this.syncLogSession)))
			{
				try
				{
					SendAsVerificationExchangeRecipientLookup sendAsVerificationExchangeRecipientLookup = new SendAsVerificationExchangeRecipientLookup();
					string text = sendAsVerificationExchangeRecipientLookup.ExchangeRecipientFor(this.subscriptionAdUser, this.syncLogSession);
					using (SendAsVerificationEmail sendAsVerificationEmail = new SendAsVerificationEmail(this.subscriptionExchangePrincipal, text, this.subscription, sharedSecret, this.syncLogSession))
					{
						smtpClient.AuthCredentials(CredentialCache.DefaultNetworkCredentials);
						smtpClient.From = text;
						smtpClient.To = new string[]
						{
							sendAsVerificationEmail.SubscriptionAddress
						};
						smtpClient.NDRRequired = false;
						smtpClient.DataStream = sendAsVerificationEmail.MessageData;
						smtpClient.Submit();
						this.syncLogSession.LogVerbose((TSLID)31UL, EmailSender.Tracer, (long)this.GetHashCode(), "Email was sent to: [{0}] from: [{1}] message id: [{2}]", new object[]
						{
							sendAsVerificationEmail.SubscriptionAddress,
							text,
							sendAsVerificationEmail.MessageId
						});
						this.messageId = sendAsVerificationEmail.MessageId;
					}
				}
				catch (FailedToGenerateVerificationEmailException ex2)
				{
					ex = ex2;
				}
				catch (UnexpectedSmtpServerResponseException ex3)
				{
					ex = ex3;
				}
				catch (AlreadyConnectedToSMTPServerException ex4)
				{
					ex = ex4;
				}
				catch (AuthFailureException ex5)
				{
					ex = ex5;
				}
				catch (FailedToConnectToSMTPServerException ex6)
				{
					ex = ex6;
				}
				catch (InvalidSmtpServerResponseException ex7)
				{
					ex = ex7;
				}
				catch (MustBeTlsForAuthException ex8)
				{
					ex = ex8;
				}
				catch (NotConnectedToSMTPServerException ex9)
				{
					ex = ex9;
				}
				catch (AuthApiFailureException ex10)
				{
					ex = ex10;
				}
				catch (SocketException ex11)
				{
					ex = ex11;
				}
				catch (IOException ex12)
				{
					if (ex12.InnerException == null || !(ex12.InnerException is SocketException))
					{
						this.syncLogSession.LogError((TSLID)32UL, EmailSender.Tracer, (long)this.GetHashCode(), "Unexpected IOException: {0}.  Rethrowing", new object[]
						{
							ex12
						});
						throw;
					}
					ex = ex12;
				}
			}
			if (ex != null)
			{
				this.syncLogSession.LogError((TSLID)33UL, EmailSender.Tracer, (long)this.GetHashCode(), "An exception was encountered while attempting to send an email: {0}", new object[]
				{
					ex
				});
				CommonLoggingHelper.EventLogger.LogEvent(TransportSyncCommonEventLogConstants.Tuple_VerificationEmailNotSent, ex.GetType().FullName, new object[]
				{
					this.subscriptionAdUser.LegacyExchangeDN,
					ex.ToString()
				});
			}
			this.sendSuccessful = (ex == null);
		}

		private bool TryPickHub(out string hubFqdn)
		{
			bool result = false;
			hubFqdn = string.Empty;
			using (ServerPickerManager serverPickerManager = new ServerPickerManager("Microsoft Exchange Transport Sync SendAsVerification", ServerRole.HubTransport, EmailSender.Tracer))
			{
				PickerServerList pickerServerList = serverPickerManager.GetPickerServerList();
				try
				{
					PickerServer pickerServer = pickerServerList.PickNextUsingRoundRobin();
					if (pickerServer == null)
					{
						this.syncLogSession.LogError((TSLID)34UL, EmailSender.Tracer, (long)this.GetHashCode(), "No hub server found to send email through.", new object[0]);
						FailedToGenerateVerificationEmailException ex = new FailedToGenerateVerificationEmailException();
						CommonLoggingHelper.EventLogger.LogEvent(TransportSyncCommonEventLogConstants.Tuple_VerificationEmailNotSent, ex.GetType().FullName, new object[]
						{
							this.subscriptionAdUser.LegacyExchangeDN,
							ex
						});
						return false;
					}
					hubFqdn = pickerServer.FQDN;
					result = true;
				}
				finally
				{
					pickerServerList.Release();
				}
			}
			return result;
		}

		private void RegisterSmtpServicePrincipalName()
		{
			int num = ServicePrincipalName.RegisterServiceClass("SmtpSvc");
			if (num != 0)
			{
				this.syncLogSession.LogError((TSLID)35UL, EmailSender.Tracer, (long)this.GetHashCode(), "Unable to register SPN.  Status value was: {0}.", new object[]
				{
					num
				});
			}
		}

		private static readonly IEmailSender nullEmailSender = new EmailSender.NullEmailSenderImplementation();

		private static readonly Trace Tracer = ExTraceGlobals.SendAsTracer;

		private PimAggregationSubscription subscription;

		private ADUser subscriptionAdUser;

		private ExchangePrincipal subscriptionExchangePrincipal;

		private string messageId;

		private bool sendAttempted;

		private bool sendSuccessful;

		private SyncLogSession syncLogSession;

		private class NullEmailSenderImplementation : IEmailSender
		{
			public bool SendAttempted
			{
				get
				{
					return false;
				}
			}

			public bool SendSuccessful
			{
				get
				{
					return false;
				}
			}

			public string MessageId
			{
				get
				{
					return string.Empty;
				}
			}

			public void SendWith(Guid sharedSecret)
			{
			}
		}
	}
}
