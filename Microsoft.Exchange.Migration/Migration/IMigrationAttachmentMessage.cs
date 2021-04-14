using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMigrationAttachmentMessage
	{
		IMigrationAttachment CreateAttachment(string name);

		IMigrationAttachment GetAttachment(string name, PropertyOpenMode openMode);

		bool TryGetAttachment(string name, PropertyOpenMode openMode, out IMigrationAttachment attachment);

		void DeleteAttachment(string name);
	}
}
