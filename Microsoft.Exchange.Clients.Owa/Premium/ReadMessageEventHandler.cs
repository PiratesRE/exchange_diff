using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventNamespace("ReadMessage")]
	internal sealed class ReadMessageEventHandler : MessageEventHandler
	{
		public static void Register()
		{
			OwaEventRegistry.RegisterHandler(typeof(ReadMessageEventHandler));
		}

		[OwaEventParameter("Subj", typeof(string), false, true)]
		[OwaEventParameter("CK", typeof(string), false, true)]
		[OwaEventParameter("Id", typeof(OwaStoreObjectId), false, true)]
		[OwaEventParameter("AudioNotes", typeof(string), false, true)]
		[OwaEventParameter("AlWbBcn", typeof(bool), false, true)]
		[OwaEventParameter("StLnkEnbl", typeof(bool), false, true)]
		[OwaEvent("Save")]
		public void Save()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "ReadMessageEventHandler.SaveMessage");
			OwaStoreObjectId owaStoreObjectId = (OwaStoreObjectId)base.GetParameter("Id");
			string changeKey = (string)base.GetParameter("CK");
			using (MessageItem item = Utilities.GetItem<MessageItem>(base.UserContext, owaStoreObjectId, changeKey, true, new PropertyDefinition[0]))
			{
				ExTraceGlobals.MailTracer.TraceDebug((long)this.GetHashCode(), "Saving message");
				base.UpdateReadMessage(item);
				Utilities.SaveItem(item, true);
				item.Load();
				base.WriteChangeKey(item);
			}
		}

		[OwaEventVerb(OwaEventVerb.Get)]
		[OwaEvent("LMD")]
		[OwaEventParameter("Id", typeof(string))]
		public void LoadMessageDetails()
		{
			string str = (string)base.GetParameter("Id");
			string path = "forms/premium/messagedetailsdialog.aspx?" + str;
			this.HttpContext.Server.Execute(path, this.Writer);
		}

		[OwaEvent("SRR")]
		[OwaEventParameter("Id", typeof(OwaStoreObjectId))]
		public void SendReadReceipt()
		{
			OwaStoreObjectId owaStoreObjectId = (OwaStoreObjectId)base.GetParameter("Id");
			using (MessageItem item = Utilities.GetItem<MessageItem>(base.UserContext, owaStoreObjectId, true, new PropertyDefinition[0]))
			{
				item.SendReadReceipt();
			}
			this.Writer.Write(LocalizedStrings.GetHtmlEncoded(641302712));
		}

		[OwaEvent("RMR")]
		[OwaEventParameter("Id", typeof(OwaStoreObjectId))]
		public void RemoveRestriction()
		{
			OwaStoreObjectId owaStoreObjectId = (OwaStoreObjectId)base.GetParameter("Id");
			using (MessageItem item = Utilities.GetItem<MessageItem>(base.UserContext, owaStoreObjectId, false, new PropertyDefinition[0]))
			{
				Utilities.IrmRemoveRestriction(item, base.UserContext);
			}
		}

		[OwaEventParameter("Vt", typeof(string))]
		[OwaEventParameter("Id", typeof(OwaStoreObjectId))]
		[OwaEventParameter("CK", typeof(string))]
		[OwaEventParameter("fId", typeof(OwaStoreObjectId))]
		[OwaEvent("ApvEd")]
		public void ApprovalEditResponse()
		{
			this.ProcessApprovalResponse(true);
		}

		[OwaEvent("ApvSnd")]
		[OwaEventParameter("Id", typeof(OwaStoreObjectId))]
		[OwaEventParameter("CK", typeof(string))]
		[OwaEventParameter("fId", typeof(OwaStoreObjectId))]
		[OwaEventParameter("Vt", typeof(string))]
		public void ApprovalSendResponseNow()
		{
			this.ProcessApprovalResponse(false);
		}

		private void ProcessApprovalResponse(bool isEdit)
		{
			MessageItem requestItem;
			MessageItem messageItem = requestItem = base.GetRequestItem<MessageItem>(ReadMessageEventHandler.ApprovalPrefetchProperties);
			try
			{
				if (!Utilities.IsValidUndecidedApprovalRequest(messageItem))
				{
					throw new OwaInvalidRequestException("The approval request was invalid or was already decided");
				}
				string[] array = (string[])messageItem.VotingInfo.GetOptionsList();
				string text = (string)base.GetParameter("Vt");
				if (string.IsNullOrEmpty(text))
				{
					throw new OwaInvalidRequestException("The approval vote was not supplied.");
				}
				if (array == null || Array.IndexOf<string>(array, text) == -1)
				{
					throw new OwaInvalidRequestException("The attempted approval vote was invalid for the approval request.");
				}
				BodyFormat replyForwardBodyFormat = ReplyForwardUtilities.GetReplyForwardBodyFormat(messageItem, base.UserContext);
				OwaStoreObjectId owaStoreObjectId = (OwaStoreObjectId)base.GetParameter("fId");
				MessageItem messageItem3;
				MessageItem messageItem2 = messageItem3 = messageItem.CreateVotingResponse(string.Empty, replyForwardBodyFormat, base.UserContext.TryGetMyDefaultFolderId(DefaultFolderType.Drafts), text);
				try
				{
					messageItem2.Save(SaveMode.ResolveConflicts);
					messageItem2.Load();
					if (!isEdit)
					{
						messageItem2.Send();
					}
					else
					{
						base.WriteNewItemId(messageItem2);
					}
				}
				finally
				{
					if (messageItem3 != null)
					{
						((IDisposable)messageItem3).Dispose();
					}
				}
			}
			finally
			{
				if (requestItem != null)
				{
					((IDisposable)requestItem).Dispose();
				}
			}
		}

		public const string EventNamespace = "ReadMessage";

		public const string MethodLoadMessageDetails = "LMD";

		public const string MethodSendReadReceipt = "SRR";

		public const string MethodApprovalEditResponse = "ApvEd";

		public const string MethodApprovalSendResponseNow = "ApvSnd";

		public const string Vote = "Vt";

		public const string MethodRemoveRestriction = "RMR";

		private static readonly StorePropertyDefinition[] ApprovalPrefetchProperties = new StorePropertyDefinition[]
		{
			MessageItemSchema.ApprovalDecisionTime,
			MessageItemSchema.ApprovalDecision,
			MessageItemSchema.ApprovalDecisionMaker
		};
	}
}
