using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AnchorService.Storage
{
	internal class AnchorXmlSerializableObject<TXmlSerializable> : AnchorPersistableBase where TXmlSerializable : XMLSerializableBase
	{
		public AnchorXmlSerializableObject(AnchorContext context) : base(context)
		{
		}

		public override PropertyDefinition[] PropertyDefinitions
		{
			get
			{
				return AnchorHelper.AggregateProperties(new IList<PropertyDefinition>[]
				{
					base.PropertyDefinitions,
					new PropertyDefinition[]
					{
						ItemSchema.TextBody,
						StoreObjectSchema.ItemClass
					}
				});
			}
		}

		public TXmlSerializable PersistedObject { get; set; }

		public static string GetItemClass()
		{
			return string.Format("IPM.MS-Exchange.Anchor.{0}", typeof(TXmlSerializable).FullName);
		}

		public static AnchorRowSelectorResult SelectByItemClassAndStopProcessing(IDictionary<PropertyDefinition, object> row)
		{
			if (!AnchorXmlSerializableObject<TXmlSerializable>.GetItemClass().Equals(row[StoreObjectSchema.ItemClass]))
			{
				return AnchorRowSelectorResult.RejectRowStopProcessing;
			}
			return AnchorRowSelectorResult.AcceptRow;
		}

		public override IAnchorStoreObject FindStoreObject(IAnchorDataProvider dataProvider, StoreObjectId id, PropertyDefinition[] properties)
		{
			return dataProvider.FindMessage(id, properties);
		}

		public override void WriteToMessageItem(IAnchorStoreObject message, bool loaded)
		{
			base.WriteToMessageItem(message, loaded);
			message[StoreObjectSchema.ItemClass] = AnchorXmlSerializableObject<TXmlSerializable>.GetItemClass();
			PropertyDefinition textBody = ItemSchema.TextBody;
			TXmlSerializable persistedObject = this.PersistedObject;
			message[textBody] = persistedObject.Serialize(false);
		}

		public override bool ReadFromMessageItem(IAnchorStoreObject message)
		{
			if (!base.ReadFromMessageItem(message))
			{
				return false;
			}
			if (!AnchorXmlSerializableObject<TXmlSerializable>.GetItemClass().Equals(message[StoreObjectSchema.ItemClass]))
			{
				return false;
			}
			string text = message[ItemSchema.TextBody] as string;
			if (string.IsNullOrEmpty(text))
			{
				return false;
			}
			this.PersistedObject = XMLSerializableBase.Deserialize<TXmlSerializable>(text, true);
			return true;
		}

		protected override IAnchorStoreObject CreateStoreObject(IAnchorDataProvider dataProvider)
		{
			return dataProvider.CreateMessage();
		}
	}
}
