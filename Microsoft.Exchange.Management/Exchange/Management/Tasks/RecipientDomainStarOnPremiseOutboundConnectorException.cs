using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RecipientDomainStarOnPremiseOutboundConnectorException : LocalizedException
	{
		public RecipientDomainStarOnPremiseOutboundConnectorException() : base(Strings.RecipientDomainStarOnPremiseOutboundConnector)
		{
		}

		public RecipientDomainStarOnPremiseOutboundConnectorException(Exception innerException) : base(Strings.RecipientDomainStarOnPremiseOutboundConnector, innerException)
		{
		}

		protected RecipientDomainStarOnPremiseOutboundConnectorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
