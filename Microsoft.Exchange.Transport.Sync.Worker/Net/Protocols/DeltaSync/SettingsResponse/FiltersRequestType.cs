using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsResponse
{
	[XmlType(TypeName = "FiltersRequestType", Namespace = "HMSETTINGS:")]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Serializable]
	public class FiltersRequestType
	{
		[DispId(-4)]
		public IEnumerator GetEnumerator()
		{
			return this.FilterCollection.GetEnumerator();
		}

		public FiltersRequestTypeFilter Add(FiltersRequestTypeFilter obj)
		{
			return this.FilterCollection.Add(obj);
		}

		[XmlIgnore]
		public FiltersRequestTypeFilter this[int index]
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

		public FiltersRequestTypeFilter Remove(int index)
		{
			FiltersRequestTypeFilter filtersRequestTypeFilter = this.FilterCollection[index];
			this.FilterCollection.Remove(filtersRequestTypeFilter);
			return filtersRequestTypeFilter;
		}

		public void Remove(object obj)
		{
			this.FilterCollection.Remove(obj);
		}

		[XmlIgnore]
		public FiltersRequestTypeFilterCollection FilterCollection
		{
			get
			{
				if (this.internalFilterCollection == null)
				{
					this.internalFilterCollection = new FiltersRequestTypeFilterCollection();
				}
				return this.internalFilterCollection;
			}
			set
			{
				this.internalFilterCollection = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(FiltersRequestTypeFilter), ElementName = "Filter", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		public FiltersRequestTypeFilterCollection internalFilterCollection;
	}
}
