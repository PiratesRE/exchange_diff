using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using Microsoft.Exchange.Data.Directory.Sync;

namespace Microsoft.Exchange.Data.Directory.DirSync
{
	internal static class ADDirSyncHelper
	{
		internal static MultiValuedPropertyBase GetAddedRemovedLinks(ADPropertyDefinition propertyDefinition, SearchResultAttributeCollection attributeCollection, List<ADPropertyDefinition> rangedProperties, List<ValidationError> errors)
		{
			MultiValuedProperty<ADObjectId> links = ADDirSyncHelper.GetLinks(propertyDefinition, false, attributeCollection, rangedProperties, errors);
			MultiValuedProperty<ADObjectId> links2 = ADDirSyncHelper.GetLinks(propertyDefinition, true, attributeCollection, rangedProperties, errors);
			if (links != null || links2 != null)
			{
				List<ADDirSyncLink> list = new List<ADDirSyncLink>();
				ADDirSyncHelper.AddLinks(list, links, LinkState.Added, propertyDefinition);
				ADDirSyncHelper.AddLinks(list, links2, LinkState.Removed, propertyDefinition);
				return ADValueConvertor.CreateGenericMultiValuedProperty(propertyDefinition, true, list, ADDirSyncHelper.EmptyList, null);
			}
			return null;
		}

		internal static void AddLinks(List<ADDirSyncLink> result, MultiValuedProperty<ADObjectId> links, LinkState state, ADPropertyDefinition propertyDefinition)
		{
			if (links != null)
			{
				foreach (ADObjectId link in links)
				{
					result.Add(ADDirSyncHelper.CreateSyncLinks(link, state, propertyDefinition));
				}
			}
		}

		internal static MultiValuedProperty<ADObjectId> GetLinks(ADPropertyDefinition propertyDefinition, bool deleted, SearchResultAttributeCollection attributeCollection, List<ADPropertyDefinition> rangedProperties, List<ValidationError> errors)
		{
			int num = deleted ? 0 : 1;
			IntRange range = new IntRange(num, num);
			ADPropertyDefinition adpropertyDefinition = new ADPropertyDefinition(propertyDefinition.Name, propertyDefinition.VersionAdded, typeof(ADObjectId), propertyDefinition.LdapDisplayName, propertyDefinition.Flags | ADPropertyDefinitionFlags.MultiValued, propertyDefinition.DefaultValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
			adpropertyDefinition = RangedPropertyHelper.CreateRangedProperty(adpropertyDefinition, range);
			DirectoryAttribute directoryAttribute = attributeCollection[adpropertyDefinition.LdapDisplayName];
			if (directoryAttribute != null)
			{
				return ADValueConvertor.GetValueFromDirectoryAttribute(null, directoryAttribute, adpropertyDefinition, true, rangedProperties, null, errors, null, false) as MultiValuedProperty<ADObjectId>;
			}
			return null;
		}

		internal static string GetDirSyncLinkAttribute(string ldapAttribute, bool deleted)
		{
			string text = deleted ? "0" : "1";
			return ADSession.GetAttributeNameWithRange(ldapAttribute, text, text);
		}

		internal static bool IsDirSyncLinkProperty(ADPropertyDefinition propertyDefinition)
		{
			return typeof(ADDirSyncLink).IsAssignableFrom(propertyDefinition.Type);
		}

		internal static ADDirSyncResult CreateAndInitializeDirSyncResult(PropertyBag bag, ADDirSyncResult dummyInstance)
		{
			bag.SetField(ADObjectSchema.ObjectState, ObjectState.Unchanged);
			ADDirSyncResult addirSyncResult = dummyInstance.CreateInstance(bag);
			addirSyncResult.SetIsReadOnly(true);
			addirSyncResult.ResetChangeTracking(true);
			return addirSyncResult;
		}

		internal static bool ContainsProperty(PropertyBag propertyBag, ProviderPropertyDefinition property)
		{
			if (property.IsCalculated)
			{
				foreach (ProviderPropertyDefinition key in property.SupportingProperties)
				{
					if (!propertyBag.Contains(key))
					{
						return false;
					}
				}
				return true;
			}
			return propertyBag.Contains(property);
		}

		internal static IDictionary<ADPropertyDefinition, object> GetChangedProperties(ICollection properties, PropertyBag propertyBag)
		{
			Dictionary<ADPropertyDefinition, object> dictionary = new Dictionary<ADPropertyDefinition, object>(properties.Count);
			foreach (object obj in properties)
			{
				ADPropertyDefinition adpropertyDefinition = (ADPropertyDefinition)obj;
				if (ADDirSyncHelper.ContainsProperty(propertyBag, adpropertyDefinition))
				{
					dictionary[adpropertyDefinition] = propertyBag[adpropertyDefinition];
				}
			}
			return dictionary;
		}

		private static ADDirSyncLink CreateSyncLinks(ADObjectId link, LinkState state, PropertyDefinition propertyDefinition)
		{
			Type type = propertyDefinition.Type;
			if (typeof(SyncLink) == type)
			{
				return new SyncLink(link, state);
			}
			return new ADDirSyncLink(link, state);
		}

		private const int DeletedLinkRangeBound = 0;

		private const int AddedLinkRangeBound = 1;

		internal static readonly IList EmptyList = new object[0];
	}
}
