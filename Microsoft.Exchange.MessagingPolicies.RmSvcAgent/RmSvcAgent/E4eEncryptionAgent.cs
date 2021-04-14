using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage.RightsManagement;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Data.Transport.Routing;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Security.RightsManagement;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Agent.AntiSpam.Common;
using Microsoft.Exchange.Transport.Configuration;
using Microsoft.Exchange.Transport.RightsManagement;

namespace Microsoft.Exchange.MessagingPolicies.RmSvcAgent
{
	internal sealed class E4eEncryptionAgent : RoutingAgent
	{
		public E4eEncryptionAgent(E4eEncryptionAgentFactory factory)
		{
			this.agentFactory = factory;
			base.OnRoutedMessage += this.OnRoutedEventHandler;
			this.breadcrumbs.Drop("AgentInitialized");
		}

		private void OnRoutedEventHandler(RoutedMessageEventSource source, QueuedMessageEventArgs args)
		{
			this.breadcrumbs.Drop("E4eEncryptionAgent Started");
			this.tenantId = args.MailItem.TenantId;
			this.source = source;
			this.args = args;
			bool flag = E4eHelper.GetDirectionality(args.MailItem) == MailDirectionality.Originating;
			bool isSupportedMapiMessageClass = Utils.IsSupportedMapiMessageClass(args.MailItem.Message);
			bool flag2 = E4eHelper.IsHeaderSetToTrue(args.MailItem.Message, "X-MS-Exchange-Organization-E4eMessageEncrypted");
			this.shouldReEncryptMessageOrGenerateHtml = E4eEncryptionHelper.ShouldReEncryptMessageOrGenerateHtml(args.MailItem.Message);
			if (!E4eEncryptionHelper.ShouldEncryptMessageOrGenrateHtml(args.MailItem.Message, flag, isSupportedMapiMessageClass, flag2) && !this.shouldReEncryptMessageOrGenerateHtml)
			{
				return;
			}
			if (E4eHelper.IsHeaderSetToTrue(args.MailItem.Message, "X-MS-Exchange-Organization-E4eReEncryptMessage") || E4eHelper.IsHeaderSetToTrue(args.MailItem.Message, "X-MS-Exchange-Organization-E4ePortal"))
			{
				Header xheader = Utils.GetXHeader(args.MailItem.Message, "X-MS-Exchange-Organization-E4eMessageOriginalSender");
				if (xheader == null || string.IsNullOrWhiteSpace(xheader.Value))
				{
					E4eLog.Instance.LogError(args.MailItem.Message.MessageId, "Will be NDRed. XMSExchangeOrganizationE4eMessageOriginalSender header IsNullOrWhiteSpace.", new object[0]);
					Utils.NDRMessage(args.MailItem, args.MailItem.Message.MessageId, Utils.GetResponseForNDR(E4eEncryptionAgent.ndrTextHeadersMissing));
					return;
				}
				Header xheader2 = Utils.GetXHeader(args.MailItem.Message, "X-MS-Exchange-Organization-E4eMessageOriginalSenderOrgId");
				if (xheader2 == null || string.IsNullOrWhiteSpace(xheader2.Value))
				{
					E4eLog.Instance.LogError(args.MailItem.Message.MessageId, "Will be NDRed. XMSExchangeOrganizationE4eMessageOriginalSenderOrgId header IsNullOrWhiteSpace.", new object[0]);
					Utils.NDRMessage(args.MailItem, args.MailItem.Message.MessageId, Utils.GetResponseForNDR(E4eEncryptionAgent.ndrTextHeadersMissing));
					return;
				}
			}
			this.currentP2Sender = E4eHelper.GetP2From(args.MailItem.Message);
			this.originalSender = E4eHelper.GetOriginalSender(args.MailItem);
			if (!E4eAgentCommon.RunUnderExceptionHandler(args.MailItem, source, true, "Microsoft.Exchange.E4eEncryptionAgent.DeferralCount", delegate
			{
				this.originalSenderOrgId = E4eHelper.GetOriginalSenderOrgId(args.MailItem);
			}, new E4eHelper.CompleteProcessDelegate(this.CompleteProcess)))
			{
				return;
			}
			if (!E4eHelper.IsHeaderSetToTrue(args.MailItem.Message, "X-MS-Exchange-Organization-E4eReEncryptMessage") && !E4eHelper.IsHeaderSetToTrue(args.MailItem.Message, "X-MS-Exchange-Organization-E4ePortal") && !flag2)
			{
				E4eEncryptionHelper.ValidateOriginalSender("5afe0b00-7697-4969-b663-5eab37d5f47e", args.MailItem.Message.MessageId, this.originalSenderOrgId, ref this.originalSender);
			}
			if (string.IsNullOrWhiteSpace(this.originalSender) || !RoutingAddress.IsValidAddress(this.originalSender))
			{
				E4eLog.Instance.LogError(args.MailItem.Message.MessageId, "Will be NDRed. originalSender '{0}' IsNullOrWhiteSpace or not a valid address.", new object[]
				{
					string.IsNullOrWhiteSpace(this.originalSender) ? "<blank>" : this.originalSender
				});
				Utils.NDRMessage(args.MailItem, args.MailItem.Message.MessageId, Utils.GetResponseForNDR(E4eEncryptionAgent.ndrTextOriginalSenderInvalid));
				return;
			}
			MiniRecipient miniRecipient = E4eHelper.GetMiniRecipient(args.MailItem, this.originalSender, this.originalSenderOrgId);
			this.encryptionHelper = E4eHelper.GetE4eEncryptionHelper(miniRecipient);
			if (flag && !flag2 && !E4eHelper.IsHeaderSetToTrue(args.MailItem.Message, "X-MS-Exchange-Organization-E4ePortal") && !E4eHelper.IsHeaderSetToTrue(args.MailItem.Message, "X-MS-Exchange-Organization-E4eReEncryptMessage") && !E4eHelper.IsFlightingFeatureEnabledForSender(this.args.MailItem, this.originalSender, this.originalSenderOrgId))
			{
				E4eLog.Instance.LogError(args.MailItem.Message.MessageId, "Will be NDRed. Flighting is not enabled for sender: {0}.", new object[]
				{
					this.originalSender
				});
				Utils.NDRMessage(args.MailItem, args.MailItem.Message.MessageId, Utils.GetResponseForNDR(E4eEncryptionAgent.ndrTextFeatureDisabled));
				return;
			}
			if (!flag2 && E4eHelper.IsHeaderSetToTrue(args.MailItem.Message, "X-MS-Exchange-Organization-E4eDecryptMessage"))
			{
				List<EnvelopeRecipient> internalRecipients = this.GetInternalRecipients();
				if (internalRecipients.Count > 0)
				{
					if (internalRecipients.Count < this.args.MailItem.Recipients.Count)
					{
						source.Fork(internalRecipients);
					}
					E4eHelper.RemoveHeader(this.args.MailItem.Message, "X-MS-Exchange-Organization-E4eReEncryptMessage");
					E4eLog.Instance.LogInfo(args.MailItem.Message.MessageId, "Encryption skipped for {0} internal recipients listed above.", new object[]
					{
						internalRecipients.Count
					});
					return;
				}
			}
			E4eLog.Instance.LogInfo(args.MailItem.Message.MessageId, "[E]FileVersion: {0}", new object[]
			{
				"15.00.1497.015"
			});
			E4eLog.Instance.LogInfo(args.MailItem.Message.MessageId, "[E]IsOriginatingMail: '{0}'", new object[]
			{
				flag
			});
			E4eLog.Instance.LogInfo(args.MailItem.Message.MessageId, "[E]P1 Sender: '{0}'", new object[]
			{
				args.MailItem.FromAddress.ToString()
			});
			E4eLog.Instance.LogInfo(args.MailItem.Message.MessageId, "[E]P2 Sender: '{0}'", new object[]
			{
				this.currentP2Sender
			});
			E4eHelper.LogAllE4eHeaders(args.MailItem.Message, "[E]");
			if (!E4eAgentCommon.RunUnderExceptionHandler(args.MailItem, source, true, "Microsoft.Exchange.E4eEncryptionAgent.DeferralCount", delegate
			{
				string text;
				CultureInfo cultureInfo;
				Encoding encoding;
				E4eHelper.GetCultureInfo(args.MailItem.Message, out text, out cultureInfo, out encoding);
				this.cultureInfo = cultureInfo;
			}, new E4eHelper.CompleteProcessDelegate(this.CompleteProcess)))
			{
				E4eLog.Instance.LogError(args.MailItem.Message.MessageId, "GetCultureInfo failed in OnRoutedEventHandler.", new object[0]);
				return;
			}
			bool flag3;
			if (!flag2)
			{
				flag3 = E4eAgentCommon.RunUnderExceptionHandler(args.MailItem, source, true, "Microsoft.Exchange.E4eEncryptionAgent.DeferralCount", delegate
				{
					if (!this.shouldReEncryptMessageOrGenerateHtml && !RmsClientManager.IRMConfig.IsInternalLicensingEnabledForTenant(this.originalSenderOrgId))
					{
						E4eLog.Instance.LogError(args.MailItem.Message.MessageId, "Will be NDRed. Internal licensing is not enabled for tenant id: {0}.", new object[]
						{
							this.tenantId
						});
						Utils.NDRMessage(args.MailItem, args.MailItem.Message.MessageId, Utils.GetResponseForNDR(E4eEncryptionAgent.ndrTextEncryptionDisabled));
						return;
					}
					if (this.shouldReEncryptMessageOrGenerateHtml)
					{
						E4eLog.Instance.LogInfo(args.MailItem.Message.MessageId, "[E]Re-encryption scenario.", new object[0]);
						string text;
						string text2;
						Uri uri;
						E4eHelper.GetTransportPLAndULAndLicenseUri(args.MailItem, out text, out text2, out uri);
						if (string.IsNullOrWhiteSpace(text))
						{
							E4eLog.Instance.LogError(args.MailItem.Message.MessageId, "Re-encryption fail b/c PL is null or empty.", new object[0]);
							this.args.MailItem.DsnFormatRequested = DsnFormatRequested.Headers;
							Utils.NDRMessage(this.args.MailItem, this.args.MailItem.Message.MessageId, Utils.GetResponseForNDR(this.unableToReEncryptNoPL));
							return;
						}
						if (string.IsNullOrWhiteSpace(text2))
						{
							E4eLog.Instance.LogError(args.MailItem.Message.MessageId, "Re-encryption fail b/c UL is null or empty.", new object[0]);
							this.args.MailItem.DsnFormatRequested = DsnFormatRequested.Headers;
							Utils.NDRMessage(this.args.MailItem, this.args.MailItem.Message.MessageId, Utils.GetResponseForNDR(this.unableToReEncryptNoUL));
							return;
						}
						if (uri == null)
						{
							E4eLog.Instance.LogError(args.MailItem.Message.MessageId, "Re-encryption fail b/c LicenseUri is null or empty.", new object[0]);
							this.args.MailItem.DsnFormatRequested = DsnFormatRequested.Headers;
							Utils.NDRMessage(this.args.MailItem, this.args.MailItem.Message.MessageId, Utils.GetResponseForNDR(this.unableToReEncryptNoLicenseUri));
							return;
						}
						if (!this.TryPromoteActiveAgent())
						{
							return;
						}
						this.rmsEncryptor = this.encryptionHelper.CreateRmsEncryptor(args.MailItem, this.originalSenderOrgId, text, text2, uri, this.breadcrumbs);
						return;
					}
					else
					{
						if (E4eHelper.IsHeaderSetToTrue(args.MailItem.Message, "X-MS-Exchange-Organization-E4ePortal"))
						{
							E4eLog.Instance.LogInfo(args.MailItem.Message.MessageId, "[E]First time encryption triggered via E4E portal.", new object[0]);
						}
						else
						{
							E4eLog.Instance.LogInfo(args.MailItem.Message.MessageId, "[E]First time encryption triggered via Transport Rules.", new object[0]);
						}
						if (!this.TryPromoteActiveAgent())
						{
							return;
						}
						List<string> p2Recipients = this.encryptionHelper.GetP2Recipients(args.MailItem);
						List<string> p1Recipients = this.encryptionHelper.GetP1Recipients(args.MailItem);
						args.MailItem.Properties["Microsoft.Exchange.RMSEncryptionAgent.RecipientListForPL"] = p2Recipients.Union(p1Recipients, StringComparer.OrdinalIgnoreCase).ToList<string>();
						this.rmsEncryptor = this.encryptionHelper.CreateRmsEncryptor(args.MailItem, this.originalSenderOrgId, this.breadcrumbs);
						return;
					}
				}, new E4eHelper.CompleteProcessDelegate(this.CompleteProcess));
				if (flag3 && this.rmsEncryptor != null)
				{
					try
					{
						this.encryptionHelper.EncryptMessage(this.rmsEncryptor, new AsyncCallback(this.OnEncryptionCompleted), new AgentAsyncState(base.GetAgentAsyncContext()));
						return;
					}
					catch (Exception exception)
					{
						this.IncrementFailureCount();
						this.CompleteProcess(null);
						E4eAgentCommon.HandlePermanentException(args.MailItem, exception, null, true);
						return;
					}
				}
				this.IncrementFailureCount();
				this.CompleteProcess(null);
				return;
			}
			E4eLog.Instance.LogInfo(args.MailItem.Message.MessageId, "[E]Generate html scenario.", new object[0]);
			flag3 = this.AfterEncryptionCompleted();
			if (flag3)
			{
				E4eAgentPerfCounters.AfterEncryptionSuccessCount.Increment();
				E4eLog.Instance.LogInfo(args.MailItem.Message.MessageId, "AfterEncryptionCompleted success in OnRoutedEventHandler.", new object[0]);
				return;
			}
			E4eAgentPerfCounters.AfterEncryptionFailureCount.Increment();
			E4eLog.Instance.LogError(args.MailItem.Message.MessageId, "AfterEncryptionCompleted failed in OnRoutedEventHandler.", new object[0]);
		}

