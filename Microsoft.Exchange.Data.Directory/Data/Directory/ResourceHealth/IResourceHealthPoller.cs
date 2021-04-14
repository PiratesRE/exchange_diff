using System;

namespace Microsoft.Exchange.Data.Directory.ResourceHealth
{
	internal interface IResourceHealthPoller
	{
		TimeSpan Interval { get; }

		bool IsActive { get; }

		void Execute();
	}
}
