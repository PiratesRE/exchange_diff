using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnableToCreateGatewayObjectException : LocalizedException
	{
		public UnableToCreateGatewayObjectException(string msg) : base(Strings.UnableToCreateGatewayObjectException(msg))
		{
			this.msg = msg;
		}

		public UnableToCreateGatewayObjectException(string msg, Exception innerException) : base(Strings.UnableToCreateGatewayObjectException(msg), innerException)
		{
			this.msg = msg;
		}

		protected UnableToCreateGatewayObjectException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.msg = (string)info.GetValue("msg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("msg", this.msg);
		}

		public string Msg
		{
			get
			{
				return this.msg;
			}
		}

		private readonly string msg;
	}
}
