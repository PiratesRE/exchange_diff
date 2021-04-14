using System;
using System.Collections;
using System.ComponentModel;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.ItemOperationsRequest
{
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Serializable]
	public class ReportAsNotSpamTypeCollection : ArrayList
	{
		public ReportAsNotSpamType Add(ReportAsNotSpamType obj)
		{
			base.Add(obj);
			return obj;
		}

		public ReportAsNotSpamType Add()
		{
			return this.Add(new ReportAsNotSpamType());
		}

		public void Insert(int index, ReportAsNotSpamType obj)
		{
			base.Insert(index, obj);
		}

		public void Remove(ReportAsNotSpamType obj)
		{
			base.Remove(obj);
		}

		public ReportAsNotSpamType this[int index]
		{
			get
			{
				return (ReportAsNotSpamType)base[index];
			}
			set
			{
				base[index] = value;
			}
		}
	}
}
