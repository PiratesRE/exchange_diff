using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FileCheckEseutilErrorException : FileCheckException
	{
		public FileCheckEseutilErrorException(string errorMessage) : base(ReplayStrings.FileCheckEseutilError(errorMessage))
		{
			this.errorMessage = errorMessage;
		}

		public FileCheckEseutilErrorException(string errorMessage, Exception innerException) : base(ReplayStrings.FileCheckEseutilError(errorMessage), innerException)
		{
			this.errorMessage = errorMessage;
		}

		protected FileCheckEseutilErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
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
