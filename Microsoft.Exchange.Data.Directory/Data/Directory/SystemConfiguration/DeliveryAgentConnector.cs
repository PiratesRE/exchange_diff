using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class DeliveryAgentConnector : MailGateway
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return DeliveryAgentConnector.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return DeliveryAgentConnector.MostDerivedClass;
			}
		}

		[Parameter]
		public override bool Enabled
		{
			get
			{
				return (bool)this[DeliveryAgentConnectorSchema.Enabled];
			}
			set
			{
				this[DeliveryAgentConnectorSchema.Enabled] = value;
			}
		}

		[Parameter]
		public string DeliveryProtocol
		{
			get
			{
				return (string)this[DeliveryAgentConnectorSchema.DeliveryProtocol];
			}
			set
			{
				this[DeliveryAgentConnectorSchema.DeliveryProtocol] = value;
			}
		}

		[Parameter]
		public int MaxConcurrentConnections
		{
			get
			{
				return (int)this[DeliveryAgentConnectorSchema.MaxConcurrentConnections];
			}
			set
			{
				this[DeliveryAgentConnectorSchema.MaxConcurrentConnections] = value;
			}
		}

		[Parameter]
		public int MaxMessagesPerConnection
		{
			get
			{
				return (int)this[DeliveryAgentConnectorSchema.MaxMessagesPerConnection];
			}
			set
			{
				this[DeliveryAgentConnectorSchema.MaxMessagesPerConnection] = value;
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, this.MostDerivedObjectClass);
			}
		}

		public new static string MostDerivedClass = "msExchDeliveryAgentConnector";

		private static DeliveryAgentConnectorSchema schema = ObjectSchema.GetInstance<DeliveryAgentConnectorSchema>();
	}
}
