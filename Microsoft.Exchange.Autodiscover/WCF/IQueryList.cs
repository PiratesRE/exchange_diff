using System;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	internal interface IQueryList
	{
		void Add(UserResultMapping userResultMapping);

		void Execute();
	}
}
