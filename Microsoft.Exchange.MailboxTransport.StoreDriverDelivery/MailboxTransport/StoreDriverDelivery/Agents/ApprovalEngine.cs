using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Approval;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverDelivery;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxTransport.StoreDriverCommon;
using Microsoft.Exchange.SecureMail;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal class ApprovalEngine
	{
		private ApprovalEngine(EmailMessage incomingMessage, RoutingAddress sender, RoutingAddress recipient, MessageItem messageItem, MbxTransportMailItem mbxTransportMailItem, ApprovalEngine.ApprovalRequestCreateDelegate requestCreate)
		{
			this.message = incomingMessage;
			this.sender = sender;
			this.recipient = recipient;
			this.requestCreate = requestCreate;
			this.messageItem = messageItem;
			this.mbxTransportMailItem = mbxTransportMailItem;
		}

		public static ApprovalEngine GetApprovalEngineInstance(EmailMessage incomingMessage, RoutingAddress sender, RoutingAddress recipient, MessageItem messageItem, MbxTransportMailItem mbxTransportMailItem, ApprovalEngine.ApprovalRequestCreateDelegate requestCreate)
		{
			return new ApprovalEngine(incomingMessage, sender, recipient, messageItem, mbxTransportMailItem, requestCreate);
		}

		public ApprovalEngine.ApprovalProcessResults ProcessMessage()
		{
			if (!MultilevelAuth.IsInternalMail(this.message))
			{
				return ApprovalEngine.ApprovalProcessResults.Invalid;
			}
			DecisionHandler decisionHandler = null;
			ApprovalEngine.ApprovalProcessResults result;
			try
			{
				InitiationMessage initiationMessage;
				NdrOofHandler ndrOofHandler;
				if (InitiationMessage.TryCreate(this.message, out initiationMessage))
				{
					result = this.HandleInitiationMessage(initiationMessage);
				}
				else if (DecisionHandler.TryCreate(this.messageItem, this.sender.ToString(), this.mbxTransportMailItem.OrganizationId, out decisionHandler))
				{
					ApprovalEngine.ApprovalProcessResults approvalProcessResults = decisionHandler.Process();
					result = approvalProcessResults;
				}
				else if (NdrOofHandler.TryCreate(this.messageItem, out ndrOofHandler))
				{
					result = ndrOofHandler.Process();
				}
				else
				{
					result = ApprovalEngine.ApprovalProcessResults.Invalid;
				}
			}
			finally
			{
				if (decisionHandler != null)
				{
					decisionHandler.Dispose();
				}
			}
			return result;
		}

		public ApprovalEngine.ProcessResult CreateAndSubmitApprovalRequests(int? messageLocaleId)
		{
			InitiationMessage initiationMessage;
			if (InitiationMessage.TryCreate(this.message, out initiationMessage))
			{
				InitiationProcessor initiationProcessor = new InitiationProcessor(this.mbxTransportMailItem, initiationMessage, this.messageItem, this.requestCreate, this.recipient);
				return initiationProcessor.CreateAndSubmitApprovalRequests(messageLocaleId);
			}
			return ApprovalEngine.ProcessResult.Invalid;
		}

		private ApprovalEngine.ApprovalProcessResults HandleInitiationMessage(InitiationMessage initiationMessage)
		{
			if (initiationMessage.IsMapiInitiator)
			{
				if (!this.sender.Equals(this.recipient))
				{
					return ApprovalEngine.ApprovalProcessResults.Invalid;
				}
			}
			else if (string.Equals("ModeratedTransport", initiationMessage.ApprovalInitiator, StringComparison.OrdinalIgnoreCase))
			{
				this.messageItem[MessageItemSchema.ApprovalApplicationId] = 1;
				HeaderList headers = this.message.MimeDocument.RootPart.Headers;
				TextHeader textHeader = headers.FindFirst("X-MS-Exchange-Organization-Moderation-Data") as TextHeader;
				string value;
				if (textHeader != null && textHeader.TryGetValue(out value) && !string.IsNullOrEmpty(value))
				{
					this.messageItem[MessageItemSchema.ApprovalApplicationData] = value;
				}
				this.StampModeratedTransportExpiry();
			}
			if (initiationMessage.DecisionMakers == null)
			{
				return ApprovalEngine.ApprovalProcessResults.Invalid;
			}
			if (InitiationProcessor.CheckDuplicateInitiationAndUpdateIdIfNecessary(this.messageItem))
			{
				return ApprovalEngine.ApprovalProcessResults.DuplicateInitiation;
			}
			InitiationProcessor initiationProcessor = new InitiationProcessor(this.mbxTransportMailItem, initiationMessage, this.messageItem, this.requestCreate, this.recipient);
			return initiationProcessor.PrepareApprovalRequestData();
		}

		private void StampModeratedTransportExpiry()
		{
			byte[] policyTag = null;
			string text = string.Empty;
			int retentionPeriod = 2;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.mbxTransportMailItem.OrganizationId), 361, "StampModeratedTransportExpiry", "f:\\15.00.1497\\sources\\dev\\MailboxTransport\\src\\MailboxTransportDelivery\\StoreDriver\\agents\\approval\\ApprovalEngine.cs");
				ADObjectId descendantId = tenantOrTopologyConfigurationSession.GetOrgContainerId().GetDescendantId(ApprovalApplication.ParentPathInternal);
				ADObjectId childId = descendantId.GetChildId("ModeratedRecipients");
				ApprovalEngine.diag.TraceDebug<ADObjectId>((long)this.GetHashCode(), "Reading ModeratedRecipient app from {0}", childId);
				if (childId != null)
				{
					ApprovalApplication approvalApplication = tenantOrTopologyConfigurationSession.Read<ApprovalApplication>(childId);
					if (approvalApplication != null)
					{
						ADObjectId elcretentionPolicyTag = approvalApplication.ELCRetentionPolicyTag;
						ApprovalEngine.diag.TraceDebug<ADObjectId>((long)this.GetHashCode(), "Read ModeratedRecipient, now reading Recipient Policy Tag {0}", elcretentionPolicyTag);
						RetentionPolicyTag retentionPolicyTag = null;
						if (elcretentionPolicyTag != null)
						{
							retentionPolicyTag = tenantOrTopologyConfigurationSession.Read<RetentionPolicyTag>(elcretentionPolicyTag);
						}
						else
						{
							IConfigurationSession configurationSession = SharedConfiguration.CreateScopedToSharedConfigADSession(this.mbxTransportMailItem.OrganizationId);
							if (configurationSession != null)
							{
								IList<RetentionPolicyTag> defaultRetentionPolicyTag = ApprovalUtils.GetDefaultRetentionPolicyTag(configurationSession, ApprovalApplicationId.ModeratedRecipient, 1);
								if (defaultRetentionPolicyTag != null && defaultRetentionPolicyTag.Count > 0)
								{
									retentionPolicyTag = defaultRetentionPolicyTag[0];
								}
							}
						}
						if (retentionPolicyTag != null)
						{
							ADPagedReader<ElcContentSettings> elccontentSettings = retentionPolicyTag.GetELCContentSettings();
							using (IEnumerator<ElcContentSettings> enumerator = elccontentSettings.GetEnumerator())
							{
								if (enumerator.MoveNext())
								{
									ElcContentSettings elcContentSettings = enumerator.Current;
									retentionPeriod = (int)elcContentSettings.AgeLimitForRetention.Value.TotalDays;
								}
							}
							policyTag = retentionPolicyTag.RetentionId.ToByteArray();
						}
					}
				}
			});
			if (!adoperationResult.Succeeded)
			{
				if (adoperationResult.Exception is TransientException)
				{
					throw adoperationResult.Exception;
				}
				text = adoperationResult.Exception.ToString();
				ApprovalEngine.diag.TraceError<string>((long)this.GetHashCode(), "Can't get PolicyTag guid {0}, NDRing.", text);
			}
			if (policyTag == null)
			{
				ApprovalEngine.diag.TraceError((long)this.GetHashCode(), "PolicyTag not read. NDRing");
				string text2 = this.mbxTransportMailItem.OrganizationId.ToString();
				StoreDriverDeliveryDiagnostics.LogEvent(MailboxTransportEventLogConstants.Tuple_ApprovalCannotStampExpiry, text2, new object[]
				{
					text2,
					text
				});
				throw new SmtpResponseException(AckReason.ApprovalCannotReadExpiryPolicy);
			}
			if (retentionPeriod < 2)
			{
				retentionPeriod = 2;
			}
			else if (retentionPeriod > 30)
			{
				retentionPeriod = 30;
			}
			this.messageItem[ItemSchema.RetentionDate] = ExDateTime.UtcNow.AddDays((double)retentionPeriod);
			this.messageItem[StoreObjectSchema.RetentionPeriod] = retentionPeriod;
			this.messageItem[StoreObjectSchema.PolicyTag] = policyTag;
		}

		private const int MailRetentionInDaysMin = 2;

		private const int MailRetentionInDaysMax = 30;

		private static readonly Trace diag = ExTraceGlobals.ApprovalAgentTracer;

		private EmailMessage message;

		private RoutingAddress sender;

		private RoutingAddress recipient;

		private ApprovalEngine.ApprovalRequestCreateDelegate requestCreate;

		private MessageItem messageItem;

		private MbxTransportMailItem mbxTransportMailItem;

		public delegate MessageItemApprovalRequest ApprovalRequestCreateDelegate(MbxTransportMailItem originalMailItem);

		public enum ProcessResult
		{
			Invalid,
			InitiationMessageOk,
			UnauthorizedMessage,
			InitiationNotFoundForDecision,
			DecisionMarked,
			DecisionAlreadyMade,
			InitiationMessageDuplicate,
			NdrOrOofInvalid,
			NdrOrOofUpdated,
			NdrOrOofUpdateSkipped,
			InitiationNotFoundForNdrOrOof
		}

		internal class ApprovalProcessResults
		{
			public ApprovalProcessResults()
			{
			}

			public ApprovalProcessResults(ApprovalEngine.ProcessResult processResults)
			{
				this.processResults = processResults;
			}

			public ApprovalProcessResults(ApprovalEngine.ProcessResult processResults, long initiationSearchTimeMilliseconds)
			{
				this.processResults = processResults;
				this.initiationMessageSearchTimeMilliseconds = initiationSearchTimeMilliseconds;
			}

			public byte[] ApprovalTrackingBlob
			{
				get
				{
					return this.approvalTrackingBlob;
				}
				internal set
				{
					this.approvalTrackingBlob = value;
				}
			}

			public string ApprovalRequestMessageId
			{
				get
				{
					return this.approvalRequestMessageId;
				}
				internal set
				{
					this.approvalRequestMessageId = value;
				}
			}

			public int TotalDecisionMakers
			{
				get
				{
					return this.totalDecisionMakers;
				}
				internal set
				{
					this.totalDecisionMakers = value;
				}
			}

			public ApprovalEngine.ProcessResult ProcessResults
			{
				get
				{
					return this.processResults;
				}
				internal set
				{
					this.processResults = value;
				}
			}

			public long InitiationMessageSearchTimeMilliseconds
			{
				get
				{
					return this.initiationMessageSearchTimeMilliseconds;
				}
				internal set
				{
					this.initiationMessageSearchTimeMilliseconds = value;
				}
			}

			public string ExistingDecisionMakerAddress
			{
				get
				{
					return this.existingDecisionMakerAddress;
				}
				internal set
				{
					this.existingDecisionMakerAddress = value;
				}
			}

			public ApprovalStatus? ExistingApprovalStatus
			{
				get
				{
					return this.existingApprovalStatus;
				}
				internal set
				{
					this.existingApprovalStatus = value;
				}
			}

			public ExDateTime? ExistingDecisionTime
			{
				get
				{
					return this.existingDecisionTime;
				}
				internal set
				{
					this.existingDecisionTime = value;
				}
			}

			public static readonly ApprovalEngine.ApprovalProcessResults Invalid = new ApprovalEngine.ApprovalProcessResults(ApprovalEngine.ProcessResult.Invalid);

			public static readonly ApprovalEngine.ApprovalProcessResults NdrOofInvalid = new ApprovalEngine.ApprovalProcessResults(ApprovalEngine.ProcessResult.NdrOrOofInvalid);

			public static readonly ApprovalEngine.ApprovalProcessResults DuplicateInitiation = new ApprovalEngine.ApprovalProcessResults(ApprovalEngine.ProcessResult.InitiationMessageDuplicate);

			public static readonly ApprovalEngine.ApprovalProcessResults InitiationNotFoundForDecision = new ApprovalEngine.ApprovalProcessResults(ApprovalEngine.ProcessResult.InitiationNotFoundForDecision);

			private byte[] approvalTrackingBlob;

			private string approvalRequestMessageId;

			private int totalDecisionMakers;

			private ApprovalEngine.ProcessResult processResults;

			private long initiationMessageSearchTimeMilliseconds;

			private string existingDecisionMakerAddress;

			private ApprovalStatus? existingApprovalStatus;

			private ExDateTime? existingDecisionTime;
		}
	}
}
