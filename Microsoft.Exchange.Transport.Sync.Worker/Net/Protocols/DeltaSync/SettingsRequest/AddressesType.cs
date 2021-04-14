using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsRequest
{
	[XmlType(TypeName = "AddressesType", Namespace = "HMSETTINGS:")]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Serializable]
	public class AddressesType
	{
		[DispId(-4)]
		public IEnumerator GetEnumerator()
		{
			return this.AddressCollection.GetEnumerator();
		}

		public string Add(string obj)
		{
			return this.AddressCollection.Add(obj);
		}

		[XmlIgnore]
		public string this[int index]
		{
			get
			{
				return this.AddressCollection[index];
			}
		}

		[XmlIgnore]
		public int Count
		{
			get
			{
				return this.AddressCollection.Count;
			}
		}

		public void Clear()
		{
			this.AddressCollection.Clear();
		}

		public string Remove(int index)
		{
			string text = this.AddressCollection[index];
			this.AddressCollection.Remove(text);
			return text;
		}

		public void Remove(object obj)
		{
			this.AddressCollection.Remove(obj);
		}

		[XmlIgnore]
		public AddressCollection AddressCollection
		{
			get
			{
				if (this.internalAddressCollection == null)
				{
					this.internalAddressCollection = new AddressCollection();
				}
				return this.internalAddressCollection;
			}
			set
			{
				this.internalAddressCollection = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(string), ElementName = "Address", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string", Namespace = "HMSETTINGS:")]
		public AddressCollection internalAddressCollection;
	}
}
