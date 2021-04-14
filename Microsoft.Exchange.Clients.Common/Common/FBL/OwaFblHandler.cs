using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Exchange.Clients.EventLogs;
using Microsoft.Exchange.Common.Net.Cryptography;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Management.Powershell.CentralAdmin;
using Microsoft.Exchange.Net.ExSmtpClient;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Clients.Common.FBL
{
	internal class OwaFblHandler
	{
		internal OwaFblHandler() : this(DirectorySessionFactory.NonCacheSessionFactory.CreateTenantRecipientSession(false, ConsistencyMode.FullyConsistent, ADSessionSettings.FromConsumerOrganization(), 72, ".ctor", "f:\\15.00.1497\\sources\\dev\\clients\\src\\common\\FBL\\OwaFblHandler.cs"))
		{
		}

		internal OwaFblHandler(ITenantRecipientSession tenantRecipientSession)
		{
			this.tenantRecipientSession = tenantRecipientSession;
			this.FblEnabled = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).OwaClient.EnableFBL.Enabled;
		}

		protected bool FblEnabled { get; set; }

		internal bool TryProcessFbl(string queryParams, out bool? isClassifyRequest, out bool? isOptIn)
		{
			isClassifyRequest = null;
			isOptIn = null;
			FblRequestParameters fblRequestParameters = null;
			bool result;
			try
			{
				FBLPerfCounters.NumberOfFblRequestsReceived.Increment();
				if (!this.FblEnabled)
				{
					result = true;
				}
				else if (string.IsNullOrEmpty(queryParams))
				{
					FBLPerfCounters.NumberOfFblRequestsFailed.Increment();
					result = false;
				}
				else
				{
					fblRequestParameters = this.DecryptAndParseUrlParameters(queryParams);
					if (fblRequestParameters == null)
					{
						result = false;
					}
					else
					{
						if (fblRequestParameters.IsClassifyRequest())
						{
							isClassifyRequest = new bool?(true);
							this.ProcessClassificationRequest(fblRequestParameters);
						}
						else
						{
							isClassifyRequest = new bool?(false);
							isOptIn = new bool?(fblRequestParameters.OptIn);
							if (!this.TryProcessSubscriptionRequest(fblRequestParameters))
							{
								FBLPerfCounters.NumberOfFblRequestsFailed.Increment();
								return false;
							}
						}
						FBLPerfCounters.NumberOfFblRequestsSuccessfullyProcessed.Increment();
						result = true;
					}
				}
			}
			catch (Exception ex)
			{
				FBLPerfCounters.NumberOfFblRequestsFailed.Increment();
				LoggingUtilities.LogEvent(ClientsEventLogConstants.Tuple_FblUnableToProcessRequest, new object[]
				{
					ex.InnerException ?? ex
				});
				result = (fblRequestParameters != null && fblRequestParameters.IsClassifyRequest());
			}
			return result;
		}

		protected virtual void SendMessage(EmailMessage msg, string recipientAddress)
		{
			if (msg == null || string.IsNullOrEmpty(recipientAddress))
			{
				return;
			}
			using (ServerPickerManager serverPickerManager = new ServerPickerManager("OwaFblHandler", ServerRole.HubTransport, ExTraceGlobals.CoreTracer))
			{
				PickerServerList pickerServerList = null;
				try
				{
					pickerServerList = serverPickerManager.GetPickerServerList();
					PickerServer pickerServer = pickerServerList.PickNextUsingRoundRobinPreferringLocal();
					string machineName = pickerServer.MachineName;
					using (SmtpClient smtpClient = new SmtpClient(machineName, 25, new OwaFblHandler.SmtpClientDebugOutput()))
					{
						smtpClient.AuthCredentials(CredentialCache.DefaultNetworkCredentials);
						smtpClient.To = new string[]
						{
							recipientAddress
						};
						using (MemoryStream memoryStream = new MemoryStream())
						{
							msg.MimeDocument.WriteTo(memoryStream);
							smtpClient.DataStream = memoryStream;
							smtpClient.Submit();
						}
					}
				}
				finally
				{
					if (pickerServerList != null)
					{
						pickerServerList.Release();
					}
				}
			}
		}

		protected virtual bool TryGetWIMSPartnerKey(out string partnerKey)
		{
			return OobmCommon.TryGetSecureDigiLogon("EopFblXO1TransportPassword", ref partnerKey);
		}

		private void ProcessClassificationRequest(FblRequestParameters fblRequestParameters)
		{
			if (fblRequestParameters == null)
			{
				return;
			}
			Task.Factory.StartNew(delegate()
			{
				FBLPerfCounters.NumberOfFblClassificationRequestsReceived.Increment();
				ADRawEntry adrawEntry = this.GetADRawEntry(fblRequestParameters.Puid, fblRequestParameters.PrimaryEmail);
				if (!this.IsFblUser(adrawEntry))
				{
					FBLPerfCounters.NumberOfFblClassificationRequestsSuccessfullyProcessed.Increment();
					return;
				}
				if (this.TryPrepareAndSendXmrMessage(fblRequestParameters, "xo1_classify_5WB37Tz5dw899z@outlook.com"))
				{
					FBLPerfCounters.NumberOfFblClassificationRequestsSuccessfullyProcessed.Increment();
					return;
				}
				FBLPerfCounters.NumberOfFblClassificationRequestsFailed.Increment();
			}).ContinueWith(delegate(Task t)
			{
				if (t.Exception != null)
				{
					FBLPerfCounters.NumberOfFblClassificationRequestsFailed.Increment();
					Exception innerException = t.Exception.Flatten().InnerException;
					LoggingUtilities.LogEvent(ClientsEventLogConstants.Tuple_FblUnableToProcessRequest, new object[]
					{
						innerException
					});
				}
			}, TaskContinuationOptions.OnlyOnFaulted);
		}

		private bool TryProcessSubscriptionRequest(FblRequestParameters fblRequestParameters)
		{
			if (fblRequestParameters == null)
			{
				return false;
			}
			ExPerformanceCounter exPerformanceCounter = fblRequestParameters.OptIn ? FBLPerfCounters.NumberOfFblOptInRequestsReceived : FBLPerfCounters.NumberOfFblOptOutRequestsReceived;
			ExPerformanceCounter successSubscriptionCounter = fblRequestParameters.OptIn ? FBLPerfCounters.NumberOfFblOptInRequestsSuccessfullyProcessed : FBLPerfCounters.NumberOfFblOptOutRequestsSuccessfullyProcessed;
			ExPerformanceCounter failedSubscriptionCounter = fblRequestParameters.OptIn ? FBLPerfCounters.NumberOfFblOptInRequestsFailed : FBLPerfCounters.NumberOfFblOptOutRequestsFailed;
			try
			{
				FBLPerfCounters.NumberOfFblSubscriptionRequestsReceived.Increment();
				exPerformanceCounter.Increment();
				ADUser aduser = this.GetADUser(fblRequestParameters.Puid, fblRequestParameters.PrimaryEmail);
				if (aduser == null || !this.TryUpdateMservEntry(aduser, fblRequestParameters))
				{
					FBLPerfCounters.NumberOfFblSubscriptionRequestsFailed.Increment();
					failedSubscriptionCounter.Increment();
					return false;
				}
				Task.Factory.StartNew(delegate()
				{
					if (this.TryPrepareAndSendXmrMessage(fblRequestParameters, "xo1_opt_Dy8l1J4V9X39u6@outlook.com"))
					{
						FBLPerfCounters.NumberOfFblSubscriptionRequestsSuccessfullyProcessed.Increment();
						successSubscriptionCounter.Increment();
						return;
					}
					FBLPerfCounters.NumberOfFblSubscriptionRequestsFailed.Increment();
					failedSubscriptionCounter.Increment();
				}).ContinueWith(delegate(Task t)
				{
					if (t.Exception != null)
					{
						FBLPerfCounters.NumberOfFblSubscriptionRequestsFailed.Increment();
						failedSubscriptionCounter.Increment();
						Exception innerException = t.Exception.Flatten().InnerException;
						LoggingUtilities.LogEvent(ClientsEventLogConstants.Tuple_FblUnableToProcessRequest, new object[]
						{
							innerException
						});
					}
				}, TaskContinuationOptions.OnlyOnFaulted);
			}
			catch (Exception ex)
			{
				FBLPerfCounters.NumberOfFblSubscriptionRequestsFailed.Increment();
				failedSubscriptionCounter.Increment();
				LoggingUtilities.LogEvent(ClientsEventLogConstants.Tuple_FblUnableToProcessRequest, new object[]
				{
					ex.InnerException ?? ex
				});
				return false;
			}
			return true;
		}

		private bool TryPrepareAndSendXmrMessage(FblRequestParameters fblRequestParameters, string recipientAddress)
		{
			if (fblRequestParameters == null || string.IsNullOrEmpty(recipientAddress))
			{
				return false;
			}
			try
			{
				EmailMessage msg = this.CreateFblRequestMessage(fblRequestParameters);
				this.SendMessage(msg, recipientAddress);
				FBLPerfCounters.NumberOfXMRMessagesSuccessfullySent.Increment();
				return true;
			}
			catch (FailedToConnectToSMTPServerException ex)
			{
				LoggingUtilities.LogEvent(ClientsEventLogConstants.Tuple_FblFailedToConnectToSmtpServer, new object[]
				{
					ex
				});
			}
			catch (UnexpectedSmtpServerResponseException ex2)
			{
				LoggingUtilities.LogEvent(ClientsEventLogConstants.Tuple_FblSmtpServerResponse, new object[]
				{
					ex2
				});
			}
			catch (Exception ex3)
			{
				LoggingUtilities.LogEvent(ClientsEventLogConstants.Tuple_FblErrorSendingMessage, new object[]
				{
					ex3.InnerException ?? ex3
				});
			}
			FBLPerfCounters.NumberOfXMRMessagesFailedToSend.Increment();
			return false;
		}

		private FblRequestParameters DecryptAndParseUrlParameters(string queryParams)
		{
			if (string.IsNullOrEmpty(queryParams))
			{
				return null;
			}
			NameValueCollection encryptedQueryString = HttpUtility.ParseQueryString(queryParams);
			NameValueCollection queryParams2 = AuthkeyAuthenticationRequest.DecryptSignedUrl(encryptedQueryString);
			return new FblRequestParameters(queryParams2);
		}

		private EmailMessage CreateFblRequestMessage(FblRequestParameters requestParameters)
		{
			if (requestParameters == null)
			{
				throw new ArgumentNullException("requestParameters", "Cannot create FBL Request Message from null request parameters.");
			}
			EmailMessage emailMessage = EmailMessage.Create();
			this.AddHeader(emailMessage, "X-Version", "1");
			this.AddHeader(emailMessage, "X-GuidCustomer", requestParameters.CustomerGuid.ToString());
			if (requestParameters.IsClassifyRequest())
			{
				this.AddHeader(emailMessage, "X-GuidMail", requestParameters.MailGuid.ToString());
				this.AddHeader(emailMessage, "X-Class", requestParameters.MessageClass);
			}
			else
			{
				RoutingAddress routingAddress = new RoutingAddress(requestParameters.PrimaryEmail);
				this.AddHeader(emailMessage, "X-UserName", routingAddress.LocalPart);
				this.AddHeader(emailMessage, "X-Domain", routingAddress.DomainPart);
				this.AddHeader(emailMessage, "X-OptIn", requestParameters.OptIn ? "1" : "0");
			}
			this.AddEncryptedMessageRoutingHeaderToMessage(emailMessage);
			return emailMessage;
		}

		private bool IsFblUser(ADRawEntry user)
		{
			return user != null && (bool)user[ADUserSchema.FblEnabled];
		}

		private bool TryUpdateMservEntry(ADUser user, FblRequestParameters fblRequestParameters)
		{
			bool result;
			try
			{
				if (user == null || fblRequestParameters == null)
				{
					result = false;
				}
				else
				{
					if (this.tenantRecipientSession is IAggregateSession)
					{
						((IAggregateSession)this.tenantRecipientSession).MbxReadMode = MbxReadMode.NoMbxRead;
					}
					user[ADUserSchema.FblEnabled] = fblRequestParameters.OptIn;
					this.tenantRecipientSession.Save(user);
					FBLPerfCounters.NumberOfSuccessfulMSERVWriteRequests.Increment();
					result = true;
				}
			}
			catch (Exception ex)
			{
				FBLPerfCounters.NumberOfFailedMSERVWriteRequests.Increment();
				LoggingUtilities.LogEvent(ClientsEventLogConstants.Tuple_TransientFblErrorUpdatingMServ, new object[]
				{
					ex.InnerException ?? ex
				});
				result = false;
			}
			return result;
		}

		private ADRawEntry GetADRawEntry(ulong puid, string emailAddress)
		{
			ADRawEntry result;
			try
			{
				List<ADPropertyDefinition> properties = new List<ADPropertyDefinition>(new ADPropertyDefinition[]
				{
					ADMailboxRecipientSchema.ExchangeGuid,
					ADUserSchema.NetID,
					ADMailboxRecipientSchema.Database,
					ADUserSchema.PrimaryMailboxSource,
					ADUserSchema.SatchmoClusterIp,
					ADUserSchema.SatchmoDGroup,
					ADUserSchema.FblEnabled
				});
				ADRawEntry adrawEntry = this.tenantRecipientSession.FindByExchangeGuid(ConsumerIdentityHelper.GetExchangeGuidFromPuid(puid), properties) ?? this.tenantRecipientSession.FindByProxyAddress(ProxyAddress.Parse(emailAddress), properties);
				FBLPerfCounters.NumberOfSuccessfulMSERVReadRequests.Increment();
				result = adrawEntry;
			}
			catch (Exception ex)
			{
				FBLPerfCounters.NumberOfFailedMSERVReadRequests.Increment();
				LoggingUtilities.LogEvent(ClientsEventLogConstants.Tuple_TransientFblErrorReadingMServ, new object[]
				{
					ex.InnerException ?? ex
				});
				throw;
			}
			return result;
		}

		private ADUser GetADUser(ulong puid, string emailAddress)
		{
			ADUser result;
			try
			{
				ADUser aduser = ((ADUser)this.tenantRecipientSession.FindByObjectGuid(ConsumerIdentityHelper.GetExchangeGuidFromPuid(puid))) ?? this.tenantRecipientSession.FindByProxyAddress<ADUser>(ProxyAddress.Parse(emailAddress));
				FBLPerfCounters.NumberOfSuccessfulMSERVReadRequests.Increment();
				result = aduser;
			}
			catch (Exception ex)
			{
				FBLPerfCounters.NumberOfFailedMSERVReadRequests.Increment();
				LoggingUtilities.LogEvent(ClientsEventLogConstants.Tuple_TransientFblErrorReadingMServ, new object[]
				{
					ex.InnerException ?? ex
				});
				throw;
			}
			return result;
		}

		private void AddEncryptedMessageRoutingHeaderToMessage(EmailMessage msg)
		{
			if (msg == null)
			{
				throw new ArgumentNullException("msg", "Cannot encrypt null email message");
			}
			string password;
			if (!this.TryGetWIMSPartnerKey(out password))
			{
				throw new InvalidDataException("Unable to retrieve partner key");
			}
			CryptoHelper.BuildCryptoHelperTable(password);
			CryptoHelper instance = CryptoHelper.GetInstance(password);
			WIMSAuthHeaderCryptoHelper wimsauthHeaderCryptoHelper = new WIMSAuthHeaderCryptoHelper(instance, "XO1TransportConvergence")
			{
				Sender = string.Empty,
				ExpiredAt = DateTime.UtcNow.AddDays(2.0),
				AuthHeaderType = 28
			};
			this.AddHeader(msg, "X-Message-Routing", wimsauthHeaderCryptoHelper.EncryptWithHintFromProperties());
		}

		private void AddHeader(EmailMessage msg, string headerName, string headerValue)
		{
			if (msg == null || string.IsNullOrEmpty(headerName) || string.IsNullOrEmpty(headerValue))
			{
				return;
			}
			Header header = Header.Create(headerName);
			header.Value = headerValue;
			msg.MimeDocument.RootPart.Headers.AppendChild(header);
		}

		private const string SubscriptionRecipientAddress = "xo1_opt_Dy8l1J4V9X39u6@outlook.com";

		private const string ClassificationRecipientAddress = "xo1_classify_5WB37Tz5dw899z@outlook.com";

		private const string PartnerName = "XO1TransportConvergence";

		private const string PartnerKeySettingName = "EopFblXO1TransportPassword";

		private const int MessageType = 28;

		private readonly ITenantRecipientSession tenantRecipientSession;

		private class SmtpClientDebugOutput : ISmtpClientDebugOutput
		{
			public void Output(Trace tracer, object context, string message, params object[] args)
			{
				if (message != null)
				{
					tracer.TraceDebug((long)((context != null) ? context.GetHashCode() : 0), message, args);
				}
			}
		}
	}
}
