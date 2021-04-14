using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.ServiceCommands
{
	internal class GetFlowConversation : ServiceCommand<GetFlowConversationResponse>
	{
		public GetFlowConversation(CallContext callContext, BaseFolderId folderId, int requestedConversationCount) : base(callContext)
		{
			this.folderId = folderId;
			this.requestedConversationCount = Math.Min((long)requestedConversationCount, (long)((ulong)GetFlowConversation.MAX_ITEMS));
			this.replyAllExtractor = new ReplyAllExtractor(base.MailboxIdentityMailboxSession, XSOFactory.Default);
		}

		protected override GetFlowConversationResponse InternalExecute()
		{
			QueryFilter flowConversationFilter = GetFlowConversation.GetFlowConversationFilter(this.folderId, base.MailboxIdentityMailboxSession);
			IdAndSession folderIdAndSession = GetFlowConversation.GetFolderIdAndSession(this.folderId, base.MailboxIdentityMailboxSession, base.IdConverter);
			List<FlowConversationItem> list = new List<FlowConversationItem>();
			new ConversationFactory(base.MailboxIdentityMailboxSession);
			using (Folder folder = Folder.Bind((MailboxSession)folderIdAndSession.Session, folderIdAndSession.Id))
			{
				using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, flowConversationFilter, GetFlowConversation.SortByArray, GetFlowConversation.ItemQueryRequiredProperties))
				{
					IStorePropertyBag[] propertyBags = queryResult.GetPropertyBags(10000);
					foreach (IStorePropertyBag storePropertyBag in propertyBags)
					{
						if ((long)list.Count == this.requestedConversationCount)
						{
							break;
						}
						ParticipantSet participantSet;
						string text = this.GenerateParticipantsHash(storePropertyBag, out participantSet);
						if (!this.allItemsData.ContainsKey(text))
						{
							FlowConversationItem flowConversationItem = new FlowConversationItem();
							ExDateTime valueOrDefault = storePropertyBag.GetValueOrDefault<ExDateTime>(ItemSchema.ReceivedTime, ExDateTime.Now);
							string valueOrDefault2 = storePropertyBag.GetValueOrDefault<string>(ItemSchema.Preview, null);
							string valueOrDefault3 = storePropertyBag.GetValueOrDefault<string>(ItemSchema.Subject, null);
							bool valueOrDefault4 = storePropertyBag.GetValueOrDefault<bool>(MessageItemSchema.IsRead, true);
							IParticipant valueOrDefault5 = storePropertyBag.GetValueOrDefault<IParticipant>(ItemSchema.From, null);
							VersionedId valueOrDefault6 = storePropertyBag.GetValueOrDefault<VersionedId>(ItemSchema.Id, null);
							storePropertyBag.GetValueOrDefault<ConversationId>(ItemSchema.ConversationId, null);
							List<IParticipant> list2 = new List<IParticipant>(participantSet.Count + 1);
							participantSet.ExceptWith(new IParticipant[]
							{
								valueOrDefault5
							});
							list2.Add(valueOrDefault5);
							list2.AddRange(participantSet);
							flowConversationItem.FlowConversationId = text;
							flowConversationItem.LastItemId = IdConverter.ConvertStoreItemIdToItemId(valueOrDefault6, folderIdAndSession.Session);
							flowConversationItem.Participants = ReplyToProperty.Render(list2).ToArray();
							flowConversationItem.SenderPhotoEmailAddress = valueOrDefault5.SmtpEmailAddress;
							flowConversationItem.Preview = valueOrDefault2;
							flowConversationItem.Subject = valueOrDefault3;
							flowConversationItem.TotalCount = 1;
							flowConversationItem.UnReadCount = (valueOrDefault4 ? 0 : 1);
							flowConversationItem.ReceivedTimeUtc = ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime(valueOrDefault);
							this.allItemsData.Add(text, flowConversationItem);
							list.Add(flowConversationItem);
						}
						else
						{
							if (!storePropertyBag.GetValueOrDefault<bool>(MessageItemSchema.IsRead, true))
							{
								this.allItemsData[text].UnReadCount++;
							}
							this.allItemsData[text].TotalCount++;
						}
					}
				}
			}
			return new GetFlowConversationResponse
			{
				Conversations = list.ToArray()
			};
		}

		private string GenerateParticipantsHash(IStorePropertyBag propertyBag, out ParticipantSet participants)
		{
			participants = this.replyAllExtractor.RetrieveReplyAllParticipants(propertyBag);
			List<string> list = new List<string>(participants.Count);
			foreach (IParticipant participant in participants)
			{
				list.Add(participant.DisplayName.ToLower());
			}
			StringBuilder stringBuilder = new StringBuilder(list.Count * 255);
			list.Sort(StringComparer.InvariantCultureIgnoreCase);
			foreach (string value in list)
			{
				stringBuilder.Append(value);
				stringBuilder.Append(";");
			}
			string result;
			using (SHA256CryptoServiceProvider sha256CryptoServiceProvider = new SHA256CryptoServiceProvider())
			{
				result = Convert.ToBase64String(sha256CryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(stringBuilder.ToString())));
			}
			return result;
		}

		internal static IdAndSession GetFolderIdAndSession(BaseFolderId folderId, MailboxSession mailboxSession, IdConverter converter)
		{
			if (folderId != null)
			{
				return converter.ConvertFolderIdToIdAndSession(folderId, IdConverter.ConvertOption.IgnoreChangeKey);
			}
			return new IdAndSession(mailboxSession.GetDefaultFolderId(DefaultFolderType.AllItems), mailboxSession);
		}

		internal static QueryFilter GetFlowConversationFilter(BaseFolderId folderId, MailboxSession mailboxSession)
		{
			QueryFilter queryFilter3;
			if (folderId == null)
			{
				QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ParentItemId, mailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox));
				QueryFilter queryFilter2 = new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ParentItemId, mailboxSession.GetDefaultFolderId(DefaultFolderType.SentItems));
				queryFilter3 = new OrFilter(new QueryFilter[]
				{
					queryFilter,
					queryFilter2
				});
			}
			else
			{
				queryFilter3 = new TrueFilter();
			}
			return new AndFilter(new QueryFilter[]
			{
				queryFilter3,
				GetFlowConversation.ItemClassFilter
			});
		}

		private static readonly PropertyDefinition[] ItemQueryRequiredProperties = new PropertyDefinition[]
		{
			ItemSchema.Id,
			StoreObjectSchema.ItemClass,
			ItemSchema.From,
			ItemSchema.DisplayTo,
			ItemSchema.DisplayCc,
			ItemSchema.Preview,
			ItemSchema.ReceivedTime,
			ItemSchema.InternetMessageId,
			MessageItemSchema.IsRead,
			ItemSchema.Subject,
			ItemSchema.ConversationId,
			StoreObjectSchema.ParentItemId
		};

		private static readonly PropertyDefinition[] ConversationCreatorRelevantProperties = new PropertyDefinition[]
		{
			ItemSchema.Id
		};

		private static readonly QueryFilter ItemClassFilter = new TextFilter(StoreObjectSchema.ItemClass, "IPM.Note", MatchOptions.ExactPhrase, MatchFlags.IgnoreCase);

		private static readonly SortBy[] SortByArray = new SortBy[]
		{
			new SortBy(ItemSchema.ReceivedTime, SortOrder.Descending)
		};

		private static readonly uint MAX_ITEMS = 100U;

		private readonly ReplyAllExtractor replyAllExtractor;

		private readonly BaseFolderId folderId;

		private readonly Dictionary<string, FlowConversationItem> allItemsData = new Dictionary<string, FlowConversationItem>();

		private readonly long requestedConversationCount;
	}
}
