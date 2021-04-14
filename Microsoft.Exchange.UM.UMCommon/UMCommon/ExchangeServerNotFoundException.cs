using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ExchangeServerNotFoundException : LocalizedException
	{
		public ExchangeServerNotFoundException(string s) : base(Strings.ExceptionExchangeServerNotFound(s))
		{
			this.s = s;
		}

		public ExchangeServerNotFoundException(string s, Exception innerException) : base(Strings.ExceptionExchangeServerNotFound(s), innerException)
		{
			this.s = s;
		}

		protected ExchangeServerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
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
