using System;
using System.Linq.Expressions;
using Microsoft.SharePoint.Client;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.OneDrive
{
	public abstract class ClientObjectWrapper<T> : IClientObject<T> where T : ClientObject
	{
		protected ClientObjectWrapper(T clientObject)
		{
			this.backingClientObject = clientObject;
		}

		public void Load(IClientContext context, params Expression<Func<T, object>>[] retrievals)
		{
			((ClientContextWrapper)context).BackingClientContext.Load<T>(this.backingClientObject, retrievals);
		}

		private T backingClientObject;
	}
}
