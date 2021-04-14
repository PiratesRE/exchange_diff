using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MessagingPolicies
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class JournalingTargetDGEmptyException : LocalizedException
	{
		public JournalingTargetDGEmptyException(string distributionGroup) : base(TransportRulesStrings.JournalingTargetDGEmptyDescription(distributionGroup))
		{
			this.distributionGroup = distributionGroup;
		}

		public JournalingTargetDGEmptyException(string distributionGroup, Exception innerException) : base(TransportRulesStrings.JournalingTargetDGEmptyDescription(distributionGroup), innerException)
		{
			this.distributionGroup = distributionGroup;
		}

		protected JournalingTargetDGEmptyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.distributionGroup = (string)info.GetValue("distributionGroup", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("distributionGroup", this.distributionGroup);
		}

		public string DistributionGroup
		{
			get
			{
				return this.distributionGroup;
			}
		}

		private readonly string distributionGroup;
	}
}
