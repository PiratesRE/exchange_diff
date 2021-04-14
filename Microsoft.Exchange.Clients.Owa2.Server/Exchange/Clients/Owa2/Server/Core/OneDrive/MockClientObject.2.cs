using System;
using System.Linq.Expressions;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.OneDrive
{
	public abstract class MockClientObject<T> : MockClientObject, IClientObject<T>
	{
		public void Load(IClientContext context, params Expression<Func<T, object>>[] retrievals)
		{
			((MockClientContext)context).Load(this);
		}
	}
}
