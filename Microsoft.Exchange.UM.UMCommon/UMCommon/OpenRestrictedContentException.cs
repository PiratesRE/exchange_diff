using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class OpenRestrictedContentException : LocalizedException
	{
		public OpenRestrictedContentException(string reason) : base(Strings.OpenRestrictedContentException(reason))
		{
			this.reason = reason;
		}

		public OpenRestrictedContentException(string reason, Exception innerException) : base(Strings.OpenRestrictedContentException(reason), innerException)
		{
			this.reason = reason;
		}

		protected OpenRestrictedContentException(SerializationInfo info, StreamingContext context) : base(info, context)
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
