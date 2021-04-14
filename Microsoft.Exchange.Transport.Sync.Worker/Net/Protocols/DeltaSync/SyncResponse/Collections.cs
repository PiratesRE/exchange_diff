using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SyncResponse
{
	[XmlType(TypeName = "Collections", Namespace = "AirSync:")]
	[Serializable]
	public class Collections
	{
		[DispId(-4)]
		public IEnumerator GetEnumerator()
		{
			return this.CollectionCollection.GetEnumerator();
		}

		public Collection Add(Collection obj)
		{
			return this.CollectionCollection.Add(obj);
		}

		[XmlIgnore]
		public Collection this[int index]
		{
			get
			{
				return this.CollectionCollection[index];
			}
		}

		[XmlIgnore]
		public int Count
		{
			get
			{
				return this.CollectionCollection.Count;
			}
		}

		public void Clear()
		{
			this.CollectionCollection.Clear();
		}

		public Collection Remove(int index)
		{
			Collection collection = this.CollectionCollection[index];
			this.CollectionCollection.Remove(collection);
			return collection;
		}

		public void Remove(object obj)
		{
			this.CollectionCollection.Remove(obj);
		}

		[XmlIgnore]
		public CollectionCollection CollectionCollection
		{
			get
			{
				if (this.internalCollectionCollection == null)
				{
					this.internalCollectionCollection = new CollectionCollection();
				}
				return this.internalCollectionCollection;
			}
			set
			{
				this.internalCollectionCollection = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(Collection), ElementName = "Collection", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "AirSync:")]
		public CollectionCollection internalCollectionCollection;
	}
}
