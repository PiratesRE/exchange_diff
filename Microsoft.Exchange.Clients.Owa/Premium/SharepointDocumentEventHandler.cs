using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.DocumentLibrary;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventNamespace("SharepointDocument")]
	internal sealed class SharepointDocumentEventHandler : DocumentEventHandler
	{
		protected override void PreDataBind()
		{
			if (!DocumentLibraryUtilities.IsNavigationToWSSAllowed(base.UserContext))
			{
				throw new OwaSegmentationException("Access to Sharepoint documents is disabled");
			}
			this.contentTypePropertyDefinition = SharepointDocumentSchema.FileType;
		}

		public const string EventNamespace = "SharepointDocument";
	}
}
