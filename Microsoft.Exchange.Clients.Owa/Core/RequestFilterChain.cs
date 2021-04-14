using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public abstract class RequestFilterChain
	{
		internal RequestFilterChain Next
		{
			get
			{
				return this.next;
			}
			set
			{
				this.next = value;
			}
		}

		internal abstract bool FilterRequest(object source, EventArgs e, RequestEventType eventType);

		internal bool ExecuteRequestFilterChain(object source, EventArgs e, RequestEventType eventType)
		{
			bool flag = this.FilterRequest(source, e, eventType);
			if (!flag && this.Next != null)
			{
				flag = this.Next.ExecuteRequestFilterChain(source, e, eventType);
			}
			return flag;
		}

		private RequestFilterChain next;
	}
}
