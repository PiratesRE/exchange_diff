using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidTransportRuleEventSourceTypeException : ExchangeConfigurationException
	{
		public InvalidTransportRuleEventSourceTypeException(string typeName) : base(TransportRulesStrings.InvalidTransportRuleEventSourceType(typeName))
		{
			this.typeName = typeName;
		}

		public InvalidTransportRuleEventSourceTypeException(string typeName, Exception innerException) : base(TransportRulesStrings.InvalidTransportRuleEventSourceType(typeName), innerException)
		{
			this.typeName = typeName;
		}

		protected InvalidTransportRuleEventSourceTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.typeName = (string)info.GetValue("typeName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("typeName", this.typeName);
		}

		public string TypeName
		{
			get
			{
				return this.typeName;
			}
		}

		private readonly string typeName;
	}
}
