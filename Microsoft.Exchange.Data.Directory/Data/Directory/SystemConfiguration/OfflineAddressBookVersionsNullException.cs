using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class OfflineAddressBookVersionsNullException : LocalizedException
	{
		public OfflineAddressBookVersionsNullException() : base(DirectoryStrings.OabVersionsNullException)
		{
		}

		public OfflineAddressBookVersionsNullException(Exception innerException) : base(DirectoryStrings.OabVersionsNullException, innerException)
		{
		}

		protected OfflineAddressBookVersionsNullException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
