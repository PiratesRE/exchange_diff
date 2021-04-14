using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class AbstractAttachment : AbstractStorePropertyBag, IAttachment, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		public AttachmentType AttachmentType
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual bool IsContactPhoto
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual AttachmentId Id
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual string ContentType
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual string CalculatedContentType
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual string DisplayName
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual string FileExtension
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual string FileName
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual bool IsInline
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual ExDateTime LastModifiedTime
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual long Size
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual void Save()
		{
			throw new NotImplementedException();
		}

		public virtual void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}
