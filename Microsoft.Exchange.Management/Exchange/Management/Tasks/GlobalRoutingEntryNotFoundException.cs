using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class GlobalRoutingEntryNotFoundException : LocalizedException
	{
		public GlobalRoutingEntryNotFoundException(string phoneNumber) : base(Strings.GlobalRoutingEntryNotFound(phoneNumber))
		{
			this.phoneNumber = phoneNumber;
		}

		public GlobalRoutingEntryNotFoundException(string phoneNumber, Exception innerException) : base(Strings.GlobalRoutingEntryNotFound(phoneNumber), innerException)
		{
			this.phoneNumber = phoneNumber;
		}

		protected GlobalRoutingEntryNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.phoneNumber = (string)info.GetValue("phoneNumber", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("phoneNumber", this.phoneNumber);
		}

		public string PhoneNumber
		{
			get
			{
				return this.phoneNumber;
			}
		}

		private readonly string phoneNumber;
	}
}
