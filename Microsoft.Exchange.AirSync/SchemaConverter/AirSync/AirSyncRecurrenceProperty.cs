using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	[Serializable]
	internal class AirSyncRecurrenceProperty : AirSyncNestedProperty
	{
		public AirSyncRecurrenceProperty(string xmlNodeNamespace, string airSyncTagName, TypeOfRecurrence recurrenceType, bool requiresClientSupport, int protocolVersion) : base(xmlNodeNamespace, airSyncTagName, new RecurrenceData(recurrenceType, protocolVersion), requiresClientSupport)
		{
		}

		protected override void InternalCopyFrom(IProperty srcProperty)
		{
			INestedProperty nestedProperty = srcProperty as INestedProperty;
			if (nestedProperty == null)
			{
				throw new UnexpectedTypeException("INestedProperty", srcProperty);
			}
			RecurrenceData recurrenceData = nestedProperty.NestedData as RecurrenceData;
			if (recurrenceData == null)
			{
				throw new UnexpectedTypeException("RecurrenceData", nestedProperty.NestedData);
			}
			if (srcProperty.State != PropertyState.Modified)
			{
				return;
			}
			base.XmlNode = base.XmlParentNode.OwnerDocument.CreateElement(base.AirSyncTagNames[0], base.Namespace);
			for (int i = 0; i < recurrenceData.Keys.Length; i++)
			{
				string text = recurrenceData.SubProperties[recurrenceData.Keys[i]] as string;
				if (text == null && recurrenceData.Keys[i] == "Type")
				{
					throw new ConversionException("Type of Recurrence has to be specified");
				}
				if (text != null)
				{
					base.AppendChildNode(recurrenceData.Keys[i], text);
				}
			}
			base.XmlParentNode.AppendChild(base.XmlNode);
		}
	}
}
