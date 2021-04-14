using System;
using Microsoft.Exchange.Clients.Owa.Core.Controls;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public interface IGroupRange
	{
		string Header { get; }

		bool IsInGroup(IListViewDataSource dataSource, Column column);
	}
}
