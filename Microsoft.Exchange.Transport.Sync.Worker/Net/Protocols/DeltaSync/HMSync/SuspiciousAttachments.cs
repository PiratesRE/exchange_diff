using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.HMSync
{
	[XmlType(TypeName = "SuspiciousAttachments", Namespace = "HMSYNC:")]
	[Serializable]
	public class SuspiciousAttachments
	{
		[DispId(-4)]
		public IEnumerator GetEnumerator()
		{
			return this.AttachmentCollection.GetEnumerator();
		}

		public AttachmentVirusInfoType Add(AttachmentVirusInfoType obj)
		{
			return this.AttachmentCollection.Add(obj);
		}

		[XmlIgnore]
		public AttachmentVirusInfoType this[int index]
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

		public AttachmentVirusInfoType Remove(int index)
		{
			AttachmentVirusInfoType attachmentVirusInfoType = this.AttachmentCollection[index];
			this.AttachmentCollection.Remove(attachmentVirusInfoType);
			return attachmentVirusInfoType;
		}

		public void Remove(object obj)
		{
			this.AttachmentCollection.Remove(obj);
		}

		[XmlIgnore]
		public AttachmentVirusInfoTypeCollection AttachmentCollection
		{
			get
			{
				if (this.internalAttachmentCollection == null)
				{
					this.internalAttachmentCollection = new AttachmentVirusInfoTypeCollection();
				}
				return this.internalAttachmentCollection;
			}
			set
			{
				this.internalAttachmentCollection = value;
			}
		}

		[XmlElement(Type = typeof(AttachmentVirusInfoType), ElementName = "Attachment", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSYNC:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public AttachmentVirusInfoTypeCollection internalAttachmentCollection;
	}
}
