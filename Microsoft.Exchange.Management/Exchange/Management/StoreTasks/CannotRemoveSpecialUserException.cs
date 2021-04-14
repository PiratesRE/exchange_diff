using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotRemoveSpecialUserException : StoragePermanentException
	{
		public CannotRemoveSpecialUserException() : base(Strings.ErrorCannotRemoveSpecialUser)
		{
		}

		public CannotRemoveSpecialUserException(Exception innerException) : base(Strings.ErrorCannotRemoveSpecialUser, innerException)
		{
		}

		protected CannotRemoveSpecialUserException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
