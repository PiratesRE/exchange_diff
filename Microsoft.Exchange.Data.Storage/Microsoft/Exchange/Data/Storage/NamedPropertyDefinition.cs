using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal abstract class NamedPropertyDefinition : NativeStorePropertyDefinition
	{
		protected NamedPropertyDefinition(PropertyTypeSpecifier propertyTypeSpecifier, string displayName, Type type, PropType mapiPropertyType, PropertyFlags childFlags, PropertyDefinitionConstraint[] constraints) : base(propertyTypeSpecifier, displayName, type, mapiPropertyType, childFlags, constraints)
		{
		}

		public abstract NamedPropertyDefinition.NamedPropertyKey GetKey();

		[Serializable]
		public abstract class NamedPropertyKey
		{
			internal static void ClearUnreferenced()
			{
				List<NamedProp> list = new List<NamedProp>();
				try
				{
					NamedPropertyDefinition.NamedPropertyKey.lockObject.EnterReadLock();
					foreach (KeyValuePair<NamedProp, WeakReference> keyValuePair in NamedPropertyDefinition.NamedPropertyKey.namedProps)
					{
						if (!keyValuePair.Value.IsAlive)
						{
							list.Add(keyValuePair.Key);
						}
					}
				}
				finally
				{
					try
					{
						NamedPropertyDefinition.NamedPropertyKey.lockObject.ExitReadLock();
					}
					catch (SynchronizationLockException)
					{
					}
				}
				if (list.Count > 0)
				{
					try
					{
						NamedPropertyDefinition.NamedPropertyKey.lockObject.EnterWriteLock();
						foreach (NamedProp key in list)
						{
							NamedPropertyDefinition.NamedPropertyKey.namedProps.Remove(key);
						}
					}
					finally
					{
						try
						{
							NamedPropertyDefinition.NamedPropertyKey.lockObject.ExitWriteLock();
						}
						catch (SynchronizationLockException)
						{
						}
					}
				}
			}

			internal static NamedProp GetSingleton(NamedProp property)
			{
				try
				{
					NamedPropertyDefinition.NamedPropertyKey.lockObject.EnterReadLock();
					WeakReference weakReference;
					if (NamedPropertyDefinition.NamedPropertyKey.namedProps.TryGetValue(property, out weakReference))
					{
						NamedProp namedProp = (NamedProp)weakReference.Target;
						if (namedProp != null)
						{
							return namedProp;
						}
					}
				}
				finally
				{
					try
					{
						NamedPropertyDefinition.NamedPropertyKey.lockObject.ExitReadLock();
					}
					catch (SynchronizationLockException)
					{
					}
				}
				try
				{
					NamedPropertyDefinition.NamedPropertyKey.lockObject.EnterWriteLock();
					if (NamedPropertyDefinition.NamedPropertyKey.namedProps.ContainsKey(property))
					{
						NamedPropertyDefinition.NamedPropertyKey.namedProps[property] = new WeakReference(property);
					}
					else
					{
						NamedPropertyDefinition.NamedPropertyKey.namedProps.Add(new NamedProp(property), new WeakReference(property));
					}
				}
				finally
				{
					try
					{
						NamedPropertyDefinition.NamedPropertyKey.lockObject.ExitWriteLock();
					}
					catch (SynchronizationLockException)
					{
					}
				}
				return property;
			}

			internal static ICollection<NamedProp> AddNamedProps(ICollection<NamedProp> props)
			{
				NamedProp[] array = new NamedProp[props.Count];
				int num = 0;
				IEnumerator<NamedProp> enumerator = props.GetEnumerator();
				try
				{
					NamedPropertyDefinition.NamedPropertyKey.lockObject.EnterReadLock();
					while (enumerator.MoveNext())
					{
						NamedProp namedProp = enumerator.Current;
						if (namedProp == null || namedProp.IsSingleInstanced)
						{
							array[num++] = namedProp;
						}
						else
						{
							WeakReference weakReference;
							if (!NamedPropertyDefinition.NamedPropertyKey.namedProps.TryGetValue(namedProp, out weakReference))
							{
								break;
							}
							NamedProp namedProp2 = (NamedProp)weakReference.Target;
							if (namedProp2 == null)
							{
								break;
							}
							array[num++] = namedProp2;
						}
					}
				}
				finally
				{
					try
					{
						NamedPropertyDefinition.NamedPropertyKey.lockObject.ExitReadLock();
					}
					catch (SynchronizationLockException)
					{
					}
				}
				if (num == props.Count)
				{
					return array;
				}
				try
				{
					NamedPropertyDefinition.NamedPropertyKey.lockObject.EnterWriteLock();
					do
					{
						NamedProp namedProp3 = enumerator.Current;
						WeakReference weakReference;
						if (namedProp3 == null || namedProp3.IsSingleInstanced)
						{
							array[num++] = namedProp3;
						}
						else if (NamedPropertyDefinition.NamedPropertyKey.namedProps.TryGetValue(namedProp3, out weakReference))
						{
							NamedProp namedProp4 = (NamedProp)weakReference.Target;
							if (namedProp4 != null)
							{
								array[num++] = namedProp4;
							}
							else
							{
								NamedPropertyDefinition.NamedPropertyKey.namedProps[namedProp3] = new WeakReference(namedProp3);
								array[num++] = namedProp3;
							}
						}
						else
						{
							NamedPropertyDefinition.NamedPropertyKey.namedProps.Add(new NamedProp(namedProp3), new WeakReference(namedProp3));
							array[num++] = namedProp3;
						}
					}
					while (enumerator.MoveNext());
				}
				finally
				{
					try
					{
						NamedPropertyDefinition.NamedPropertyKey.lockObject.ExitWriteLock();
					}
					catch (SynchronizationLockException)
					{
					}
				}
				return array;
			}

			protected NamedPropertyKey(NamedProp namedProp)
			{
				this.namedProp = namedProp;
			}

			protected NamedPropertyKey(Guid propGuid, string propName)
			{
				StoreSession storeSession = null;
				object thisObject = null;
				bool flag = false;
				try
				{
					if (storeSession != null)
					{
						storeSession.BeginMapiCall();
						storeSession.BeginServerHealthCall();
						flag = true;
					}
					if (StorageGlobals.MapiTestHookBeforeCall != null)
					{
						StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
					}
					this.namedProp = new NamedProp(propGuid, propName);
				}
				catch (MapiPermanentException ex)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiInvalidParam, ex, storeSession, thisObject, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("Unable to create property key", new object[0]),
						ex
					});
				}
				catch (MapiRetryableException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiInvalidParam, ex2, storeSession, thisObject, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("Unable to create property key", new object[0]),
						ex2
					});
				}
				finally
				{
					try
					{
						if (storeSession != null)
						{
							storeSession.EndMapiCall();
							if (flag)
							{
								storeSession.EndServerHealthCall();
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

			protected NamedPropertyKey(Guid propGuid, int propId)
			{
				StoreSession storeSession = null;
				object thisObject = null;
				bool flag = false;
				try
				{
					if (storeSession != null)
					{
						storeSession.BeginMapiCall();
						storeSession.BeginServerHealthCall();
						flag = true;
					}
					if (StorageGlobals.MapiTestHookBeforeCall != null)
					{
						StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
					}
					this.namedProp = new NamedProp(propGuid, propId);
				}
				catch (MapiPermanentException ex)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiInvalidParam, ex, storeSession, thisObject, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("Unable to create property key", new object[0]),
						ex
					});
				}
				catch (MapiRetryableException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiInvalidParam, ex2, storeSession, thisObject, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("Unable to create property key", new object[0]),
						ex2
					});
				}
				finally
				{
					try
					{
						if (storeSession != null)
						{
							storeSession.EndMapiCall();
							if (flag)
							{
								storeSession.EndServerHealthCall();
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

			internal NamedProp NamedProp
			{
				get
				{
					return this.namedProp;
				}
			}

			private readonly NamedProp namedProp;

			private static Dictionary<NamedProp, WeakReference> namedProps = new Dictionary<NamedProp, WeakReference>();

			private static ReaderWriterLockSlim lockObject = new ReaderWriterLockSlim();
		}
	}
}