		private void OnEncryptionCompleted(IAsyncResult asyncResult)
		{
			this.breadcrumbs.Drop("OnEncryptionCompleted");
			ArgumentValidator.ThrowIfNull("asyncResult", asyncResult);
			ArgumentValidator.ThrowIfNull("asyncResult.AsyncState", asyncResult.AsyncState);
			AgentAsyncState agentAsyncState = (AgentAsyncState)asyncResult.AsyncState;
			agentAsyncState.Resume();
			try
			{
				AsyncOperationResult<EmailMessage> asyncOperationResult = this.rmsEncryptor.EndEncrypt(asyncResult);
				EmailMessage data = asyncOperationResult.Data;
				Exception exception = asyncOperationResult.Exception;
				if (asyncOperationResult.IsSucceeded)
				{
					E4eLog.Instance.LogInfo(this.args.MailItem.Message.MessageId, "Successfully encrypted the mail.", new object[0]);
					E4eHelper.OverrideMime(this.args.MailItem, data);
					Utils.StampXHeader(this.args.MailItem.Message, "X-MS-Exchange-Organization-E4eMessageEncrypted", "true");
					E4eHelper.RemoveHeader(this.args.MailItem.Message, "X-MS-Exchange-OMEMessageEncrypted");
					Utils.StampXHeader(this.args.MailItem.Message, "X-MS-Exchange-OMEMessageEncrypted", "true");
					Utils.StampXHeader(this.args.MailItem.Message, "X-MS-Exchange-Organization-E4eMessageDecrypted", "true");
					E4eHelper.RemoveHeader(this.args.MailItem.Message, "X-MS-Exchange-Organization-E4eReEncryptMessage");
					E4eHelper.SetTransportPLAndULAndLicenseUri(this.args.MailItem, string.Empty, string.Empty, null);
					this.IncrementSuccessCount();
					bool flag = this.AfterEncryptionCompleted();
					if (flag)
					{
						E4eAgentPerfCounters.AfterEncryptionSuccessCount.Increment();
						E4eLog.Instance.LogInfo(this.args.MailItem.Message.MessageId, "AfterEncryptionCompleted success in OnEncryptionCompleted.", new object[0]);
						LamHelper.PublishSuccessfulE4eEncryptionToLAM(this.args.MailItem);
					}
					else
					{
						E4eAgentPerfCounters.AfterEncryptionFailureCount.Increment();
						E4eLog.Instance.LogError(this.args.MailItem.Message.MessageId, "AfterEncryptionCompleted failed in OnEncryptionCompleted.", new object[0]);
					}
				}
				else
				{
					this.IncrementFailureCount();
					this.HandleEncryptionException(this.args.MailItem, exception, this.source);
				}
			}
			finally
			{
				this.CompleteProcess(agentAsyncState);
				this.rmsEncryptor.Dispose();
			}
		}

