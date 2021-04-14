using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class LoadedItemPart : ItemPart
	{
		private void InitializeLoadedItemPart(IItem item, IStorePropertyBag propertyBag, BodyFragmentInfo bodyFragmentInfo, bool didLoadSucceed, long bytesLoaded, AttachmentCollection attachmentCollection)
		{
			this.bodyFragmentInfo = bodyFragmentInfo;
			if (this.bodyFragmentInfo == null)
			{
				base.UniqueFragmentInfo = (base.DisclaimerFragmentInfo = FragmentInfo.Empty);
			}
			this.didLoadSucceed = didLoadSucceed;
			if (this.didLoadSucceed)
			{
				this.bytesLoaded = bytesLoaded;
			}
			base.ItemId = item.Id.ObjectId;
			base.Subject = (item.TryGetProperty(ItemSchema.Subject) as string);
			if (attachmentCollection != null)
			{
				foreach (AttachmentHandle handle in attachmentCollection)
				{
					using (Attachment attachment = attachmentCollection.Open(handle, null))
					{
						base.RawAttachments.Add(new AttachmentInfo(item.Id.ObjectId, attachment));
					}
				}
			}
			IMessageItem messageItem = item as IMessageItem;
			if (messageItem != null)
			{
				if (messageItem.Sender != null)
				{
					this.displayNameToParticipant[messageItem.Sender.DisplayName] = messageItem.Sender;
				}
				if (messageItem.From != null)
				{
					this.displayNameToParticipant[messageItem.From.DisplayName] = messageItem.From;
				}
				foreach (Recipient recipient in messageItem.Recipients)
				{
					recipient.Participant.Submitted = recipient.Submitted;
					this.displayNameToParticipant[recipient.Participant.DisplayName] = recipient.Participant;
					base.Recipients.Add(recipient.RecipientItemType, new IParticipant[]
					{
						recipient.Participant
					});
				}
				foreach (Participant participant in messageItem.ReplyTo)
				{
					this.displayNameToParticipant[participant.DisplayName] = participant;
				}
			}
			if (propertyBag != null)
			{
				base.StorePropertyBag = propertyBag;
			}
		}

		public override bool DidLoadSucceed
		{
			get
			{
				return this.didLoadSucceed;
			}
		}

		public override bool IsExtractedPart
		{
			get
			{
				return false;
			}
		}

		public override IList<IParticipant> ReplyToParticipants
		{
			get
			{
				if (this.replyToParticipants == null)
				{
					ReplyTo replyTo = ReplyTo.CreateInstance(base.StorePropertyBag);
					if (replyTo == null)
					{
						this.replyToParticipants = Array<IParticipant>.Empty;
					}
					else
					{
						this.replyToParticipants = replyTo.Cast<IParticipant>().ToList<IParticipant>();
					}
				}
				return this.replyToParticipants;
			}
		}

		public ParticipantTable ReplyAllParticipants
		{
			get
			{
				if (this.replyAllParticipants == null)
				{
					this.replyAllParticipants = this.CalculateReplyAllParticipants();
				}
				return this.replyAllParticipants;
			}
		}

		internal BodyFragmentInfo BodyFragmentInfo
		{
			get
			{
				return this.bodyFragmentInfo;
			}
		}

		internal long BytesLoaded
		{
			get
			{
				return this.bytesLoaded;
			}
		}

		internal Dictionary<string, Participant> DisplayNameToParticipant
		{
			get
			{
				return this.displayNameToParticipant;
			}
		}

		internal void AddBodySummary(object bodySummary)
		{
			QueryResultPropertyBag queryResultPropertyBag = new QueryResultPropertyBag(base.StorePropertyBag, new NativeStorePropertyDefinition[]
			{
				InternalSchema.TextBody,
				InternalSchema.Preview
			}, new object[]
			{
				bodySummary,
				bodySummary
			});
			base.StorePropertyBag = queryResultPropertyBag.AsIStorePropertyBag();
		}

		private ParticipantTable CalculateReplyAllParticipants()
		{
			IParticipant valueOrDefault = base.StorePropertyBag.GetValueOrDefault<IParticipant>(ItemSchema.From, null);
			IParticipant valueOrDefault2 = base.StorePropertyBag.GetValueOrDefault<IParticipant>(ItemSchema.Sender, null);
			IDictionary<RecipientItemType, HashSet<IParticipant>> dictionary = ReplyAllParticipantsRepresentationProperty<IParticipant>.BuildReplyAllRecipients<IParticipant>(valueOrDefault2, valueOrDefault, this.ReplyToParticipants, base.Recipients.ToDictionary(), ParticipantComparer.EmailAddress);
			ParticipantTable participantTable = new ParticipantTable();
			foreach (KeyValuePair<RecipientItemType, HashSet<IParticipant>> keyValuePair in dictionary)
			{
				participantTable.Add(keyValuePair.Key, keyValuePair.Value);
			}
			return participantTable;
		}

		internal LoadedItemPart(IItem item, IStorePropertyBag propertyBag, BodyFragmentInfo bodyFragmentInfo, PropertyDefinition[] loadedProperties, ItemPartIrmInfo irmInfo, bool didLoadSucceed, long bytesLoaded, AttachmentCollection attachmentCollection) : base(loadedProperties)
		{
			this.irmInfo = irmInfo;
			this.InitializeLoadedItemPart(item, propertyBag, bodyFragmentInfo, didLoadSucceed, bytesLoaded, attachmentCollection);
		}

		public override ItemPartIrmInfo IrmInfo
		{
			get
			{
				return this.irmInfo;
			}
		}

		private Dictionary<string, Participant> displayNameToParticipant = new Dictionary<string, Participant>();

		private BodyFragmentInfo bodyFragmentInfo;

		private bool didLoadSucceed;

		private ParticipantTable replyAllParticipants;

		private long bytesLoaded;

		private IList<IParticipant> replyToParticipants;

		private ItemPartIrmInfo irmInfo;
	}
}
