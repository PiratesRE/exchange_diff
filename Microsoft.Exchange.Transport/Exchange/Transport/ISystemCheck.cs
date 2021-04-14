using System;

namespace Microsoft.Exchange.Transport
{
	internal interface ISystemCheck
	{
		bool Enabled { get; }

		void Check();
	}
}
