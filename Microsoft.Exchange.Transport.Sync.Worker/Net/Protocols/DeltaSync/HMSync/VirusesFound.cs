using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.HMSync
{
	[XmlType(TypeName = "VirusesFound", Namespace = "HMSYNC:")]
	[Serializable]
	public class VirusesFound
	{
		[DispId(-4)]
		public IEnumerator GetEnumerator()
		{
			return this.NameCollection.GetEnumerator();
		}

		public string Add(string obj)
		{
			return this.NameCollection.Add(obj);
		}

		[XmlIgnore]
		public string this[int index]
		{
			get
			{
				return this.NameCollection[index];
			}
		}

		[XmlIgnore]
		public int Count
		{
			get
			{
				return this.NameCollection.Count;
			}
		}

		public void Clear()
		{
			this.NameCollection.Clear();
		}

		public string Remove(int index)
		{
			string text = this.NameCollection[index];
			this.NameCollection.Remove(text);
			return text;
		}

		public void Remove(object obj)
		{
			this.NameCollection.Remove(obj);
		}

		[XmlIgnore]
		public NameCollection NameCollection
		{
			get
			{
				if (this.internalNameCollection == null)
				{
					this.internalNameCollection = new NameCollection();
				}
				return this.internalNameCollection;
			}
			set
			{
				this.internalNameCollection = value;
			}
		}

		[XmlElement(Type = typeof(string), ElementName = "Name", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string", Namespace = "HMSYNC:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public NameCollection internalNameCollection;
	}
}
