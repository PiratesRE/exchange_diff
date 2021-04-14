using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.Prompts.Provisioning
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class PublishingException : LocalizedException
	{
		public PublishingException(LocalizedString msg) : base(Strings.PublishingException(msg))
		{
			this.msg = msg;
		}

		public PublishingException(LocalizedString msg, Exception innerException) : base(Strings.PublishingException(msg), innerException)
		{
			this.msg = msg;
		}

		protected PublishingException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.msg = (LocalizedString)info.GetValue("msg", typeof(LocalizedString));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("msg", this.msg);
		}

		public LocalizedString Msg
		{
			get
			{
				return this.msg;
			}
		}

		private readonly LocalizedString msg;
	}
}
