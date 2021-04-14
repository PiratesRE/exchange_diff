using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.AirSync
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MobileDeviceLogException : LocalizedException
	{
		public MobileDeviceLogException(string msg) : base(Strings.MobileDeviceLogException(msg))
		{
			this.msg = msg;
		}

		public MobileDeviceLogException(string msg, Exception innerException) : base(Strings.MobileDeviceLogException(msg), innerException)
		{
			this.msg = msg;
		}

		protected MobileDeviceLogException(SerializationInfo info, StreamingContext context) : base(info, context)
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
