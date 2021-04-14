using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public sealed class UMHuntGroup : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return UMHuntGroup.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return UMHuntGroup.mostDerivedClass;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		[Parameter(Mandatory = false)]
		public string PilotIdentifier
		{
			get
			{
				return (string)this[UMHuntGroupSchema.PilotIdentifier];
			}
			set
			{
				this[UMHuntGroupSchema.PilotIdentifier] = value;
			}
		}

		public ADObjectId UMDialPlan
		{
			get
			{
				return (ADObjectId)this[UMHuntGroupSchema.UMDialPlan];
			}
			set
			{
				this[UMHuntGroupSchema.UMDialPlan] = value;
			}
		}

		public ADObjectId UMIPGateway
		{
			get
			{
				return (ADObjectId)this[UMHuntGroupSchema.UMIPGateway];
			}
		}

		public override string ToString()
		{
			return this.PilotIdentifier + ":" + this.UMDialPlan.Name;
		}

		internal static object UMIPGatewayGetter(IPropertyBag propertyBag)
		{
			ADObjectId adobjectId = (ADObjectId)propertyBag[ADObjectSchema.Id];
			if (adobjectId == null && (ObjectState)propertyBag[ADObjectSchema.ObjectState] != ObjectState.New)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.IdIsNotSet, UMHuntGroupSchema.UMIPGateway, null));
			}
			if (adobjectId != null)
			{
				return adobjectId.Parent;
			}
			return null;
		}

		private static UMHuntGroupSchema schema = ObjectSchema.GetInstance<UMHuntGroupSchema>();

		private static string mostDerivedClass = "msExchUMHuntGroup";
	}
}
