using System;
using System.Collections;
using System.ComponentModel;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.ItemOperationsResponse
{
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Serializable]
	public class ReportAsSpamCollection : ArrayList
	{
		public ReportAsSpam Add(ReportAsSpam obj)
		{
			base.Add(obj);
			return obj;
		}

		public ReportAsSpam Add()
		{
			return this.Add(new ReportAsSpam());
		}

		public void Insert(int index, ReportAsSpam obj)
		{
			base.Insert(index, obj);
		}

		public void Remove(ReportAsSpam obj)
		{
			base.Remove(obj);
		}

		public ReportAsSpam this[int index]
		{
			get
			{
				return (ReportAsSpam)base[index];
			}
			set
			{
				base[index] = value;
			}
		}
	}
}
