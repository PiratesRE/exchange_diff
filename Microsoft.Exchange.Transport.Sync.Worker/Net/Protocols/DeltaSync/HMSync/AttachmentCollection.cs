using System;
using System.Collections;
using System.ComponentModel;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.HMSync
{
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Serializable]
	public class AttachmentCollection : ArrayList
	{
		public Attachment Add(Attachment obj)
		{
			base.Add(obj);
			return obj;
		}

		public Attachment Add()
		{
			return this.Add(new Attachment());
		}

		public void Insert(int index, Attachment obj)
		{
			base.Insert(index, obj);
		}

		public void Remove(Attachment obj)
		{
			base.Remove(obj);
		}

		public Attachment this[int index]
		{
			get
			{
				return (Attachment)base[index];
			}
			set
			{
				base[index] = value;
			}
		}
	}
}
