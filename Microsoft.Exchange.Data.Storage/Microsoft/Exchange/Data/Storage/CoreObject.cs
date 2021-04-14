using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class CoreObject : DisposableObject, ICoreObject, ICoreState, IValidatable, IDisposeTrackable, IDisposable
	{
		internal CoreObject(StoreSession session, PersistablePropertyBag propertyBag, StoreObjectId storeObjectId, byte[] changeKey, Origin origin, ItemLevel itemLevel, ICollection<PropertyDefinition> prefetchProperties)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.session = session;
				this.propertyBag = propertyBag;
				this.itemLevel = itemLevel;
				this.Origin = origin;
				if (propertyBag.DisposeTracker != null)
				{
					propertyBag.DisposeTracker.AddExtraDataWithStackTrace("CoreObject owns PersistablePropertyBag propertyBag at");
				}
				((IDirectPropertyBag)CoreObject.GetPersistablePropertyBag(this)).Context.CoreObject = this;
				((IDirectPropertyBag)CoreObject.GetPersistablePropertyBag(this)).Context.Session = this.Session;
				if (prefetchProperties != null)
				{
					if (prefetchProperties == CoreObjectSchema.AllPropertiesOnStore)
					{
						this.propertyBag.PrefetchPropertyArray = CoreObjectSchema.AllPropertiesOnStore;
					}
					else
					{
						this.propertyBag.PrefetchPropertyArray = StorePropertyDefinition.GetNativePropertyDefinitions<PropertyDefinition>(PropertyDependencyType.AllRead, prefetchProperties).ToArray<NativeStorePropertyDefinition>();
					}
					this.propertyBag.Load(prefetchProperties);
				}
				this.storeObjectId = storeObjectId;
				this.id = ((changeKey != null) ? new VersionedId(this.storeObjectId, changeKey) : null);
				disposeGuard.Success();
			}
		}

		public ICorePropertyBag PropertyBag
		{
			get
			{
				this.CheckDisposed(null);
				return this.propertyBag;
			}
		}

		public StoreSession Session
		{
			get
			{
				this.CheckDisposed(null);
				return this.session;
			}
		}

		public VersionedId Id
		{
			get
			{
				this.CheckDisposed(null);
				if (this.id == null)
				{
					this.UpdateIds();
				}
				return this.id;
			}
		}

		public Origin Origin
		{
			get
			{
				this.CheckDisposed(null);
				return this.origin;
			}
			set
			{
				this.CheckDisposed(null);
				EnumValidator.ThrowIfInvalid<Origin>(value, "origin");
				this.origin = value;
			}
		}

		public ItemLevel ItemLevel
		{
			get
			{
				this.CheckDisposed(null);
				return this.itemLevel;
			}
		}

		public virtual bool IsDirty
		{
			get
			{
				return this.propertyBag != null && this.propertyBag.IsDirty;
			}
		}

		Schema IValidatable.Schema
		{
			get
			{
				this.CheckDisposed(null);
				return ((ICoreObject)this).GetCorrectSchemaForStoreObject();
			}
		}

		bool IValidatable.ValidateAllProperties
		{
			get
			{
				this.CheckDisposed(null);
				return this.enableFullValidation || this.origin == Origin.New;
			}
		}

		StoreObjectId ICoreObject.StoreObjectId
		{
			get
			{
				this.CheckDisposed(null);
				if (this.storeObjectId == null)
				{
					this.UpdateIds();
				}
				return this.storeObjectId;
			}
		}

		StoreObjectId ICoreObject.InternalStoreObjectId
		{
			get
			{
				this.CheckDisposed(null);
				return this.storeObjectId;
			}
		}

		protected abstract StorePropertyDefinition IdProperty { get; }

		void IValidatable.Validate(ValidationContext context, IList<StoreObjectValidationError> validationErrors)
		{
			this.CheckDisposed(null);
			Validation.ValidateProperties(context, this, CoreObject.GetPersistablePropertyBag(this), validationErrors);
			this.ValidateContainedObjects(validationErrors);
		}

		void ICoreObject.SetEnableFullValidation(bool enableFullValidation)
		{
			this.CheckDisposed(null);
			this.enableFullValidation = enableFullValidation;
		}

		Schema ICoreObject.GetCorrectSchemaForStoreObject()
		{
			this.CheckDisposed(null);
			return this.GetSchema();
		}

		void ICoreObject.ResetId()
		{
			this.CheckDisposed(null);
			this.id = null;
		}

		internal static PersistablePropertyBag GetPersistablePropertyBag(ICoreObject coreObject)
		{
			return PersistablePropertyBag.GetPersistablePropertyBag(coreObject.PropertyBag);
		}

		internal static PropertyError[] MapiCopyTo(MapiProp source, MapiProp destination, StoreSession sourceSession, StoreSession destSession, CopyPropertiesFlags copyPropertiesFlags, CopySubObjects copySubObjects, params NativeStorePropertyDefinition[] excludeProperties)
		{
			Util.ThrowOnNullArgument(source, "sources");
			Util.ThrowOnNullArgument(destination, "destination");
			ICollection<PropTag> excludeTags = PropertyTagCache.Cache.PropTagsFromPropertyDefinitions(source, sourceSession, true, excludeProperties);
			PropProblem[] problems = null;
			bool flag = false;
			try
			{
				if (sourceSession != null)
				{
					sourceSession.BeginMapiCall();
					sourceSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				try
				{
					problems = source.CopyTo(destination, false, CoreObject.ToCopyPropertiesFlags(copyPropertiesFlags), copySubObjects == CopySubObjects.Copy, excludeTags);
				}
				catch (MapiExceptionNamedPropsQuotaExceeded)
				{
					PropTag[] propList = source.GetPropList();
					NativeStorePropertyDefinition[] propertyDefinitions = PropertyTagCache.Cache.PropertyDefinitionsFromPropTags(NativeStorePropertyDefinition.TypeCheckingFlag.DisableTypeCheck, source, sourceSession, propList);
					ICollection<PropTag> collection = PropertyTagCache.Cache.PropTagsFromPropertyDefinitions(destination, destSession, true, true, true, propertyDefinitions);
					List<PropTag> list = new List<PropTag>(collection.Count);
					int num = 0;
					foreach (PropTag propTag in collection)
					{
						if (propTag == PropTag.Unresolved)
						{
							list.Add(propList[num]);
						}
						num++;
					}
					problems = source.CopyTo(destination, list);
				}
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCopyMapiProps, ex, sourceSession, source, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("CoreItem::MapiCopyTo.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCopyMapiProps, ex2, sourceSession, source, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("CoreItem::MapiCopyTo.", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (sourceSession != null)
					{
						sourceSession.EndMapiCall();
						if (flag)
						{
							sourceSession.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
			CoreObject.ProcessCopyPropertyProblems(problems, sourceSession, source);
			return CoreObject.ToXsoPropertyErrors(destSession, destination, problems);
		}

		internal static PropertyError[] MapiCopyProps(MapiProp source, MapiProp destination, StoreSession sourceSession, StoreSession destSession, CopyPropertiesFlags copyPropertiesFlags, params NativeStorePropertyDefinition[] includedProperties)
		{
			Util.ThrowOnNullArgument(source, "sources");
			Util.ThrowOnNullArgument(destination, "destination");
			ICollection<PropTag> collection = PropertyTagCache.Cache.PropTagsFromPropertyDefinitions(source, sourceSession, true, includedProperties);
			PropProblem[] problems = null;
			bool flag = false;
			try
			{
				if (sourceSession != null)
				{
					sourceSession.BeginMapiCall();
					sourceSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				CopyPropertiesFlags copyPropertiesFlags2 = CoreObject.ToCopyPropertiesFlags(copyPropertiesFlags);
				try
				{
					problems = source.CopyProps(destination, copyPropertiesFlags2, collection);
				}
				catch (MapiExceptionNamedPropsQuotaExceeded)
				{
					List<PropTag> list = new List<PropTag>(collection.Count);
					foreach (PropTag propTag in collection)
					{
						if (propTag != PropTag.Unresolved)
						{
							list.Add(propTag);
						}
					}
					problems = source.CopyProps(destination, copyPropertiesFlags2, list);
				}
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCopyMapiProps, ex, sourceSession, source, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("CoreItem::MapiCopyProperties.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCopyMapiProps, ex2, sourceSession, source, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("CoreItem::MapiCopyProperties.", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (sourceSession != null)
					{
						sourceSession.EndMapiCall();
						if (flag)
						{
							sourceSession.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
			CoreObject.ProcessCopyPropertyProblems(problems, sourceSession, source);
			return CoreObject.ToXsoPropertyErrors(destSession, destination, problems);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (this.propertyBag != null && this.propertyBag.DisposeTracker != null)
			{
				this.propertyBag.DisposeTracker.AddExtraDataWithStackTrace(string.Format(CultureInfo.InvariantCulture, "CoreObject.InternalDispose({0}) called with stack", new object[]
				{
					disposing
				}));
			}
			if (disposing)
			{
				Util.DisposeIfPresent(this.propertyBag);
			}
			base.InternalDispose(disposing);
		}

		protected void ValidateCoreObject()
		{
			ValidationContext context = new ValidationContext(this.Session);
			Validation.Validate(this, context);
		}

		protected abstract Schema GetSchema();

		protected virtual void ValidateContainedObjects(IList<StoreObjectValidationError> validationErrors)
		{
		}

		protected void UpdateIds()
		{
			this.id = this.PropertyBag.GetValueOrDefault<VersionedId>(this.IdProperty);
			if (this.storeObjectId == null && this.id != null)
			{
				this.storeObjectId = this.id.ObjectId;
			}
		}

		private static void ProcessCopyPropertyProblems(PropProblem[] problems, StoreSession sourceSession, MapiProp source)
		{
			if (problems != null)
			{
				for (int i = 0; i < problems.Length; i++)
				{
					int scode = problems[i].Scode;
					if (scode != -2147221233 && scode != -2147221222)
					{
						throw PropertyError.ToException(ServerStrings.MapiCopyFailedProperties, StoreObjectPropertyBag.MapiPropProblemsToPropertyErrors(sourceSession, source, problems));
					}
				}
			}
		}

		private static CopyPropertiesFlags ToCopyPropertiesFlags(CopyPropertiesFlags copyPropertiesFlags)
		{
			switch (copyPropertiesFlags)
			{
			case CopyPropertiesFlags.None:
				return CopyPropertiesFlags.None;
			case CopyPropertiesFlags.Move:
				return CopyPropertiesFlags.Move;
			case CopyPropertiesFlags.NoReplace:
				return CopyPropertiesFlags.NoReplace;
			default:
				throw new ArgumentException("copyPropertiesFlags");
			}
		}

		private static PropertyError[] ToXsoPropertyErrors(StoreSession session, MapiProp destMapiProp, PropProblem[] problems)
		{
			if (problems == null || problems.Length == 0)
			{
				return MapiPropertyBag.EmptyPropertyErrorArray;
			}
			PropTag[] array = new PropTag[problems.Length];
			int[] array2 = new int[problems.Length];
			for (int i = 0; i < problems.Length; i++)
			{
				array2[i] = problems[i].Scode;
				array[i] = problems[i].PropTag;
			}
			NativeStorePropertyDefinition[] array3 = PropertyTagCache.Cache.PropertyDefinitionsFromPropTags(NativeStorePropertyDefinition.TypeCheckingFlag.DisableTypeCheck, destMapiProp, session, array);
			PropertyError[] array4 = new PropertyError[problems.Length];
			for (int j = 0; j < problems.Length; j++)
			{
				string errorDescription;
				PropertyErrorCode error = MapiPropertyHelper.MapiErrorToXsoError(array2[j], out errorDescription);
				array4[j] = new PropertyError(array3[j], error, errorDescription);
			}
			return array4;
		}

		private readonly PersistablePropertyBag propertyBag;

		private readonly StoreSession session;

		private readonly ItemLevel itemLevel;

		private bool enableFullValidation = true;

		private Origin origin;

		private VersionedId id;

		private StoreObjectId storeObjectId;
	}
}
