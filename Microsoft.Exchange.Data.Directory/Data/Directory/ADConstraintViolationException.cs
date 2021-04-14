using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ADConstraintViolationException : ADOperationException
	{
		public ADConstraintViolationException(string server, string errorMessage) : base(DirectoryStrings.ExceptionADConstraintViolation(server, errorMessage))
		{
			this.server = server;
			this.errorMessage = errorMessage;
		}

		public ADConstraintViolationException(string server, string errorMessage, Exception innerException) : base(DirectoryStrings.ExceptionADConstraintViolation(server, errorMessage), innerException)
		{
			this.server = server;
			this.errorMessage = errorMessage;
		}

		protected ADConstraintViolationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.server = (string)info.GetValue("server", typeof(string));
			this.errorMessage = (string)info.GetValue("errorMessage", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("server", this.server);
			info.AddValue("errorMessage", this.errorMessage);
		}

		public string Server
		{
			get
			{
				return this.server;
			}
		}

		public string ErrorMessage
		{
			get
			{
				return this.errorMessage;
			}
		}

		private readonly string server;

		private readonly string errorMessage;
	}
}
