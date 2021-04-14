using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.OneDrive
{
	public interface IClientObjectCollection<T, T2> : IClientObject<T2>, IEnumerable<T>, IEnumerable
	{
		T this[int index]
		{
			get;
		}

		int Count();
	}
}
