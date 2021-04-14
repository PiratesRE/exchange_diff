using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.HMSync
{
	[XmlType(TypeName = "CleanedAttachments", Namespace = "HMSYNC:")]
	[Serializable]
	public class CleanedAttachments
	{
		[DispId(-4)]
		public IEnumerator GetEnumerator()
		{
			return this.AttachmentCollection.GetEnumerator();
		}

		public Attachment Add(Attachment obj)
		{
			return this.AttachmentCollection.Add(obj);
		}

		[XmlIgnore]
		public Attachment this[int index]
		{
			get
			{
				return this.AttachmentCollection[index];
			}
		}

		[XmlIgnore]
		public int Count
		{
			get
			{
				return this.AttachmentCollection.Count;
			}
		}

		public void Clear()
		{
			this.AttachmentCollection.Clear();
		}

		public Attachment Remove(int index)
		{
			Attachment attachment = this.AttachmentCollection[index];
			this.AttachmentCollection.Remove(attachment);
			return attachment;
		}

		public void Remove(object obj)
		{
			this.AttachmentCollection.Remove(obj);
		}

		[XmlIgnore]
		public AttachmentCollection AttachmentCollection
		{
			get
			{
				if (this.internalAttachmentCollection == null)
				{
					this.internalAttachmentCollection = new AttachmentCollection();
				}
				return this.internalAttachmentCollection;
			}
			set
			{
				this.internalAttachmentCollection = value;
			}
		}

		[XmlElement(Type = typeof(Attachment), ElementName = "Attachment", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSYNC:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public AttachmentCollection internalAttachmentCollection;
	}
}
