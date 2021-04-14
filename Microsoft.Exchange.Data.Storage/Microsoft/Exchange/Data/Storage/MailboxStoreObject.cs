using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class MailboxStoreObject : StoreObject
	{
		private MailboxStoreObject(CoreMailboxObject coreObject) : base(coreObject, false)
		{
			this.mailbox = new Mailbox(this);
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<MailboxStoreObject>(this);
		}

		internal static MailboxStoreObject Bind(StoreSession session, MapiStore mapiStore, ICollection<PropertyDefinition> requestedProperties)
		{
			return MailboxStoreObject.Bind(session, mapiStore, requestedProperties, true, false);
		}

		internal static MailboxStoreObject Bind(StoreSession session, MapiStore mapiStore, ICollection<PropertyDefinition> requestedProperties, bool getMappingSignature, bool overridePropertyList)
		{
			ICollection<PropertyDefinition> collection = InternalSchema.Combine<PropertyDefinition>(overridePropertyList ? new PropertyTagPropertyDefinition[]
			{
				MailboxSchema.MailboxType,
				MailboxSchema.MailboxTypeDetail
			} : MailboxSchema.Instance.AutoloadProperties, requestedProperties);
			PersistablePropertyBag persistablePropertyBag = null;
			CoreMailboxObject coreMailboxObject = null;
			MailboxStoreObject mailboxStoreObject = null;
			bool flag = false;
			MailboxStoreObject result;
			try
			{
				byte[] array = null;
				if (getMappingSignature)
				{
					object thisObject = null;
					bool flag2 = false;
					try
					{
						if (session != null)
						{
							session.BeginMapiCall();
							session.BeginServerHealthCall();
							flag2 = true;
						}
						if (StorageGlobals.MapiTestHookBeforeCall != null)
						{
							StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
						}
						using (MapiFolder rootFolder = mapiStore.GetRootFolder())
						{
							array = (rootFolder.GetProp(PropTag.MappingSignature).Value as byte[]);
						}
					}
					catch (MapiPermanentException ex)
					{
						throw StorageGlobals.TranslateMapiException(ServerStrings.StoreOperationFailed, ex, session, thisObject, "{0}. MapiException = {1}.", new object[]
						{
							string.Format("Failed to get mapping signature.", new object[0]),
							ex
						});
					}
					catch (MapiRetryableException ex2)
					{
						throw StorageGlobals.TranslateMapiException(ServerStrings.StoreOperationFailed, ex2, session, thisObject, "{0}. MapiException = {1}.", new object[]
						{
							string.Format("Failed to get mapping signature.", new object[0]),
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
								if (flag2)
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
				}
				if (array != null)
				{
					session.MappingSignature = Convert.ToBase64String(array);
				}
				persistablePropertyBag = new StoreObjectPropertyBag(session, mapiStore, collection);
				coreMailboxObject = new CoreMailboxObject(session, persistablePropertyBag, null, null, collection);
				mailboxStoreObject = new MailboxStoreObject(coreMailboxObject);
				flag = true;
				result = mailboxStoreObject;
			}
			finally
			{
				if (!flag)
				{
					if (mailboxStoreObject != null)
					{
						mailboxStoreObject.Dispose();
						mailboxStoreObject = null;
					}
					if (coreMailboxObject != null)
					{
						coreMailboxObject.Dispose();
						coreMailboxObject = null;
					}
					if (persistablePropertyBag != null)
					{
						persistablePropertyBag.Dispose();
						persistablePropertyBag = null;
					}
				}
			}
			return result;
		}

		public override Schema Schema
		{
			get
			{
				this.CheckDisposed("Schema::get");
				return MailboxSchema.Instance;
			}
		}

		internal bool IsContentIndexingEnabled
		{
			get
			{
				this.CheckDisposed("IsContentIndexingEnabled::get");
				return base.GetValueOrDefault<bool>(InternalSchema.IsContentIndexingEnabled);
			}
		}

		internal void Save()
		{
			FolderSaveResult folderSaveResult = ((CoreMailboxObject)base.CoreObject).Save();
			if (folderSaveResult.OperationResult != OperationResult.Succeeded)
			{
				throw folderSaveResult.ToException(ServerStrings.ErrorFolderSave(base.CoreObject.Id.ObjectId.ToString(), folderSaveResult.ToString()));
			}
		}

		public override string ClassName
		{
			get
			{
				this.CheckDisposed("ClassName::get.");
				return "Mailbox";
			}
			set
			{
				this.CheckDisposed("ClassName::set.");
				throw new NotSupportedException();
			}
		}

		internal void ForceReload(params PropertyDefinition[] propsToLoad)
		{
			this.CheckDisposed("ForceReload");
			if (propsToLoad == null)
			{
				throw new ArgumentNullException("propsToLoad");
			}
			StoreObjectPropertyBag storeObjectPropertyBag = (StoreObjectPropertyBag)base.PropertyBag;
			storeObjectPropertyBag.ForceReload(propsToLoad);
		}

		internal Mailbox Mailbox
		{
			get
			{
				this.CheckDisposed("Mailbox::get");
				return this.mailbox;
			}
		}

		internal MapiStore MapiStore
		{
			get
			{
				return (MapiStore)base.MapiProp;
			}
		}

		private readonly Mailbox mailbox;
	}
}
