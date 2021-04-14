using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsRequest
{
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[XmlType(TypeName = "LocalPartsType", Namespace = "HMSETTINGS:")]
	[Serializable]
	public class LocalPartsType
	{
		[DispId(-4)]
		public IEnumerator GetEnumerator()
		{
			return this.LocalPartCollection.GetEnumerator();
		}

		public string Add(string obj)
		{
			return this.LocalPartCollection.Add(obj);
		}

		[XmlIgnore]
		public string this[int index]
		{
			get
			{
				return this.LocalPartCollection[index];
			}
		}

		[XmlIgnore]
		public int Count
		{
			get
			{
				return this.LocalPartCollection.Count;
			}
		}

		public void Clear()
		{
			this.LocalPartCollection.Clear();
		}

		public string Remove(int index)
		{
			string text = this.LocalPartCollection[index];
			this.LocalPartCollection.Remove(text);
			return text;
		}

		public void Remove(object obj)
		{
			this.LocalPartCollection.Remove(obj);
		}

		[XmlIgnore]
		public LocalPartCollection LocalPartCollection
		{
			get
			{
				if (this.internalLocalPartCollection == null)
				{
					this.internalLocalPartCollection = new LocalPartCollection();
				}
				return this.internalLocalPartCollection;
			}
			set
			{
				this.internalLocalPartCollection = value;
			}
		}

		[XmlElement(Type = typeof(string), ElementName = "LocalPart", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string", Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public LocalPartCollection internalLocalPartCollection;
	}
}
