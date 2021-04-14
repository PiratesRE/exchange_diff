using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Security.AccessControl;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	public abstract class ADObject : ADRawEntry, IVersionable, IADObject, IADRawEntry, IConfigurable, IPropertyBag, IReadOnlyPropertyBag
	{
		internal override object this[PropertyDefinition propertyDefinition]
		{
			get
			{
				object result;
				try
				{
					result = base[propertyDefinition];
				}
				catch (ArgumentException)
				{
					ExWatson.AddExtraData(string.Format("\r\nthis.Identity = {0}\r\nthis.ObjectState = {1}\r\nthis.IsReadOnly = {2}\r\nthis.session = {3}\r\nthis.session.ReadOnly = {4}\r\nthis.propertyBag.Keys.Count = {5}\r\nthis.ObjectSchema.AllProperties.Count = {6}\r\n", new object[]
					{
						this.Identity,
						base.ObjectState,
						base.IsReadOnly,
						(this.m_Session == null) ? "<null>" : this.m_Session.ToString(),
						(this.m_Session == null) ? "<null>" : this.m_Session.ReadOnly.ToString(),
						this.propertyBag.Keys.Count,
						this.Schema.AllProperties.Count
					}));
					ExWatson.AddExtraData("this.propertyBag.Keys = " + ADObject.CollectionToString(this.propertyBag.Keys));
					ExWatson.AddExtraData("this.Schema.AllProperties = " + ADObject.CollectionToString(this.Schema.AllProperties));
					throw;
				}
				return result;
			}
			set
			{
				base[propertyDefinition] = value;
			}
		}

		internal virtual bool IsShareable
		{
			get
			{
				return false;
			}
		}

		private static string CollectionToString(ICollection collection)
		{
			List<PropertyDefinition> list = new List<PropertyDefinition>(collection.Cast<PropertyDefinition>());
			List<string> list2 = new List<string>(list.Count);
			foreach (PropertyDefinition propertyDefinition in list)
			{
				string item = string.Empty;
				if (propertyDefinition != null)
				{
					item = propertyDefinition.ToString();
				}
				list2.Add(item);
			}
			list2.Sort(StringComparer.OrdinalIgnoreCase);
			return string.Join(", ", list2.ToArray());
		}

		internal static object PropertyValueFromEqualityFilter(SinglePropertyFilter filter)
		{
			return ADObject.PropertyValueFromComparisonFilter(filter, new List<ComparisonOperator>
			{
				ComparisonOperator.Equal
			});
		}

		internal static object PropertyValueFromComparisonFilter(SinglePropertyFilter filter, List<ComparisonOperator> allowedOperators)
		{
			string name = filter.Property.Name;
			ComparisonFilter comparisonFilter = filter as ComparisonFilter;
			if (comparisonFilter == null)
			{
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedFilterForProperty(name, filter.GetType(), typeof(ComparisonFilter)));
			}
			if (allowedOperators != null && !allowedOperators.Contains(comparisonFilter.ComparisonOperator))
			{
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedOperatorForProperty(name, comparisonFilter.ComparisonOperator.ToString()));
			}
			object propertyValue = comparisonFilter.PropertyValue;
			if (propertyValue == null)
			{
				return ((ADPropertyDefinition)filter.Property).DefaultValue;
			}
			if (!ADObject.CorrectType(filter.Property, propertyValue.GetType()))
			{
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedPropertyValueType(name, propertyValue.GetType(), filter.Property.Type));
			}
			return propertyValue;
		}

		internal static SinglePropertyFilter DummyCustomFilterBuilderDelegate(SinglePropertyFilter filter)
		{
			return filter;
		}

		private static bool CorrectType(PropertyDefinition propertyDefinition, Type valueType)
		{
			if (valueType == propertyDefinition.Type)
			{
				return true;
			}
			if (propertyDefinition.Type.GetTypeInfo().IsGenericType && propertyDefinition.Type.GetTypeInfo().GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				Type[] genericTypeArguments = propertyDefinition.Type.GetTypeInfo().GenericTypeArguments;
				return genericTypeArguments[0] == valueType;
			}
			return false;
		}

		internal static ComparisonFilter ObjectCategoryFilter(string objectCategory)
		{
			return new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, objectCategory);
		}

		internal static ComparisonFilter ObjectClassFilter(string objectClass, bool isAtomic)
		{
			return new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectClass, objectClass)
			{
				IsAtomic = isAtomic
			};
		}

		internal static ComparisonFilter ObjectClassFilter(string objectClass)
		{
			return new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectClass, objectClass);
		}

		internal static QueryFilter BoolFilterBuilder(SinglePropertyFilter filter, QueryFilter trueFilter)
		{
			if (!(filter is ComparisonFilter))
			{
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedFilterForProperty(filter.Property.Name, filter.GetType(), typeof(ComparisonFilter)));
			}
			ComparisonFilter comparisonFilter = (ComparisonFilter)filter;
			if (comparisonFilter.ComparisonOperator != ComparisonOperator.Equal && ComparisonOperator.NotEqual != comparisonFilter.ComparisonOperator)
			{
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedOperatorForProperty(comparisonFilter.Property.Name, comparisonFilter.ComparisonOperator.ToString()));
			}
			if ((comparisonFilter.ComparisonOperator == ComparisonOperator.Equal && (bool)comparisonFilter.PropertyValue) || (ComparisonOperator.NotEqual == comparisonFilter.ComparisonOperator && !(bool)comparisonFilter.PropertyValue))
			{
				return trueFilter;
			}
			return new NotFilter(trueFilter);
		}

		internal virtual void Initialize()
		{
		}

		internal override void SetIsReadOnly(bool valueToSet)
		{
			if (base.IsReadOnly && !valueToSet)
			{
				this.m_Session = null;
			}
			base.SetIsReadOnly(valueToSet);
		}

		internal bool IsApplicableToTenant()
		{
			ObjectScopeAttribute objectScopeAttribute;
			return this.IsApplicableToTenant(out objectScopeAttribute);
		}

		internal bool IsApplicableToTenant(out ObjectScopeAttribute objectScope)
		{
			objectScope = base.GetType().GetTypeInfo().GetCustomAttribute<ObjectScopeAttribute>();
			return objectScope != null && objectScope.IsTenant;
		}

		internal bool IsGlobal()
		{
			ObjectScopeAttribute customAttribute = base.GetType().GetTypeInfo().GetCustomAttribute<ObjectScopeAttribute>();
			return customAttribute != null && customAttribute.ConfigScope == ConfigScopes.Global;
		}

		public ADObject()
		{
		}

		internal abstract ADObjectSchema Schema { get; }

		internal sealed override ObjectSchema ObjectSchema
		{
			get
			{
				return this.Schema;
			}
		}

		internal abstract string MostDerivedObjectClass { get; }

		internal virtual string ObjectCategoryName
		{
			get
			{
				return this.MostDerivedObjectClass;
			}
		}

		internal virtual QueryFilter ImplicitFilter
		{
			get
			{
				return new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, this.MostDerivedObjectClass);
			}
		}

		internal virtual QueryFilter VersioningFilter
		{
			get
			{
				return new NotFilter(new AndFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ADObjectSchema.ExchangeVersion, this.MaximumSupportedExchangeObjectVersion.NextMajorVersion),
					new ExistsFilter(ADObjectSchema.ExchangeVersion)
				}));
			}
		}

		ObjectSchema IVersionable.ObjectSchema
		{
			get
			{
				return this.Schema;
			}
		}

		public new ExchangeObjectVersion ExchangeVersion
		{
			get
			{
				return base.ExchangeVersion;
			}
		}

		internal static object WhenCreatedGetter(IPropertyBag propertyBag)
		{
			string text = (string)propertyBag[ADObjectSchema.WhenCreatedRaw];
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			return new DateTime?(DateTime.ParseExact(text, "yyyyMMddHHmmss'.0Z'", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal));
		}

		internal static object WhenCreatedUTCGetter(IPropertyBag propertyBag)
		{
			string text = (string)propertyBag[ADObjectSchema.WhenCreatedRaw];
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			return new DateTime?(DateTime.ParseExact(text, "yyyyMMddHHmmss'.0Z'", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal));
		}

		internal static object WhenChangedGetter(IPropertyBag propertyBag)
		{
			string text = (string)propertyBag[ADObjectSchema.WhenChangedRaw];
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			return new DateTime?(DateTime.ParseExact(text, "yyyyMMddHHmmss'.0Z'", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal));
		}

		internal static object WhenChangedUTCGetter(IPropertyBag propertyBag)
		{
			string text = (string)propertyBag[ADObjectSchema.WhenChangedRaw];
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			return new DateTime?(DateTime.ParseExact(text, "yyyyMMddHHmmss'.0Z'", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal));
		}

		internal static object CanonicalNameGetter(IPropertyBag propertyBag)
		{
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[ADObjectSchema.RawCanonicalName];
			if (multiValuedProperty != null && multiValuedProperty.Count > 0)
			{
				return multiValuedProperty[0].TrimEnd(new char[]
				{
					'/'
				});
			}
			return string.Empty;
		}

		internal static object DistinguishedNameGetter(IPropertyBag propertyBag)
		{
			ADObjectId adobjectId = (ADObjectId)propertyBag[ADObjectSchema.Id];
			if (adobjectId != null)
			{
				return adobjectId.DistinguishedName;
			}
			return string.Empty;
		}

		internal static void DistinguishedNameSetter(object value, IPropertyBag propertyBag)
		{
			ADObjectId adobjectId = (ADObjectId)propertyBag[ADObjectSchema.Id];
			propertyBag[ADObjectSchema.Id] = new ADObjectId((string)value, (adobjectId == null) ? Guid.Empty : adobjectId.ObjectGuid);
		}

		internal static object GuidGetter(IPropertyBag propertyBag)
		{
			ADObjectId adobjectId = (ADObjectId)propertyBag[ADObjectSchema.Id];
			return (adobjectId == null) ? Guid.Empty : adobjectId.ObjectGuid;
		}

		internal static GetterDelegate FlagGetterDelegate(int mask, ProviderPropertyDefinition propertyDefinition)
		{
			return delegate(IPropertyBag bag)
			{
				int num = (int)bag[propertyDefinition];
				bool value = 0 != (mask & num);
				return BoxedConstants.GetBool(value);
			};
		}

		internal static SetterDelegate FlagSetterDelegate(int mask, ProviderPropertyDefinition propertyDefinition)
		{
			return delegate(object value, IPropertyBag bag)
			{
				int num = (int)bag[propertyDefinition];
				bag[propertyDefinition] = (((bool)value) ? (num | mask) : (num & ~mask));
			};
		}

		internal static GetterDelegate LongFlagGetterDelegate(long mask, ProviderPropertyDefinition propertyDefinition)
		{
			return delegate(IPropertyBag bag)
			{
				long num = (long)bag[propertyDefinition];
				bool value = 0L != (mask & num);
				return BoxedConstants.GetBool(value);
			};
		}

		internal static SetterDelegate LongFlagSetterDelegate(long mask, ProviderPropertyDefinition propertyDefinition)
		{
			return delegate(object value, IPropertyBag bag)
			{
				long num = (long)bag[propertyDefinition];
				bag[propertyDefinition] = (((bool)value) ? (num | mask) : (num & ~mask));
			};
		}

		internal static GetterDelegate OWAFlagGetterDelegate(int mask, ProviderPropertyDefinition propertyDefinition)
		{
			return delegate(IPropertyBag bag)
			{
				uint? num = (uint?)bag[propertyDefinition];
				if (num != null)
				{
					long num2 = (long)mask;
					uint? num3 = num;
					bool value = 0L != ((num3 != null) ? new long?(num2 & (long)((ulong)num3.GetValueOrDefault())) : null);
					return BoxedConstants.GetBool(value);
				}
				return null;
			};
		}

		internal static SetterDelegate OWAFlagSetterDelegate(int mask, ProviderPropertyDefinition propertyDefinition, string description)
		{
			return delegate(object value, IPropertyBag bag)
			{
				uint? num = (uint?)bag[propertyDefinition];
				if (num == null)
				{
					num = new uint?(0U);
				}
				if (value != null)
				{
					PropertyDefinition propertyDefinition2 = propertyDefinition;
					uint? num5;
					if (!(bool)value)
					{
						uint? num2 = num;
						long num3 = (long)(~(long)mask);
						long? num4 = (num2 != null) ? new long?((long)((ulong)num2.GetValueOrDefault() & (ulong)num3)) : null;
						num5 = ((num4 != null) ? new uint?((uint)num4.GetValueOrDefault()) : null);
					}
					else
					{
						num5 = (num | new uint?((uint)mask));
					}
					bag[propertyDefinition2] = num5;
					return;
				}
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.ExceptionOwaCannotSetPropertyOnE12VirtualDirectoryToNull(description), propertyDefinition, value), null);
			};
		}

		internal static object NameGetter(IPropertyBag propertyBag)
		{
			return propertyBag[ADObjectSchema.RawName];
		}

		internal static void NameSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[ADObjectSchema.RawName] = value;
		}

		internal static object OrganizationIdGetter(IPropertyBag propertyBag)
		{
			return OrganizationId.Getter(propertyBag);
		}

		internal static void OrganizationIdSetter(object value, IPropertyBag propertyBag)
		{
			OrganizationId.Setter(value, propertyBag);
		}

		internal static object ExchangeObjectIdGetter(IPropertyBag propertyBag)
		{
			Guid guid = (Guid)propertyBag[ADObjectSchema.ExchangeObjectIdRaw];
			if (guid.Equals(Guid.Empty))
			{
				guid = (Guid)propertyBag[ADObjectSchema.Guid];
			}
			return guid;
		}

		internal static void ExchangeObjectIdSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[ADObjectSchema.ExchangeObjectIdRaw] = (Guid)value;
		}

		internal static object CorrelationIdGetter(IPropertyBag propertyBag)
		{
			Guid guid = (Guid)propertyBag[ADObjectSchema.CorrelationIdRaw];
			if (guid.Equals(Guid.Empty))
			{
				guid = (Guid)propertyBag[ADObjectSchema.Guid];
			}
			return guid;
		}

		internal static void CorrelationIdSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[ADObjectSchema.CorrelationIdRaw] = (Guid)value;
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false)]
		public string Name
		{
			get
			{
				return (string)this[ADObjectSchema.Name];
			}
			set
			{
				this[ADObjectSchema.Name] = value;
			}
		}

		public string DistinguishedName
		{
			get
			{
				return (string)this[ADObjectSchema.DistinguishedName];
			}
			internal set
			{
				this[ADObjectSchema.DistinguishedName] = value;
			}
		}

		public override ObjectId Identity
		{
			get
			{
				ObjectId originalId = this.OriginalId;
				object obj;
				if (base.TryConvertOutputProperty(originalId, ADObjectSchema.Id, out obj))
				{
					return (ObjectId)obj;
				}
				return originalId;
			}
		}

		public Guid Guid
		{
			get
			{
				return (Guid)this[ADObjectSchema.Guid];
			}
		}

		public ADObjectId ObjectCategory
		{
			get
			{
				return (ADObjectId)this[ADObjectSchema.ObjectCategory];
			}
			internal set
			{
				this[ADObjectSchema.ObjectCategory] = value;
			}
		}

		public MultiValuedProperty<string> ObjectClass
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADObjectSchema.ObjectClass];
			}
		}

		internal ADObjectId OriginalId
		{
			get
			{
				object obj;
				this.propertyBag.TryGetOriginalValue(ADObjectSchema.Id, out obj);
				return ((ADObjectId)obj) ?? base.Id;
			}
		}

		public DateTime? WhenChanged
		{
			get
			{
				return (DateTime?)this[ADObjectSchema.WhenChanged];
			}
		}

		public DateTime? WhenCreated
		{
			get
			{
				return (DateTime?)this[ADObjectSchema.WhenCreated];
			}
		}

		public DateTime? WhenChangedUTC
		{
			get
			{
				return (DateTime?)this[ADObjectSchema.WhenChangedUTC];
			}
		}

		public DateTime? WhenCreatedUTC
		{
			get
			{
				return (DateTime?)this[ADObjectSchema.WhenCreatedUTC];
			}
		}

		internal ADObjectId SharedConfiguration
		{
			get
			{
				return (ADObjectId)this[ADObjectSchema.SharedConfiguration];
			}
			set
			{
				this[ADObjectSchema.SharedConfiguration] = value;
			}
		}

		internal byte[] ReplicationSignature
		{
			get
			{
				return (byte[])this[ADObjectSchema.ReplicationSignature];
			}
			set
			{
				this[ADObjectSchema.ReplicationSignature] = value;
			}
		}

		internal Guid CorrelationId
		{
			get
			{
				return (Guid)this[ADObjectSchema.CorrelationId];
			}
			set
			{
				this[ADObjectSchema.CorrelationId] = value;
			}
		}

		internal Guid ExchangeObjectId
		{
			get
			{
				return (Guid)this[ADObjectSchema.ExchangeObjectId];
			}
			set
			{
				this[ADObjectSchema.ExchangeObjectId] = value;
			}
		}

		protected void SetObjectClass(string valueToSet)
		{
			MultiValuedProperty<string> value = new MultiValuedProperty<string>(true, ADObjectSchema.ObjectClass, new string[]
			{
				valueToSet
			});
			this.propertyBag.SetField(ADObjectSchema.ObjectClass, value);
		}

		internal RawSecurityDescriptor ReadSecurityDescriptor()
		{
			if (this.m_Session != null)
			{
				return this.m_Session.ReadSecurityDescriptor(base.Id);
			}
			return null;
		}

		internal SecurityDescriptor ReadSecurityDescriptorBlob()
		{
			if (this.m_Session != null)
			{
				return this.m_Session.ReadSecurityDescriptorBlob(base.Id);
			}
			return null;
		}

		internal void SaveSecurityDescriptor(RawSecurityDescriptor sd)
		{
			this.SaveSecurityDescriptor(sd, false);
		}

		internal void SaveSecurityDescriptor(RawSecurityDescriptor sd, bool modifyOwner)
		{
			if (this.m_Session != null)
			{
				this.m_Session.SaveSecurityDescriptor(this, sd, modifyOwner);
			}
		}

		internal ADObjectId ConfigurationUnit
		{
			get
			{
				return (ADObjectId)this[ADObjectSchema.ConfigurationUnit];
			}
		}

		internal ADObjectId OrganizationalUnitRoot
		{
			get
			{
				return (ADObjectId)this[ADObjectSchema.OrganizationalUnitRoot];
			}
		}

		public OrganizationId OrganizationId
		{
			get
			{
				return (OrganizationId)this[ADObjectSchema.OrganizationId];
			}
			internal set
			{
				this[ADObjectSchema.OrganizationId] = value;
			}
		}

		public override string ToString()
		{
			if (this.Identity != null)
			{
				return this.Identity.ToString();
			}
			if (!string.IsNullOrEmpty(this.Name))
			{
				return this.Name;
			}
			return base.ToString();
		}

		internal void ProvisionalClone(IPropertyBag source)
		{
			foreach (PropertyDefinition propertyDefinition in this.Schema.AllProperties)
			{
				ADPropertyDefinition adpropertyDefinition = (ADPropertyDefinition)propertyDefinition;
				if (adpropertyDefinition.IncludeInProvisionalClone && !adpropertyDefinition.IsReadOnly && !adpropertyDefinition.IsCalculated && !adpropertyDefinition.IsTaskPopulated)
				{
					object obj = source[adpropertyDefinition];
					if (obj != null && this[adpropertyDefinition] != obj && (!adpropertyDefinition.IsMultivalued || ((MultiValuedPropertyBase)obj).Count != 0))
					{
						this[adpropertyDefinition] = obj;
					}
				}
			}
		}

		internal virtual void StampPersistableDefaultValues()
		{
			object obj = new object();
			foreach (PropertyDefinition propertyDefinition in this.Schema.AllProperties)
			{
				ADPropertyDefinition adpropertyDefinition = (ADPropertyDefinition)propertyDefinition;
				if (adpropertyDefinition.DefaultValue != null && !adpropertyDefinition.IsReadOnly && adpropertyDefinition.PersistDefaultValue && !this.ExchangeVersion.IsOlderThan(adpropertyDefinition.VersionAdded) && !this.propertyBag.TryGetField(adpropertyDefinition, ref obj))
				{
					this[adpropertyDefinition] = adpropertyDefinition.DefaultValue;
				}
			}
			ADLegacyVersionableObject adlegacyVersionableObject = this as ADLegacyVersionableObject;
			if (adlegacyVersionableObject != null)
			{
				adlegacyVersionableObject.StampDefaultMinAdminVersion();
			}
			this.SetObjectClass(this.MostDerivedObjectClass);
		}

		internal static ADPropertyDefinition LegacyDnProperty(ADPropertyDefinitionFlags flags)
		{
			string text = "[a-zA-Z0-9 @!%&'),-.:<>}_\"\\|\\(\\*\\+\\?\\[\\]\\{]+";
			return new ADPropertyDefinition("LegacyExchangeDN", ExchangeObjectVersion.Exchange2003, typeof(string), "legacyExchangeDN", flags, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
			{
				new NoLeadingOrTrailingWhitespaceConstraint(),
				new RegexConstraint(string.Concat(new string[]
				{
					"^(/o=",
					text,
					"|/o=",
					text,
					"/ou=",
					text,
					"(/cn=",
					text,
					")*)$"
				}), RegexOptions.Compiled | RegexOptions.Singleline, DataStrings.LegacyDNPatternDescription)
			}, SimpleProviderPropertyDefinition.None, null, null, null, null, null);
		}

		internal static GetterDelegate InvertFlagGetterDelegate(ProviderPropertyDefinition propertyDefinition, int mask)
		{
			return delegate(IPropertyBag bag)
			{
				int num = (int)bag[propertyDefinition];
				bool value = 0 == (mask & num);
				return BoxedConstants.GetBool(value);
			};
		}

		internal static GetterDelegate FlagGetterDelegate(ProviderPropertyDefinition propertyDefinition, int mask)
		{
			return delegate(IPropertyBag bag)
			{
				int num = (int)bag[propertyDefinition];
				return BoxedConstants.GetBool(0 != (mask & num));
			};
		}

		internal static SetterDelegate FlagSetterDelegate(ProviderPropertyDefinition propertyDefinition, int mask)
		{
			return delegate(object value, IPropertyBag bag)
			{
				int num = (int)bag[propertyDefinition];
				bag[propertyDefinition] = (((bool)value) ? (num | mask) : (num & ~mask));
			};
		}

		internal static SetterDelegate InvertFlagSetterDelegate(ProviderPropertyDefinition propertyDefinition, int mask)
		{
			return delegate(object value, IPropertyBag bag)
			{
				int num = (int)bag[propertyDefinition];
				bag[propertyDefinition] = (((bool)value) ? (num & ~mask) : (num | mask));
			};
		}

		internal static ADPropertyDefinition BitfieldProperty(string name, int shift, int length, ADPropertyDefinition fieldProperty)
		{
			return ADObject.BitfieldProperty(name, shift, length, fieldProperty, null);
		}

		internal static ADPropertyDefinition BitfieldProperty(string name, int shift, int length, ADPropertyDefinition fieldProperty, PropertyDefinitionConstraint constraint)
		{
			if (fieldProperty.Type == typeof(int))
			{
				return ADObject.Int32BitfieldProperty(name, shift, length, fieldProperty, constraint);
			}
			if (fieldProperty.Type == typeof(long))
			{
				return ADObject.Int64BitfieldProperty(name, shift, length, fieldProperty, constraint);
			}
			throw new Exception("BitfieldProperty does not support underlying property of type " + fieldProperty.Type.FullName);
		}

		internal static ADPropertyDefinition Int32BitfieldProperty(string name, int shift, int length, ADPropertyDefinition fieldProperty, PropertyDefinitionConstraint constraint)
		{
			object defaultValue = 0;
			GetterDelegate getterDelegate = delegate(IPropertyBag bag)
			{
				object obj = null;
				if (!(bag as ADPropertyBag).TryGetField(fieldProperty, ref obj))
				{
					return defaultValue;
				}
				if (obj == null)
				{
					return defaultValue;
				}
				int num = (int)obj;
				int num2 = (1 << length) - 1;
				return num2 & num >> shift;
			};
			SetterDelegate setterDelegate = delegate(object value, IPropertyBag bag)
			{
				int num = (int)bag[fieldProperty];
				int num2 = (1 << length) - 1;
				int num3 = (int)value;
				num &= ~(num2 << shift);
				num |= (num3 & num2) << shift;
				bag[fieldProperty] = num;
			};
			CustomFilterBuilderDelegate customFilterBuilderDelegate = delegate(SinglePropertyFilter filter)
			{
				ComparisonFilter comparisonFilter = filter as ComparisonFilter;
				if (comparisonFilter == null || comparisonFilter.ComparisonOperator != ComparisonOperator.Equal)
				{
					throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedFilterForProperty(filter.Property.Name, filter.GetType(), typeof(ComparisonFilter)));
				}
				int num = (int)comparisonFilter.PropertyValue;
				int num2 = (1 << length) - 1;
				uint num3 = (uint)((uint)(num & num2) << shift);
				uint num4 = (uint)((uint)(~(uint)num & num2) << shift);
				QueryFilter queryFilter = new NotFilter(new BitMaskOrFilter(fieldProperty, (ulong)num4));
				if (num != 0)
				{
					return new AndFilter(new QueryFilter[]
					{
						new BitMaskAndFilter(fieldProperty, (ulong)num3),
						queryFilter
					});
				}
				return queryFilter;
			};
			PropertyDefinitionConstraint[] readConstraints;
			if (constraint == null)
			{
				readConstraints = PropertyDefinitionConstraint.None;
			}
			else
			{
				readConstraints = new PropertyDefinitionConstraint[]
				{
					constraint
				};
			}
			return new ADPropertyDefinition(name, fieldProperty.VersionAdded, typeof(int), null, ADPropertyDefinitionFlags.Calculated, defaultValue, readConstraints, PropertyDefinitionConstraint.None, new ADPropertyDefinition[]
			{
				fieldProperty
			}, customFilterBuilderDelegate, getterDelegate, setterDelegate, null, null);
		}

		internal static ADPropertyDefinition Int64BitfieldProperty(string name, int shift, int length, ADPropertyDefinition fieldProperty, PropertyDefinitionConstraint constraint)
		{
			object defaultValue = 0L;
			GetterDelegate getterDelegate = delegate(IPropertyBag bag)
			{
				object obj = null;
				if (!(bag as ADPropertyBag).TryGetField(fieldProperty, ref obj))
				{
					return defaultValue;
				}
				if (obj == null)
				{
					return defaultValue;
				}
				long num = (long)obj;
				long num2 = (1L << length) - 1L;
				return num2 & num >> shift;
			};
			SetterDelegate setterDelegate = delegate(object value, IPropertyBag bag)
			{
				long num = (long)bag[fieldProperty];
				long num2 = (1L << length) - 1L;
				long num3 = (long)value;
				num &= ~(num2 << shift);
				num |= (num3 & num2) << shift;
				bag[fieldProperty] = num;
			};
			PropertyDefinitionConstraint[] readConstraints;
			if (constraint == null)
			{
				readConstraints = PropertyDefinitionConstraint.None;
			}
			else
			{
				readConstraints = new PropertyDefinitionConstraint[]
				{
					constraint
				};
			}
			return new ADPropertyDefinition(name, fieldProperty.VersionAdded, typeof(long), null, ADPropertyDefinitionFlags.Calculated, defaultValue, readConstraints, PropertyDefinitionConstraint.None, new ADPropertyDefinition[]
			{
				fieldProperty
			}, null, getterDelegate, setterDelegate, null, null);
		}

		internal static ADPropertyDefinition BitfieldProperty(string name, int shift, ADPropertyDefinition fieldProperty)
		{
			if (fieldProperty.Type == typeof(int))
			{
				return ADObject.Int32BitfieldProperty(name, shift, fieldProperty);
			}
			if (fieldProperty.Type == typeof(long))
			{
				return ADObject.Int64BitfieldProperty(name, shift, fieldProperty);
			}
			throw new Exception("BitfieldProperty does not support underlying property of type " + fieldProperty.Type.FullName);
		}

		internal static ADPropertyDefinition Int32BitfieldProperty(string name, int shift, ADPropertyDefinition fieldProperty)
		{
			CustomFilterBuilderDelegate customFilterBuilderDelegate = delegate(SinglePropertyFilter filter)
			{
				ComparisonFilter comparisonFilter = filter as ComparisonFilter;
				if (comparisonFilter == null || comparisonFilter.ComparisonOperator != ComparisonOperator.Equal)
				{
					throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedFilterForProperty(filter.Property.Name, filter.GetType(), typeof(ComparisonFilter)));
				}
				int num = 1;
				int num2 = ((bool)comparisonFilter.PropertyValue) ? 1 : 0;
				int num3 = (1 << num) - 1;
				uint num4 = (uint)((uint)(num2 & num3) << shift);
				uint num5 = (uint)((uint)(~(uint)num2 & num3) << shift);
				QueryFilter queryFilter = new NotFilter(new BitMaskOrFilter(fieldProperty, (ulong)num5));
				if (num2 != 0)
				{
					return new AndFilter(new QueryFilter[]
					{
						new BitMaskAndFilter(fieldProperty, (ulong)num4),
						queryFilter
					});
				}
				return queryFilter;
			};
			return new ADPropertyDefinition(name, fieldProperty.VersionAdded, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ADPropertyDefinition[]
			{
				fieldProperty
			}, customFilterBuilderDelegate, ADObject.FlagGetterDelegate(1 << shift, fieldProperty), ADObject.FlagSetterDelegate(1 << shift, fieldProperty), null, null);
		}

		internal static ADPropertyDefinition Int64BitfieldProperty(string name, int shift, ADPropertyDefinition fieldProperty)
		{
			return new ADPropertyDefinition(name, fieldProperty.VersionAdded, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ADPropertyDefinition[]
			{
				fieldProperty
			}, null, ADObject.LongFlagGetterDelegate(1L << shift, fieldProperty), ADObject.LongFlagSetterDelegate(1L << shift, fieldProperty), null, null);
		}

		internal static QueryFilter ExchangeObjectIdFilterBuilder(SinglePropertyFilter filter)
		{
			if (filter is ExistsFilter)
			{
				return new ExistsFilter(ADObjectSchema.ExchangeObjectIdRaw);
			}
			Guid guid = (Guid)ADObject.PropertyValueFromEqualityFilter(filter);
			return new OrFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ExchangeObjectIdRaw, guid),
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Guid, guid)
			});
		}

		internal static QueryFilter CorrelationIdFilterBuilder(SinglePropertyFilter filter)
		{
			if (filter is ExistsFilter)
			{
				return new ExistsFilter(ADObjectSchema.CorrelationIdRaw);
			}
			Guid guid = (Guid)ADObject.PropertyValueFromEqualityFilter(filter);
			return new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.CorrelationIdRaw, guid);
		}

		internal static bool IsRecipientDirSynced(bool isDirSynced)
		{
			ExTraceGlobals.ADObjectTracer.TraceDebug<string>(0L, "<ADObject::IsRecipientDirSynced> return ({0})", isDirSynced.ToString());
			return isDirSynced;
		}

		internal bool IsConsumerOrganization()
		{
			return Globals.IsConsumerOrganization(this.OrganizationId);
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			bool flag = true;
			if (this.OrganizationalUnitRoot != null && this.ConfigurationUnit != null)
			{
				string distinguishedName = base.Id.DistinguishedName;
				string distinguishedName2 = this.OrganizationalUnitRoot.DistinguishedName;
				string distinguishedName3 = this.ConfigurationUnit.DistinguishedName;
				if (!string.IsNullOrEmpty(distinguishedName3) && !distinguishedName3.ToLower().StartsWith("cn=configuration,"))
				{
					flag = false;
					errors.Add(new ObjectValidationError(DirectoryStrings.ErrorInvalidOrganizationId(distinguishedName, distinguishedName2 ?? "<null>", distinguishedName3 ?? "<null>"), this.Identity, string.Empty));
				}
			}
			else if (this.OrganizationalUnitRoot != null || this.ConfigurationUnit != null)
			{
				flag = false;
				errors.Add(new ObjectValidationError(DirectoryStrings.ErrorInvalidOrganizationId(base.Id.ToDNString(), (this.OrganizationalUnitRoot != null) ? this.OrganizationalUnitRoot.ToDNString() : "<null>", (this.ConfigurationUnit != null) ? this.ConfigurationUnit.ToDNString() : "<null>"), this.Identity, string.Empty));
			}
			foreach (ADPropertyDefinition adpropertyDefinition in this.Schema.AllADObjectLinkProperties)
			{
				MultiValuedProperty<ADObjectId> multiValuedProperty = null;
				if (adpropertyDefinition.IsMultivalued)
				{
					try
					{
						multiValuedProperty = (MultiValuedProperty<ADObjectId>)this.propertyBag[adpropertyDefinition];
						goto IL_16E;
					}
					catch (DataValidationException)
					{
						goto IL_16E;
					}
					goto IL_13E;
				}
				goto IL_13E;
				IL_16E:
				if (multiValuedProperty != null && (multiValuedProperty.Count != 1 || multiValuedProperty[0] != null))
				{
					foreach (ADObjectId adobjectId in multiValuedProperty)
					{
						if (!string.IsNullOrEmpty(adobjectId.DistinguishedName))
						{
							if ((base.Id == null || !base.Id.IsDeleted) && adobjectId.IsDeleted)
							{
								ExTraceGlobals.ValidationTracer.TraceWarning<string, string, string>(0L, "Object [{0}]. Property [{1}] is set to value [{2}], it is pointing to Deleted Objects container of Active Directory.", this.DistinguishedName.ToString(), adpropertyDefinition.Name, adobjectId.ToString());
								Globals.LogEvent(DirectoryEventLogConstants.Tuple_DeletedObjectIdLinked, adpropertyDefinition.Name, new object[]
								{
									this.DistinguishedName.ToString(),
									adpropertyDefinition.Name,
									adobjectId.ToString()
								});
							}
							if (!adpropertyDefinition.IsValidateInSameOrganization || (flag && this.ShouldValidatePropertyLinkInSameOrganization(adpropertyDefinition)))
							{
								this.ValidateSingleADObjectLinkValue(adpropertyDefinition, adobjectId, errors);
							}
						}
					}
					continue;
				}
				continue;
				IL_13E:
				ADObjectId adobjectId2 = null;
				try
				{
					adobjectId2 = (ADObjectId)this.propertyBag[adpropertyDefinition];
				}
				catch (DataValidationException)
				{
				}
				if (adobjectId2 != null)
				{
					multiValuedProperty = new MultiValuedProperty<ADObjectId>();
					multiValuedProperty.Add(adobjectId2);
					goto IL_16E;
				}
				goto IL_16E;
			}
			base.ValidateRead(errors);
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			if (base.ObjectState == ObjectState.New)
			{
				this.StampPersistableDefaultValues();
			}
			base.ValidateWrite(errors);
		}

		internal virtual bool ShouldValidatePropertyLinkInSameOrganization(ADPropertyDefinition property)
		{
			return true;
		}

		private void ValidateSingleADObjectLinkValue(ADPropertyDefinition propertyDefinition, ADObjectId value, List<ValidationError> errors)
		{
			if (this.m_Session != null && this.m_Session.GetType().Name.Equals("TopologyDiscoverySession"))
			{
				return;
			}
			if (this.m_Session != null && !value.IsDescendantOf(this.m_Session.GetRootDomainNamingContext()))
			{
				return;
			}
			if (propertyDefinition.IsValidateInFirstOrganization && this.m_Session != null)
			{
				ADObjectId adobjectId = null;
				try
				{
					adobjectId = this.m_Session.SessionSettings.RootOrgId;
				}
				catch (OrgContainerNotFoundException)
				{
				}
				if (adobjectId != null && !value.IsDescendantOf(adobjectId) && adobjectId.DomainId == value.DomainId && !value.DistinguishedName.ToLower().Contains(",cn=deleted objects,"))
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.ErrorLinkedADObjectNotInFirstOrganization(propertyDefinition.Name, value.ToString(), this.Identity.ToString(), adobjectId.ToString()), propertyDefinition, value));
					return;
				}
			}
			else if (propertyDefinition.IsValidateInSameOrganization)
			{
				if (this.OrganizationId.Equals(OrganizationId.ForestWideOrgId) && this.m_Session != null)
				{
					ADObjectId adobjectId2 = null;
					try
					{
						adobjectId2 = this.m_Session.SessionSettings.RootOrgId;
					}
					catch (OrgContainerNotFoundException)
					{
					}
					catch (TenantOrgContainerNotFoundException)
					{
					}
					bool flag = true;
					try
					{
						if (adobjectId2 != null && adobjectId2.DescendantDN(1).Name.ToLower().Equals("configuration"))
						{
							flag = false;
						}
					}
					catch (InvalidOperationException)
					{
					}
					if (adobjectId2 != null && flag)
					{
						ADObjectId childId = value.DomainId.GetChildId("OU", "Microsoft Exchange Hosted Organizations");
						ADObjectId configurationUnitsRoot = this.m_Session.GetConfigurationUnitsRoot();
						if ((value.IsDescendantOf(childId) || value.IsDescendantOf(configurationUnitsRoot)) && (!(this is ADConfigurationObject) || !base.Id.IsDescendantOf(configurationUnitsRoot)))
						{
							errors.Add(new PropertyValidationError(DirectoryStrings.ErrorLinkedADObjectNotInSameOrganization(propertyDefinition.Name, value.ToString(), this.Identity.ToString(), this.OrganizationId.ToString()), propertyDefinition, value));
							return;
						}
					}
				}
				else if (!this.OrganizationId.Equals(OrganizationId.ForestWideOrgId) && !value.DistinguishedName.ToLower().Contains(",cn=deleted objects,") && !value.IsDescendantOf(this.OrganizationId.OrganizationalUnit) && !value.IsDescendantOf(this.OrganizationId.ConfigurationUnit) && (!propertyDefinition.IsValidateInSharedConfig || this.SharedConfiguration == null || !value.IsDescendantOf(this.SharedConfiguration)))
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.ErrorLinkedADObjectNotInSameOrganization(propertyDefinition.Name, value.ToString(), this.Identity.ToString(), this.OrganizationId.ToString()), propertyDefinition, value));
				}
			}
		}

		internal override bool SkipFullPropertyValidation(ProviderPropertyDefinition propertyDefinition)
		{
			return (propertyDefinition.Name == ADObjectSchema.Name.Name && base.ObjectState == ObjectState.New) || base.SkipFullPropertyValidation(propertyDefinition);
		}

		internal const string HostedOrganizationsOrganizationalUnit = "Microsoft Exchange Hosted Organizations";

		internal static readonly string ConfigurationUnits = "ConfigurationUnits";

		internal static QueryFilter ObjectClassExistsFilter = new ExistsFilter(ADObjectSchema.ObjectClass);

		[NonSerialized]
		internal IDirectorySession m_Session;
	}
}
