using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsRequest
{
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[XmlType(TypeName = "ListsGetResponseType", Namespace = "HMSETTINGS:")]
	[Serializable]
	public class ListsGetResponseType
	{
		[DispId(-4)]
		public IEnumerator GetEnumerator()
		{
			return this.ListCollection.GetEnumerator();
		}

		public ListsGetResponseTypeList Add(ListsGetResponseTypeList obj)
		{
			return this.ListCollection.Add(obj);
		}

		[XmlIgnore]
		public ListsGetResponseTypeList this[int index]
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

		public ListsGetResponseTypeList Remove(int index)
		{
			ListsGetResponseTypeList listsGetResponseTypeList = this.ListCollection[index];
			this.ListCollection.Remove(listsGetResponseTypeList);
			return listsGetResponseTypeList;
		}

		public void Remove(object obj)
		{
			this.ListCollection.Remove(obj);
		}

		[XmlIgnore]
		public ListsGetResponseTypeListCollection ListCollection
		{
			get
			{
				if (this.internalListCollection == null)
				{
					this.internalListCollection = new ListsGetResponseTypeListCollection();
				}
				return this.internalListCollection;
			}
			set
			{
				this.internalListCollection = value;
			}
		}

		[XmlElement(Type = typeof(ListsGetResponseTypeList), ElementName = "List", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public ListsGetResponseTypeListCollection internalListCollection;
	}
}
