using System;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Runtime.Serialization;
using System.Xml.Linq;
using Microsoft.Win32;

namespace Microsoft.Exchange.DxStore.Common
{
	[KnownType(typeof(ulong))]
	[KnownType(typeof(long))]
	[KnownType(typeof(string[]))]
	[KnownType(typeof(uint))]
	[KnownType(typeof(byte[]))]
	[KnownType(typeof(string))]
	[KnownType(typeof(int))]
	[Serializable]
	public class PropertyValue
	{
		public PropertyValue()
		{
		}

		public PropertyValue(object value)
		{
			this.Value = value;
			this.Kind = (int)Utils.GetValueKind(value);
		}

		public PropertyValue(object value, RegistryValueKind kind)
		{
			this.Value = value;
			if (kind == RegistryValueKind.Unknown)
			{
				this.Kind = (int)Utils.GetValueKind(value);
				return;
			}
			this.Kind = (int)kind;
		}

		[DataMember]
		public object Value { get; set; }

		[DataMember]
		public int Kind { get; set; }

		public static PropertyValue Parse(XElement element)
		{
			string value = element.Attribute("Kind").Value;
			RegistryValueKind registryValueKind;
			if (!Enum.TryParse<RegistryValueKind>(value, true, out registryValueKind))
			{
				registryValueKind = RegistryValueKind.Unknown;
			}
			string value2 = element.Value;
			RegistryValueKind registryValueKind2 = registryValueKind;
			object value3;
			switch (registryValueKind2)
			{
			case RegistryValueKind.String:
				value3 = value2;
				goto IL_93;
			case RegistryValueKind.ExpandString:
			case (RegistryValueKind)5:
			case (RegistryValueKind)6:
				break;
			case RegistryValueKind.Binary:
				value3 = SoapHexBinary.Parse(value2).Value;
				goto IL_93;
			case RegistryValueKind.DWord:
				value3 = int.Parse(value2);
				goto IL_93;
			case RegistryValueKind.MultiString:
				value3 = Utils.GetMultistring(element);
				goto IL_93;
			default:
				if (registryValueKind2 == RegistryValueKind.QWord)
				{
					value3 = long.Parse(value2);
					goto IL_93;
				}
				break;
			}
			value3 = value2;
			IL_93:
			return new PropertyValue(value3, registryValueKind);
		}

		public string GetDebugString()
		{
			RegistryValueKind kind = (RegistryValueKind)this.Kind;
			string empty = string.Empty;
			RegistryValueKind registryValueKind = kind;
			switch (registryValueKind)
			{
			case RegistryValueKind.String:
			{
				string text = (string)this.Value;
				int length = text.Length;
				if (length > 15)
				{
					text = text.Substring(0, 12) + "...";
				}
				return string.Format("kind={0}, len={1}, value={2}", RegistryValueKind.String, length, text);
			}
			case RegistryValueKind.ExpandString:
			case (RegistryValueKind)5:
			case (RegistryValueKind)6:
				goto IL_EC;
			case RegistryValueKind.Binary:
				return string.Format("kind={0}, size={1}", RegistryValueKind.Binary, ((byte[])this.Value).Length);
			case RegistryValueKind.DWord:
				break;
			case RegistryValueKind.MultiString:
				return string.Format("kind={0}, count={1}", RegistryValueKind.MultiString, ((string[])this.Value).Length);
			default:
				if (registryValueKind != RegistryValueKind.QWord)
				{
					goto IL_EC;
				}
				break;
			}
			return string.Format("kind={0}, value={1}", kind, this.Value);
			IL_EC:
			return string.Format("kind={0}, isKnown=false", kind);
		}

		public PropertyValue Clone()
		{
			return (PropertyValue)base.MemberwiseClone();
		}

		public XElement ToXElement(string propertyName)
		{
			if (this.Kind == 0)
			{
				this.Kind = (int)Utils.GetValueKind(this.Value);
			}
			XElement xelement = new XElement("Value", new object[]
			{
				new XAttribute("Name", propertyName),
				new XAttribute("Kind", this.Kind.ToString())
			});
			if (this.Kind == 7)
			{
				string[] array = (from o in (object[])this.Value
				select o.ToString()).ToArray<string>();
				foreach (string value in array)
				{
					XElement content = new XElement("String")
					{
						Value = value
					};
					xelement.Add(content);
				}
			}
			else if (this.Kind == 3)
			{
				SoapHexBinary soapHexBinary = new SoapHexBinary((byte[])this.Value);
				xelement.Value = soapHexBinary.ToString();
			}
			else
			{
				xelement.Value = this.Value.ToString();
			}
			return xelement;
		}
	}
}
