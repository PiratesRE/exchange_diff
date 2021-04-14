using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IAttachmentCollection : IEnumerable<AttachmentHandle>, IEnumerable
	{
		int Count { get; }

		bool Remove(AttachmentId attachmentId);

		bool Remove(AttachmentHandle handle);

		IAttachment CreateIAttachment(AttachmentType type);

		IAttachment CreateIAttachment(AttachmentType type, IAttachment attachment);

		IAttachment OpenIAttachment(AttachmentHandle handle);

		IList<AttachmentHandle> GetHandles();
	}
}
