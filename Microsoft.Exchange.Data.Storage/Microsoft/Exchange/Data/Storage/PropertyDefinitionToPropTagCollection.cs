using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PropertyDefinitionToPropTagCollection : ICollection<PropTag>, IEnumerable<PropTag>, IEnumerable
	{
		private static HashSet<string> PromotableInternetHeaders
		{
			get
			{
				if (PropertyDefinitionToPropTagCollection.promotableInternetHeaders == null)
				{
					lock (PropertyDefinitionToPropTagCollection.lockObject)
					{
						PropertyDefinitionToPropTagCollection.promotableInternetHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
						foreach (FieldInfo fieldInfo in ReflectionHelper.AggregateTypeHierarchy<FieldInfo>(typeof(InternalSchema), new AggregateType<FieldInfo>(ReflectionHelper.AggregateStaticFields)))
						{
							object value = fieldInfo.GetValue(null);
							GuidNamePropertyDefinition guidNamePropertyDefinition = value as GuidNamePropertyDefinition;
							if (guidNamePropertyDefinition != null && guidNamePropertyDefinition.Guid == WellKnownPropertySet.InternetHeaders)
							{
								PropertyDefinitionToPropTagCollection.promotableInternetHeaders.Add(guidNamePropertyDefinition.PropertyName);
							}
						}
					}
				}
				return PropertyDefinitionToPropTagCollection.promotableInternetHeaders;
			}
		}

		public PropertyDefinitionToPropTagCollection(MapiProp mapiProp, StoreSession storeSession, bool allowUnresolvedHeaders, bool allowCreate, bool allowCreateHeaders, IEnumerable<NativeStorePropertyDefinition> propertyDefinitions)
		{
			this.mapiProp = mapiProp;
			this.storeSession = storeSession;
			this.allowUnresolvedHeaders = allowUnresolvedHeaders;
			this.allowCreate = allowCreate;
			this.allowCreateHeaders = allowCreateHeaders;
			this.properties = propertyDefinitions;
		}

		public int Count
		{
			get
			{
				if (this.count == -1)
				{
					this.count = this.properties.Count<NativeStorePropertyDefinition>();
				}
				return this.count;
			}
		}

		bool ICollection<PropTag>.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		private NamedPropMap NamedPropertyMap
		{
			get
			{
				if (this.namedPropertyMap == null)
				{
					this.namedPropertyMap = NamedPropMapCache.Default.GetMapping(this.storeSession);
					if (this.namedPropertyMap == null)
					{
						this.namedPropertyMap = new NamedPropMap(null);
					}
				}
				return this.namedPropertyMap;
			}
		}

		public void Add(PropTag item)
		{
			throw new NotSupportedException();
		}

		public void Clear()
		{
			throw new NotSupportedException();
		}

		public bool Remove(PropTag item)
		{
			throw new NotSupportedException();
		}

		public bool Contains(PropTag item)
		{
			EnumValidator.ThrowIfInvalid<PropTag>(item, "item");
			foreach (PropTag propTag in this)
			{
				if (propTag == item)
				{
					return true;
				}
			}
			return false;
		}

		public void CopyTo(PropTag[] array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			foreach (PropTag propTag in this)
			{
				if (index >= array.Length)
				{
					throw new ArgumentException("Destination array is too small", "index");
				}
				array[index++] = propTag;
			}
		}

		public PropertyDefinitionToPropTagCollection.Enumerator GetEnumerator()
		{
			return new PropertyDefinitionToPropTagCollection.Enumerator(this, this.properties.GetEnumerator());
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new PropertyDefinitionToPropTagCollection.Enumerator(this, this.properties.GetEnumerator());
		}

		IEnumerator<PropTag> IEnumerable<PropTag>.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private bool ResolveAllNamedProperties(int startFrom)
		{
			if (this.resolved)
			{
				return false;
			}
			this.resolved = true;
			NamedPropMap namedPropMap = this.NamedPropertyMap;
			IEnumerator<NativeStorePropertyDefinition> enumerator = this.properties.GetEnumerator();
			for (int i = 0; i < startFrom - 1; i++)
			{
				enumerator.MoveNext();
			}
			List<NamedProp> list = null;
			List<NamedProp> list2 = null;
			while (enumerator.MoveNext())
			{
				NativeStorePropertyDefinition nativeStorePropertyDefinition = enumerator.Current;
				if (nativeStorePropertyDefinition.SpecifiedWith != PropertyTypeSpecifier.PropertyTag)
				{
					NamedProp namedProp = ((NamedPropertyDefinition)nativeStorePropertyDefinition).GetKey().NamedProp;
					ushort num;
					if (!namedPropMap.TryGetPropIdFromNamedProp(namedProp, out num))
					{
						if (namedProp.Guid == WellKnownPropertySet.InternetHeaders && !PropertyDefinitionToPropTagCollection.PromotableInternetHeaders.Contains(namedProp.Name))
						{
							if (list2 == null)
							{
								list2 = new List<NamedProp>(20);
							}
							list2.Add(namedProp);
						}
						else
						{
							if (list == null)
							{
								list = new List<NamedProp>(20);
							}
							list.Add(namedProp);
						}
					}
				}
			}
			if (this.allowCreate == this.allowCreateHeaders && list2 != null)
			{
				if (list == null)
				{
					list = list2;
				}
				else
				{
					list.AddRange(list2);
				}
				list2 = null;
			}
			this.GetIdsFromNamedPropsWithRetry(this.allowCreate, list);
			this.GetIdsFromNamedPropsWithRetry(this.allowCreateHeaders, list2);
			return true;
		}

		private void GetIdsFromNamedPropsWithRetry(bool allowCreate, List<NamedProp> resolveList)
		{
			if (resolveList != null)
			{
				try
				{
					this.GetIdsFromNamedProps(allowCreate, resolveList);
				}
				catch (QuotaExceededException)
				{
					if (!allowCreate)
					{
						throw;
					}
					this.GetIdsFromNamedProps(false, resolveList);
				}
			}
		}

		private void GetIdsFromNamedProps(bool allowCreate, IList<NamedProp> namedProps)
		{
			PropTag[] array = null;
			StoreSession storeSession = this.storeSession;
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
				array = this.mapiProp.GetIDsFromNames(allowCreate, namedProps);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetIDFromNames, ex, storeSession, thisObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("PropertyDefinitionToPropTagCollection.GetIdsFromNamedProps failed.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetIDFromNames, ex2, storeSession, thisObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("PropertyDefinitionToPropTagCollection.GetIdsFromNamedProps failed.", new object[0]),
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
			ushort[] array2 = new ushort[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				PropTag propTag = array[i];
				array2[i] = ((propTag == PropTag.Unresolved) ? 0 : ((ushort)propTag.Id()));
			}
			NamedPropMap namedPropMap = this.NamedPropertyMap;
			namedPropMap.AddMapping(allowCreate, array2, namedProps);
		}

		private const PropTag PropTagUnresolved = PropTag.Unresolved;

		private static object lockObject = new object();

		private static HashSet<string> promotableInternetHeaders = null;

		private IEnumerable<NativeStorePropertyDefinition> properties;

		private MapiProp mapiProp;

		private StoreSession storeSession;

		private bool allowUnresolvedHeaders;

		private bool allowCreateHeaders;

		private bool allowCreate;

		private int count = -1;

		private NamedPropMap namedPropertyMap;

		private bool resolved;

		public struct Enumerator : IEnumerator<PropTag>, IDisposable, IEnumerator
		{
			public Enumerator(PropertyDefinitionToPropTagCollection parent, IEnumerator<NativeStorePropertyDefinition> propertyDefinitions)
			{
				this.parent = parent;
				this.definitions = propertyDefinitions;
				this.currentIndex = 0;
			}

			public PropTag Current
			{
				get
				{
					if (this.currentIndex == 0)
					{
						throw new InvalidOperationException();
					}
					NativeStorePropertyDefinition nativeStorePropertyDefinition = this.definitions.Current;
					return (PropTag)((nativeStorePropertyDefinition.SpecifiedWith == PropertyTypeSpecifier.PropertyTag) ? ((PropertyTagPropertyDefinition)nativeStorePropertyDefinition).PropertyTag : ((uint)this.GetPropertyTagFromNamedProperty((NamedPropertyDefinition)nativeStorePropertyDefinition)));
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public bool MoveNext()
			{
				if (!this.definitions.MoveNext())
				{
					return false;
				}
				if (this.definitions.Current != null)
				{
					this.currentIndex++;
					return true;
				}
				throw new NullReferenceException(string.Format("Null NativeStorePropertyDefinition in list.  Current Index: {0}", this.currentIndex));
			}

			public void Dispose()
			{
				this.definitions.Dispose();
			}

			void IEnumerator.Reset()
			{
				this.definitions.Reset();
				this.currentIndex = 0;
			}

			private PropTag GetPropertyTagFromNamedProperty(NamedPropertyDefinition propertyDefinition)
			{
				NamedProp namedProp = propertyDefinition.GetKey().NamedProp;
				NamedPropMap namedPropertyMap = this.parent.NamedPropertyMap;
				ushort num;
				if (!namedPropertyMap.TryGetPropIdFromNamedProp(namedProp, out num) && (!this.parent.ResolveAllNamedProperties(this.currentIndex) || !namedPropertyMap.TryGetPropIdFromNamedProp(namedProp, out num)))
				{
					num = 0;
				}
				if (num != 0)
				{
					return PropTagHelper.PropTagFromIdAndType((int)num, propertyDefinition.MapiPropertyType);
				}
				if (!this.parent.storeSession.Capabilities.IsReadOnly && (!this.parent.allowUnresolvedHeaders || namedProp.Guid != WellKnownPropertySet.InternetHeaders || (namedProp.Guid == WellKnownPropertySet.InternetHeaders && PropertyDefinitionToPropTagCollection.PromotableInternetHeaders.Contains(namedProp.Name))))
				{
					LocalizedString localizedString = ServerStrings.ExInvalidNamedProperty(propertyDefinition.ToString());
					ExTraceGlobals.StorageTracer.TraceError(0L, localizedString);
					throw new StoragePermanentException(localizedString);
				}
				return PropTag.Unresolved;
			}

			private IEnumerator<NativeStorePropertyDefinition> definitions;

			private PropertyDefinitionToPropTagCollection parent;

			private int currentIndex;
		}
	}
}
