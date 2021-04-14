using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Mime.Internal;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Approval;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverDelivery;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal class InitiationProcessor
	{
		static InitiationProcessor()
		{
			if (ClientCultures.IsCultureSupportedForDsn(CultureInfo.CurrentCulture))
			{
				InitiationProcessor.DefaultFallBackCulture = CultureInfo.CurrentCulture;
				return;
			}
			InitiationProcessor.DefaultFallBackCulture = CultureInfo.GetCultureInfo("en-US");
		}

		internal InitiationProcessor(MbxTransportMailItem mbxTransportMailItem, InitiationMessage initiationMessage, MessageItem initiationMessageItem, ApprovalEngine.ApprovalRequestCreateDelegate requestCreate, RoutingAddress approvalRequestSender)
		{
			this.mbxTransportMailItem = mbxTransportMailItem;
			this.initiationMessage = initiationMessage;
			this.initiationMessageItem = initiationMessageItem;
			this.requestCreate = requestCreate;
			this.approvalRequestSender = approvalRequestSender;
			CultureInfo internalDsnDefaultLanguage = this.mbxTransportMailItem.TransportSettings.InternalDsnDefaultLanguage;
			if (internalDsnDefaultLanguage == null || !ClientCultures.IsCultureSupportedForDsn(internalDsnDefaultLanguage))
			{
				this.organizationFallbackCulture = InitiationProcessor.DefaultFallBackCulture;
				return;
			}
			this.organizationFallbackCulture = internalDsnDefaultLanguage;
		}

		internal static bool CheckDuplicateInitiationAndUpdateIdIfNecessary(MessageItem message)
		{
			message.Load(InitiationProcessor.PropertiesForUniquenessCheck);
			string internetMessageId = message.InternetMessageId;
			string text;
			if (!InitiationProcessor.TryGetIdentiferFromInitiationMessageId(internetMessageId, out text))
			{
				return false;
			}
			InitiationProcessor.diag.TraceDebug<string, string>(0L, "Find dup for init message messageid='{0}' identifier='{1}'.", internetMessageId, text);
			string text2 = message.TryGetProperty(MessageItemSchema.ApprovalRequestor) as string;
			string text3 = message.TryGetProperty(MessageItemSchema.ApprovalApplicationData) as string;
			MailboxSession session = (MailboxSession)message.Session;
			IStorePropertyBag[] array;
			try
			{
				array = AllItemsFolderHelper.FindItemsFromInternetId(session, text, InitiationProcessor.PropertiesForUniquenessCheck);
			}
			catch (ObjectNotFoundException arg)
			{
				InitiationProcessor.diag.TraceError<ObjectNotFoundException>(0L, "Exception - {0}", arg);
				return false;
			}
			bool flag = false;
			bool flag2 = false;
			if (array != null && array.Length > 0)
			{
				InitiationProcessor.diag.TraceDebug<int, string>(0L, "Found {0} initiation messages with the message id. messageId={1}.", array.Length, text);
				for (int i = 0; i < array.Length; i++)
				{
					flag = true;
					string text4 = array[i].TryGetProperty(MessageItemSchema.ApprovalRequestor) as string;
					string text5 = array[i].TryGetProperty(MessageItemSchema.ApprovalApplicationData) as string;
					if (string.Equals(text4, text2, StringComparison.OrdinalIgnoreCase) && string.Equals(text5, text3, StringComparison.OrdinalIgnoreCase))
					{
						flag2 = true;
						break;
					}
					InitiationProcessor.diag.TraceDebug(0L, "Matching message-id={0} unmatched data requestor={1} data={2} found: requestor={3} data={4}.", new object[]
					{
						text,
						text2,
						text3,
						text4,
						text5
					});
				}
			}
			InitiationProcessor.diag.TraceDebug<string, bool, bool>(0L, "'{0}' found={1}, duplicateInitiation = {2}", text, flag, flag2);
			if (!flag2)
			{
				if (flag)
				{
					message.InternetMessageId = Guid.NewGuid().ToString().Substring(0, 5) + "_" + internetMessageId;
					InitiationProcessor.diag.TraceDebug<string, string>(0L, "Rewriting from message id from {0} to {1}", internetMessageId, message.InternetMessageId);
				}
				else if (!flag)
				{
					message.InternetMessageId = text;
				}
			}
			return flag2;
		}

		internal ApprovalEngine.ApprovalProcessResults PrepareApprovalRequestData()
		{
			this.initiationMessageItem.Load(InitiationProcessor.PropertiesFromInitiationForApprovalRequestCreation);
			this.CopyPropertiesToInitiationMessageFromMessageItem();
			string local = Guid.NewGuid().ToString();
			byte[] approvalTrackingBlob = Guid.NewGuid().ToByteArray();
			string domainPart = this.approvalRequestSender.DomainPart;
			return new ApprovalEngine.ApprovalProcessResults(ApprovalEngine.ProcessResult.InitiationMessageOk)
			{
				ApprovalTrackingBlob = approvalTrackingBlob,
				ApprovalRequestMessageId = ApprovalRequestWriter.FormatStoredApprovalRequestMessageId(local, domainPart),
				TotalDecisionMakers = this.initiationMessage.DecisionMakers.Count
			};
		}

		internal ApprovalEngine.ProcessResult CreateAndSubmitApprovalRequests(int? messageLocaleId)
		{
			this.initiationMessageItem.Load(InitiationProcessor.PropertiesFromInitiationForApprovalRequestCreation);
			this.CopyPropertiesToInitiationMessageFromMessageItem();
			this.initiationMessage.MessageItemLocale = messageLocaleId;
			ApprovalApplicationId? valueAsNullable = this.initiationMessageItem.GetValueAsNullable<ApprovalApplicationId>(MessageItemSchema.ApprovalApplicationId);
			string valueOrDefault = this.initiationMessageItem.GetValueOrDefault<string>(MessageItemSchema.ApprovalRequestMessageId);
			if (string.IsNullOrEmpty(valueOrDefault) || this.initiationMessageItem.VotingInfo == null)
			{
				return ApprovalEngine.ProcessResult.Invalid;
			}
			string local;
			string domain;
			if (!FindMessageUtils.TryParseMessageId(valueOrDefault, out local, out domain))
			{
				return ApprovalEngine.ProcessResult.Invalid;
			}
			byte[] messageCorrelationBlob = this.initiationMessageItem.VotingInfo.MessageCorrelationBlob;
			using (ApprovalRequestWriter instance = ApprovalRequestWriter.GetInstance(valueAsNullable, this.mbxTransportMailItem.OrganizationId, this.initiationMessage))
			{
				Dictionary<CultureInfo, List<RoutingAddress>> dictionary = null;
				if (instance.SupportMultipleRequestsForDifferentCultures && ApprovalProcessor.TryGetCulturesForDecisionMakers(this.initiationMessage.DecisionMakers, this.mbxTransportMailItem.ADRecipientCache.ADSession, this.organizationFallbackCulture, out dictionary))
				{
					int num = 0;
					using (Dictionary<CultureInfo, List<RoutingAddress>>.KeyCollection.Enumerator enumerator = dictionary.Keys.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							CultureInfo cultureInfo = enumerator.Current;
							IList<RoutingAddress> list = dictionary[cultureInfo];
							string text = ApprovalRequestWriter.FormatApprovalRequestMessageId(local, num++, domain, false);
							this.SendApprovalRequest(instance, list, cultureInfo, text, messageCorrelationBlob);
							InitiationProcessor.diag.TraceDebug<string, int, long>((long)this.GetHashCode(), "Approval request '{0}' submitted for {1} decision makers, init message internal id '{2}'", text, list.Count, this.mbxTransportMailItem.RecordId);
						}
						goto IL_19C;
					}
				}
				string text2 = ApprovalRequestWriter.FormatApprovalRequestMessageId(local, 0, domain, false);
				this.SendApprovalRequest(instance, null, this.organizationFallbackCulture, text2, messageCorrelationBlob);
				InitiationProcessor.diag.TraceDebug<string, long>((long)this.GetHashCode(), "Approval request '{0}' submitted for all decision makers for init message with internal id '{1}'", text2, this.mbxTransportMailItem.RecordId);
				IL_19C:;
			}
			return ApprovalEngine.ProcessResult.InitiationMessageOk;
		}

		private static string GetAdditionalExceptionInformation(Exception exception)
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			stringBuilder.AppendFormat("Exception Message: {0}", (exception.Message != null) ? exception.Message : string.Empty);
			if (exception.InnerException != null)
			{
				Exception innerException = exception.InnerException;
				stringBuilder.AppendFormat("Inner Exception Message: {0}", (innerException.Message != null) ? innerException.Message : string.Empty);
				stringBuilder.AppendFormat("Inner Exception Stack trace : {0}", (innerException.StackTrace != null) ? innerException.StackTrace : string.Empty);
			}
			return stringBuilder.ToString();
		}

		private static void AddVotingToApprovalRequest(MessageItemApprovalRequest approvalRequest, MimeConstant.ApprovalAllowedAction votingAction, CultureInfo cultureInfoWritten)
		{
			bool allowComments = votingAction == MimeConstant.ApprovalAllowedAction.ApproveRejectComments || votingAction == MimeConstant.ApprovalAllowedAction.ApproveRejectCommentOnApprove;
			bool allowComments2 = votingAction == MimeConstant.ApprovalAllowedAction.ApproveRejectComments || votingAction == MimeConstant.ApprovalAllowedAction.ApproveRejectCommentOnReject;
			approvalRequest.AddVotingOption(SystemMessages.ApproveButtonText.ToString(cultureInfoWritten), allowComments);
			approvalRequest.AddVotingOption(SystemMessages.RejectButtonText.ToString(cultureInfoWritten), allowComments2);
		}

		private static string ConstructApprovalRequestReferences(MessageItem messageItem)
		{
			string references = messageItem.References;
			string internetMessageId = messageItem.InternetMessageId;
			if (!string.IsNullOrEmpty(references))
			{
				return references + " " + internetMessageId;
			}
			return internetMessageId;
		}

		private static bool TryGetIdentiferFromInitiationMessageId(string messageId, out string identifier)
		{
			identifier = string.Empty;
			if (string.IsNullOrEmpty(messageId))
			{
				InitiationProcessor.diag.TraceDebug(0L, "Null or empty message id in TryGetIdentiferFromInitiationMessageId. ignored");
				return false;
			}
			int num = (messageId[0] == '<') ? 1 : 0;
			int num2 = messageId.IndexOf(FindMessageUtils.MessageIdDomainPartDivider);
			if (num2 >= 0)
			{
				if (num + 56 > num2)
				{
					InitiationProcessor.diag.TraceDebug<string>(0L, "Id too short for duplicate detection: '{0}'.", messageId);
					return false;
				}
				identifier = messageId.Substring(num, 56);
				return true;
			}
			else
			{
				if (messageId.Length < 56 + num)
				{
					InitiationProcessor.diag.TraceDebug<string>(0L, "Id too short for duplicate detection - no @ case: '{0}'.", messageId);
					return false;
				}
				identifier = messageId.Substring(num, 56);
				return true;
			}
		}

		private void CopyPropertiesToInitiationMessageFromMessageItem()
		{
			this.initiationMessage.ApprovalData = this.initiationMessageItem.GetValueOrDefault<string>(MessageItemSchema.ApprovalApplicationData);
			this.initiationMessage.MessageItemLocale = this.initiationMessageItem.GetValueAsNullable<int>(MessageItemSchema.MessageLocaleId);
		}

		private void SendApprovalRequest(ApprovalRequestWriter writer, ICollection<RoutingAddress> p1DecisionMakers, CultureInfo cultureInfo, string messageId, byte[] corelationBlob)
		{
			IList<RoutingAddress> decisionMakers = this.initiationMessage.DecisionMakers;
			byte[] conversationIndex = this.initiationMessageItem.ConversationIndex;
			(string)this.initiationMessage.Requestor;
			using (MessageItemApprovalRequest messageItemApprovalRequest = this.requestCreate(this.mbxTransportMailItem))
			{
				messageItemApprovalRequest.SetSender(this.approvalRequestSender);
				foreach (RoutingAddress routingAddress in decisionMakers)
				{
					bool flag = p1DecisionMakers == null || p1DecisionMakers.Contains(routingAddress);
					messageItemApprovalRequest.AddRecipient(routingAddress, flag);
					InitiationProcessor.diag.TraceDebug<RoutingAddress, bool>(0L, "Added recipient '{0}' with promote to P1={1}.", routingAddress, flag);
				}
				CultureInfo cultureInfo2;
				if (!writer.WriteSubjectAndBody(messageItemApprovalRequest, cultureInfo, out cultureInfo2))
				{
					InitiationProcessor.diag.TraceError<ApprovalRequestWriter>(0L, "Approval request body cannot be written with writer {0}", writer);
				}
				messageItemApprovalRequest.ApprovalRequestor = this.initiationMessage.Requestor;
				messageItemApprovalRequest.MessageItem[ItemSchema.NormalizedSubject] = this.initiationMessageItem.ConversationTopic;
				messageItemApprovalRequest.MessageItem.Importance = this.initiationMessageItem.Importance;
				messageItemApprovalRequest.MessageItem.References = InitiationProcessor.ConstructApprovalRequestReferences(this.initiationMessageItem);
				ConversationIndex o = ConversationIndex.CreateFromParent(conversationIndex);
				if (!ConversationIndex.Empty.Equals(o))
				{
					messageItemApprovalRequest.MessageItem.ConversationIndex = o.ToByteArray();
				}
				if (cultureInfo2 != null)
				{
					InitiationProcessor.diag.TraceDebug<CultureInfo>(0L, "Approval request written in {0}", cultureInfo2);
					messageItemApprovalRequest.MessageItem[MessageItemSchema.MessageLocaleId] = cultureInfo2.LCID;
				}
				foreach (Attachment attachment in this.initiationMessage.EmailMessage.Attachments)
				{
					messageItemApprovalRequest.AddAttachment(attachment, this.mbxTransportMailItem.ADRecipientCache.ADSession);
				}
				if (this.initiationMessage.EmailMessage.RootPart != null && this.initiationMessage.EmailMessage.RootPart.Headers != null)
				{
					HeaderList headers = this.initiationMessage.EmailMessage.RootPart.Headers;
					Header[] array = headers.FindAll("X-MS-Exchange-Inbox-Rules-Loop");
					if (array != null && array.Length > 0)
					{
						string[] array2 = new string[array.Length];
						for (int i = 0; i < array.Length; i++)
						{
							array2[i] = array[i].Value;
						}
						messageItemApprovalRequest.MessageItem.SafeSetProperty(MessageItemSchema.XLoop, array2);
					}
				}
				InitiationProcessor.AddVotingToApprovalRequest(messageItemApprovalRequest, this.initiationMessage.VotingActions, cultureInfo2);
				messageItemApprovalRequest.Send(messageId, corelationBlob, this.mbxTransportMailItem);
			}
		}

		public const int InitiationMessageIdentifierLength = 56;

		private static readonly Trace diag = ExTraceGlobals.ApprovalAgentTracer;

		private static readonly PropertyDefinition[] PropertiesForUniquenessCheck = new PropertyDefinition[]
		{
			ItemSchema.InternetMessageId,
			MessageItemSchema.ApprovalRequestor,
			MessageItemSchema.ApprovalApplicationData,
			ItemSchema.Id
		};

		private static readonly PropertyDefinition[] PropertiesFromInitiationForApprovalRequestCreation = new PropertyDefinition[]
		{
			MessageItemSchema.MessageLocaleId,
			MessageItemSchema.ApprovalRequestor,
			MessageItemSchema.ApprovalApplicationData,
			MessageItemSchema.ApprovalApplicationId,
			MessageItemSchema.ApprovalRequestMessageId
		};

		private static readonly CultureInfo DefaultFallBackCulture;

		private MbxTransportMailItem mbxTransportMailItem;

		private InitiationMessage initiationMessage;

		private MessageItem initiationMessageItem;

		private ApprovalEngine.ApprovalRequestCreateDelegate requestCreate;

		private CultureInfo organizationFallbackCulture;

		private RoutingAddress approvalRequestSender;
	}
}
