using System;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.QueueViewerTasks
{
	[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	[Serializable]
	internal sealed class PreconditionAttribute : BasicPreconditionAttribute
	{
		public QueueViewerStrings.IDs FailureDescriptionId
		{
			get
			{
				return this.failureDescriptionId;
			}
			set
			{
				base.FailureDescription = QueueViewerStrings.GetLocalizedString(value);
				this.failureDescriptionId = value;
			}
		}

		private QueueViewerStrings.IDs failureDescriptionId;
	}
}
