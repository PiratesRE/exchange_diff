using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class FolderPropertyBag : StoreObjectPropertyBag
	{
		protected StoreSession Session
		{
			get
			{
				return this.storeSession;
			}
		}

		internal FolderPropertyBag(StoreSession session, MapiFolder mapiFolder, ICollection<PropertyDefinition> properties) : base(session, mapiFolder, properties)
		{
			this.storeSession = session;
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<FolderPropertyBag>(this);
		}

		internal sealed override void FlushChanges()
		{
			throw new InvalidOperationException(ServerStrings.ExFolderPropertyBagCannotSaveChanges);
		}

		internal sealed override void SaveChanges(bool force)
		{
			throw new InvalidOperationException(ServerStrings.ExFolderPropertyBagCannotSaveChanges);
		}

		public sealed override Stream OpenPropertyStream(PropertyDefinition propertyDefinition, PropertyOpenMode openMode)
		{
			EnumValidator.AssertValid<PropertyOpenMode>(openMode);
			NativeStorePropertyDefinition nativeStorePropertyDefinition = propertyDefinition as NativeStorePropertyDefinition;
			if (nativeStorePropertyDefinition == null)
			{
				throw new InvalidOperationException(ServerStrings.ExPropertyNotStreamable(propertyDefinition.ToString()));
			}
			return new FolderPropertyStream(base.MapiPropertyBag, nativeStorePropertyDefinition, openMode);
		}

		internal virtual FolderSaveResult SaveFolderPropertyBag(bool needVersionCheck)
		{
			base.CheckDisposed("SaveFolderPropertyBag");
			base.BindToMapiPropertyBag();
			LocalizedException ex = null;
			List<PropertyError> list = new List<PropertyError>();
			try
			{
				if (needVersionCheck)
				{
					this.SaveFlags |= PropertyBagSaveFlags.SaveFolderPropertyBagConditional;
				}
				else
				{
					this.SaveFlags &= ~PropertyBagSaveFlags.SaveFolderPropertyBagConditional;
				}
				base.MapiPropertyBag.SaveFlags = this.SaveFlags;
				list.AddRange(base.FlushSetProperties());
			}
			catch (FolderSaveConditionViolationException ex2)
			{
				list.AddRange(this.ConvertSetPropsToErrors(PropertyErrorCode.FolderHasChanged, ServerStrings.ExFolderSetPropsFailed(ex2.ToString())));
				return new FolderSaveResult(OperationResult.Failed, ex2, list.ToArray());
			}
			catch (StoragePermanentException ex3)
			{
				ex = ex3;
			}
			catch (StorageTransientException ex4)
			{
				ex = ex4;
			}
			finally
			{
				if (ex != null)
				{
					ExTraceGlobals.StorageTracer.TraceDebug<LocalizedException>((long)this.GetHashCode(), "FolderPropertyBag::SaveFolderPropertyBag. Exception caught while setting properties. Exception = {0}.", ex);
					PropertyErrorCode errorCode = (ex is StorageTransientException) ? PropertyErrorCode.TransientMapiCallFailed : PropertyErrorCode.MapiCallFailed;
					if (ex is ObjectExistedException && base.MemoryPropertyBag.ChangeList.Contains(InternalSchema.DisplayName))
					{
						errorCode = PropertyErrorCode.FolderNameConflict;
					}
					list.AddRange(this.ConvertSetPropsToErrors(errorCode, ServerStrings.ExFolderSetPropsFailed(ex.ToString())));
				}
			}
			LocalizedException ex5 = null;
			try
			{
				list.AddRange(base.FlushDeleteProperties());
			}
			catch (StoragePermanentException ex6)
			{
				ex5 = ex6;
			}
			catch (StorageTransientException ex7)
			{
				ex5 = ex7;
			}
			finally
			{
				if (ex5 != null)
				{
					ExTraceGlobals.StorageTracer.TraceDebug<LocalizedException>((long)this.GetHashCode(), "FolderPropertyBag::SaveFolderPropertyBag. Exception caught while deleting properties. Exception = {0}.", ex5);
					PropertyErrorCode error = (ex5 is StorageTransientException) ? PropertyErrorCode.TransientMapiCallFailed : PropertyErrorCode.MapiCallFailed;
					foreach (PropertyDefinition propertyDefinition in base.MemoryPropertyBag.DeleteList)
					{
						list.Add(new PropertyError(propertyDefinition, error, ServerStrings.ExFolderDeletePropsFailed(ex5.ToString())));
					}
				}
			}
			try
			{
				base.MapiPropertyBag.SaveChanges(false);
			}
			catch (StoragePermanentException arg)
			{
				ExTraceGlobals.StorageTracer.TraceError<StoragePermanentException>((long)this.GetHashCode(), "FolderPropertyBag::SaveFolderPropertyBag. Exception caught when calling MapiFolder.SaveChanges. Exception = {0}.", arg);
				if (this.Session != null && this.Session.IsMoveUser)
				{
					throw;
				}
			}
			catch (StorageTransientException arg2)
			{
				ExTraceGlobals.StorageTracer.TraceError<StorageTransientException>((long)this.GetHashCode(), "FolderPropertyBag::SaveFolderPropertyBag. Exception caught when calling MapiFolder.SaveChanges. Exception = {0}.", arg2);
				if (this.Session != null && this.Session.IsMoveUser)
				{
					throw;
				}
			}
			this.Clear();
			if (list.Count == 0)
			{
				return FolderPropertyBag.SuccessfulSave;
			}
			return new FolderSaveResult(OperationResult.PartiallySucceeded, ex5 ?? ex, list.ToArray());
		}

		protected override bool ShouldSkipProperty(PropertyDefinition property, out PropertyErrorCode? error)
		{
			return base.ShouldSkipProperty(property, out error) || !this.Session.IsValidOperation(this.Context.CoreObject, property, out error);
		}

		public override bool CanIgnoreUnchangedProperties
		{
			get
			{
				return false;
			}
		}

		private IList<PropertyError> ConvertSetPropsToErrors(PropertyErrorCode errorCode, string errorMessage)
		{
			List<PropertyError> list = new List<PropertyError>();
			foreach (PropertyDefinition propertyDefinition in base.MemoryPropertyBag.ChangeList)
			{
				object obj = base.MemoryPropertyBag.TryGetProperty(propertyDefinition);
				if (!(obj is PropertyError))
				{
					list.Add(new PropertyError(propertyDefinition, errorCode, errorMessage));
				}
			}
			return list;
		}

		internal static readonly FolderSaveResult SuccessfulSave = new FolderSaveResult(OperationResult.Succeeded, null, Array<PropertyError>.Empty);

		private readonly StoreSession storeSession;
	}
}
