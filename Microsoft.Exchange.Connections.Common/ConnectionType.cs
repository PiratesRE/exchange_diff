using System;

namespace Microsoft.Exchange.Connections.Common
{
	[Flags]
	[Serializable]
	public enum ConnectionType
	{
		Unknown = 0,
		Imap = 1,
		Pop = 2,
		DeltaSyncMail = 4,
		Facebook = 16,
		LinkedIn = 32,
		AllEMail = 7,
		AllPeople = 48,
		AllThatSupportSendAs = 7,
		AllThatSupportPolicyInducedDeletion = 48,
		All = 255
	}
}
