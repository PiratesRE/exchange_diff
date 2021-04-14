using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AnchorService.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IAnchorAttachmentMessage
	{
		AnchorAttachment CreateAttachment(string name);

		AnchorAttachment GetAttachment(string name, PropertyOpenMode openMode);

		void DeleteAttachment(string name);
	}
}
