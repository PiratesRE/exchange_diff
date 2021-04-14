using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RuleCollectionNotInAdException : ExchangeConfigurationException
	{
		public RuleCollectionNotInAdException(string name) : base(TransportRulesStrings.RuleCollectionNotInAd(name))
		{
			this.name = name;
		}

		public RuleCollectionNotInAdException(string name, Exception innerException) : base(TransportRulesStrings.RuleCollectionNotInAd(name), innerException)
		{
			this.name = name;
		}

		protected RuleCollectionNotInAdException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.name = (string)info.GetValue("name", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("name", this.name);
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		private readonly string name;
	}
}
