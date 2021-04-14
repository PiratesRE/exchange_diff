using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PropertyTagCache
	{
		public NativeStorePropertyDefinition[] GetPropertyDefinitionsIgnoreTypeChecking(StoreSession session, ICorePropertyBag corePropertyBag, uint[] propertyTags)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(corePropertyBag, "corePropertyBag");
			Util.ThrowOnNullArgument(propertyTags, "propertyTags");
			NativeStorePropertyDefinition[] result = null;
			uint num;
			if (!this.TryGetPropertyDefinitionsFromPropertyTags(session, corePropertyBag, propertyTags, NativeStorePropertyDefinition.TypeCheckingFlag.DisableTypeCheck, out result, out num))
			{
				throw new ResolvePropertyDefinitionException(num, ServerStrings.CannotResolvePropertyTagsToPropertyDefinitions(num));
			}
			return result;
		}

		public bool TryGetPropertyDefinitionsFromPropertyTagsWithCompatibleTypes(StoreSession session, ICorePropertyBag corePropertyBag, uint[] propertyTags, out NativeStorePropertyDefinition[] propertyDefinitions)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(corePropertyBag, "corePropertyBag");
			Util.ThrowOnNullArgument(propertyTags, "propertyTags");
			uint num;
			return this.TryGetPropertyDefinitionsFromPropertyTags(session, corePropertyBag, propertyTags, NativeStorePropertyDefinition.TypeCheckingFlag.AllowCompatibleType, out propertyDefinitions, out num);
		}

		public bool TryGetPropertyDefinitionsFromPropertyTags(StoreSession session, ICorePropertyBag corePropertyBag, uint[] propertyTags, out NativeStorePropertyDefinition[] propertyDefinitions)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(corePropertyBag, "corePropertyBag");
			Util.ThrowOnNullArgument(propertyTags, "propertyTags");
			uint num;
			return this.TryGetPropertyDefinitionsFromPropertyTags(session, corePropertyBag, propertyTags, NativeStorePropertyDefinition.TypeCheckingFlag.DoNotCreateInvalidType, out propertyDefinitions, out num);
		}

		public ICollection<uint> PropertyTagsFromPropertyDefinitions(StoreSession session, ICollection<NativeStorePropertyDefinition> propertyDefinitions)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(propertyDefinitions, "propertyDefinitions");
			ICollection<PropTag> first = this.PropTagsFromPropertyDefinitions(session.Mailbox.MapiStore, session, propertyDefinitions);
			return from propTag in first
			select (uint)propTag;
		}

		public void Reset()
		{
			NamedPropConverter.Reset();
		}

		public NativeStorePropertyDefinition[] PropertyDefinitionsFromPropTags(NativeStorePropertyDefinition.TypeCheckingFlag propertyTypeCheckingFlag, MapiProp mapiProp, StoreSession storeSession, params PropTag[] propTags)
		{
			int num;
			return this.InternalPropertyDefinitionsFromPropTags(propertyTypeCheckingFlag, mapiProp, storeSession, propTags, out num);
		}

		internal static void ResolveAndFilterPropertyValues(NativeStorePropertyDefinition.TypeCheckingFlag typeCheckingFlag, StoreSession storeSession, MapiProp mapiProp, ExTimeZone exTimeZone, PropValue[] mapiPropValues, out NativeStorePropertyDefinition[] propertyDefinitions, out PropTag[] mapiPropTags, out object[] propertyValues)
		{
			PropTag[] array = new PropTag[mapiPropValues.Length];
			for (int i = 0; i < mapiPropValues.Length; i++)
			{
				array[i] = mapiPropValues[i].PropTag;
			}
			int num;
			NativeStorePropertyDefinition[] array2 = PropertyTagCache.Cache.InternalPropertyDefinitionsFromPropTags(typeCheckingFlag, mapiProp, storeSession, array, out num);
			propertyDefinitions = new NativeStorePropertyDefinition[num];
			mapiPropTags = new PropTag[num];
			propertyValues = new object[num];
			int num2 = 0;
			for (int j = 0; j < array2.Length; j++)
			{
				if (array2[j] != null)
				{
					object valueFromPropValue = MapiPropertyBag.GetValueFromPropValue(storeSession, exTimeZone, array2[j], mapiPropValues[j]);
					propertyDefinitions[num2] = array2[j];
					mapiPropTags[num2] = PropTagHelper.PropTagFromIdAndType(array[j].Id(), array2[j].MapiPropertyType);
					propertyValues[num2] = valueFromPropValue;
					num2++;
				}
			}
		}

		internal static UnresolvedPropertyDefinition[] UnresolvedPropertyDefinitionsFromPropTags(IList<PropTag> propTags)
		{
			UnresolvedPropertyDefinition[] array = new UnresolvedPropertyDefinition[propTags.Count];
			for (int i = 0; i < propTags.Count; i++)
			{
				array[i] = UnresolvedPropertyDefinition.Create(propTags[i]);
			}
			return array;
		}

		internal PropTag PropTagFromPropertyDefinition(MapiProp mapiProp, StoreSession storeSession, NativeStorePropertyDefinition propertyDefinition)
		{
			return this.PropTagFromPropertyDefinition(mapiProp, storeSession, false, true, propertyDefinition);
		}

		internal PropTag PropTagFromPropertyDefinition(MapiProp mapiProp, StoreSession storeSession, bool allowUnresolvedHeaders, bool allowCreate, NativeStorePropertyDefinition propertyDefinition)
		{
			PropertyTagPropertyDefinition propertyTagPropertyDefinition = propertyDefinition as PropertyTagPropertyDefinition;
			if (propertyTagPropertyDefinition != null)
			{
				return (PropTag)propertyTagPropertyDefinition.PropertyTag;
			}
			ICollection<PropTag> source = this.PropTagsFromPropertyDefinitions(mapiProp, storeSession, allowUnresolvedHeaders, allowCreate, true, new NativeStorePropertyDefinition[]
			{
				propertyDefinition
			});
			return source.First<PropTag>();
		}

		internal ICollection<PropTag> PropTagsFromPropertyDefinitions(MapiProp mapiProp, StoreSession storeSession, IEnumerable<NativeStorePropertyDefinition> propertyDefinitions)
		{
			return this.PropTagsFromPropertyDefinitions(mapiProp, storeSession, false, propertyDefinitions);
		}

		internal ICollection<PropTag> PropTagsFromPropertyDefinitions<T>(MapiProp mapiProp, StoreSession storeSession, bool allowUnresolvedHeaders, IEnumerable<T> propertyDefinitions) where T : PropertyDefinition
		{
			return this.PropTagsFromPropertyDefinitions(mapiProp, storeSession, allowUnresolvedHeaders, true, true, propertyDefinitions.Cast<T, NativeStorePropertyDefinition>());
		}

		internal ICollection<PropTag> PropTagsFromPropertyDefinitions<T>(MapiProp mapiProp, StoreSession storeSession, bool allowUnresolvedHeaders, bool allowCreate, bool allowCreateInternetHeaders, IEnumerable<T> propertyDefinitions) where T : PropertyDefinition
		{
			return this.PropTagsFromPropertyDefinitions(mapiProp, storeSession, allowUnresolvedHeaders, allowCreate, allowCreateInternetHeaders, propertyDefinitions.Cast<T, NativeStorePropertyDefinition>());
		}

		internal ICollection<PropTag> PropTagsFromPropertyDefinitions(MapiProp mapiProp, StoreSession storeSession, bool allowUnresolvedHeaders, IEnumerable<NativeStorePropertyDefinition> propertyDefinitions)
		{
			return this.PropTagsFromPropertyDefinitions(mapiProp, storeSession, allowUnresolvedHeaders, true, true, propertyDefinitions);
		}

		internal ICollection<PropTag> PropTagsFromPropertyDefinitions(MapiProp mapiProp, StoreSession storeSession, bool allowUnresolvedHeaders, bool allowCreate, bool allowCreateInternet, IEnumerable<NativeStorePropertyDefinition> propertyDefinitions)
		{
			if (mapiProp == null)
			{
				throw new ArgumentNullException("mapiProp");
			}
			if (storeSession == null)
			{
				throw new ArgumentNullException("storeSession");
			}
			if (propertyDefinitions == null)
			{
				throw new ArgumentNullException("propertyDefinitions");
			}
			return new PropertyDefinitionToPropTagCollection(mapiProp, storeSession, allowUnresolvedHeaders, allowCreate, allowCreateInternet, propertyDefinitions);
		}

		internal NativeStorePropertyDefinition[] SafePropertyDefinitionsFromPropTags(StoreSession session, PropTag[] propTags)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(propTags, "propTags");
			NativeStorePropertyDefinition[] array;
			try
			{
				array = PropertyTagCache.Cache.PropertyDefinitionsFromPropTags(session.IsMoveSource ? NativeStorePropertyDefinition.TypeCheckingFlag.DisableTypeCheck : NativeStorePropertyDefinition.TypeCheckingFlag.ThrowOnInvalidType, session.Mailbox.MapiStore, session, propTags);
			}
			catch (InvalidPropertyTypeException innerException)
			{
				throw new CorruptDataException(ServerStrings.ExCorruptPropertyTag, innerException);
			}
			NativeStorePropertyDefinition[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				if (array2[i] == null)
				{
					throw new CorruptDataException(ServerStrings.ExCorruptPropertyTag);
				}
			}
			return array;
		}

		internal NativeStorePropertyDefinition[] InternalPropertyDefinitionsFromPropTags(NativeStorePropertyDefinition.TypeCheckingFlag propertyTypeCheckingFlag, MapiProp mapiProp, StoreSession storeSession, PropTag[] propTags, out int resolvedPropertyCount)
		{
			EnumValidator.ThrowIfInvalid<NativeStorePropertyDefinition.TypeCheckingFlag>(propertyTypeCheckingFlag, "propertyTypeCheckingFlag");
			resolvedPropertyCount = 0;
			NativeStorePropertyDefinition[] array = new NativeStorePropertyDefinition[propTags.Length];
			List<PropertyTagCache.NamedPropertyToResolve> list = null;
			PropTag[] array2 = null;
			for (int i = 0; i < propTags.Length; i++)
			{
				PropTag propTag = propTags[i];
				if (!PropertyTagCache.TryFixPropTagWithErrorType(storeSession, mapiProp, ref array2, ref propTag))
				{
					ExTraceGlobals.PropertyMappingTracer.TraceError<PropTag>((long)storeSession.GetHashCode(), "Failed to infer the property type for PropertyTag {0:X}", propTag);
				}
				else
				{
					PropertyTagCache.ChangeStringPropTagTypeToUnicode(ref propTag);
					int num = propTag.Id();
					if (num < 32768)
					{
						NativeStorePropertyDefinition nativeStorePropertyDefinition = PropertyTagPropertyDefinition.InternalCreateCustom(string.Empty, propTag, PropertyFlags.None, propertyTypeCheckingFlag);
						array[i] = nativeStorePropertyDefinition;
						if (nativeStorePropertyDefinition != null)
						{
							resolvedPropertyCount++;
						}
					}
					else
					{
						if (list == null)
						{
							list = new List<PropertyTagCache.NamedPropertyToResolve>();
						}
						list.Add(new PropertyTagCache.NamedPropertyToResolve((ushort)num, propTag.ValueType(), i));
					}
				}
			}
			if (list != null)
			{
				NamedProp[] namedPropsFromIds = NamedPropConverter.GetNamedPropsFromIds(storeSession, mapiProp, from namedPropertyToResolve in list
				select namedPropertyToResolve.PropId);
				int num2 = 0;
				foreach (PropertyTagCache.NamedPropertyToResolve namedPropertyToResolve2 in list)
				{
					NativeStorePropertyDefinition propDefByMapiNamedProp = PropertyTagCache.GetPropDefByMapiNamedProp(namedPropsFromIds[num2++], namedPropertyToResolve2.PropType, propertyTypeCheckingFlag);
					array[namedPropertyToResolve2.Index] = propDefByMapiNamedProp;
					if (propDefByMapiNamedProp != null)
					{
						resolvedPropertyCount++;
					}
					else
					{
						ExTraceGlobals.PropertyMappingTracer.TraceDebug<ushort, PropType>((long)storeSession.GetHashCode(), "Failed to resolve a named property from PropertyId {0:X} [{1:X}]", namedPropertyToResolve2.PropId, namedPropertyToResolve2.PropType);
					}
				}
			}
			return array;
		}

		private PropertyTagCache()
		{
		}

		private static NativeStorePropertyDefinition GetPropDefByMapiNamedProp(NamedProp prop, PropType type, NativeStorePropertyDefinition.TypeCheckingFlag propertyTypeCheckingFlag)
		{
			if (prop == null)
			{
				return null;
			}
			switch (prop.Kind)
			{
			case NamedPropKind.Id:
				return GuidIdPropertyDefinition.InternalCreateCustom(string.Empty, type, prop.Guid, prop.Id, PropertyFlags.None, propertyTypeCheckingFlag, new PropertyDefinitionConstraint[0]);
			case NamedPropKind.String:
				if (GuidNamePropertyDefinition.IsValidName(prop.Guid, prop.Name))
				{
					return GuidNamePropertyDefinition.InternalCreate(string.Empty, InternalSchema.ClrTypeFromPropTagType(type), type, prop.Guid, prop.Name, PropertyFlags.None, propertyTypeCheckingFlag, true, PropertyDefinitionConstraint.None);
				}
				return null;
			default:
				throw new ArgumentOutOfRangeException("prop.Kind");
			}
		}

		private static bool TryFixPropTagWithErrorType(StoreSession session, MapiProp mapiProp, ref PropTag[] completePropTagList, ref PropTag propTag)
		{
			if (propTag.ValueType() != PropType.Error && propTag.ValueType() != PropType.Unspecified)
			{
				return true;
			}
			if (completePropTagList == null)
			{
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
					completePropTagList = mapiProp.GetPropList();
				}
				catch (MapiPermanentException ex)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.ExGetPropsFailed, ex, session, thisObject, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("PropertyTagCache.IsGoodMapiPropTag failed.", new object[0]),
						ex
					});
				}
				catch (MapiRetryableException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.ExGetPropsFailed, ex2, session, thisObject, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("PropertyTagCache.IsGoodMapiPropTag failed.", new object[0]),
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
			int num = propTag.Id();
			for (int i = 0; i < completePropTagList.Length; i++)
			{
				if (completePropTagList[i].Id() == num)
				{
					propTag = completePropTagList[i];
					return true;
				}
			}
			return false;
		}

		private static void ChangeStringPropTagTypeToUnicode(ref PropTag propTag)
		{
			PropType propType = propTag.ValueType();
			int propId = propTag.Id();
			if (propType == PropType.AnsiString)
			{
				propTag = PropTagHelper.PropTagFromIdAndType(propId, PropType.String);
				return;
			}
			if (propType == PropType.AnsiStringArray)
			{
				propTag = PropTagHelper.PropTagFromIdAndType(propId, PropType.StringArray);
			}
		}

		private bool TryGetPropertyDefinitionsFromPropertyTags(StoreSession session, ICorePropertyBag corePropertyBag, uint[] propertyTags, NativeStorePropertyDefinition.TypeCheckingFlag typeCheckingFlag, out NativeStorePropertyDefinition[] propertyDefinitions, out uint unresolvablePropTag)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(corePropertyBag, "corePropertyBag");
			Util.ThrowOnNullArgument(propertyTags, "propertyTags");
			EnumValidator.ThrowIfInvalid<NativeStorePropertyDefinition.TypeCheckingFlag>(typeCheckingFlag, PropertyTagCache.validOptionSet);
			unresolvablePropTag = 0U;
			PropTag[] array = new PropTag[propertyTags.Length];
			for (int i = 0; i < propertyTags.Length; i++)
			{
				array[i] = (PropTag)propertyTags[i];
			}
			propertyDefinitions = this.PropertyDefinitionsFromPropTags(typeCheckingFlag, PersistablePropertyBag.GetPersistablePropertyBag(corePropertyBag).MapiProp, session, array);
			for (int j = 0; j < propertyDefinitions.Length; j++)
			{
				if (propertyDefinitions[j] == null)
				{
					unresolvablePropTag = (uint)array[j];
					return false;
				}
			}
			return true;
		}

		internal const PropTag PropTagUnresolved = PropTag.Unresolved;

		public static readonly PropertyTagCache Cache = new PropertyTagCache();

		private static NativeStorePropertyDefinition.TypeCheckingFlag[] validOptionSet = new NativeStorePropertyDefinition.TypeCheckingFlag[]
		{
			NativeStorePropertyDefinition.TypeCheckingFlag.DisableTypeCheck,
			NativeStorePropertyDefinition.TypeCheckingFlag.DoNotCreateInvalidType,
			NativeStorePropertyDefinition.TypeCheckingFlag.AllowCompatibleType
		};

		private struct NamedPropertyToResolve
		{
			public NamedPropertyToResolve(ushort propId, PropType propType, int index)
			{
				this.PropId = propId;
				this.PropType = propType;
				this.Index = index;
			}

			public readonly ushort PropId;

			public readonly PropType PropType;

			public readonly int Index;
		}
	}
}
