using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class EWSUMMailboxAccessException : LocalizedException
	{
		public EWSUMMailboxAccessException(string reason) : base(Strings.EWSUMMailboxAccessException(reason))
		{
			this.reason = reason;
		}

		public EWSUMMailboxAccessException(string reason, Exception innerException) : base(Strings.EWSUMMailboxAccessException(reason), innerException)
		{
			this.reason = reason;
		}

		protected EWSUMMailboxAccessException(SerializationInfo info, StreamingContext context) : base(info, context)
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
