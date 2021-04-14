using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class EstablishCallFailureException : LocalizedException
	{
		public EstablishCallFailureException(string s) : base(Strings.OutboundCallFailure(s))
		{
			this.s = s;
		}

		public EstablishCallFailureException(string s, Exception innerException) : base(Strings.OutboundCallFailure(s), innerException)
		{
			this.s = s;
		}

		protected EstablishCallFailureException(SerializationInfo info, StreamingContext context) : base(info, context)
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