		private bool AfterEncryptionCompleted()
		{
			bool flag = true;
			object obj;
			this.args.MailItem.Properties.TryGetValue("Microsoft.Exchange.Encryption.TransportRpmsg", out obj);
			if (string.IsNullOrWhiteSpace((string)obj))
			{
				flag = E4eAgentCommon.RunUnderExceptionHandler(this.args.MailItem, this.source, true, "Microsoft.Exchange.E4eEncryptionAgent.DeferralCount", delegate
				{
					this.args.MailItem.Properties["Microsoft.Exchange.Encryption.TransportRpmsg"] = this.encryptionHelper.EmailMessageToString(this.args.MailItem.Message, Utils.GetOutboundConversionOptions(this.args.MailItem));
				}, new E4eHelper.CompleteProcessDelegate(this.CompleteProcess));
				if (flag)
				{
					E4eLog.Instance.LogInfo(this.args.MailItem.Message.MessageId, "EmailMessageToString success in OnEncryptionCompleted.", new object[0]);
				}
				else
				{
					E4eLog.Instance.LogError(this.args.MailItem.Message.MessageId, "EmailMessageToString failed in OnEncryptionCompleted.", new object[0]);
				}
			}
			if (flag)
			{
				if (this.args.MailItem.Recipients.Count > 1)
				{
					List<EnvelopeRecipient> list = new List<EnvelopeRecipient>();
					list.Add(this.args.MailItem.Recipients[0]);
					this.source.Fork(list);
				}
				flag = E4eAgentCommon.RunUnderExceptionHandler(this.args.MailItem, this.source, true, "Microsoft.Exchange.E4eEncryptionAgent.DeferralCount", delegate
				{
					this.encryptionHelper.ReplaceRpmsgWithE4eMsg(this.args.MailItem, this.originalSenderOrgId, this.originalSender, this.currentP2Sender, this.cultureInfo);
					Utils.StampXHeader(this.args.MailItem.Message, "X-MS-Exchange-Organization-E4eHtmlFileGenerated", "true");
				}, new E4eHelper.CompleteProcessDelegate(this.CompleteProcess));
			}
			return flag;
		}

