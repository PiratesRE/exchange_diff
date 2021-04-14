using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConversationAggregatorFactory
	{
		public ConversationAggregatorFactory(IMailboxSession session, IMailboxOwner mailboxOwner, IXSOFactory xsoFactory, ConversationIndexTrackingEx indexTrackingEx)
		{
			this.mailboxOwner = mailboxOwner;
			this.session = session;
			this.xsoFactory = xsoFactory;
			this.indexTrackingEx = indexTrackingEx;
		}

		public static bool TryInstantiateAggregatorForSave(IMailboxSession session, CoreItemOperation saveOperation, ICoreItem item, ConversationIndexTrackingEx indexTrackingEx, out IConversationAggregator aggregator)
		{
			MailboxOwnerFactory mailboxOwnerFactory = new MailboxOwnerFactory(session);
			ConversationAggregatorFactory conversationAggregatorFactory = new ConversationAggregatorFactory(session, mailboxOwnerFactory.Create(), XSOFactory.Default, indexTrackingEx);
			return conversationAggregatorFactory.TryInstantiateAggregatorForSave(saveOperation, item, out aggregator);
		}

		public static bool TryInstantiateAggregatorForDelivery(IMailboxSession session, MiniRecipient miniRecipient, ConversationIndexTrackingEx indexTrackingEx, out IConversationAggregator aggregator)
		{
			MailboxOwnerFactory mailboxOwnerFactory = new MailboxOwnerFactory(session);
			return ConversationAggregatorFactory.TryInstantiateAggregatorForDelivery(session, mailboxOwnerFactory.Create(miniRecipient), indexTrackingEx, out aggregator);
		}

		public static bool TryInstantiateAggregatorForDelivery(IMailboxSession session, IMailboxOwner mailboxOwner, ConversationIndexTrackingEx indexTrackingEx, out IConversationAggregator aggregator)
		{
			ConversationAggregatorFactory conversationAggregatorFactory = new ConversationAggregatorFactory(session, mailboxOwner, XSOFactory.Default, indexTrackingEx);
			return conversationAggregatorFactory.TryInstantiateAggregatorForDelivery(out aggregator);
		}

		public bool TryInstantiateAggregatorForSave(CoreItemOperation saveOperation, ICoreItem item, out IConversationAggregator aggregator)
		{
			ConversationAggregatorFactory.AggregationApproach approach = this.IdentifyAggregationApproachForSave(saveOperation, item);
			return this.TryInstantiateAggregator(approach, false, out aggregator);
		}

		public bool TryInstantiateAggregatorForDelivery(out IConversationAggregator aggregator)
		{
			ConversationAggregatorFactory.AggregationApproach approach = this.IdentifyAggregationApproachForDelivery();
			return this.TryInstantiateAggregator(approach, true, out aggregator);
		}

		private bool TryInstantiateAggregator(ConversationAggregatorFactory.AggregationApproach approach, bool scenarioSupportsSearchForDuplicatedMessages, out IConversationAggregator aggregator)
		{
			aggregator = null;
			if (approach == ConversationAggregatorFactory.AggregationApproach.NoOp)
			{
				return false;
			}
			IAggregationByItemClassReferencesSubjectProcessor aggregationByItemClassReferencesSubjectProcessor = AggregationByItemClassReferencesSubjectProcessor.CreateInstance(this.xsoFactory, this.session, this.mailboxOwner.RequestExtraPropertiesWhenSearching, this.indexTrackingEx);
			if (approach == ConversationAggregatorFactory.AggregationApproach.SameConversation)
			{
				aggregator = new SameConversationAggregator(aggregationByItemClassReferencesSubjectProcessor);
				return true;
			}
			ConversationAggregationDiagnosticsFrame diagnosticsFrame = new ConversationAggregationDiagnosticsFrame(this.session);
			AggregationByParticipantProcessor participantProcessor = AggregationByParticipantProcessor.CreateInstance(this.session, this.xsoFactory);
			switch (approach)
			{
			case ConversationAggregatorFactory.AggregationApproach.SideConversation:
				aggregator = new SideConversationAggregator(this.mailboxOwner, scenarioSupportsSearchForDuplicatedMessages, aggregationByItemClassReferencesSubjectProcessor, participantProcessor, diagnosticsFrame);
				return true;
			case ConversationAggregatorFactory.AggregationApproach.ThreadedConversation:
				aggregator = new ThreadedConversationAggregator(this.mailboxOwner, scenarioSupportsSearchForDuplicatedMessages, aggregationByItemClassReferencesSubjectProcessor, participantProcessor, diagnosticsFrame);
				return true;
			default:
				return false;
			}
		}

		private ConversationAggregatorFactory.AggregationApproach IdentifyAggregationApproachForSave(CoreItemOperation saveOperation, ICoreItem item)
		{
			if (this.session.LogonType == LogonType.Transport)
			{
				return ConversationAggregatorFactory.AggregationApproach.NoOp;
			}
			ICorePropertyBag propertyBag = item.PropertyBag;
			string valueOrDefault = propertyBag.GetValueOrDefault<string>(InternalSchema.ItemClass, string.Empty);
			if (string.IsNullOrEmpty(valueOrDefault) || ConversationIdFromIndexProperty.CheckExclusionList(valueOrDefault) || propertyBag.GetValueOrDefault<bool>(InternalSchema.IsAssociated))
			{
				return ConversationAggregatorFactory.AggregationApproach.NoOp;
			}
			if (this.mailboxOwner.ThreadedConversationProcessingEnabled || this.mailboxOwner.SideConversationProcessingEnabled)
			{
				if (saveOperation == CoreItemOperation.Send || (item.Origin == Origin.New && !ConversationIndex.WasMessageEverProcessed(propertyBag)))
				{
					if (!this.mailboxOwner.ThreadedConversationProcessingEnabled)
					{
						return ConversationAggregatorFactory.AggregationApproach.SideConversation;
					}
					return ConversationAggregatorFactory.AggregationApproach.ThreadedConversation;
				}
				else
				{
					if (this.mailboxOwner.IsGroupMailbox && !propertyBag.GetValueOrDefault<bool>(InternalSchema.IsDraft))
					{
						return ConversationAggregatorFactory.AggregationApproach.SideConversation;
					}
					return ConversationAggregatorFactory.AggregationApproach.NoOp;
				}
			}
			else
			{
				if (item.Origin == Origin.New && !ConversationIndex.WasMessageEverProcessed(propertyBag))
				{
					return ConversationAggregatorFactory.AggregationApproach.SameConversation;
				}
				return ConversationAggregatorFactory.AggregationApproach.NoOp;
			}
		}

		private ConversationAggregatorFactory.AggregationApproach IdentifyAggregationApproachForDelivery()
		{
			if (this.mailboxOwner.ThreadedConversationProcessingEnabled)
			{
				return ConversationAggregatorFactory.AggregationApproach.ThreadedConversation;
			}
			if (this.mailboxOwner.SideConversationProcessingEnabled)
			{
				return ConversationAggregatorFactory.AggregationApproach.SideConversation;
			}
			return ConversationAggregatorFactory.AggregationApproach.SameConversation;
		}

		private readonly IMailboxOwner mailboxOwner;

		private readonly IMailboxSession session;

		private readonly IXSOFactory xsoFactory;

		private readonly ConversationIndexTrackingEx indexTrackingEx;

		private enum AggregationApproach
		{
			NoOp,
			SameConversation,
			SideConversation,
			ThreadedConversation
		}
	}
}
