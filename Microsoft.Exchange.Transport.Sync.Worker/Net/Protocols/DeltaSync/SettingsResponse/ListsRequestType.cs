using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsResponse
{
	[XmlType(TypeName = "ListsRequestType", Namespace = "HMSETTINGS:")]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Serializable]
	public class ListsRequestType
	{
		[DispId(-4)]
		public IEnumerator GetEnumerator()
		{
			return this.ListCollection.GetEnumerator();
		}

		public ListsRequestTypeList Add(ListsRequestTypeList obj)
		{
			return this.ListCollection.Add(obj);
		}

		[XmlIgnore]
		public ListsRequestTypeList this[int index]
		{
			get
			{
				return this.ListCollection[index];
			}
		}

		[XmlIgnore]
		public int Count
		{
			get
			{
				return this.ListCollection.Count;
			}
		}

		public void Clear()
		{
			this.ListCollection.Clear();
		}

		public ListsRequestTypeList Remove(int index)
		{
			ListsRequestTypeList listsRequestTypeList = this.ListCollection[index];
			this.ListCollection.Remove(listsRequestTypeList);
			return listsRequestTypeList;
		}

		public void Remove(object obj)
		{
			this.ListCollection.Remove(obj);
		}

		[XmlIgnore]
		public ListsRequestTypeListCollection ListCollection
		{
			get
			{
				if (this.internalListCollection == null)
				{
					this.internalListCollection = new ListsRequestTypeListCollection();
				}
				return this.internalListCollection;
			}
			set
			{
				this.internalListCollection = value;
			}
		}

		[XmlElement(Type = typeof(ListsRequestTypeList), ElementName = "List", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public ListsRequestTypeListCollection internalListCollection;
	}
}
