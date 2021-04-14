using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class ProgressTrackerTransmitter : DisposableWrapper<IDataImport>, IDataImport, IDisposable
	{
		public ProgressTrackerTransmitter(IDataImport destination, BaseJob job) : base(destination, true)
		{
			this.destination = base.WrappedObject;
			this.job = job;
		}

		IDataMessage IDataImport.SendMessageAndWaitForReply(IDataMessage request)
		{
			this.UpdateTracker(request);
			IDataMessage dataMessage = this.destination.SendMessageAndWaitForReply(request);
			this.UpdateTracker(dataMessage);
			return dataMessage;
		}

		void IDataImport.SendMessage(IDataMessage message)
		{
			this.UpdateTracker(message);
			this.destination.SendMessage(message);
		}

		private void UpdateTracker(IDataMessage message)
		{
			if (this.job.ProgressTracker != null && message != null)
			{
				uint size = (uint)message.GetSize();
				this.job.ProgressTracker.AddBytes(size);
				MRSResource.Cache.GetInstance(MRSResource.Id.ObjectGuid, this.job.WorkloadTypeFromJob).Charge(size);
			}
		}

		private IDataImport destination;

		private BaseJob job;
	}
}
