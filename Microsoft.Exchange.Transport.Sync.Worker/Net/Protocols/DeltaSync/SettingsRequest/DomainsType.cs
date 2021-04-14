using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsRequest
{
	[XmlType(TypeName = "DomainsType", Namespace = "HMSETTINGS:")]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Serializable]
	public class DomainsType
	{
		[DispId(-4)]
		public IEnumerator GetEnumerator()
		{
			return this.DomainCollection.GetEnumerator();
		}

		public string Add(string obj)
		{
			return this.DomainCollection.Add(obj);
		}

		[XmlIgnore]
		public string this[int index]
		{
			get
			{
				return this.DomainCollection[index];
			}
		}

		[XmlIgnore]
		public int Count
		{
			get
			{
				return this.DomainCollection.Count;
			}
		}

		public void Clear()
		{
			this.DomainCollection.Clear();
		}

		public string Remove(int index)
		{
			string text = this.DomainCollection[index];
			this.DomainCollection.Remove(text);
			return text;
		}

		public void Remove(object obj)
		{
			this.DomainCollection.Remove(obj);
		}

		[XmlIgnore]
		public DomainCollection DomainCollection
		{
			get
			{
				if (this.internalDomainCollection == null)
				{
					this.internalDomainCollection = new DomainCollection();
				}
				return this.internalDomainCollection;
			}
			set
			{
				this.internalDomainCollection = value;
			}
		}

		[XmlElement(Type = typeof(string), ElementName = "Domain", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string", Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public DomainCollection internalDomainCollection;
	}
}
