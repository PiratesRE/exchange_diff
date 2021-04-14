using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ADDriverStoreAccessTransientException : ADTransientException
	{
		public ADDriverStoreAccessTransientException() : base(DirectoryStrings.ADDriverStoreAccessTransientError)
		{
		}

		public ADDriverStoreAccessTransientException(Exception innerException) : base(DirectoryStrings.ADDriverStoreAccessTransientError, innerException)
		{
		}

		protected ADDriverStoreAccessTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
