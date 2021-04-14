using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SendConnectorNonSmtpAddressSpaceOnDNSConnectorException : LocalizedException
	{
		public SendConnectorNonSmtpAddressSpaceOnDNSConnectorException(string addressSpace) : base(Strings.SendConnectorNonSmtpAddressSpaceOnDNSConnector(addressSpace))
		{
			this.addressSpace = addressSpace;
		}

		public SendConnectorNonSmtpAddressSpaceOnDNSConnectorException(string addressSpace, Exception innerException) : base(Strings.SendConnectorNonSmtpAddressSpaceOnDNSConnector(addressSpace), innerException)
		{
			this.addressSpace = addressSpace;
		}

		protected SendConnectorNonSmtpAddressSpaceOnDNSConnectorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.addressSpace = (string)info.GetValue("addressSpace", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("addressSpace", this.addressSpace);
		}

		public string AddressSpace
		{
			get
			{
				return this.addressSpace;
			}
		}

		private readonly string addressSpace;
	}
}
