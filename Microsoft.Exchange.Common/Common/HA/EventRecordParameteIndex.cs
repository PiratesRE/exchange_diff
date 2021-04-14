using System;

namespace Microsoft.Exchange.Common.HA
{
	internal enum EventRecordParameteIndex
	{
		Version,
		Namespace,
		Tag,
		Guid,
		InstanceName,
		ComponentName,
		IsIoErrorSpecified,
		IsNotifyEventSpecified,
		IoErrorCategory,
		IoErrorFileName,
		IoErrorOffset,
		IoErrorSize,
		NotifyeventId,
		NotifyeventParambufferSize,
		NotifyeventParambuffer,
		Message
	}
}