		private void CompleteProcess(AgentAsyncState agentAsyncState)
		{
			this.breadcrumbs.Drop("E4eEncryptionAgent CleanedUp");
			if (this.isActive)
			{
				this.agentFactory.InstanceController.MakeInactive(this.tenantId);
				this.isActive = false;
			}
			if (agentAsyncState != null)
			{
				agentAsyncState.Complete();
			}
		}

		private bool TryPromoteActiveAgent()
		{
			E4eLog.Instance.LogInfo(this.args.MailItem.Message.MessageId, "E4eEncryptionAgent is trying to promote itself as Active Agent ...", new object[0]);
			this.isActive = this.agentFactory.InstanceController.TryMakeActive(this.tenantId);
			if (!this.isActive)
			{
				E4eLog.Instance.LogInfo(this.args.MailItem.Message.MessageId, "Unable to make active E4eEncryptionAgent - deferring message.", new object[0]);
				this.source.Defer(RmsClientManager.AppSettings.ActiveAgentCapDeferInterval, Utils.GetResponseForDeferral(E4eAgentCommon.AgentCapReachedDeferralText));
			}
			else
			{
				E4eLog.Instance.LogInfo(this.args.MailItem.Message.MessageId, "E4eEncryptionAgent is promoted as Active Agent.", new object[0]);
			}
			return this.isActive;
		}

