using System;
using System.Collections;
using System.ComponentModel;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.HMSync
{
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Serializable]
	public class AttachmentVirusInfoTypeCollection : ArrayList
	{
		public AttachmentVirusInfoType Add(AttachmentVirusInfoType obj)
		{
			base.Add(obj);
			return obj;
		}

		public AttachmentVirusInfoType Add()
		{
			return this.Add(new AttachmentVirusInfoType());
		}

		public void Insert(int index, AttachmentVirusInfoType obj)
		{
			base.Insert(index, obj);
		}

		public void Remove(AttachmentVirusInfoType obj)
		{
			base.Remove(obj);
		}

		public AttachmentVirusInfoType this[int index]
		{
			get
			{
				return (AttachmentVirusInfoType)base[index];
			}
			set
			{
				base[index] = value;
			}
		}
	}
}
