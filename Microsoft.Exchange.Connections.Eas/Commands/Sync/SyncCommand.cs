using System;
using System.Net;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.Sync
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SyncCommand : EasServerCommand<SyncRequest, SyncResponse, SyncStatus>
	{
		internal SyncCommand(EasConnectionSettings easConnectionSettings) : base(Command.Sync, easConnectionSettings)
		{
		}

		protected override void AddWebRequestHeaders(HttpWebRequest request)
		{
			base.AddWebRequestHeaders(request);
			EasExtensionCapabilities extensionCapabilities = base.EasConnectionSettings.ExtensionCapabilities;
			if (extensionCapabilities != null && extensionCapabilities.SupportsExtensions(EasExtensionsVersion1.SystemCategories))
			{
				request.Headers.Add("X-OLK-Extension", extensionCapabilities.RequestExtensions(EasExtensionsVersion1.SystemCategories));
			}
		}
	}
}
