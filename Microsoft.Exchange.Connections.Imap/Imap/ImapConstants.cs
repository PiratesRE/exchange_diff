using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Imap
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	public sealed class ImapConstants
	{
		internal const string InboxFolderName = "INBOX";

		internal const int DefaultPort = 143;

		internal const int SslPort = 993;

		internal const char DefaultHierarchySeparator = '/';

		internal const string ImapVersionString = "imap4rev1";

		internal const string ImapComponentId = "IMAP";

		internal const int MaxFolderLevelDepth = 20;

		internal const int MaxLinesToLog = 10;

		internal const string NullString = "Null";

		internal const string NilValue = "NIL";
	}
}
