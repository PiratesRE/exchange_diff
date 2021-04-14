using System;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public interface ITaskEvent
	{
		event EventHandler<EventArgs> PreInit;

		event EventHandler<EventArgs> InitCompleted;

		event EventHandler<EventArgs> PreIterate;

		event EventHandler<EventArgs> IterateCompleted;

		event EventHandler<EventArgs> PreRelease;

		event EventHandler<EventArgs> Release;

		event EventHandler<EventArgs> PreStop;

		event EventHandler<EventArgs> Stop;

		event EventHandler<GenericEventArg<TaskErrorEventArg>> Error;
	}
}
