using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Deployment
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SetupLogInitializeException : LocalizedException
	{
		public SetupLogInitializeException(string msg) : base(Strings.SetupLogInitializeFailure(msg))
		{
			this.msg = msg;
		}

		public SetupLogInitializeException(string msg, Exception innerException) : base(Strings.SetupLogInitializeFailure(msg), innerException)
		{
			this.msg = msg;
		}

		protected SetupLogInitializeException(SerializationInfo info, StreamingContext context) : base(info, context)
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
