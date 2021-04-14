using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Approval;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverDelivery;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal class DecisionHandler
	{
		private DecisionHandler(MessageItem messageItem, string sender, bool needDisposing)
		{
			this.messageItem = messageItem;
			this.sender = sender;
			this.needDisposing = needDisposing;
		}

		public static HashSet<string> ApproveTextList
		{
			get
			{
				return DecisionHandler.approveTextList;
			}
			set
			{
				DecisionHandler.approveTextList = value;
			}
		}

		public bool NeedDisposing
		{
			get
			{
				return this.needDisposing;
			}
		}

		public static bool TryCreate(MessageItem messageItem, string sender, OrganizationId organizationId, out DecisionHandler decisionHandler)
		{
			decisionHandler = null;
			MessageItem messageItem2;
			bool flag;
			if (!DecisionHandler.IsDecision(messageItem, out messageItem2, out flag))
			{
				return false;
			}
			decisionHandler = new DecisionHandler(messageItem2, sender, flag);
			return true;
		}

		public ApprovalEngine.ApprovalProcessResults Process()
		{
			MailboxSession mailboxSession = (MailboxSession)this.messageItem.Session;
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox);
			VersionedId correlatedItem = this.messageItem.VotingInfo.GetCorrelatedItem(defaultFolderId);
			stopwatch.Stop();
			long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
			if (correlatedItem == null)
			{
				DecisionHandler.diag.TraceDebug((long)this.GetHashCode(), "Initiation message not found");
				return new ApprovalEngine.ApprovalProcessResults(ApprovalEngine.ProcessResult.InitiationNotFoundForDecision, elapsedMilliseconds);
			}
			string existingDecisionMakerAddress;
			ApprovalStatus? existingApprovalStatus;
			ExDateTime? existingDecisionTime;
			DecisionConflict conflict;
			if (DecisionHandler.ApproveTextList.Contains(this.messageItem.VotingInfo.Response))
			{
				conflict = ApprovalProcessor.ApproveRequest(mailboxSession, correlatedItem.ObjectId, (SmtpAddress)this.sender, this.messageItem.Body, out existingDecisionMakerAddress, out existingApprovalStatus, out existingDecisionTime);
			}
			else
			{
				conflict = ApprovalProcessor.RejectRequest(mailboxSession, correlatedItem.ObjectId, (SmtpAddress)this.sender, this.messageItem.Body, out existingDecisionMakerAddress, out existingApprovalStatus, out existingDecisionTime);
			}
			return DecisionHandler.GetApprovalProcessResults(conflict, existingDecisionMakerAddress, existingApprovalStatus, existingDecisionTime, elapsedMilliseconds);
		}

		public void Dispose()
		{
			if (this.messageItem != null && this.needDisposing)
			{
				this.messageItem.Dispose();
			}
		}

		private static HashSet<string> CreateApproveTextList()
		{
			DecisionHandler.ApproveTextList = new HashSet<string>();
			foreach (CultureInfo formatProvider in LanguagePackInfo.GetInstalledLanguagePackCultures(LanguagePackType.Client))
			{
				DecisionHandler.ApproveTextList.TryAdd(SystemMessages.ApproveButtonText.ToString(formatProvider));
			}
			return DecisionHandler.ApproveTextList;
		}

		private static ApprovalEngine.ApprovalProcessResults GetApprovalProcessResults(DecisionConflict conflict, string existingDecisionMakerAddress, ApprovalStatus? existingApprovalStatus, ExDateTime? existingDecisionTime, long messageSearchElapsedMilliseconds)
		{
			ApprovalEngine.ApprovalProcessResults approvalProcessResults = new ApprovalEngine.ApprovalProcessResults();
			approvalProcessResults.InitiationMessageSearchTimeMilliseconds = messageSearchElapsedMilliseconds;
			if (conflict == DecisionConflict.NoConflict || conflict == DecisionConflict.SameApproverAndDecision || conflict == DecisionConflict.DifferentApproverSameDecision)
			{
				approvalProcessResults.ProcessResults = ApprovalEngine.ProcessResult.DecisionMarked;
			}
			else if (conflict == DecisionConflict.MissingItem)
			{
				approvalProcessResults.ProcessResults = ApprovalEngine.ProcessResult.InitiationNotFoundForDecision;
			}
			else if (conflict == DecisionConflict.Unauthorized)
			{
				approvalProcessResults.ProcessResults = ApprovalEngine.ProcessResult.UnauthorizedMessage;
			}
			else
			{
				approvalProcessResults.ProcessResults = ApprovalEngine.ProcessResult.DecisionAlreadyMade;
				approvalProcessResults.ExistingDecisionMakerAddress = existingDecisionMakerAddress;
				approvalProcessResults.ExistingDecisionTime = existingDecisionTime;
				approvalProcessResults.ExistingApprovalStatus = existingApprovalStatus;
			}
			return approvalProcessResults;
		}

		private static bool IsDecision(MessageItem messageItem, out MessageItem decisionMessageItem, out bool needsDisposing)
		{
			decisionMessageItem = messageItem;
			needsDisposing = false;
			if (messageItem == null || messageItem.Session == null)
			{
				return false;
			}
			if (!string.IsNullOrEmpty(messageItem.VotingInfo.Response))
			{
				return true;
			}
			if (messageItem.AttachmentCollection != null && messageItem.AttachmentCollection.Count == 1)
			{
				IList<AttachmentHandle> handles = messageItem.AttachmentCollection.GetHandles();
				if (handles != null && handles.Count > 0)
				{
					using (ItemAttachment itemAttachment = messageItem.AttachmentCollection.Open(handles[0], AttachmentType.EmbeddedMessage) as ItemAttachment)
					{
						if (itemAttachment != null)
						{
							decisionMessageItem = itemAttachment.GetItemAsMessage();
							needsDisposing = true;
							bool flag = decisionMessageItem != null && decisionMessageItem.Session != null && !string.IsNullOrEmpty(decisionMessageItem.VotingInfo.Response);
							if (decisionMessageItem != null && !flag)
							{
								decisionMessageItem.Dispose();
								needsDisposing = false;
							}
							return flag;
						}
					}
					return false;
				}
			}
			return false;
		}

		private static readonly Microsoft.Exchange.Diagnostics.Trace diag = ExTraceGlobals.ApprovalAgentTracer;

		private static HashSet<string> approveTextList = DecisionHandler.CreateApproveTextList();

		private readonly bool needDisposing;

		private readonly string sender;

		private MessageItem messageItem;
	}
}
