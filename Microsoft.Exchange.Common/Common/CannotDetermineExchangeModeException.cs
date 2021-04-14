using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotDetermineExchangeModeException : LocalizedException
	{
		public CannotDetermineExchangeModeException(string reason) : base(CommonStrings.CannotDetermineExchangeModeException(reason))
		{
			this.reason = reason;
		}

		public CannotDetermineExchangeModeException(string reason, Exception innerException) : base(CommonStrings.CannotDetermineExchangeModeException(reason), innerException)
		{
			this.reason = reason;
		}

		protected CannotDetermineExchangeModeException(SerializationInfo info, StreamingContext context) : base(info, context)
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