		private void HandleEncryptionException(MailItem mailItem, Exception exception, QueuedMessageEventSource source)
		{
			bool flag = false;
			string[] additionalInfo = null;
			if (exception is RightsManagementException)
			{
				RightsManagementException ex = (RightsManagementException)exception;
				if (!ex.IsPermanent)
				{
					flag = true;
				}
				RightsManagementFailureCode failureCode = ex.FailureCode;
				if (failureCode == RightsManagementFailureCode.TemplateAcquisitionFailed || failureCode == RightsManagementFailureCode.TemplateDoesNotExist)
				{
					additionalInfo = new string[]
					{
						string.Format(CultureInfo.InvariantCulture, "A failure occurred when trying to look up Rights Management Server template '{0}'.", new object[]
						{
							"81E24817-F117-4943-8959-60E1477E67B6"
						})
					};
				}
			}
			else if (exception is ExchangeConfigurationException)
			{
				flag = true;
			}
			else
			{
				if (!(exception is MessageConversionException))
				{
					throw new InvalidOperationException("Unexpected exception from RmsEncryptor.Encrypt {0}", exception);
				}
				MessageConversionException ex2 = (MessageConversionException)exception;
				if (ex2.IsTransient)
				{
					flag = true;
				}
			}
			if (flag)
			{
				E4eAgentCommon.HandleTransientException(mailItem, exception, additionalInfo, source, true, "Microsoft.Exchange.E4eEncryptionAgent.DeferralCount");
				return;
			}
			E4eAgentCommon.HandlePermanentException(mailItem, exception, additionalInfo, true);
		}

