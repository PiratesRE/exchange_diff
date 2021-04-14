using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	[Serializable]
	internal class AirSyncAttendeesProperty : AirSyncProperty, IAttendeesProperty, IMultivaluedProperty<AttendeeData>, IProperty, IEnumerable<AttendeeData>, IEnumerable
	{
		public AirSyncAttendeesProperty(string xmlNodeNamespace, string airSyncTagName, bool requiresClientSupport) : base(xmlNodeNamespace, airSyncTagName, requiresClientSupport)
		{
		}

		public int Count
		{
			get
			{
				return base.XmlNode.ChildNodes.Count;
			}
		}

		public IEnumerator<AttendeeData> GetEnumerator()
		{
			foreach (object obj in base.XmlNode.ChildNodes)
			{
				XmlNode childNode = (XmlNode)obj;
				if (childNode.FirstChild.Name == "Name")
				{
					AttendeeData returnVal = new AttendeeData(childNode.LastChild.InnerText, childNode.FirstChild.InnerText);
					yield return returnVal;
				}
				else
				{
					AttendeeData returnVal = new AttendeeData(childNode.FirstChild.InnerText, childNode.LastChild.InnerText);
					yield return returnVal;
				}
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		protected override void InternalCopyFrom(IProperty srcProperty)
		{
			IAttendeesProperty attendeesProperty = srcProperty as IAttendeesProperty;
			if (attendeesProperty == null)
			{
				throw new UnexpectedTypeException("IAttendeesProperty", srcProperty);
			}
			base.XmlNode = base.XmlParentNode.OwnerDocument.CreateElement(base.AirSyncTagNames[0], base.Namespace);
			foreach (AttendeeData attendeeData in attendeesProperty)
			{
				XmlNode xmlNode = base.XmlParentNode.OwnerDocument.CreateElement("Attendee", base.Namespace);
				XmlNode xmlNode2 = base.XmlParentNode.OwnerDocument.CreateElement("Email", base.Namespace);
				XmlNode xmlNode3 = base.XmlParentNode.OwnerDocument.CreateElement("Name", base.Namespace);
				xmlNode2.InnerText = attendeeData.EmailAddress;
				xmlNode3.InnerText = attendeeData.DisplayName;
				xmlNode.AppendChild(xmlNode2);
				xmlNode.AppendChild(xmlNode3);
				base.XmlNode.AppendChild(xmlNode);
			}
			if (base.XmlNode.HasChildNodes)
			{
				base.XmlParentNode.AppendChild(base.XmlNode);
			}
		}
	}
}
