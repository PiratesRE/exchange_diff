using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Security.RightsManagement
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidRpmsgFormatException : LocalizedException
	{
		public InvalidRpmsgFormatException(string reason) : base(DrmStrings.InvalidRpmsgFormat(reason))
		{
			this.reason = reason;
		}

		public InvalidRpmsgFormatException(string reason, Exception innerException) : base(DrmStrings.InvalidRpmsgFormat(reason), innerException)
		{
			this.reason = reason;
		}

		protected InvalidRpmsgFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.reason = (string)info.GetValue("reason", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("reason", this.reason);
		}

		public string Reason
		{
			get
			{
				return this.reason;
			}
		}

		private readonly string reason;
	}
}
