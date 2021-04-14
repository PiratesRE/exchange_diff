using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class ResourceBookingConfig : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ResourceBookingConfig.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ResourceBookingConfig.mostDerivedClass;
			}
		}

		internal override bool IsShareable
		{
			get
			{
				return true;
			}
		}

		public new string Name
		{
			get
			{
				return base.Name;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> ResourcePropertySchema
		{
			get
			{
				return (MultiValuedProperty<string>)this[ResourceBookingConfigSchema.ResourcePropertySchema];
			}
			set
			{
				this[ResourceBookingConfigSchema.ResourcePropertySchema] = value;
			}
		}

		internal static ADObjectId GetWellKnownLocation(ADObjectId orgContainerId)
		{
			ADObjectId relativePath = new ADObjectId("CN=Resource Schema");
			return ResourceBookingConfig.GetWellKnownParentLocation(orgContainerId).GetDescendantId(relativePath);
		}

		internal static ADObjectId GetWellKnownParentLocation(ADObjectId orgContainerId)
		{
			ADObjectId relativePath = new ADObjectId("CN=Global Settings");
			return orgContainerId.GetDescendantId(relativePath);
		}

		internal static MultiValuedProperty<string> GetAllowedProperties(IConfigurationSession configSession, ExchangeResourceType? resourceType)
		{
			MultiValuedProperty<string> multiValuedProperty = new MultiValuedProperty<string>();
			if (resourceType != null)
			{
				string text = resourceType.ToString() + '/';
				ResourceBookingConfig resourceBookingConfig = configSession.Read<ResourceBookingConfig>(ResourceBookingConfig.GetWellKnownLocation(configSession.GetOrgContainerId()));
				foreach (string text2 in resourceBookingConfig.ResourcePropertySchema)
				{
					if (string.Compare(text, 0, text2, 0, text.Length, StringComparison.OrdinalIgnoreCase) == 0)
					{
						multiValuedProperty.Add(text2.Substring(text.Length));
					}
				}
			}
			return multiValuedProperty;
		}

		internal bool IsPropAllowedOnResourceType(string resourceType, string resourceProperty)
		{
			string strB = resourceType + '/' + resourceProperty;
			foreach (string strA in this.ResourcePropertySchema)
			{
				if (string.Compare(strA, strB, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return true;
				}
			}
			return false;
		}

		public const char LocationDelimiter = '/';

		private static ResourceBookingConfigSchema schema = ObjectSchema.GetInstance<ResourceBookingConfigSchema>();

		private static string mostDerivedClass = "msExchResourceSchema";
	}
}
