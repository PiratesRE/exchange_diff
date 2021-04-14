using System;
using System.Collections;
using System.ComponentModel;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.ItemOperationsRequest
{
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Serializable]
	public class ReportAsSpamTypeCollection : ArrayList
	{
		public ReportAsSpamType Add(ReportAsSpamType obj)
		{
			base.Add(obj);
			return obj;
		}

		public ReportAsSpamType Add()
		{
			return this.Add(new ReportAsSpamType());
		}

		public void Insert(int index, ReportAsSpamType obj)
		{
			base.Insert(index, obj);
		}

		public void Remove(ReportAsSpamType obj)
		{
			base.Remove(obj);
		}

		public ReportAsSpamType this[int index]
		{
			get
			{
				return (ReportAsSpamType)base[index];
			}
			set
			{
				base[index] = value;
			}
		}
	}
}
