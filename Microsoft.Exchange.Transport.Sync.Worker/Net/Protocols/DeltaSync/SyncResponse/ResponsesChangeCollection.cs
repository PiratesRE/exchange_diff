using System;
using System.Collections;
using System.ComponentModel;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SyncResponse
{
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Serializable]
	public class ResponsesChangeCollection : ArrayList
	{
		public ResponsesChange Add(ResponsesChange obj)
		{
			base.Add(obj);
			return obj;
		}

		public ResponsesChange Add()
		{
			return this.Add(new ResponsesChange());
		}

		public void Insert(int index, ResponsesChange obj)
		{
			base.Insert(index, obj);
		}

		public void Remove(ResponsesChange obj)
		{
			base.Remove(obj);
		}

		public ResponsesChange this[int index]
		{
			get
			{
				return (ResponsesChange)base[index];
			}
			set
			{
				base[index] = value;
			}
		}
	}
}
