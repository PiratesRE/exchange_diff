using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ItemPart
	{
		protected ItemPart(IEnumerable<PropertyDefinition> loadedProperties)
		{
			this.rawAttachments = new List<AttachmentInfo>();
			this.replyToParticipants = new List<IParticipant>();
			this.attachments = new ReadOnlyCollection<AttachmentInfo>(this.rawAttachments);
			this.recipients = new ParticipantTable();
			this.loadedProperties = new HashSet<PropertyDefinition>(loadedProperties);
		}

		internal ItemPart(StoreObjectId itemId, string subject, FragmentInfo uniqueBodyFragment, FragmentInfo disclaimerFragment, ParticipantTable recipients, IList<Participant> replyToParticipants, IStorePropertyBag storePropertyBag, PropertyDefinition[] loadedProperties) : this(loadedProperties)
		{
			this.itemId = itemId;
			this.subject = subject;
			this.storePropertyBag = storePropertyBag;
			this.uniqueBodyFragment = uniqueBodyFragment;
			this.disclaimerFragment = disclaimerFragment;
			this.recipients = recipients;
			this.replyToParticipants.AddRange(replyToParticipants);
		}

		public virtual bool DidLoadSucceed
		{
			get
			{
				return true;
			}
		}

		public StoreObjectId ObjectId
		{
			get
			{
				return ((VersionedId)this.StorePropertyBag.TryGetProperty(ItemSchema.Id)).ObjectId;
			}
		}

		public string Subject
		{
			get
			{
				return this.subject;
			}
			protected set
			{
				if (this.subject != null)
				{
					throw new InvalidOperationException();
				}
				this.subject = value;
			}
		}

		public string BodyPart
		{
			get
			{
				if (this.bodyPart == null && this.uniqueBodyFragment != null)
				{
					using (StringWriter stringWriter = new StringWriter())
					{
						using (HtmlWriter htmlWriter = new HtmlWriter(stringWriter))
						{
							this.uniqueBodyFragment.WriteHtml(htmlWriter);
						}
						this.bodyPart = stringWriter.ToString();
					}
				}
				return this.bodyPart;
			}
		}

		public StoreObjectId ItemId
		{
			get
			{
				return this.itemId;
			}
			protected set
			{
				if (this.itemId != null)
				{
					throw new InvalidOperationException();
				}
				this.itemId = value;
			}
		}

		public virtual IList<IParticipant> ReplyToParticipants
		{
			get
			{
				return this.replyToParticipants;
			}
		}

		public ParticipantTable Recipients
		{
			get
			{
				return this.recipients;
			}
		}

		public IList<AttachmentInfo> Attachments
		{
			get
			{
				return this.attachments;
			}
		}

		public IStorePropertyBag StorePropertyBag
		{
			get
			{
				return this.storePropertyBag;
			}
			protected set
			{
				if (this.StorePropertyBag != null && value == null)
				{
					throw new InvalidOperationException();
				}
				this.storePropertyBag = value;
			}
		}

		public bool MayHaveHiddenDisclaimer
		{
			get
			{
				return this.DisclaimerFragmentInfo != null && !this.DisclaimerFragmentInfo.IsEmpty;
			}
		}

		public virtual bool IsExtractedPart
		{
			get
			{
				return true;
			}
		}

		public ParticipantSet AllRecipients
		{
			get
			{
				if (this.allRecipients == null)
				{
					this.allRecipients = this.CalculateAllRecipients();
				}
				return this.allRecipients;
			}
		}

		private ParticipantSet CalculateAllRecipients()
		{
			ParticipantSet participantSet = new ParticipantSet();
			foreach (PropertyDefinition propertyDefinition in MessageItemSchema.SingleRecipientProperties)
			{
				if (this.loadedProperties.Contains(propertyDefinition))
				{
					Participant valueOrDefault = this.StorePropertyBag.GetValueOrDefault<Participant>(propertyDefinition, null);
					if (valueOrDefault != null)
					{
						participantSet.Add(valueOrDefault);
					}
				}
			}
			participantSet.UnionWith(this.Recipients.ToList());
			participantSet.UnionWith(this.ReplyToParticipants);
			return participantSet;
		}

		internal FragmentInfo DisclaimerFragmentInfo
		{
			get
			{
				return this.disclaimerFragment;
			}
			set
			{
				if (this.disclaimerFragment != null && this.disclaimerFragment != FragmentInfo.Empty)
				{
					throw new InvalidOperationException("can't set DisclaimerFragmentInfo if it's already set");
				}
				this.disclaimerFragment = value;
			}
		}

		internal FragmentInfo UniqueFragmentInfo
		{
			get
			{
				return this.uniqueBodyFragment;
			}
			set
			{
				if (this.uniqueBodyFragment != null && this.uniqueBodyFragment != FragmentInfo.Empty)
				{
					throw new InvalidOperationException("can't set UniqeBodyFragment if it's already set");
				}
				this.uniqueBodyFragment = value;
			}
		}

		public void WriteDisclaimer(HtmlWriter writer)
		{
			this.DisclaimerFragmentInfo.WriteHtml(writer);
		}

		public void WriteUniquePart(HtmlWriter writer)
		{
			this.UniqueFragmentInfo.WriteHtml(writer);
		}

		public void WriteUniquePartWithoutQuotedText(HtmlWriter writer)
		{
			this.UniqueFragmentInfo.FragmentWithoutQuotedText.WriteHtml(writer);
		}

		public void WriteUniquePartQuotedText(HtmlWriter writer)
		{
			this.UniqueFragmentInfo.QuotedTextFragment.WriteHtml(writer);
		}

		protected List<AttachmentInfo> RawAttachments
		{
			get
			{
				return this.rawAttachments;
			}
		}

		public virtual ItemPartIrmInfo IrmInfo
		{
			get
			{
				return ItemPartIrmInfo.NotRestricted;
			}
		}

		private StoreObjectId itemId;

		private string subject;

		private string bodyPart;

		private List<IParticipant> replyToParticipants;

		private FragmentInfo uniqueBodyFragment;

		private FragmentInfo disclaimerFragment;

		private List<AttachmentInfo> rawAttachments;

		private ReadOnlyCollection<AttachmentInfo> attachments;

		private ParticipantTable recipients;

		private IStorePropertyBag storePropertyBag;

		private HashSet<PropertyDefinition> loadedProperties;

		private ParticipantSet allRecipients;
	}
}
