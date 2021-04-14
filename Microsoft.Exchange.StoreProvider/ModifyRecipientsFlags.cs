using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum ModifyRecipientsFlags
	{
		AddRecipients = 2,
		ModifyRecipients = 4,
		RemoveRecipients = 8
	}
}
