using System;
using System.Collections;
using System.ComponentModel;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.ItemOperationsResponse
{
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Serializable]
	public class ReportAsNotSpamCollection : ArrayList
	{
		public ReportAsNotSpam Add(ReportAsNotSpam obj)
		{
			base.Add(obj);
			return obj;
		}

		public ReportAsNotSpam Add()
		{
			return this.Add(new ReportAsNotSpam());
		}

		public void Insert(int index, ReportAsNotSpam obj)
		{
			base.Insert(index, obj);
		}

		public void Remove(ReportAsNotSpam obj)
		{
			base.Remove(obj);
		}

		public ReportAsNotSpam this[int index]
		{
			get
			{
				return (ReportAsNotSpam)base[index];
			}
			set
			{
				base[index] = value;
			}
		}
	}
}
