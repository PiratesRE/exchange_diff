using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class UnexpectedNullFromSprocException : LocalizedException
	{
		public UnexpectedNullFromSprocException(string msg) : base(MigrationMonitorStrings.ErrorUnexpectedNullFromSproc(msg))
		{
			this.msg = msg;
		}

		public UnexpectedNullFromSprocException(string msg, Exception innerException) : base(MigrationMonitorStrings.ErrorUnexpectedNullFromSproc(msg), innerException)
		{
			this.msg = msg;
		}

		protected UnexpectedNullFromSprocException(SerializationInfo info, StreamingContext context) : base(info, context)
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
