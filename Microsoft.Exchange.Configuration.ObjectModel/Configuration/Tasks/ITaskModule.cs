using System;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public interface ITaskModule
	{
		void Init(ITaskEvent task);

		void Dispose();
	}
}
