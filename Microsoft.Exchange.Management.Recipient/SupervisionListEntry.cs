using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Serializable]
	public sealed class SupervisionListEntry : ConfigurableObject
	{
		public SupervisionListEntry(string entryName, string tag, SupervisionRecipientType recipientType) : base(new SupervisionListEntryPropertyBag())
		{
			if (entryName == null)
			{
				throw new ArgumentNullException("entryName");
			}
			if (tag == null)
			{
				throw new ArgumentNullException("tag");
			}
			this.EntryName = entryName;
			this.Tag = tag;
			this.RecipientType = recipientType;
			this.Identity = new SupervisionListEntryId(this);
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return SupervisionListEntry.schema;
			}
		}

		private new bool IsValid
		{
			get
			{
				return true;
			}
		}

		public new ObjectId Identity
		{
			get
			{
				return (ObjectId)this.propertyBag[SupervisionListEntrySchema.Identity];
			}
			internal set
			{
				this.propertyBag[SupervisionListEntrySchema.Identity] = value;
			}
		}

		public string EntryName
		{
			get
			{
				return (string)this.propertyBag[SupervisionListEntrySchema.EntryName];
			}
			internal set
			{
				this.propertyBag[SupervisionListEntrySchema.EntryName] = value;
			}
		}

		public string Tag
		{
			get
			{
				return (string)this.propertyBag[SupervisionListEntrySchema.Tag];
			}
			internal set
			{
				this.propertyBag[SupervisionListEntrySchema.Tag] = value;
			}
		}

		public SupervisionRecipientType RecipientType
		{
			get
			{
				return (SupervisionRecipientType)this.propertyBag[SupervisionListEntrySchema.RecipientType];
			}
			internal set
			{
				this.propertyBag[SupervisionListEntrySchema.RecipientType] = value;
			}
		}

		private static SupervisionListEntrySchema schema = ObjectSchema.GetInstance<SupervisionListEntrySchema>();
	}
}
