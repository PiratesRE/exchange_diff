using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidContentFilterPermanentException : ContentFilterPermanentException
	{
		public InvalidContentFilterPermanentException(string msg) : base(MrsStrings.ContentFilterIsInvalid(msg))
		{
			this.msg = msg;
		}

		public InvalidContentFilterPermanentException(string msg, Exception innerException) : base(MrsStrings.ContentFilterIsInvalid(msg), innerException)
		{
			this.msg = msg;
		}

		protected InvalidContentFilterPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
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
