using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AdminLimitExceededException : ADOperationException
	{
		public AdminLimitExceededException() : base(DirectoryStrings.ExceptionAdminLimitExceeded)
		{
		}

		public AdminLimitExceededException(Exception innerException) : base(DirectoryStrings.ExceptionAdminLimitExceeded, innerException)
		{
		}

		protected AdminLimitExceededException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
