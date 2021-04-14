using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsResponse
{
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[XmlType(TypeName = "FiltersResponseType", Namespace = "HMSETTINGS:")]
	[Serializable]
	public class FiltersResponseType
	{
		[DispId(-4)]
		public IEnumerator GetEnumerator()
		{
			return this.FilterCollection.GetEnumerator();
		}

		public Filter Add(Filter obj)
		{
			return this.FilterCollection.Add(obj);
		}

		[XmlIgnore]
		public Filter this[int index]
		{
			get
			{
				return this.FilterCollection[index];
			}
		}

		[XmlIgnore]
		public int Count
		{
			get
			{
				return this.FilterCollection.Count;
			}
		}

		public void Clear()
		{
			this.FilterCollection.Clear();
		}

		public Filter Remove(int index)
		{
			Filter filter = this.FilterCollection[index];
			this.FilterCollection.Remove(filter);
			return filter;
		}

		public void Remove(object obj)
		{
			this.FilterCollection.Remove(obj);
		}

		[XmlIgnore]
		public FilterCollection FilterCollection
		{
			get
			{
				if (this.internalFilterCollection == null)
				{
					this.internalFilterCollection = new FilterCollection();
				}
				return this.internalFilterCollection;
			}
			set
			{
				this.internalFilterCollection = value;
			}
		}

		[XmlElement(Type = typeof(Filter), ElementName = "Filter", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public FilterCollection internalFilterCollection;
	}
}
