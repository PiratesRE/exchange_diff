using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SendRequest
{
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[XmlType(TypeName = "RecipientsType", Namespace = "Send:")]
	[Serializable]
	public class RecipientsType
	{
		[DispId(-4)]
		public IEnumerator GetEnumerator()
		{
			return this.RecipientCollection.GetEnumerator();
		}

		public string Add(string obj)
		{
			return this.RecipientCollection.Add(obj);
		}

		[XmlIgnore]
		public string this[int index]
		{
			get
			{
				return this.RecipientCollection[index];
			}
		}

		[XmlIgnore]
		public int Count
		{
			get
			{
				return this.RecipientCollection.Count;
			}
		}

		public void Clear()
		{
			this.RecipientCollection.Clear();
		}

		public string Remove(int index)
		{
			string text = this.RecipientCollection[index];
			this.RecipientCollection.Remove(text);
			return text;
		}

		public void Remove(object obj)
		{
			this.RecipientCollection.Remove(obj);
		}

		[XmlIgnore]
		public RecipientCollection RecipientCollection
		{
			get
			{
				if (this.internalRecipientCollection == null)
				{
					this.internalRecipientCollection = new RecipientCollection();
				}
				return this.internalRecipientCollection;
			}
			set
			{
				this.internalRecipientCollection = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(string), ElementName = "Recipient", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string", Namespace = "Send:")]
		public RecipientCollection internalRecipientCollection;
	}
}
