using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Core.Types.Conversations;

namespace Microsoft.Exchange.Services.Core.Conversations.ResponseBuilders
{
	internal class ThreadedConversationResponseBuilder : ConversationDataResponseBuilderBase<IThreadedConversation, IConversationThread, ThreadedConversationResponseType, ConversationThreadType>
	{
		public ThreadedConversationResponseBuilder(IMailboxSession mailboxSession, IThreadedConversation conversation, IModernConversationNodeFactory conversationNodeFactory, IParticipantResolver resolver, ConversationNodeLoadingList loadingList, ConversationRequestArguments requestArguments) : base(mailboxSession, conversation, requestArguments, loadingList, conversationNodeFactory, resolver)
		{
		}

		protected override IEnumerable<Tuple<IConversationThread, ConversationThreadType>> XsoAndEwsConversationNodes
		{
			get
			{
				List<IConversationThread> threads = base.Conversation.Threads.ToList<IConversationThread>();
				for (int i = 0; i < threads.Count; i++)
				{
					yield return new Tuple<IConversationThread, ConversationThreadType>(threads[i], base.Response.ConversationThreads[i]);
				}
				yield break;
			}
		}

		protected override ThreadedConversationResponseType BuildSkeleton()
		{
			ThreadedConversationResponseType threadedConversationResponseType = new ThreadedConversationResponseType();
			int num = base.Conversation.Threads.Count<IConversationThread>();
			ConversationThreadType[] array = new ConversationThreadType[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = new ConversationThreadType();
			}
			threadedConversationResponseType.ConversationThreads = array;
			return threadedConversationResponseType;
		}

		private BaseItemId[] GetItemIds(StoreObjectId[] itemIds)
		{
			return ConversationDataConverter.GetItemIds(base.MailboxSession, itemIds);
		}

		protected override void BuildNodes()
		{
			base.BuildNodes();
			base.PopulateResponseWith(delegate(IConversationThread c, ConversationThreadType r)
			{
				r.ThreadId = this.GetConversationId(c.ThreadId);
			});
			base.PopulateResponseWith(delegate(IConversationThread c, ConversationThreadType r)
			{
				r.Preview = c.Preview;
			});
			base.PopulateResponseWith(delegate(IConversationThread c, ConversationThreadType r)
			{
				r.UniqueSenders = base.ParticipantResolver.ResolveToEmailAddressWrapper(c.UniqueSenders);
			});
			base.PopulateResponseWith(delegate(IConversationThread c, ConversationThreadType r)
			{
				r.LastDeliveryTime = ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime(c.LastDeliveryTime.GetValueOrDefault());
			});
			base.PopulateResponseWith(delegate(IConversationThread c, ConversationThreadType r)
			{
				r.GlobalHasAttachments = c.HasAttachments;
			});
			base.PopulateResponseWith(delegate(IConversationThread c, ConversationThreadType r)
			{
				r.GlobalHasIrm = c.HasIrm;
			});
			base.PopulateResponseWith(delegate(IConversationThread c, ConversationThreadType r)
			{
				r.GlobalImportance = (ImportanceType)c.Importance;
			});
			base.PopulateResponseWith(delegate(IConversationThread c, ConversationThreadType r)
			{
				r.GlobalIconIndex = (IconIndexType)c.IconIndex;
			});
			base.PopulateResponseWith(delegate(IConversationThread c, ConversationThreadType r)
			{
				r.GlobalFlagStatus = (FlagStatusType)c.FlagStatus;
			});
			base.PopulateResponseWith(delegate(IConversationThread c, ConversationThreadType r)
			{
				r.GlobalMessageCount = c.ItemCount;
			});
			base.PopulateResponseWith(delegate(IConversationThread c, ConversationThreadType r)
			{
				r.UnreadCount = c.UnreadCount;
			});
			base.PopulateResponseWith(delegate(IConversationThread c, ConversationThreadType r)
			{
				r.InitialMessage = base.ConversationNodeFactory.CreateInstance(c.FirstNode);
			});
			base.PopulateResponseWith(delegate(IConversationThread c, ConversationThreadType r)
			{
				r.GlobalItemIds = this.GetItemIds(c.ItemIds);
			});
			base.PopulateResponseWith(delegate(IConversationThread c, ConversationThreadType r)
			{
				r.GlobalRichContent = c.RichContent;
			});
			base.PopulateResponseWith(delegate(IConversationThread c, ConversationThreadType r)
			{
				r.GlobalItemClasses = c.ItemClasses;
			});
			base.PopulateResponseWith(delegate(IConversationThread c, ConversationThreadType r)
			{
				r.DraftItemIds = this.GetItemIds(c.DraftItemIds);
			});
			base.Response.TotalThreadCount = base.Response.ConversationThreads.Length;
		}
	}
}
