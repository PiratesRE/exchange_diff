using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FileCheckIsamErrorException : FileCheckException
	{
		public FileCheckIsamErrorException(string errorMessage) : base(ReplayStrings.FileCheckIsamError(errorMessage))
		{
			this.errorMessage = errorMessage;
		}

		public FileCheckIsamErrorException(string errorMessage, Exception innerException) : base(ReplayStrings.FileCheckIsamError(errorMessage), innerException)
		{
			this.errorMessage = errorMessage;
		}

		protected FileCheckIsamErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.errorMessage = (string)info.GetValue("errorMessage", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("errorMessage", this.errorMessage);
		}

		public string ErrorMessage
		{
			get
			{
				return this.errorMessage;
			}
		}

		private readonly string errorMessage;
	}
}
