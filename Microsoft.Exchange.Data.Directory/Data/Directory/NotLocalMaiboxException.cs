using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NotLocalMaiboxException : ADOperationException
	{
		public NotLocalMaiboxException() : base(DirectoryStrings.NotLocalMaiboxException)
		{
		}

		public NotLocalMaiboxException(Exception innerException) : base(DirectoryStrings.NotLocalMaiboxException, innerException)
		{
		}

		protected NotLocalMaiboxException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
