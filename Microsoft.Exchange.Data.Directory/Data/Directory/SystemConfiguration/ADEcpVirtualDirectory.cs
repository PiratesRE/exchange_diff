using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public sealed class ADEcpVirtualDirectory : ExchangeWebAppVirtualDirectory
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ADEcpVirtualDirectory.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADEcpVirtualDirectory.MostDerivedClass;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, this.MostDerivedObjectClass);
			}
		}

		[Parameter]
		public bool AdminEnabled
		{
			get
			{
				return (bool)this[ADEcpVirtualDirectorySchema.AdminEnabled];
			}
			set
			{
				this[ADEcpVirtualDirectorySchema.AdminEnabled] = value;
			}
		}

		[Parameter]
		public bool OwaOptionsEnabled
		{
			get
			{
				return (bool)this[ADEcpVirtualDirectorySchema.OwaOptionsEnabled];
			}
			set
			{
				this[ADEcpVirtualDirectorySchema.OwaOptionsEnabled] = value;
			}
		}

		private static readonly ADEcpVirtualDirectorySchema schema = ObjectSchema.GetInstance<ADEcpVirtualDirectorySchema>();

		public static readonly string MostDerivedClass = "msExchEcpVirtualDirectory";
	}
}
