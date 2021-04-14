using System;
using System.Net;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.ItemOperations
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ItemOperationsCommand : EasServerCommand<ItemOperationsRequest, ItemOperationsResponse, ItemOperationsStatus>
	{
		internal ItemOperationsCommand(EasConnectionSettings easConnectionSettings) : base(Command.ItemOperations, easConnectionSettings)
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
