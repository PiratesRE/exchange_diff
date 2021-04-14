using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.DocumentLibrary;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventNamespace("UncDocument")]
	internal sealed class UncDocumentEventHandler : DocumentEventHandler
	{
		protected override void PreDataBind()
		{
			if (!DocumentLibraryUtilities.IsNavigationToUNCAllowed(base.UserContext))
			{
				throw new OwaSegmentationException("Access to Unc documents is disabled");
			}
			this.contentTypePropertyDefinition = UncDocumentSchema.FileType;
		}

		public const string EventNamespace = "UncDocument";
	}
}
