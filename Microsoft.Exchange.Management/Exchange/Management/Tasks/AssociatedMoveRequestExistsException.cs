using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AssociatedMoveRequestExistsException : LocalizedException
	{
		public AssociatedMoveRequestExistsException() : base(Strings.ErrorAssociatedMoveRequestExists)
		{
		}

		public AssociatedMoveRequestExistsException(Exception innerException) : base(Strings.ErrorAssociatedMoveRequestExists, innerException)
		{
		}

		protected AssociatedMoveRequestExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
