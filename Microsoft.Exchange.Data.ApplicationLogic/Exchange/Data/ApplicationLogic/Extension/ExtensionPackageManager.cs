using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.ApplicationLogic;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	public static class ExtensionPackageManager
	{
		public static string GetFaiName(string extensionId, string version)
		{
			return "ClientExtension" + '.' + ExtensionDataHelper.FormatExtensionId(extensionId).Replace("-", string.Empty);
		}

		internal static StoreObjectId GetExtensionFolderId(MailboxSession mailboxSession)
		{
			return mailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox);
		}

		private const string ExtensionFaiPrefix = "ClientExtension";

		private static readonly Trace Tracer = ExTraceGlobals.ExtensionTracer;
	}
}