		private void IncrementSuccessCount()
		{
			if (!this.args.MailItem.IsProbeMessage)
			{
				if (this.shouldReEncryptMessageOrGenerateHtml)
				{
					E4eAgentPerfCounters.ReEncryptionSuccessCount.Increment();
					return;
				}
				E4eAgentPerfCounters.EncryptionSuccessCount.Increment();
			}
		}

		private void IncrementFailureCount()
		{
			if (!this.args.MailItem.IsProbeMessage)
			{
				if (this.shouldReEncryptMessageOrGenerateHtml)
				{
					E4eAgentPerfCounters.ReEncryptionFailureCount.Increment();
					return;
				}
				E4eAgentPerfCounters.EncryptionFailureCount.Increment();
			}
		}

		private List<EnvelopeRecipient> GetInternalRecipients()
		{
			List<EnvelopeRecipient> list = new List<EnvelopeRecipient>();
			foreach (EnvelopeRecipient envelopeRecipient in this.args.MailItem.Recipients)
			{
				if (!CommonUtils.IsExternalRecipient(envelopeRecipient, ExTraceGlobals.RightsManagementTracer, this.GetHashCode()))
				{
					E4eLog.Instance.LogInfo(this.args.MailItem.Message.MessageId, "IsExternalRecipient is false. Encryption will be skipped for internal recipient: {0}.", new object[]
					{
						envelopeRecipient.Address
					});
					list.Add(envelopeRecipient);
				}
				else
				{
					OrganizationId organizationId = Utils.OrgIdFromMailItem(this.args.MailItem);
					PerTenantAcceptedDomainTable perTenantAcceptedDomainTable;
					if (Components.Configuration.TryGetAcceptedDomainTable(organizationId, out perTenantAcceptedDomainTable) && perTenantAcceptedDomainTable.AcceptedDomainTable != null)
					{
						if (perTenantAcceptedDomainTable.AcceptedDomainTable.Find(envelopeRecipient.Address.DomainPart) != null)
						{
							E4eLog.Instance.LogInfo(this.args.MailItem.Message.MessageId, "Recipient domain found in ADT. Encryption will be skipped for internal recipient: {0}.", new object[]
							{
								envelopeRecipient.Address
							});
							list.Add(envelopeRecipient);
						}
					}
					else
					{
						E4eLog.Instance.LogError(this.args.MailItem.Message.MessageId, "Unable to get ADT for org: {0}.", new object[]
						{
							organizationId.ToString()
						});
					}
				}
			}
			return list;
		}

		private const string SafeSenderName = "5afe0b00-7697-4969-b663-5eab37d5f47e";

		private static readonly string[] ndrTextEncryptionDisabled = new string[]
		{
			"Cannot OME protect the message because Encryption is disabled in Microsoft Exchange Transport."
		};

		private static readonly string[] ndrTextFeatureDisabled = new string[]
		{
			string.Format("Cannot OME protect the message. Error Code: {0}.", 1)
		};

		private static readonly string[] ndrTextHeadersMissing = new string[]
		{
			string.Format("Cannot OME protect the message. Error Code: {0}.", 2)
		};

		private static readonly string[] ndrTextOriginalSenderInvalid = new string[]
		{
			string.Format("Cannot OME protect the message. Error Code: {0}.", 3)
		};

		private readonly Breadcrumbs<string> breadcrumbs = new Breadcrumbs<string>(8);

		private bool isActive;

		private RoutedMessageEventSource source;

		private QueuedMessageEventArgs args;

		private E4eEncryptionHelper encryptionHelper;

		private Guid tenantId;

		private RmsEncryptor rmsEncryptor;

		private string originalSender;

		private string currentP2Sender;

		private OrganizationId originalSenderOrgId;

		private CultureInfo cultureInfo;

		private bool shouldReEncryptMessageOrGenerateHtml;

		private E4eEncryptionAgentFactory agentFactory;

		private readonly string[] unableToReEncryptNoPL = new string[]
		{
			"Microsoft Exchange Transport cannot RMS re-encrypt the message (ATTR1)."
		};

		private readonly string[] unableToReEncryptNoUL = new string[]
		{
			"Microsoft Exchange Transport cannot RMS re-encrypt the message (ATTR2)."
		};

		private readonly string[] unableToReEncryptNoLicenseUri = new string[]
		{
			"Microsoft Exchange Transport cannot RMS re-encrypt the message (ATTR4)."
		};
	}
}
