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
	public sealed class IPBlockListConfig : MessageHygieneAgentConfig
	{
		public static AsciiString DefaultMachineRejectionResponse
		{
			get
			{
				return IPBlockListConfigSchema.DefaultMachineRejectionResponse;
			}
		}

		public static AsciiString DefaultStaticRejectionResponse
		{
			get
			{
				return IPBlockListConfigSchema.DefaultStaticRejectionResponse;
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
		public AsciiString MachineEntryRejectionResponse
		{
			get
			{
				return (AsciiString)this[IPBlockListConfigSchema.MachineEntryRejectionResponse];
			}
			set
			{
				this[IPBlockListConfigSchema.MachineEntryRejectionResponse] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public AsciiString StaticEntryRejectionResponse
		{
			get
			{
				return (AsciiString)this[IPBlockListConfigSchema.StaticEntryRejectionResponse];
			}
			set
			{
				this[IPBlockListConfigSchema.StaticEntryRejectionResponse] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return IPBlockListConfig.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchMessageHygieneIPBlockListConfig";
			}
		}

		public const string CanonicalName = "IPBlockListConfig";

		private const string MostDerivedClass = "msExchMessageHygieneIPBlockListConfig";

		private static readonly IPBlockListConfigSchema schema = ObjectSchema.GetInstance<IPBlockListConfigSchema>();
	}
}
