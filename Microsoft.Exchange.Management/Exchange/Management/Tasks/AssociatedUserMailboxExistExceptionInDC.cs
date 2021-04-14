using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AssociatedUserMailboxExistExceptionInDC : LocalizedException
	{
		public AssociatedUserMailboxExistExceptionInDC() : base(Strings.ErrorAssociatedUserMailboxExistInDC)
		{
		}

		public AssociatedUserMailboxExistExceptionInDC(Exception innerException) : base(Strings.ErrorAssociatedUserMailboxExistInDC, innerException)
		{
		}

		protected AssociatedUserMailboxExistExceptionInDC(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
