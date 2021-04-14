using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ProvisioningFederatedExchangeException : FederationException
	{
		public ProvisioningFederatedExchangeException(string details) : base(Strings.ErrorProvisioningFederatedExchange(details))
		{
			this.details = details;
		}

		public ProvisioningFederatedExchangeException(string details, Exception innerException) : base(Strings.ErrorProvisioningFederatedExchange(details), innerException)
		{
			this.details = details;
		}

		protected ProvisioningFederatedExchangeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.details = (string)info.GetValue("details", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("details", this.details);
		}

		public string Details
		{
			get
			{
				return this.details;
			}
		}

		private readonly string details;
	}
}
