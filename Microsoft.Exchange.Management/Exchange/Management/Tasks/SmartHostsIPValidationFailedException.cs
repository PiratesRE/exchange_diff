using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SmartHostsIPValidationFailedException : LocalizedException
	{
		public SmartHostsIPValidationFailedException(string ipAddress) : base(Strings.SmartHostsIPValidationFailedId(ipAddress))
		{
			this.ipAddress = ipAddress;
		}

		public SmartHostsIPValidationFailedException(string ipAddress, Exception innerException) : base(Strings.SmartHostsIPValidationFailedId(ipAddress), innerException)
		{
			this.ipAddress = ipAddress;
		}

		protected SmartHostsIPValidationFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.ipAddress = (string)info.GetValue("ipAddress", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ipAddress", this.ipAddress);
		}

		public string IpAddress
		{
			get
			{
				return this.ipAddress;
			}
		}

		private readonly string ipAddress;
	}
}
