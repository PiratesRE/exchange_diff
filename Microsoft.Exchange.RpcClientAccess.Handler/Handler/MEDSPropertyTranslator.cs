using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	internal static class MEDSPropertyTranslator
	{
		internal static PropertyValue[] TranslatePropertyValues(StoreSession session, ICollection<PropertyTag> propertyTags, IList<object> xsoPropertyValues, bool useUnicodeForRestrictions)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(propertyTags, "propertyTags");
			Util.ThrowOnNullArgument(xsoPropertyValues, "xsoPropertyValues");
			if (propertyTags.Count != xsoPropertyValues.Count)
			{
				throw new ArgumentException(string.Format("propertyTags.Count = {0}, xsoPropertyValues.Count = {1}.", propertyTags.Count, xsoPropertyValues.Count));
			}
			PropertyValue[] array = new PropertyValue[xsoPropertyValues.Count];
			PropertyTag[] array2 = propertyTags as PropertyTag[];
			if (array2 != null)
			{
				for (int i = 0; i < array2.Length; i++)
				{
					array[i] = MEDSPropertyTranslator.TranslatePropertyValue(session, array2[i], xsoPropertyValues[i], useUnicodeForRestrictions);
				}
			}
			else
			{
				int num = 0;
				foreach (PropertyTag propertyTag in propertyTags)
				{
					array[num] = MEDSPropertyTranslator.TranslatePropertyValue(session, propertyTag, xsoPropertyValues[num], useUnicodeForRestrictions);
					num++;
				}
			}
			return array;
		}

		internal static object TranslatePropertyValue(StoreSession session, PropertyValue propertyValue)
		{
			EnumValidator.ThrowIfInvalid<PropertyType>(propertyValue.PropertyTag.PropertyType);
			PropertyType propertyType = propertyValue.PropertyTag.PropertyType;
			if (propertyType == PropertyType.Unspecified || propertyType == PropertyType.Error)
			{
				throw new RopExecutionException(string.Format("Cannot set property of type Error or Unspecified: {0}", propertyValue), (ErrorCode)2147942487U);
			}
			switch (propertyType)
			{
			case PropertyType.Restriction:
			{
				FilterRestrictionTranslator filterRestrictionTranslator = new FilterRestrictionTranslator(session);
				return filterRestrictionTranslator.Translate(propertyValue.GetValueAssert<Restriction>());
			}
			case PropertyType.Actions:
			{
				RuleAction[] valueAssert = propertyValue.GetValueAssert<RuleAction[]>();
				RuleAction[] array = new RuleAction[valueAssert.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = RuleActionTranslator.Translate(session, valueAssert[i]);
				}
				return array;
			}
			default:
				return propertyValue.Value;
			}
		}

		internal static NativeStorePropertyDefinition[] GetPropertyDefinitionsIgnoreTypeChecking(StoreSession session, ICorePropertyBag corePropertyBag, PropertyTag[] propertyTags)
		{
			uint[] array = new uint[propertyTags.Length];
			for (int i = 0; i < propertyTags.Length; i++)
			{
				array[i] = (propertyTags[i] & 4294959103U);
			}
			NativeStorePropertyDefinition[] propertyDefinitionsIgnoreTypeChecking;
			try
			{
				propertyDefinitionsIgnoreTypeChecking = PropertyTagCache.Cache.GetPropertyDefinitionsIgnoreTypeChecking(session, corePropertyBag, array);
			}
			catch (ResolvePropertyDefinitionException innerException)
			{
				throw new RopExecutionException("Unable to resolve a PropertyTag.", (ErrorCode)2147942487U, innerException);
			}
			return propertyDefinitionsIgnoreTypeChecking;
		}

		internal static bool TryGetPropertyDefinitionsFromPropertyTags(StoreSession session, ICorePropertyBag corePropertyBag, PropertyTag[] propertyTags, out NativeStorePropertyDefinition[] propertyDefinitions)
		{
			return MEDSPropertyTranslator.TryGetPropertyDefinitionsFromPropertyTags(session, corePropertyBag, propertyTags, false, out propertyDefinitions);
		}

		internal static bool TryGetPropertyDefinitionsFromPropertyTags(StoreSession session, ICorePropertyBag corePropertyBag, PropertyTag[] propertyTags, bool supportsCompatibleType, out NativeStorePropertyDefinition[] propertyDefinitions)
		{
			uint[] array = new uint[propertyTags.Length];
			for (int i = 0; i < propertyTags.Length; i++)
			{
				array[i] = (propertyTags[i] & 4294959103U);
			}
			if (!supportsCompatibleType)
			{
				return PropertyTagCache.Cache.TryGetPropertyDefinitionsFromPropertyTags(session, corePropertyBag, array, out propertyDefinitions);
			}
			return PropertyTagCache.Cache.TryGetPropertyDefinitionsFromPropertyTagsWithCompatibleTypes(session, corePropertyBag, array, out propertyDefinitions);
		}

		internal static ICollection<PropertyTag> PropertyTagsFromPropertyDefinitions<T>(StoreSession propertyMappingReference, ICollection<T> properties, bool useUnicodeType) where T : PropertyDefinition
		{
			return MEDSPropertyTranslator.PropertyTagsFromNativePropertyDefinitions(propertyMappingReference, properties.Select(delegate(T propertyDefinition)
			{
				if (propertyDefinition == null)
				{
					throw new NullReferenceException("propertyDefinition");
				}
				NativeStorePropertyDefinition nativeStorePropertyDefinition = propertyDefinition as NativeStorePropertyDefinition;
				if (nativeStorePropertyDefinition == null)
				{
					throw new InvalidCastException(string.Format("Can't convert {0} to {1}. Property must be of type {1} to use this method.  Is actually type {2}.", propertyDefinition, typeof(NativeStorePropertyDefinition), propertyDefinition.GetType()));
				}
				return nativeStorePropertyDefinition;
			}), useUnicodeType);
		}

		internal static PropertyTag[] PropertyTagsFromUnresolvedPropertyDefinitions(ICollection<UnresolvedPropertyDefinition> propertyDefinitions, PropertyConverter propertyConverter)
		{
			PropertyTag[] array = new PropertyTag[propertyDefinitions.Count];
			int num = 0;
			foreach (UnresolvedPropertyDefinition unresolvedPropertyDefinition in propertyDefinitions)
			{
				PropertyTag propertyTag = new PropertyTag(unresolvedPropertyDefinition.PropertyTag);
				array[num++] = propertyConverter.ConvertPropertyTagToClient(propertyTag);
			}
			return array;
		}

		internal static NativeStorePropertyDefinition PropertyDefinitionFromPropertyTag(ICoreObject propertyMappingReference, PropertyTag propertyTag)
		{
			NativeStorePropertyDefinition[] array;
			if (!MEDSPropertyTranslator.TryGetPropertyDefinitionsFromPropertyTags(propertyMappingReference.Session, propertyMappingReference.PropertyBag, new PropertyTag[]
			{
				propertyTag
			}, out array))
			{
				throw new RopExecutionException(string.Format("Property {0} cannot be resolved", propertyTag), (ErrorCode)2147746050U);
			}
			return array[0];
		}

		internal static ErrorCode PropertyErrorCodeToErrorCode(PropertyErrorCode propertyErrorCode)
		{
			switch (propertyErrorCode)
			{
			case PropertyErrorCode.NotFound:
				return (ErrorCode)2147746063U;
			case PropertyErrorCode.NotEnoughMemory:
			case PropertyErrorCode.RequireStreamed:
			case PropertyErrorCode.PropertyValueTruncated:
				return (ErrorCode)2147942414U;
			case PropertyErrorCode.NullValue:
			case PropertyErrorCode.ConstraintViolation:
				return (ErrorCode)2147746561U;
			case PropertyErrorCode.IncorrectValueType:
				return (ErrorCode)2147746564U;
			case PropertyErrorCode.SetCalculatedPropertyError:
			case PropertyErrorCode.SetStoreComputedPropertyError:
			case PropertyErrorCode.GetCalculatedPropertyError:
				return (ErrorCode)2147746074U;
			case PropertyErrorCode.NotSupported:
				return (ErrorCode)2147746050U;
			case PropertyErrorCode.CorruptedData:
				return (ErrorCode)2147746075U;
			case PropertyErrorCode.FolderNameConflict:
				return (ErrorCode)2147747332U;
			case PropertyErrorCode.FolderHasChanged:
				return (ErrorCode)2147746057U;
			case PropertyErrorCode.AccessDenied:
				return (ErrorCode)2147942405U;
			case PropertyErrorCode.PropertyNotPromoted:
				return ErrorCode.PropertyNotPromoted;
			}
			return (ErrorCode)2147500037U;
		}

		internal static PropertyProblem[] ToPropertyProblems(StoreSession propertyMappingReference, PropertyError[] propertyErrors, PropertyConverter propertyConverter)
		{
			Util.ThrowOnNullArgument(propertyErrors, "propertyErrors");
			Util.ThrowOnNullArgument(propertyMappingReference, "propertyMappingReference");
			bool useUnicodeType = true;
			if (propertyErrors.Length == 0)
			{
				return Array<PropertyProblem>.Empty;
			}
			PropertyProblem[] array = new PropertyProblem[propertyErrors.Length];
			ICollection<PropertyTag> collection = MEDSPropertyTranslator.PropertyTagsFromPropertyDefinitions<PropertyDefinition>(propertyMappingReference, (from var in propertyErrors
			select var.PropertyDefinition).ToList<PropertyDefinition>(), useUnicodeType);
			if (propertyErrors.Length > 65535)
			{
				throw new RopExecutionException("Too many property errors found.", (ErrorCode)2147942414U);
			}
			ushort num = 0;
			foreach (PropertyTag propertyTag in collection)
			{
				PropertyTag propertyTag2 = propertyConverter.ConvertPropertyTagToClient(propertyTag);
				array[(int)num] = new PropertyProblem(num, propertyTag2, MEDSPropertyTranslator.PropertyErrorCodeToErrorCode(propertyErrors[(int)num].PropertyErrorCode));
				num += 1;
			}
			return array;
		}

		internal static PropertyOpenMode OpenModeToPropertyOpenMode(OpenMode openMode, ErrorCode errorCode)
		{
			switch (openMode)
			{
			case OpenMode.ReadOnly:
				return PropertyOpenMode.ReadOnly;
			case OpenMode.ReadWrite:
				return PropertyOpenMode.Modify;
			case OpenMode.Create:
				return PropertyOpenMode.Create;
			}
			throw new RopExecutionException("OpenMode could not be mapped to PropertyOpenMode.", errorCode);
		}

		internal static NamedProperty ToNamedProperty(this NamedPropertyDefinition.NamedPropertyKey namedPropertyKey)
		{
			GuidNamePropertyDefinition.GuidNameKey guidNameKey = namedPropertyKey as GuidNamePropertyDefinition.GuidNameKey;
			if (guidNameKey != null)
			{
				return new NamedProperty(guidNameKey.PropertyGuid, guidNameKey.PropertyName);
			}
			GuidIdPropertyDefinition.GuidIdKey guidIdKey = namedPropertyKey as GuidIdPropertyDefinition.GuidIdKey;
			if (guidIdKey != null)
			{
				return new NamedProperty(guidIdKey.PropertyGuid, (uint)guidIdKey.PropertyId);
			}
			return null;
		}

		internal static NamedPropertyDefinition.NamedPropertyKey ToNamedPropertyKey(this NamedProperty namedProperty)
		{
			if (!namedProperty.IsMapiNamespace)
			{
				if (namedProperty.Kind == NamedPropertyKind.String)
				{
					return new GuidNamePropertyDefinition.GuidNameKey(namedProperty.Guid, namedProperty.Name);
				}
				if (namedProperty.Kind == NamedPropertyKind.Id)
				{
					return new GuidIdPropertyDefinition.GuidIdKey(namedProperty.Guid, (int)namedProperty.Id);
				}
			}
			return null;
		}

		private static PropertyValue XsoErrorCodeToMapiError(PropertyTag oldPropTag, PropertyErrorCode xsoErrorCode)
		{
			return PropertyValue.Error(oldPropTag.PropertyId, MEDSPropertyTranslator.PropertyErrorCodeToErrorCode(xsoErrorCode));
		}

		private static ICollection<PropertyTag> PropertyTagsFromNativePropertyDefinitions(StoreSession propertyMappingReference, ICollection<NativeStorePropertyDefinition> nativeProperties, bool useUnicodeType)
		{
			ICollection<uint> first = PropertyTagCache.Cache.PropertyTagsFromPropertyDefinitions(propertyMappingReference, nativeProperties);
			Func<uint, PropertyTag> selector;
			if (useUnicodeType)
			{
				selector = ((uint propertyTag) => new PropertyTag(propertyTag));
			}
			else
			{
				selector = delegate(uint propertyTag)
				{
					PropertyTag result = new PropertyTag(propertyTag);
					if (result.PropertyType == PropertyType.Unicode)
					{
						result = new PropertyTag(result.PropertyId, PropertyType.String8);
					}
					else if (result.PropertyType == PropertyType.MultiValueUnicode)
					{
						result = new PropertyTag(result.PropertyId, PropertyType.MultiValueString8);
					}
					return result;
				};
			}
			return first.Select(selector);
		}

		internal static PropertyValue TranslatePropertyValue(StoreSession session, PropertyTag propertyTag, object xsoPropertyValue, bool useUnicodeForRestrictions)
		{
			PropertyError propertyError = xsoPropertyValue as PropertyError;
			if (propertyError != null)
			{
				return MEDSPropertyTranslator.XsoErrorCodeToMapiError(propertyTag, propertyError.PropertyErrorCode);
			}
			if (propertyTag.PropertyType == PropertyType.Restriction)
			{
				QueryFilter queryFilter = xsoPropertyValue as QueryFilter;
				if (queryFilter != null)
				{
					FilterRestrictionTranslator filterRestrictionTranslator = new FilterRestrictionTranslator(session);
					Restriction value = filterRestrictionTranslator.Translate(queryFilter, useUnicodeForRestrictions);
					return new PropertyValue(propertyTag, value);
				}
				throw new ArgumentException(string.Format("Value is of the incorrect type. Expected MEDS.QueryFilter. Found = {0}", xsoPropertyValue), "xsoPropertyValue");
			}
			else
			{
				if (propertyTag.PropertyType != PropertyType.Actions)
				{
					if (propertyTag.IsMultiValueInstanceProperty)
					{
						propertyTag = new PropertyTag(propertyTag.PropertyId, propertyTag.ElementPropertyType);
					}
					return MEDSPropertyTranslator.CreateAndFixPropertyValue(propertyTag, xsoPropertyValue);
				}
				RuleAction[] array = xsoPropertyValue as RuleAction[];
				if (array != null)
				{
					RuleAction[] array2 = new RuleAction[array.Length];
					for (int i = 0; i < array2.Length; i++)
					{
						array2[i] = RuleActionTranslator.Translate(session, array[i]);
					}
					return new PropertyValue(propertyTag, array2);
				}
				throw new ArgumentException(string.Format("Value is of the incorrect type. Expected MEDS.RuleAction[]. Found = {0}", xsoPropertyValue), "xsoPropertyValue");
			}
		}

		private static PropertyValue CreateAndFixPropertyValue(PropertyTag propertyTag, object value)
		{
			if (propertyTag.PropertyType == PropertyType.Int32 && value is short)
			{
				return new PropertyValue(propertyTag, Convert.ToInt32(value));
			}
			if (propertyTag.PropertyType == PropertyType.Int16 && value is int)
			{
				short num;
				try
				{
					num = Convert.ToInt16(value);
				}
				catch (OverflowException)
				{
					return MEDSPropertyTranslator.XsoErrorCodeToMapiError(propertyTag, PropertyErrorCode.IncorrectValueType);
				}
				return new PropertyValue(propertyTag, num);
			}
			return new PropertyValue(propertyTag, value);
		}
	}
}
