using System;
using System.Collections;
using System.ComponentModel;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SyncResponse
{
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Serializable]
	public class ResponsesAddCollection : ArrayList
	{
		public ResponsesAdd Add(ResponsesAdd obj)
		{
			base.Add(obj);
			return obj;
		}

		public ResponsesAdd Add()
		{
			return this.Add(new ResponsesAdd());
		}

		public void Insert(int index, ResponsesAdd obj)
		{
			base.Insert(index, obj);
		}

		public void Remove(ResponsesAdd obj)
		{
			base.Remove(obj);
		}

		public ResponsesAdd this[int index]
		{
			get
			{
				return (ResponsesAdd)base[index];
			}
			set
			{
				base[index] = value;
			}
		}
	}
}
