using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal class PreviewItem : Dictionary<PropertyDefinition, object>, IComparable
	{
		public PreviewItem(Dictionary<PropertyDefinition, object> properties, Guid mailboxGuid, Uri owaLink, ReferenceItem sortValue, UniqueItemHash itemHash) : this(properties, mailboxGuid, owaLink, sortValue, itemHash, null)
		{
		}

		public PreviewItem(Dictionary<PropertyDefinition, object> properties, Guid mailboxGuid, Uri owaLink, ReferenceItem sortValue, UniqueItemHash itemHash, List<PropertyDefinition> additionalProperties) : base(properties)
		{
			Util.ThrowOnNull(sortValue, "sortValue");
			if (mailboxGuid == Guid.Empty)
			{
				throw new ArgumentException("Invalid mailbox guid");
			}
			this.owaLink = owaLink;
			this.sortValue = sortValue;
			this.mailboxGuid = mailboxGuid;
			this.itemHash = itemHash;
			this.additionalPropertyValues = Util.GetSelectedAdditionalPropertyValues(properties, additionalProperties);
		}

		public StoreId Id
		{
			get
			{
				return this.GetProperty<StoreId>(ItemSchema.Id, null);
			}
		}

		public StoreId ParentItemId
		{
			get
			{
				return this.GetProperty<StoreId>(StoreObjectSchema.ParentItemId, null);
			}
		}

		public string ItemClass
		{
			get
			{
				return this.GetProperty<string>(StoreObjectSchema.ItemClass, string.Empty);
			}
		}

		public UniqueItemHash ItemHash
		{
			get
			{
				return this.itemHash;
			}
		}

		public string InternetMessageId
		{
			get
			{
				return this.GetProperty<string>(ItemSchema.InternetMessageId, string.Empty);
			}
		}

		public Guid MailboxGuid
		{
			get
			{
				return this.mailboxGuid;
			}
		}

		public Uri OwaLink
		{
			get
			{
				return this.owaLink;
			}
		}

		public string Sender
		{
			get
			{
				return this.GetProperty<string>(MessageItemSchema.SenderDisplayName, string.Empty);
			}
		}

		public string[] ToRecipients
		{
			get
			{
				return this.GetStringArrayProperty(ItemSchema.DisplayTo);
			}
		}

		public string[] CcRecipients
		{
			get
			{
				return this.GetStringArrayProperty(ItemSchema.DisplayCc);
			}
		}

		public string[] BccRecipients
		{
			get
			{
				return this.GetStringArrayProperty(ItemSchema.DisplayBcc);
			}
		}

		public ExDateTime CreationTime
		{
			get
			{
				return this.GetProperty<ExDateTime>(StoreObjectSchema.CreationTime, ExDateTime.MinValue);
			}
		}

		public ExDateTime ReceivedTime
		{
			get
			{
				return this.GetProperty<ExDateTime>(ItemSchema.ReceivedTime, ExDateTime.MinValue);
			}
		}

		public ExDateTime SentTime
		{
			get
			{
				return this.GetProperty<ExDateTime>(ItemSchema.SentTime, ExDateTime.MinValue);
			}
		}

		public string Subject
		{
			get
			{
				return this.GetProperty<string>(ItemSchema.Subject, null);
			}
		}

		public int Size
		{
			get
			{
				return this.GetProperty<int>(ItemSchema.Size, 0);
			}
		}

		public string Preview
		{
			get
			{
				return string.Empty;
			}
		}

		public string Importance
		{
			get
			{
				return this.GetProperty<string>(ItemSchema.Importance, string.Empty);
			}
		}

		public bool Read
		{
			get
			{
				return this.GetProperty<bool>(MessageItemSchema.IsRead, false);
			}
		}

		public bool HasAttachment
		{
			get
			{
				return this.GetProperty<bool>(ItemSchema.HasAttachment, false);
			}
		}

		public ReferenceItem SortValue
		{
			get
			{
				return this.sortValue;
			}
		}

		public Dictionary<PropertyDefinition, object> AdditionalPropertyValues
		{
			get
			{
				return this.additionalPropertyValues;
			}
		}

		public MailboxInfo MailboxInfo { get; set; }

		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			PreviewItem previewItem = obj as PreviewItem;
			if (previewItem != null)
			{
				return this.SortValue.CompareTo(previewItem.SortValue);
			}
			throw new ArgumentException("Object is not a PreviewItem");
		}

		private static string[] SplitRecipients(string recipient)
		{
			return (from addr in recipient.Split(PreviewItem.RecipientSeparators, StringSplitOptions.RemoveEmptyEntries)
			where !string.IsNullOrWhiteSpace(addr)
			select addr).ToArray<string>();
		}

		private T GetProperty<T>(StorePropertyDefinition propertyDef, T defaultValue)
		{
			if (base.ContainsKey(propertyDef) && base[propertyDef] is T)
			{
				return (T)((object)base[propertyDef]);
			}
			return defaultValue;
		}

		private string[] GetStringArrayProperty(StorePropertyDefinition propertyDef)
		{
			string property = this.GetProperty<string>(propertyDef, string.Empty);
			if (!string.IsNullOrEmpty(property))
			{
				return PreviewItem.SplitRecipients(property);
			}
			return null;
		}

		private readonly Uri owaLink;

		private readonly ReferenceItem sortValue;

		private readonly Guid mailboxGuid;

		private readonly UniqueItemHash itemHash;

		private readonly Dictionary<PropertyDefinition, object> additionalPropertyValues;

		private static readonly string[] RecipientSeparators = new string[]
		{
			";",
			"; "
		};
	}
}
