using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.PushNotifications.Server.Core
{
	internal interface IServiceCommand : ITask
	{
		IAsyncResult CommandAsyncResult { get; }

		void Initialize(IBudget budget);

		void Complete(Exception error = null);
	}
}
