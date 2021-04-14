using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AssociatedUserMailboxExistException : LocalizedException
	{
		public AssociatedUserMailboxExistException() : base(Strings.ErrorAssociatedUserMailboxExist)
		{
		}

		public AssociatedUserMailboxExistException(Exception innerException) : base(Strings.ErrorAssociatedUserMailboxExist, innerException)
		{
		}

		protected AssociatedUserMailboxExistException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
