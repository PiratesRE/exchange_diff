using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Scheduler.Contracts;
using Microsoft.Exchange.Transport.Storage.Messaging;

namespace Microsoft.Exchange.Transport
{
	internal class ProcessingSchedulerAdminWrapper : IProcessingSchedulerAdmin
	{
		public ProcessingSchedulerAdminWrapper(IProcessingSchedulerAdmin processingSchedulerAdmin, IMessagingDatabaseComponent messagingDatabaseComponent)
		{
			ArgumentValidator.ThrowIfNull("processingSchedulerAdmin", processingSchedulerAdmin);
			ArgumentValidator.ThrowIfNull("messagingDatabaseComponent", messagingDatabaseComponent);
			this.processingSchedulerAdmin = processingSchedulerAdmin;
			this.queueStorage = messagingDatabaseComponent.GetOrAddQueue(NextHopSolutionKey.Submission);
			if (this.queueStorage.Suspended)
			{
				this.processingSchedulerAdmin.Pause();
				return;
			}
			this.processingSchedulerAdmin.Resume();
		}

		public bool IsRunning
		{
			get
			{
				return this.processingSchedulerAdmin.IsRunning;
			}
		}

		public void Pause()
		{
			this.processingSchedulerAdmin.Pause();
			this.queueStorage.Suspended = true;
			this.queueStorage.Commit();
		}

		public void Resume()
		{
			this.processingSchedulerAdmin.Resume();
			this.queueStorage.Suspended = false;
			this.queueStorage.Commit();
		}

		public SchedulerDiagnosticsInfo GetDiagnosticsInfo()
		{
			return this.processingSchedulerAdmin.GetDiagnosticsInfo();
		}

		public bool Shutdown(int timeoutMilliseconds = -1)
		{
			return this.processingSchedulerAdmin.Shutdown(timeoutMilliseconds);
		}

		private readonly IProcessingSchedulerAdmin processingSchedulerAdmin;

		private readonly RoutedQueueBase queueStorage;
	}
}
