using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11")]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[Serializable]
	public enum DirSyncDrainageCode
	{
		Completed,
		ContextNotFound,
		ContextOutOfScope,
		PartitionUnavailable,
		ContextDeleted,
		DirSyncStatusMismatch,
		InProgress,
		UnspecifiedError
	}
}
