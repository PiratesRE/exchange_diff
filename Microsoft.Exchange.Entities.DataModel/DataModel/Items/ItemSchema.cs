using System;
using System.Collections.Generic;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.DataModel.Items
{
	public class ItemSchema : StorageEntitySchema
	{
		public ItemSchema()
		{
			base.RegisterPropertyDefinition(ItemSchema.StaticAttachmentsProperty);
			base.RegisterPropertyDefinition(ItemSchema.StaticBodyProperty);
			base.RegisterPropertyDefinition(ItemSchema.StaticCategoriesProperty);
			base.RegisterPropertyDefinition(ItemSchema.StaticDateTimeCreatedProperty);
			base.RegisterPropertyDefinition(ItemSchema.StaticHasAttachmentsProperty);
			base.RegisterPropertyDefinition(ItemSchema.StaticImportanceProperty);
			base.RegisterPropertyDefinition(ItemSchema.StaticLastModifiedTimeProperty);
			base.RegisterPropertyDefinition(ItemSchema.StaticPreviewProperty);
			base.RegisterPropertyDefinition(ItemSchema.StaticReceivedTimeProperty);
			base.RegisterPropertyDefinition(ItemSchema.StaticSensitivityProperty);
			base.RegisterPropertyDefinition(ItemSchema.StaticSubjectProperty);
		}

		public TypedPropertyDefinition<List<IAttachment>> AttachmentsProperty
		{
			get
			{
				return ItemSchema.StaticAttachmentsProperty;
			}
		}

		public TypedPropertyDefinition<ItemBody> BodyProperty
		{
			get
			{
				return ItemSchema.StaticBodyProperty;
			}
		}

		public TypedPropertyDefinition<List<string>> CategoriesProperty
		{
			get
			{
				return ItemSchema.StaticCategoriesProperty;
			}
		}

		public TypedPropertyDefinition<ExDateTime> DateTimeCreatedProperty
		{
			get
			{
				return ItemSchema.StaticDateTimeCreatedProperty;
			}
		}

		public TypedPropertyDefinition<bool> HasAttachmentsProperty
		{
			get
			{
				return ItemSchema.StaticHasAttachmentsProperty;
			}
		}

		public TypedPropertyDefinition<Importance> ImportanceProperty
		{
			get
			{
				return ItemSchema.StaticImportanceProperty;
			}
		}

		public TypedPropertyDefinition<ExDateTime> LastModifiedTimeProperty
		{
			get
			{
				return ItemSchema.StaticLastModifiedTimeProperty;
			}
		}

		public TypedPropertyDefinition<string> PreviewProperty
		{
			get
			{
				return ItemSchema.StaticPreviewProperty;
			}
		}

		public TypedPropertyDefinition<ExDateTime> ReceivedTimeProperty
		{
			get
			{
				return ItemSchema.StaticReceivedTimeProperty;
			}
		}

		public TypedPropertyDefinition<Sensitivity> SensitivityProperty
		{
			get
			{
				return ItemSchema.StaticSensitivityProperty;
			}
		}

		public TypedPropertyDefinition<string> SubjectProperty
		{
			get
			{
				return ItemSchema.StaticSubjectProperty;
			}
		}

		private static readonly TypedPropertyDefinition<List<IAttachment>> StaticAttachmentsProperty = new TypedPropertyDefinition<List<IAttachment>>("Item.Attachments", null, false);

		private static readonly TypedPropertyDefinition<ItemBody> StaticBodyProperty = new TypedPropertyDefinition<ItemBody>("Item.Body", null, false);

		private static readonly TypedPropertyDefinition<List<string>> StaticCategoriesProperty = new TypedPropertyDefinition<List<string>>("Item.Categories", null, true);

		private static readonly TypedPropertyDefinition<ExDateTime> StaticDateTimeCreatedProperty = new TypedPropertyDefinition<ExDateTime>("Item.DateTimeCreated", default(ExDateTime), true);

		private static readonly TypedPropertyDefinition<bool> StaticHasAttachmentsProperty = new TypedPropertyDefinition<bool>("Item.HasAttachments", false, true);

		private static readonly TypedPropertyDefinition<Importance> StaticImportanceProperty = new TypedPropertyDefinition<Importance>("Item.Importance", Importance.Low, true);

		private static readonly TypedPropertyDefinition<ExDateTime> StaticLastModifiedTimeProperty = new TypedPropertyDefinition<ExDateTime>("Item.LastModifiedTime", default(ExDateTime), true);

		private static readonly TypedPropertyDefinition<string> StaticPreviewProperty = new TypedPropertyDefinition<string>("Item.Preview", null, true);

		private static readonly TypedPropertyDefinition<ExDateTime> StaticReceivedTimeProperty = new TypedPropertyDefinition<ExDateTime>("Item.ReceivedTime", default(ExDateTime), true);

		private static readonly TypedPropertyDefinition<Sensitivity> StaticSensitivityProperty = new TypedPropertyDefinition<Sensitivity>("Item.Sensitivity", Sensitivity.Normal, true);

		private static readonly TypedPropertyDefinition<string> StaticSubjectProperty = new TypedPropertyDefinition<string>("Item.Subject", null, true);
	}
}
