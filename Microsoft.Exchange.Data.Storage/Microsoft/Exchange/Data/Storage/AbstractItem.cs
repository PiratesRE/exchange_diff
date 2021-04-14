using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class AbstractItem : AbstractStoreObject, IItem, IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		public virtual AttachmentCollection AttachmentCollection
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

		public virtual Body Body
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual IBody IBody
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual ItemCategoryList Categories
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual ICoreItem CoreItem
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public ItemCharsetDetector CharsetDetector
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual IAttachmentCollection IAttachmentCollection
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

		public virtual MapiMessage MapiMessage
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public PropertyBagSaveFlags SaveFlags
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

		public virtual void OpenAsReadWrite()
		{
			throw new NotImplementedException();
		}

		public virtual ConflictResolutionResult Save(SaveMode saveMode)
		{
			throw new NotImplementedException();
		}
	}
}
