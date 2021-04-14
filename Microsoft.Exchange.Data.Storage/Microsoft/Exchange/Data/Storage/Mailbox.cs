using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class Mailbox : IXSOMailbox, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, INotificationSource
	{
		internal Mailbox(MailboxStoreObject mailboxStoreObject)
		{
			this.mailboxStoreObject = mailboxStoreObject;
		}

		public bool IsDirty
		{
			get
			{
				return this.mailboxStoreObject.IsDirty;
			}
		}

		public void Load()
		{
			this.Load(null);
		}

		public void Load(params PropertyDefinition[] propertyDefinitions)
		{
			this.Load((ICollection<PropertyDefinition>)propertyDefinitions);
		}

		public void Load(ICollection<PropertyDefinition> propertyDefinitions)
		{
			this.mailboxStoreObject.Load(propertyDefinitions);
		}

		public Stream OpenPropertyStream(PropertyDefinition propertyDefinition, PropertyOpenMode openMode)
		{
			return this.mailboxStoreObject.OpenPropertyStream(propertyDefinition, openMode);
		}

		public object TryGetProperty(PropertyDefinition propertyDefinition)
		{
			return this.mailboxStoreObject.TryGetProperty(propertyDefinition);
		}

		public bool IsPropertyDirty(PropertyDefinition propertyDefinition)
		{
			return this.mailboxStoreObject.IsPropertyDirty(propertyDefinition);
		}

		public void Delete(PropertyDefinition propertyDefinition)
		{
			this.mailboxStoreObject.Delete(propertyDefinition);
		}

		public T GetValueOrDefault<T>(PropertyDefinition propertyDefinition, T defaultValue)
		{
			return this.mailboxStoreObject.GetValueOrDefault<T>(propertyDefinition, defaultValue);
		}

		public void SetOrDeleteProperty(PropertyDefinition propertyDefinition, object propertyValue)
		{
			this.mailboxStoreObject.SetOrDeleteProperty(propertyDefinition, propertyValue);
		}

		public object this[PropertyDefinition propertyDefinition]
		{
			get
			{
				return this.mailboxStoreObject[propertyDefinition];
			}
			set
			{
				this.mailboxStoreObject[propertyDefinition] = value;
			}
		}

		public object[] GetProperties(ICollection<PropertyDefinition> propertyDefinitionArray)
		{
			return this.mailboxStoreObject.GetProperties(propertyDefinitionArray);
		}

		public void SetProperties(ICollection<PropertyDefinition> propertyDefinitionArray, object[] propertyValuesArray)
		{
			this.mailboxStoreObject.SetProperties(propertyDefinitionArray, propertyValuesArray);
		}

		public void Save()
		{
			this.mailboxStoreObject.Save();
		}

		public Guid InstanceGuid
		{
			get
			{
				StoreSession session = this.CoreObject.Session;
				bool flag = false;
				Guid mailboxInstanceGuid;
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
					mailboxInstanceGuid = this.mailboxStoreObject.MapiStore.GetMailboxInstanceGuid();
				}
				catch (MapiPermanentException ex)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetProperties, ex, session, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("Could not retrieve a Mailbox InstanceGuid", new object[0]),
						ex
					});
				}
				catch (MapiRetryableException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetProperties, ex2, session, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("Could not retrieve a Mailbox InstanceGuid", new object[0]),
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
				return mailboxInstanceGuid;
			}
		}

		public bool IsContentIndexingEnabled
		{
			get
			{
				return this.mailboxStoreObject.IsContentIndexingEnabled;
			}
		}

		public void ForceReload(params PropertyDefinition[] propsToLoad)
		{
			this.mailboxStoreObject.ForceReload(propsToLoad);
		}

		public ICoreObject CoreObject
		{
			get
			{
				return this.mailboxStoreObject.CoreObject;
			}
		}

		public MapiStore MapiStore
		{
			get
			{
				return this.mailboxStoreObject.MapiStore;
			}
		}

		internal StoreObjectId StoreObjectId
		{
			get
			{
				return this.mailboxStoreObject.StoreObjectId;
			}
		}

		internal MapiNotificationHandle Advise(byte[] entryId, AdviseFlags eventMask, MapiNotificationHandler handler, NotificationCallbackMode callbackMode)
		{
			StoreSession session = this.CoreObject.Session;
			object thisObject = null;
			bool flag = false;
			MapiNotificationHandle result;
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
				result = this.MapiStore.Advise(entryId, eventMask, handler, callbackMode, (MapiNotificationClientFlags)0);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotAddNotification, ex, session, thisObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Mailbox::Advise.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotAddNotification, ex2, session, thisObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Mailbox::Advise.", new object[0]),
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
			return result;
		}

		public void Unadvise(object notificationHandle)
		{
			Util.ThrowOnNullArgument(notificationHandle, "notificationHandle");
			StoreSession session = this.CoreObject.Session;
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
				if (!this.MapiStore.IsDisposed && !this.MapiStore.IsDead)
				{
					this.MapiStore.Unadvise((MapiNotificationHandle)notificationHandle);
				}
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotRemoveNotification, ex, session, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Mailbox::Unadvise. NotificationHandle = {0}", notificationHandle),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotRemoveNotification, ex2, session, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Mailbox::Unadvise. NotificationHandle = {0}", notificationHandle),
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
		}

		public bool IsDisposedOrDead
		{
			get
			{
				return this.mailboxStoreObject == null || this.mailboxStoreObject.IsDisposed || this.MapiStore == null || this.MapiStore.IsDisposed || this.MapiStore.IsDead;
			}
		}

		private readonly MailboxStoreObject mailboxStoreObject;
	}
}
