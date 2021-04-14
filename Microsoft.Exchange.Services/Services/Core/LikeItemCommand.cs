using System;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActivityLog;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class LikeItemCommand : ServiceCommand<LikeItemResponse>
	{
		public LikeItemCommand(CallContext callContext, LikeItemRequest request) : base(callContext)
		{
			WcfServiceCommandBase.ThrowIfNull(request, "request", "LikeItemCommand::ctor");
			this.request = request;
			this.callContext = callContext;
		}

		protected override LikeItemResponse InternalExecute()
		{
			this.request.Validate();
			IMailboxInfo mailboxInfo = this.callContext.AccessingPrincipal.MailboxInfo;
			Participant likingParticipant = new Participant(mailboxInfo.DisplayName, mailboxInfo.PrimarySmtpAddress.ToString(), "SMTP");
			using (MailboxSession mailboxSession = MailboxSession.OpenAsAdmin(this.callContext.MailboxIdentityPrincipal, this.callContext.ClientCulture, StoreSessionCacheBase.BuildMapiApplicationId(this.callContext, null)))
			{
				using (MessageItem messageItem = MessageItem.Bind(mailboxSession, IdConverter.EwsIdToMessageStoreObjectId(this.request.ItemId.Id), Likers.RequiredProperties))
				{
					Participant participant = messageItem.Likers.FirstOrDefault((Participant liker) => liker.EmailAddress == likingParticipant.EmailAddress);
					bool flag = participant != null;
					if (this.request.IsUnlike && flag)
					{
						messageItem.OpenAsReadWrite();
						messageItem.Likers.Remove(participant);
						messageItem.Save(SaveMode.ResolveConflicts);
					}
					else
					{
						if (this.request.IsUnlike || flag)
						{
							throw new ServiceInvalidOperationException((CoreResources.IDs)4151155219U);
						}
						messageItem.OpenAsReadWrite();
						messageItem.Likers.Add(likingParticipant);
						messageItem.Save(SaveMode.ResolveConflicts);
					}
					if (mailboxSession.ActivitySession != null)
					{
						mailboxSession.ActivitySession.CaptureActivity(this.request.IsUnlike ? ActivityId.Unlike : ActivityId.Like, messageItem.StoreObjectId, null, base.CallContext.GetAccessingInformation());
					}
				}
			}
			return new LikeItemResponse();
		}

		private static readonly PropertyDefinition[] PropertiesToFetch = new PropertyDefinition[]
		{
			MessageItemSchema.LikeCount,
			MessageItemSchema.LikersBlob
		};

		private readonly LikeItemRequest request;

		private readonly CallContext callContext;
	}
}
