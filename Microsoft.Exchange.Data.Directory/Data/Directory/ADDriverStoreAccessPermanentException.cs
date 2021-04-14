using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ADDriverStoreAccessPermanentException : ADOperationException
	{
		public ADDriverStoreAccessPermanentException() : base(DirectoryStrings.ADDriverStoreAccessPermanentError)
		{
		}

		public ADDriverStoreAccessPermanentException(Exception innerException) : base(DirectoryStrings.ADDriverStoreAccessPermanentError, innerException)
		{
		}

		protected ADDriverStoreAccessPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
