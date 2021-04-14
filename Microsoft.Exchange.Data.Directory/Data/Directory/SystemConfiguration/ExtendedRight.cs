using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(ConfigScopes.Global)]
	[Serializable]
	public sealed class ExtendedRight : ADNonExchangeObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ExtendedRight.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ExtendedRight.mostDerivedClass;
			}
		}

		[Parameter(Mandatory = false)]
		public string DisplayName
		{
			get
			{
				return (string)this[ExtendedRightSchema.DisplayName];
			}
			set
			{
				this[ExtendedRightSchema.DisplayName] = value;
			}
		}

		public Guid RightsGuid
		{
			get
			{
				return (Guid)this[ExtendedRightSchema.RightsGuid];
			}
		}

		private static ExtendedRightSchema schema = ObjectSchema.GetInstance<ExtendedRightSchema>();

		private static string mostDerivedClass = "controlAccessRight";
	}
}
