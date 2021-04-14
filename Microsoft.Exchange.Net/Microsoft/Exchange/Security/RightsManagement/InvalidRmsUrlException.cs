using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Security.RightsManagement
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InvalidRmsUrlException : LocalizedException
	{
		public InvalidRmsUrlException(string s) : base(DrmStrings.InvalidRmsUrl(s))
		{
			this.s = s;
		}

		public InvalidRmsUrlException(string s, Exception innerException) : base(DrmStrings.InvalidRmsUrl(s), innerException)
		{
			this.s = s;
		}

		protected InvalidRmsUrlException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.s = (string)info.GetValue("s", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("s", this.s);
		}

		public string S
		{
			get
			{
				return this.s;
			}
		}

		private readonly string s;
	}
}
