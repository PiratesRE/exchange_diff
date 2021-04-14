using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.HostedServices.AdminCenter.UI.Services
{
	[KnownType(typeof(DomainConfigurationSettings))]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "3.0.0.0")]
	[DataContract(Name = "ConfigurationSettings", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services")]
	[KnownType(typeof(CompanyConfigurationSettings))]
	[Serializable]
	internal class ConfigurationSettings : IExtensibleDataObject
	{
		public ExtensionDataObject ExtensionData
		{
			get
			{
				return this.extensionDataField;
			}
			set
			{
				this.extensionDataField = value;
			}
		}

		[DataMember]
		internal int CompanyId
		{
			get
			{
				return this.CompanyIdField;
			}
			set
			{
				this.CompanyIdField = value;
			}
		}

		[DataMember]
		internal EdgeBlockMode? DirectoryEdgeBlockMode
		{
			get
			{
				return this.DirectoryEdgeBlockModeField;
			}
			set
			{
				this.DirectoryEdgeBlockModeField = value;
			}
		}

		[DataMember]
		internal IPListConfig InternalServerIPList
		{
			get
			{
				return this.InternalServerIPListField;
			}
			set
			{
				this.InternalServerIPListField = value;
			}
		}

		[DataMember]
		internal IPListConfig OnPremiseGatewayIPList
		{
			get
			{
				return this.OnPremiseGatewayIPListField;
			}
			set
			{
				this.OnPremiseGatewayIPListField = value;
			}
		}

		[DataMember]
		internal IPListConfig OutboundIPList
		{
			get
			{
				return this.OutboundIPListField;
			}
			set
			{
				this.OutboundIPListField = value;
			}
		}

		[DataMember]
		internal bool? RecipientLevelRouting
		{
			get
			{
				return this.RecipientLevelRoutingField;
			}
			set
			{
				this.RecipientLevelRoutingField = value;
			}
		}

		[DataMember]
		internal bool? SkipList
		{
			get
			{
				return this.SkipListField;
			}
			set
			{
				this.SkipListField = value;
			}
		}

		[NonSerialized]
		private ExtensionDataObject extensionDataField;

		[OptionalField]
		private int CompanyIdField;

		[OptionalField]
		private EdgeBlockMode? DirectoryEdgeBlockModeField;

		[OptionalField]
		private IPListConfig InternalServerIPListField;

		[OptionalField]
		private IPListConfig OnPremiseGatewayIPListField;

		[OptionalField]
		private IPListConfig OutboundIPListField;

		[OptionalField]
		private bool? RecipientLevelRoutingField;

		[OptionalField]
		private bool? SkipListField;
	}
}
