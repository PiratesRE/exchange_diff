using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CoreMailboxObject : CoreObject
	{
		internal CoreMailboxObject(StoreSession session, PersistablePropertyBag propertyBag, StoreObjectId storeObjectId, byte[] changeKey, ICollection<PropertyDefinition> prefetchProperties) : base(session, propertyBag, storeObjectId, changeKey, Origin.Existing, ItemLevel.TopLevel, prefetchProperties)
		{
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<CoreMailboxObject>(this);
		}

		public FolderSaveResult Save()
		{
			this.CheckDisposed(null);
			this.ValidateStoreObject();
			try
			{
				CoreObject.GetPersistablePropertyBag(this).FlushChanges();
			}
			catch (PropertyErrorException ex)
			{
				return new FolderSaveResult(OperationResult.Failed, ex, ex.PropertyErrors);
			}
			CoreObject.GetPersistablePropertyBag(this).SaveChanges(false);
			return FolderPropertyBag.SuccessfulSave;
		}

		protected override Schema GetSchema()
		{
			this.CheckDisposed(null);
			return MailboxSchema.Instance;
		}

		protected override StorePropertyDefinition IdProperty
		{
			get
			{
				return InternalSchema.MailboxId;
			}
		}

		private void ValidateStoreObject()
		{
			ValidationContext context = new ValidationContext(base.Session);
			Validation.Validate(this, context);
		}

		public void GetLocalReplicationIds(uint numberOfIds, out Guid replicationGuid, out byte[] globCount)
		{
			this.CheckDisposed(null);
			Guid empty = Guid.Empty;
			byte[] array = null;
			StoreSession session = base.Session;
			object thisObject = null;
			bool flag = false;
			try
			{
				if (session != null)
				{
					session.BeginMapiCall();
					session.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				base.Session.Mailbox.MapiStore.GetLocalRepIds(numberOfIds, out empty, out array);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetLocalRepIds, ex, session, thisObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Failed to get local replication IDs from store", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetLocalRepIds, ex2, session, thisObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Failed to get local replication IDs from store", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (session != null)
					{
						session.EndMapiCall();
						if (flag)
						{
							session.EndServerHealthCall();
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
			replicationGuid = empty;
			globCount = array;
		}
	}
}
