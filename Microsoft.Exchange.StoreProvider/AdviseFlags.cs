using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum AdviseFlags
	{
		CriticalError = 1,
		NewMail = 2,
		ObjectCreated = 4,
		ObjectDeleted = 8,
		ObjectModified = 16,
		ObjectMoved = 32,
		ObjectCopied = 64,
		SearchComplete = 128,
		TableModified = 256,
		StatusObjectModified = 512,
		MailSubmitted = 1024,
		ConnectionDropped = 2048,
		Extended = -2147483648
	}
}
