using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MapiAttachmentProvider : IAttachmentProvider, IDisposable
	{
		internal MapiAttachmentProvider()
		{
		}

		public void SetCollection(CoreAttachmentCollection attachmentCollection)
		{
			this.attachmentCollection = attachmentCollection;
		}

		public NativeStorePropertyDefinition[] AttachmentTablePropertyList
		{
			get
			{
				return this.AttachmentCollection.AttachmentTablePropertyList;
			}
		}

		public void OnAttachmentLoad(AttachmentPropertyBag attachmentBag)
		{
			if (!attachmentBag.UseCreateFlagOnConnect)
			{
				AttachmentId attachmentId = attachmentBag.AttachmentId;
				if (attachmentId != null)
				{
					this.AttachmentCollection.UpdateAttachmentId(attachmentId, attachmentBag.AttachmentNumber);
				}
			}
		}

		public void OnBeforeAttachmentSave(AttachmentPropertyBag attachmentBag)
		{
			this.AttachmentCollection.OnBeforeAttachmentSave(attachmentBag);
		}

		public void OnAfterAttachmentSave(AttachmentPropertyBag attachmentBag)
		{
			this.AttachmentCollection.OnAfterAttachmentSave(attachmentBag.AttachmentNumber);
		}

		public void OnAttachmentDisconnected(AttachmentPropertyBag attachmentBag, PersistablePropertyBag dataPropertyBag)
		{
			if (dataPropertyBag != null)
			{
				dataPropertyBag.Dispose();
			}
		}

		public void OnCollectionDisposed(AttachmentPropertyBag attachmentBag, PersistablePropertyBag dataPropertyBag)
		{
			this.OnAttachmentDisconnected(attachmentBag, dataPropertyBag);
			if (attachmentBag != null)
			{
				attachmentBag.Dispose();
			}
		}

		public PersistablePropertyBag OpenAttachment(ICollection<PropertyDefinition> prefetchProperties, AttachmentPropertyBag attachmentBag)
		{
			ICollection<PropertyDefinition> attachmentLoadList = this.AttachmentCollection.GetAttachmentLoadList(prefetchProperties, attachmentBag.Schema);
			int attachmentNumber = attachmentBag.AttachmentNumber;
			MapiAttach mapiAttach = null;
			StoreObjectPropertyBag storeObjectPropertyBag = null;
			bool flag = false;
			try
			{
				StoreSession session = this.AttachmentCollection.ContainerItem.Session;
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
					mapiAttach = this.AttachmentCollection.ContainerItem.MapiMessage.OpenAttach(attachmentNumber);
				}
				catch (MapiPermanentException ex)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotOpenAttachment, ex, session, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("MapiAttachmentProvider::OpenMapiAttachment({0})", attachmentNumber),
						ex
					});
				}
				catch (MapiRetryableException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotOpenAttachment, ex2, session, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("MapiAttachmentProvider::OpenMapiAttachment({0})", attachmentNumber),
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
				storeObjectPropertyBag = this.CreateStorePropertyBag(mapiAttach, attachmentLoadList);
				byte[] array = ((IDirectPropertyBag)storeObjectPropertyBag).GetValue(InternalSchema.RecordKey) as byte[];
				if (array != null)
				{
					AttachmentId attachmentId = new AttachmentId(array);
					if (!attachmentId.Equals(attachmentBag.AttachmentId))
					{
						throw new StoragePermanentException(ServerStrings.MapiCannotMatchAttachmentIds(attachmentId.ToBase64String(), attachmentBag.AttachmentId.ToBase64String()));
					}
				}
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					if (storeObjectPropertyBag != null)
					{
						storeObjectPropertyBag.Dispose();
						storeObjectPropertyBag = null;
					}
					if (mapiAttach != null)
					{
						mapiAttach.Dispose();
						mapiAttach = null;
					}
				}
			}
			return storeObjectPropertyBag;
		}

		public bool SupportsCreateClone(AttachmentPropertyBag propertyBag)
		{
			return propertyBag != null && propertyBag.MapiProp is MapiAttach;
		}

		public bool ExistsInCollection(AttachmentPropertyBag attachmentBag)
		{
			return this.AttachmentCollection.Exists(attachmentBag.AttachmentNumber);
		}

		public PersistablePropertyBag CreateAttachment(ICollection<PropertyDefinition> propertiesToLoad, CoreAttachment attachmentToClone, IItem itemToAttach, out int attachmentNumber)
		{
			MapiAttach mapiAttach = null;
			PersistablePropertyBag persistablePropertyBag = null;
			bool flag = false;
			int num = 0;
			try
			{
				StoreSession session = this.AttachmentCollection.ContainerItem.Session;
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
					mapiAttach = this.AttachmentCollection.ContainerItem.MapiMessage.CreateAttach(out num);
				}
				catch (MapiPermanentException ex)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotOpenAttachment, ex, session, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("MapiAttachmentProvider::CreateMapiAttachment", new object[0]),
						ex
					});
				}
				catch (MapiRetryableException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotOpenAttachment, ex2, session, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("MapiAttachmentProvider::CreateMapiAttachment", new object[0]),
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
				attachmentNumber = num;
				if (attachmentToClone != null)
				{
					MapiAttachmentProvider.CopySavedMapiAttachment(true, this, this.AttachmentCollection.ContainerItem.Session, (MapiAttach)attachmentToClone.PropertyBag.MapiProp, mapiAttach);
				}
				else if (itemToAttach != null)
				{
					MapiMessage mapiMessage = itemToAttach.MapiMessage;
					MapiMessage mapiMessage2 = null;
					StoreSession session2 = this.AttachmentCollection.ContainerItem.Session;
					bool flag3 = false;
					try
					{
						if (session2 != null)
						{
							session2.BeginMapiCall();
							session2.BeginServerHealthCall();
							flag3 = true;
						}
						if (StorageGlobals.MapiTestHookBeforeCall != null)
						{
							StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
						}
						mapiMessage2 = mapiAttach.OpenEmbeddedMessage(OpenPropertyFlags.Create);
					}
					catch (MapiPermanentException ex3)
					{
						throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotOpenEmbeddedMessage, ex3, session2, this, "{0}. MapiException = {1}.", new object[]
						{
							string.Format("MapiAttachmentProvider::CreateMapiAttachment", new object[0]),
							ex3
						});
					}
					catch (MapiRetryableException ex4)
					{
						throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotOpenEmbeddedMessage, ex4, session2, this, "{0}. MapiException = {1}.", new object[]
						{
							string.Format("MapiAttachmentProvider::CreateMapiAttachment", new object[0]),
							ex4
						});
					}
					finally
					{
						try
						{
							if (session2 != null)
							{
								session2.EndMapiCall();
								if (flag3)
								{
									session2.EndServerHealthCall();
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
					using (mapiMessage2)
					{
						PropProblem[] array = null;
						StoreSession session3 = this.AttachmentCollection.ContainerItem.Session;
						bool flag4 = false;
						try
						{
							if (session3 != null)
							{
								session3.BeginMapiCall();
								session3.BeginServerHealthCall();
								flag4 = true;
							}
							if (StorageGlobals.MapiTestHookBeforeCall != null)
							{
								StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
							}
							array = mapiMessage.CopyTo(mapiMessage2, new PropTag[]
							{
								(PropTag)InternalSchema.UrlCompName.PropertyTag
							});
						}
						catch (MapiPermanentException ex5)
						{
							throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCopyFailedProperties, ex5, session3, this, "{0}. MapiException = {1}.", new object[]
							{
								string.Format("MapiAttachmentProvider::CreateMapiAttachment", new object[0]),
								ex5
							});
						}
						catch (MapiRetryableException ex6)
						{
							throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCopyFailedProperties, ex6, session3, this, "{0}. MapiException = {1}.", new object[]
							{
								string.Format("MapiAttachmentProvider::CreateMapiAttachment", new object[0]),
								ex6
							});
						}
						finally
						{
							try
							{
								if (session3 != null)
								{
									session3.EndMapiCall();
									if (flag4)
									{
										session3.EndServerHealthCall();
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
						if (array != null)
						{
							int num2 = -1;
							for (int i = 0; i < array.Length; i++)
							{
								int scode = array[i].Scode;
								if (scode == -2147221233 || scode == -2147221222)
								{
									ExTraceGlobals.StorageTracer.TraceDebug<int>((long)this.GetHashCode(), "Storage.MapiAttachmentProvider.AddExisting Item: CopyTo returned ignorable scode = {0}", scode);
								}
								else
								{
									ExTraceGlobals.StorageTracer.TraceError<int>((long)this.GetHashCode(), "Storage.MapiAttachmentProvider.AddExisting Item: CopyTo returned fatal scode = {0}", scode);
									num2 = i;
								}
								if (num2 != -1)
								{
									throw PropertyError.ToException(ServerStrings.ExUnableToCopyAttachments, StoreObjectPropertyBag.MapiPropProblemsToPropertyErrors(null, mapiMessage, array));
								}
							}
						}
						StoreSession session4 = this.AttachmentCollection.ContainerItem.Session;
						bool flag5 = false;
						try
						{
							if (session4 != null)
							{
								session4.BeginMapiCall();
								session4.BeginServerHealthCall();
								flag5 = true;
							}
							if (StorageGlobals.MapiTestHookBeforeCall != null)
							{
								StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
							}
							mapiMessage2.SaveChanges();
						}
						catch (MapiPermanentException ex7)
						{
							throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSaveChanges, ex7, session4, this, "{0}. MapiException = {1}.", new object[]
							{
								string.Format("MapiAttachmentProvider::CreateMapiAttachment", new object[0]),
								ex7
							});
						}
						catch (MapiRetryableException ex8)
						{
							throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSaveChanges, ex8, session4, this, "{0}. MapiException = {1}.", new object[]
							{
								string.Format("MapiAttachmentProvider::CreateMapiAttachment", new object[0]),
								ex8
							});
						}
						finally
						{
							try
							{
								if (session4 != null)
								{
									session4.EndMapiCall();
									if (flag5)
									{
										session4.EndServerHealthCall();
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
				}
				persistablePropertyBag = this.CreateStorePropertyBag(mapiAttach, propertiesToLoad);
				persistablePropertyBag.ExTimeZone = this.ExTimeZone;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					if (persistablePropertyBag != null)
					{
						persistablePropertyBag.Dispose();
					}
					if (mapiAttach != null)
					{
						mapiAttach.Dispose();
					}
				}
			}
			return persistablePropertyBag;
		}

		public ICoreItem OpenAttachedItem(ICollection<PropertyDefinition> propertiesToLoad, AttachmentPropertyBag attachmentBag, bool isNew)
		{
			MapiMessage mapiMessage = null;
			PersistablePropertyBag persistablePropertyBag = null;
			CoreItem coreItem = null;
			bool flag = false;
			StoreObjectId storeObjectId = null;
			byte[] array = null;
			ICoreItem result;
			try
			{
				StoreObjectPropertyBag storeObjectPropertyBag = (StoreObjectPropertyBag)attachmentBag.PersistablePropertyBag;
				MapiAttach mapiAttach = (MapiAttach)storeObjectPropertyBag.MapiProp;
				StoreSession session = this.AttachmentCollection.ContainerItem.Session;
				OpenPropertyFlags openPropertyFlags = isNew ? OpenPropertyFlags.Create : (this.AttachmentCollection.IsReadOnly ? OpenPropertyFlags.BestAccess : OpenPropertyFlags.BestAccess);
				openPropertyFlags |= OpenPropertyFlags.DeferredErrors;
				string text = storeObjectPropertyBag.TryGetProperty(InternalSchema.ItemClass) as string;
				Schema schema = (text != null) ? ObjectClass.GetSchema(text) : MessageItemSchema.Instance;
				propertiesToLoad = InternalSchema.Combine<PropertyDefinition>(schema.AutoloadProperties, propertiesToLoad);
				StoreSession session2 = this.AttachmentCollection.ContainerItem.Session;
				bool flag2 = false;
				try
				{
					if (session2 != null)
					{
						session2.BeginMapiCall();
						session2.BeginServerHealthCall();
						flag2 = true;
					}
					if (StorageGlobals.MapiTestHookBeforeCall != null)
					{
						StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
					}
					mapiMessage = mapiAttach.OpenEmbeddedMessage(openPropertyFlags);
				}
				catch (MapiPermanentException ex)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotOpenEmbeddedMessage, ex, session2, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("MapiAttachmentProvider::OpenAttachedItem", new object[0]),
						ex
					});
				}
				catch (MapiRetryableException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotOpenEmbeddedMessage, ex2, session2, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("MapiAttachmentProvider::OpenAttachedItem", new object[0]),
						ex2
					});
				}
				finally
				{
					try
					{
						if (session2 != null)
						{
							session2.EndMapiCall();
							if (flag2)
							{
								session2.EndServerHealthCall();
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
				persistablePropertyBag = new StoreObjectPropertyBag(session, mapiMessage, propertiesToLoad);
				if (!isNew)
				{
					StoreObjectType storeObjectType = ItemBuilder.ReadStoreObjectTypeFromPropertyBag(persistablePropertyBag);
					ItemCreateInfo itemCreateInfo = ItemCreateInfo.GetItemCreateInfo(storeObjectType);
					propertiesToLoad = InternalSchema.Combine<PropertyDefinition>(itemCreateInfo.Schema.AutoloadProperties, propertiesToLoad);
					if (this.AttachmentCollection.IsReadOnly)
					{
						StoreId.SplitStoreObjectIdAndChangeKey(StoreObjectId.DummyId, out storeObjectId, out array);
					}
					persistablePropertyBag = new AcrPropertyBag(persistablePropertyBag, itemCreateInfo.AcrProfile, storeObjectId, new RetryBagFactory(session), array);
				}
				coreItem = new CoreItem(session, persistablePropertyBag, storeObjectId, array, isNew ? Origin.New : Origin.Existing, ItemLevel.Attached, propertiesToLoad, ItemBindOption.None);
				if (text != null && isNew)
				{
					coreItem.PropertyBag[InternalSchema.ItemClass] = text;
				}
				flag = true;
				result = coreItem;
			}
			finally
			{
				if (!flag)
				{
					if (coreItem != null)
					{
						coreItem.Dispose();
						coreItem = null;
					}
					if (persistablePropertyBag != null)
					{
						persistablePropertyBag.Dispose();
						persistablePropertyBag = null;
					}
					if (mapiMessage != null)
					{
						mapiMessage.Dispose();
						mapiMessage = null;
					}
				}
			}
			return result;
		}

		public void DeleteAttachment(int attachmentNumber)
		{
			try
			{
				StoreSession session = this.AttachmentCollection.ContainerItem.Session;
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
					this.AttachmentCollection.ContainerItem.MapiMessage.DeleteAttach(attachmentNumber);
				}
				catch (MapiPermanentException ex)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotDeleteAttachment, ex, session, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("MapiAttachmentProvider::RemoveAttachment.", new object[0]),
						ex
					});
				}
				catch (MapiRetryableException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotDeleteAttachment, ex2, session, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("MapiAttachmentProvider::RemoveAttachment.", new object[0]),
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
			catch (ObjectNotFoundException)
			{
			}
		}

		private StoreObjectPropertyBag CreateStorePropertyBag(MapiAttach mapiAttach, ICollection<PropertyDefinition> prefetchPropertyArray)
		{
			StoreObjectPropertyBag storeObjectPropertyBag = null;
			bool flag = false;
			try
			{
				storeObjectPropertyBag = new StoreObjectPropertyBag(this.AttachmentCollection.ContainerItem.Session, mapiAttach, prefetchPropertyArray);
				storeObjectPropertyBag.PrefetchPropertyArray = prefetchPropertyArray;
				storeObjectPropertyBag.ExTimeZone = this.ExTimeZone;
				flag = true;
			}
			finally
			{
				if (!flag && storeObjectPropertyBag != null)
				{
					storeObjectPropertyBag.Dispose();
					storeObjectPropertyBag = null;
				}
			}
			return storeObjectPropertyBag;
		}

		internal static void CopySavedMapiAttachment(bool failOnProblems, object thisObject, StoreSession session, MapiAttach source, MapiAttach destination)
		{
			PropProblem[] array = null;
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
				array = source.CopyTo(destination, new PropTag[0]);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotOpenAttachment, ex, session, thisObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("MapiAttachmentProvider::CopySavedMapiAttachment", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotOpenAttachment, ex2, session, thisObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("MapiAttachmentProvider::CopySavedMapiAttachment", new object[0]),
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
			if (array != null)
			{
				ExTraceGlobals.StorageTracer.TraceError<int>((long)((thisObject == null) ? 0 : thisObject.GetHashCode()), "MapiAttachmentProvider.CreateNewAttachment: MapiAttach.CopyTo returned scode = {0}", array[0].Scode);
				if (failOnProblems)
				{
					throw PropertyError.ToException(ServerStrings.ExUnableToCopyAttachments, StoreObjectPropertyBag.MapiPropProblemsToPropertyErrors(null, destination, array));
				}
			}
		}

		public PropertyBag[] QueryAttachmentTable(NativeStorePropertyDefinition[] properties)
		{
			PropValue[][] array = null;
			MapiTable mapiTable = null;
			StoreSession session = this.AttachmentCollection.ContainerItem.Session;
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
				mapiTable = this.AttachmentCollection.ContainerItem.MapiMessage.GetAttachmentTable();
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetAttachmentTable, ex, session, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("MapiAttachmentProvider::QueryAttachmentTable", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetAttachmentTable, ex2, session, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("MapiAttachmentProvider::QueryAttachmentTable", new object[0]),
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
			using (mapiTable)
			{
				ICollection<PropTag> propTags = PropertyTagCache.Cache.PropTagsFromPropertyDefinitions(this.AttachmentCollection.ContainerItem.MapiMessage, this.AttachmentCollection.ContainerItem.Session, properties);
				StoreSession session2 = this.AttachmentCollection.ContainerItem.Session;
				bool flag2 = false;
				try
				{
					if (session2 != null)
					{
						session2.BeginMapiCall();
						session2.BeginServerHealthCall();
						flag2 = true;
					}
					if (StorageGlobals.MapiTestHookBeforeCall != null)
					{
						StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
					}
					array = mapiTable.QueryAllRows(null, propTags);
				}
				catch (MapiPermanentException ex3)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetAttachmentTable, ex3, session2, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("MapiAttachmentProvider::QueryAttachmentTable", new object[0]),
						ex3
					});
				}
				catch (MapiRetryableException ex4)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetAttachmentTable, ex4, session2, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("MapiAttachmentProvider::QueryAttachmentTable", new object[0]),
						ex4
					});
				}
				finally
				{
					try
					{
						if (session2 != null)
						{
							session2.EndMapiCall();
							if (flag2)
							{
								session2.EndServerHealthCall();
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
			PropertyBag[] array2 = null;
			if (array != null && array.Length > 0)
			{
				Dictionary<StorePropertyDefinition, int> propertyPositionsDictionary = QueryResultPropertyBag.CreatePropertyPositionsDictionary(properties);
				array2 = new PropertyBag[array.Length];
				for (int num = 0; num != array.Length; num++)
				{
					QueryResultPropertyBag queryResultPropertyBag = new QueryResultPropertyBag(this.AttachmentCollection.ContainerItem.Session, propertyPositionsDictionary);
					queryResultPropertyBag.ExTimeZone = this.ExTimeZone;
					queryResultPropertyBag.ReturnErrorsOnTruncatedProperties = true;
					queryResultPropertyBag.SetQueryResultRow(array[num]);
					array2[num] = queryResultPropertyBag;
				}
			}
			return array2;
		}

		private CoreAttachmentCollection AttachmentCollection
		{
			get
			{
				return this.attachmentCollection;
			}
		}

		private ExTimeZone ExTimeZone
		{
			get
			{
				return this.AttachmentCollection.ExTimeZone;
			}
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}

		private CoreAttachmentCollection attachmentCollection;
	}
}
