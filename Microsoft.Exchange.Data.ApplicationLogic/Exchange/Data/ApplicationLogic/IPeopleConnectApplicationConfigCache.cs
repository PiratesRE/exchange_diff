using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IPeopleConnectApplicationConfigCache
	{
		bool TryGetValue(string key, out IPeopleConnectApplicationConfig value);

		void Add(string key, IPeopleConnectApplicationConfig value);
	}
}
