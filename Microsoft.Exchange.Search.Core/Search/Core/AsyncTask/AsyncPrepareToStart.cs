using System;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;

namespace Microsoft.Exchange.Search.Core.AsyncTask
{
	internal sealed class AsyncPrepareToStart : AsyncTask
	{
		internal AsyncPrepareToStart(IStartStop component)
		{
			Util.ThrowOnNullArgument(component, "component");
			this.component = component;
		}

		public override string ToString()
		{
			return string.Format("AsyncPrepareToStart for {0}", this.component);
		}

		internal override void InternalExecute()
		{
			this.component.BeginPrepareToStart(delegate(IAsyncResult ar)
			{
				ComponentException exception = null;
				try
				{
					this.component.EndPrepareToStart(ar);
				}
				catch (ComponentException ex)
				{
					exception = ex;
				}
				base.Complete(exception);
			}, null);
		}

		private readonly IStartStop component;
	}
}
