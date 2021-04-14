using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class SkipPhotoHandlerQueryStringParameters
	{
		internal const string FileSystem = "skipfs";

		internal const string FileSystemWithLeadingAmpersand = "&skipfs=1";

		internal const string Mailbox = "skipmbx";

		internal const string MailboxWithLeadingAmpersand = "&skipmbx=1";

		internal const string ActiveDirectory = "skipad";

		internal const string ActiveDirectoryWithLeadingAmpersand = "&skipad=1";

		internal const string Caching = "skipcaching";

		internal const string CachingWithLeadingAmpersand = "&skipcaching=1";

		internal const string Http = "skiphttp";

		internal const string HttpWithLeadingAmpersand = "&skiphttp=1";

		internal const string Private = "skipprv";

		internal const string PrivateWithLeadingAmpersand = "&skipprv=1";

		internal const string RemoteForest = "skiprf";

		internal const string RemoteForestWithLeadingAmpersand = "&skiprf=1";
	}
}
