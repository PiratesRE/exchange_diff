using System;
using System.Collections.Generic;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public class ExtendedSecurityPrincipal : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ExtendedSecurityPrincipal.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ExtendedSecurityPrincipal.mostDerivedClass;
			}
		}

		public SecurityIdentifier SID
		{
			get
			{
				return (SecurityIdentifier)this[IADSecurityPrincipalSchema.Sid];
			}
		}

		public string DisplayName
		{
			get
			{
				return (string)this[ExtendedSecurityPrincipalSchema.DisplayName];
			}
		}

		public RecipientTypeDetails RecipientTypeDetails
		{
			get
			{
				return (RecipientTypeDetails)this[ExtendedSecurityPrincipalSchema.RecipientTypeDetails];
			}
		}

		public SecurityPrincipalType Type
		{
			get
			{
				return this.securityPrincipalType;
			}
		}

		public string InFolder
		{
			get
			{
				return this.inFolder;
			}
		}

		public string UserFriendlyName { get; internal set; }

		internal static void SecurityPrincipalTypeDetailsSetter(object value, IPropertyBag propertyBag)
		{
		}

		internal static object SecurityPrincipalTypeDetailsGetter(IPropertyBag propertyBag)
		{
			Exception ex = null;
			try
			{
				ADObjectId adobjectId = (ADObjectId)propertyBag[ADObjectSchema.ObjectCategory];
				if (adobjectId != null)
				{
					MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[ADObjectSchema.ObjectClass];
					string text = adobjectId.Name.ToLower();
					if (text == ADUser.ObjectCategoryNameInternal && multiValuedProperty.Count > 0 && multiValuedProperty.Contains(ADUser.MostDerivedClass))
					{
						return SecurityPrincipalType.User;
					}
					if (text == ADGroup.MostDerivedClass)
					{
						return SecurityPrincipalType.Group;
					}
					if (text == "foreign-security-principal")
					{
						return SecurityPrincipalType.WellknownSecurityPrincipal;
					}
					if (text == ExtendedSecurityPrincipal.computer && multiValuedProperty.Count > 0 && multiValuedProperty.Contains(ADUser.MostDerivedClass))
					{
						return SecurityPrincipalType.Computer;
					}
					ex = new ArgumentException(DirectoryStrings.ExArgumentException("ObjectCategory", text));
				}
				else
				{
					ex = new ArgumentNullException(DirectoryStrings.ExArgumentNullException("ObjectCategory"));
				}
			}
			catch (InvalidOperationException ex2)
			{
				ex = ex2;
			}
			throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("SecurityPrincipalTypes", ex.Message), ExtendedSecurityPrincipalSchema.SecurityPrincipalTypes, propertyBag[ADObjectSchema.ObjectCategory]), ex);
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			base.ValidateRead(errors);
			this.securityPrincipalType = (SecurityPrincipalType)this[ExtendedSecurityPrincipalSchema.SecurityPrincipalTypes];
			if (this.securityPrincipalType == SecurityPrincipalType.User || this.securityPrincipalType == SecurityPrincipalType.Group)
			{
				this.inFolder = base.Id.Parent.ToCanonicalName();
				return;
			}
			if (this.securityPrincipalType == SecurityPrincipalType.WellknownSecurityPrincipal)
			{
				this.inFolder = null;
				return;
			}
			errors.Add(new PropertyValidationError(DataStrings.BadEnumValue(typeof(SecurityPrincipalType)), ADObjectSchema.ObjectClass, null));
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return new OrFilter(new QueryFilter[]
				{
					new AndFilter(new QueryFilter[]
					{
						new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, ADUser.ObjectCategoryNameInternal),
						ADObject.ObjectClassFilter(ADUser.MostDerivedClass, true)
					}),
					new AndFilter(new QueryFilter[]
					{
						new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, ADGroup.MostDerivedClass),
						new BitMaskOrFilter(ADGroupSchema.GroupType, (ulong)int.MinValue)
					}),
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, ExtendedSecurityPrincipal.WellknownSecurityPrincipalClassName)
				});
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ADRecipient.PublicFolderMailboxObjectVersion;
			}
		}

		private const string WellknownSecurityPrincipalName = "foreign-security-principal";

		internal static string WellknownSecurityPrincipalClassName = "foreignSecurityPrincipal";

		private static ExtendedSecurityPrincipalSchema schema = ObjectSchema.GetInstance<ExtendedSecurityPrincipalSchema>();

		private static string mostDerivedClass = "securityPrincipal";

		private static string computer = "computer";

		private SecurityPrincipalType securityPrincipalType;

		private string inFolder;
	}
}
