using System;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;

namespace Microsoft.Exchange.Search.Core.AsyncTask
{
	internal sealed class AsyncStart : AsyncTask
	{
		internal AsyncStart(IStartStop component)
		{
			Util.ThrowOnNullArgument(component, "component");
			this.component = component;
		}

		public override string ToString()
		{
			return string.Format("AsyncStart for {0}", this.component);
		}

		internal override void InternalExecute()
		{
			this.component.BeginStart(delegate(IAsyncResult ar)
			{
				ComponentException exception = null;
				try
				{
					this.component.EndStart(ar);
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
