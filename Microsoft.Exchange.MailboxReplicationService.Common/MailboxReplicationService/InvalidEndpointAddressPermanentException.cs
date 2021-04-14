using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidEndpointAddressPermanentException : MailboxReplicationPermanentException
	{
		public InvalidEndpointAddressPermanentException(string serviceURI) : base(MrsStrings.InvalidEndpointAddressError(serviceURI))
		{
			this.serviceURI = serviceURI;
		}

		public InvalidEndpointAddressPermanentException(string serviceURI, Exception innerException) : base(MrsStrings.InvalidEndpointAddressError(serviceURI), innerException)
		{
			this.serviceURI = serviceURI;
		}

		protected InvalidEndpointAddressPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serviceURI = (string)info.GetValue("serviceURI", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("serviceURI", this.serviceURI);
		}

		public string ServiceURI
		{
			get
			{
				return this.serviceURI;
			}
		}

		private readonly string serviceURI;
	}
}
