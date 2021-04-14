using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ErrorMoveUnableToGetCopyStatusException : LocalizedException
	{
		public ErrorMoveUnableToGetCopyStatusException(string server, string errorMsg) : base(Strings.ErrorMoveUnableToGetCopyStatusException(server, errorMsg))
		{
			this.server = server;
			this.errorMsg = errorMsg;
		}

		public ErrorMoveUnableToGetCopyStatusException(string server, string errorMsg, Exception innerException) : base(Strings.ErrorMoveUnableToGetCopyStatusException(server, errorMsg), innerException)
		{
			this.server = server;
			this.errorMsg = errorMsg;
		}

		protected ErrorMoveUnableToGetCopyStatusException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.server = (string)info.GetValue("server", typeof(string));
			this.errorMsg = (string)info.GetValue("errorMsg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("server", this.server);
			info.AddValue("errorMsg", this.errorMsg);
		}

		public string Server
		{
			get
			{
				return this.server;
			}
		}

		public string ErrorMsg
		{
			get
			{
				return this.errorMsg;
			}
		}

		private readonly string server;

		private readonly string errorMsg;
	}
}
