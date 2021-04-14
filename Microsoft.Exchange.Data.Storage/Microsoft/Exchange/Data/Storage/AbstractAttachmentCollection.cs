using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class AbstractAttachmentCollection : IAttachmentCollection, IEnumerable<AttachmentHandle>, IEnumerable
	{
		public virtual int Count
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual IEnumerator<AttachmentHandle> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public virtual IAttachment CreateIAttachment(AttachmentType type)
		{
			throw new NotImplementedException();
		}

		public virtual IAttachment CreateIAttachment(AttachmentType type, IAttachment attachment)
		{
			throw new NotImplementedException();
		}

		public virtual IAttachment OpenIAttachment(AttachmentHandle handle)
		{
			throw new NotImplementedException();
		}

		public virtual bool Remove(AttachmentId attachmentId)
		{
			throw new NotImplementedException();
		}

		public virtual bool Remove(AttachmentHandle handle)
		{
			throw new NotImplementedException();
		}

		public virtual IList<AttachmentHandle> GetHandles()
		{
			throw new NotImplementedException();
		}
	}
}
