using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class AbstractStoreObject : AbstractStorePropertyBag, IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		public virtual StoreObjectId StoreObjectId
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual PersistablePropertyBag PropertyBag
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

		public virtual string ClassName
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

		public virtual bool IsNew
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual IStoreSession Session
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual StoreObjectId ParentId
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual VersionedId Id
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

		public virtual byte[] RecordKey
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual LocationIdentifierHelper LocationIdentifierHelperInstance
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}
