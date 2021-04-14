using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;
using Microsoft.Exchange.DxStore.Common;

namespace Microsoft.Exchange.DxStore.Server
{
	[DataContract(Namespace = "http://www.outlook.com/highavailability/dxstore/v1/")]
	[Serializable]
	public class KeyContainer
	{
		public KeyContainer()
		{
		}

		public KeyContainer(string name, KeyContainer parent)
		{
			this.Name = name;
			this.Parent = parent;
			this.FullName = name;
			if (parent != null)
			{
				this.FullName = Utils.CombinePathNullSafe(parent.FullName, name);
			}
			this.SubKeys = new Dictionary<string, KeyContainer>();
			this.Properties = new Dictionary<string, PropertyValue>();
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public Dictionary<string, KeyContainer> SubKeys { get; set; }

		[DataMember]
		public Dictionary<string, PropertyValue> Properties { get; set; }

		[IgnoreDataMember]
		public string FullName { get; set; }

		[IgnoreDataMember]
		public KeyContainer Parent { get; set; }

		public static KeyContainer Create(XElement element, KeyContainer parent = null)
		{
			string value = element.Attribute("Name").Value;
			KeyContainer keyContainer = new KeyContainer(value, parent);
			if (parent != null)
			{
				parent.SubKeys[value] = keyContainer;
			}
			List<XElement> list = new List<XElement>();
			if (element.HasElements)
			{
				foreach (XElement xelement in element.Elements())
				{
					string localName = xelement.Name.LocalName;
					if (Utils.IsEqual(localName, "Key", StringComparison.OrdinalIgnoreCase))
					{
						list.Add(xelement);
					}
					else if (Utils.IsEqual(localName, "Value", StringComparison.OrdinalIgnoreCase))
					{
						string value2 = xelement.Attribute("Name").Value;
						PropertyValue value3 = PropertyValue.Parse(xelement);
						keyContainer.Properties[value2] = value3;
					}
				}
			}
			foreach (XElement element2 in list)
			{
				KeyContainer.Create(element2, keyContainer);
			}
			return keyContainer;
		}

		public XElement GetSnapshot(bool isRootKey = false)
		{
			XName name = "Key";
			object[] array = new object[2];
			array[0] = new XAttribute("Name", isRootKey ? "\\" : this.Name);
			array[1] = from kvp in this.Properties
			select kvp.Value.ToXElement(kvp.Key);
			XElement xelement = new XElement(name, array);
			foreach (KeyContainer keyContainer in this.SubKeys.Values)
			{
				XElement snapshot = keyContainer.GetSnapshot(false);
				xelement.Add(snapshot);
			}
			return xelement;
		}
	}
}
