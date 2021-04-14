using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UserAlreadyExistsInPermissionEntryException : StoragePermanentException
	{
		public UserAlreadyExistsInPermissionEntryException(string userId) : base(Strings.ErrorUserAlreadyExistsInPermissionEntry(userId))
		{
			this.userId = userId;
		}

		public UserAlreadyExistsInPermissionEntryException(string userId, Exception innerException) : base(Strings.ErrorUserAlreadyExistsInPermissionEntry(userId), innerException)
		{
			this.userId = userId;
		}

		protected UserAlreadyExistsInPermissionEntryException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.userId = (string)info.GetValue("userId", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("userId", this.userId);
		}

		public string UserId
		{
			get
			{
				return this.userId;
			}
		}

		private readonly string userId;
	}
}
