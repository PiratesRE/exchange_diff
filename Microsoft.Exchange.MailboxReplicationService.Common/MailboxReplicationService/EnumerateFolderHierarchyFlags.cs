using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	[DataContract]
	internal enum EnumerateFolderHierarchyFlags
	{
		[EnumMember]
		None = 0,
		[EnumMember]
		IncludeExtendedData = 1,
		[EnumMember]
		WellKnownPublicFoldersOnly = 2
	}
}
