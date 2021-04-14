using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CommandExecutionException : LocalizedException
	{
		public CommandExecutionException(int innerErrorCode, string command) : base(Strings.ExceptionMDACommandExcutionError(innerErrorCode, command))
		{
			this.innerErrorCode = innerErrorCode;
			this.command = command;
		}

		public CommandExecutionException(int innerErrorCode, string command, Exception innerException) : base(Strings.ExceptionMDACommandExcutionError(innerErrorCode, command), innerException)
		{
			this.innerErrorCode = innerErrorCode;
			this.command = command;
		}

		protected CommandExecutionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.innerErrorCode = (int)info.GetValue("innerErrorCode", typeof(int));
			this.command = (string)info.GetValue("command", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("innerErrorCode", this.innerErrorCode);
			info.AddValue("command", this.command);
		}

		public int InnerErrorCode
		{
			get
			{
				return this.innerErrorCode;
			}
		}

		public string Command
		{
			get
			{
				return this.command;
			}
		}

		private readonly int innerErrorCode;

		private readonly string command;
	}
}
