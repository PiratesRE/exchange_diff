using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CouldNotFindLocalhostDirectoryEntryException : LocalizedException
	{
		public CouldNotFindLocalhostDirectoryEntryException() : base(Strings.CouldNotFindLocalhostDirectoryEntryException)
		{
		}

		public CouldNotFindLocalhostDirectoryEntryException(Exception innerException) : base(Strings.CouldNotFindLocalhostDirectoryEntryException, innerException)
		{
		}

		protected CouldNotFindLocalhostDirectoryEntryException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
