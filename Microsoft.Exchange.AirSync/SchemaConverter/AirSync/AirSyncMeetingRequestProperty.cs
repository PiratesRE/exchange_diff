using System;
using System.Xml;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	[Serializable]
	internal class AirSyncMeetingRequestProperty : AirSyncNestedProperty
	{
		public AirSyncMeetingRequestProperty(string xmlNodeNamespace, string airSyncTagName, bool requiresClientSupport, int protocolVersion) : base(xmlNodeNamespace, airSyncTagName, new MeetingRequestData(protocolVersion), requiresClientSupport)
		{
			this.protocolVersion = protocolVersion;
		}

		protected override void InternalCopyFrom(IProperty srcProperty)
		{
			INestedProperty nestedProperty = srcProperty as INestedProperty;
			if (nestedProperty == null)
			{
				throw new UnexpectedTypeException("INestedProperty", srcProperty);
			}
			MeetingRequestData meetingRequestData = nestedProperty.NestedData as MeetingRequestData;
			if (meetingRequestData == null)
			{
				throw new UnexpectedTypeException("MeetingRequestData", nestedProperty.NestedData);
			}
			if (PropertyState.Modified != srcProperty.State)
			{
				return;
			}
			base.XmlNode = base.XmlParentNode.OwnerDocument.CreateElement(base.AirSyncTagNames[0], base.Namespace);
			string[] keysForVersion = MeetingRequestData.GetKeysForVersion(this.protocolVersion);
			for (int i = 0; i < keysForVersion.Length; i++)
			{
				object obj = meetingRequestData.SubProperties[keysForVersion[i]];
				string text = obj as string;
				if (text != null)
				{
					base.AppendChildNode(base.XmlNode, keysForVersion[i], text, MeetingRequestData.GetEmailNamespaceForKey(i));
				}
				else
				{
					RecurrenceData recurrenceData = obj as RecurrenceData;
					if (recurrenceData != null)
					{
						if (!(recurrenceData.SubProperties["Type"] is string))
						{
							throw new ConversionException("Type of Recurrence has to be specified");
						}
						XmlNode xmlNode = base.XmlParentNode.OwnerDocument.CreateElement(MeetingRequestData.Tags.Recurrences.ToString(), base.Namespace);
						XmlNode xmlNode2 = base.XmlParentNode.OwnerDocument.CreateElement("Recurrence", base.Namespace);
						for (int j = 0; j < recurrenceData.Keys.Length; j++)
						{
							string text2 = recurrenceData.SubProperties[recurrenceData.Keys[j]] as string;
							if (text2 != null)
							{
								base.AppendChildNode(xmlNode2, recurrenceData.Keys[j], text2, RecurrenceData.GetEmailNamespaceForKey(j));
							}
						}
						xmlNode.AppendChild(xmlNode2);
						base.XmlNode.AppendChild(xmlNode);
					}
					else
					{
						EnhancedLocationData enhancedLocationData = obj as EnhancedLocationData;
						if (enhancedLocationData != null && this.protocolVersion >= 160)
						{
							XmlNode xmlNode3 = base.XmlParentNode.OwnerDocument.CreateElement("Location", "AirSyncBase:");
							base.AppendToXmlNode(xmlNode3, enhancedLocationData, "AirSyncBase:");
							base.XmlNode.AppendChild(xmlNode3);
						}
					}
				}
			}
			base.XmlParentNode.AppendChild(base.XmlNode);
		}

		private int protocolVersion;
	}
}
