using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class CoreObjectWrapper : ICoreObject, ICoreState, IValidatable, IDisposeTrackable, IDisposable
	{
		internal CoreObjectWrapper(ICoreObject coreObject)
		{
			this.coreObject = coreObject;
			this.disposeTracker = this.GetDisposeTracker();
		}

		public abstract DisposeTracker GetDisposeTracker();

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public ICorePropertyBag PropertyBag
		{
			get
			{
				return this.coreObject.PropertyBag;
			}
		}

		public Schema GetCorrectSchemaForStoreObject()
		{
			return this.coreObject.GetCorrectSchemaForStoreObject();
		}

		public StoreSession Session
		{
			get
			{
				return this.coreObject.Session;
			}
		}

		public StoreObjectId StoreObjectId
		{
			get
			{
				return this.coreObject.StoreObjectId;
			}
		}

		public StoreObjectId InternalStoreObjectId
		{
			get
			{
				return this.coreObject.InternalStoreObjectId;
			}
		}

		public VersionedId Id
		{
			get
			{
				return this.coreObject.Id;
			}
		}

		public Origin Origin
		{
			get
			{
				return this.coreObject.Origin;
			}
			set
			{
				this.coreObject.Origin = value;
			}
		}

		public ItemLevel ItemLevel
		{
			get
			{
				return this.coreObject.ItemLevel;
			}
		}

		public void ResetId()
		{
			this.coreObject.ResetId();
		}

		public void SetEnableFullValidation(bool enableFullValidation)
		{
			this.coreObject.SetEnableFullValidation(enableFullValidation);
		}

		public bool IsDirty
		{
			get
			{
				return this.coreObject.IsDirty;
			}
		}

		void IValidatable.Validate(ValidationContext context, IList<StoreObjectValidationError> validationErrors)
		{
			this.coreObject.Validate(context, validationErrors);
		}

		Schema IValidatable.Schema
		{
			get
			{
				return this.coreObject.Schema;
			}
		}

		bool IValidatable.ValidateAllProperties
		{
			get
			{
				return this.coreObject.ValidateAllProperties;
			}
		}

		public void Dispose()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
			}
			this.UnadviseEvents();
		}

		protected ICoreObject CoreObject
		{
			get
			{
				return this.coreObject;
			}
		}

		protected virtual void UnadviseEvents()
		{
		}

		private readonly DisposeTracker disposeTracker;

		private readonly ICoreObject coreObject;
	}
}
