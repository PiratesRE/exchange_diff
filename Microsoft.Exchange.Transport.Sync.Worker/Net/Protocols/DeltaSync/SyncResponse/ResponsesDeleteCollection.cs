using System;
using System.Collections;
using System.ComponentModel;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SyncResponse
{
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Serializable]
	public class ResponsesDeleteCollection : ArrayList
	{
		public ResponsesDelete Add(ResponsesDelete obj)
		{
			base.Add(obj);
			return obj;
		}

		public ResponsesDelete Add()
		{
			return this.Add(new ResponsesDelete());
		}

		public void Insert(int index, ResponsesDelete obj)
		{
			base.Insert(index, obj);
		}

		public void Remove(ResponsesDelete obj)
		{
			base.Remove(obj);
		}

		public ResponsesDelete this[int index]
		{
			get
			{
				return (ResponsesDelete)base[index];
			}
			set
			{
				base[index] = value;
			}
		}
	}
}
