using System;
using System.Net;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.FolderSync
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class FolderSyncCommand : EasServerCommand<FolderSyncRequest, FolderSyncResponse, FolderSyncStatus>
	{
		internal FolderSyncCommand(EasConnectionSettings easConnectionSettings) : base(Command.FolderSync, easConnectionSettings)
		{
		}

		protected override void AddWebRequestHeaders(HttpWebRequest request)
		{
			base.AddWebRequestHeaders(request);
			EasExtensionCapabilities extensionCapabilities = base.EasConnectionSettings.ExtensionCapabilities;
			if (extensionCapabilities != null && extensionCapabilities.SupportsExtensions(EasExtensionsVersion1.FolderTypes))
			{
				request.Headers.Add("X-OLK-Extension", extensionCapabilities.RequestExtensions(EasExtensionsVersion1.FolderTypes));
			}
		}
	}
}
