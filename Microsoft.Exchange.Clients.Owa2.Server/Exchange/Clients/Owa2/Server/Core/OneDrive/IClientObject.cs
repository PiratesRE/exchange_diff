using System;
using System.Linq.Expressions;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.OneDrive
{
	public interface IClientObject<T>
	{
		void Load(IClientContext context, params Expression<Func<T, object>>[] retrievals);
	}
}
