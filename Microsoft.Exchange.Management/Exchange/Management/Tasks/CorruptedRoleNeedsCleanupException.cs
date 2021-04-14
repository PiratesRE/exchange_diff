using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CorruptedRoleNeedsCleanupException : LocalizedException
	{
		public CorruptedRoleNeedsCleanupException(string roleId, string error) : base(Strings.CorruptedRoleNeedsCleanupException(roleId, error))
		{
			this.roleId = roleId;
			this.error = error;
		}

		public CorruptedRoleNeedsCleanupException(string roleId, string error, Exception innerException) : base(Strings.CorruptedRoleNeedsCleanupException(roleId, error), innerException)
		{
			this.roleId = roleId;
			this.error = error;
		}

		protected CorruptedRoleNeedsCleanupException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.roleId = (string)info.GetValue("roleId", typeof(string));
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("roleId", this.roleId);
			info.AddValue("error", this.error);
		}

		public string RoleId
		{
			get
			{
				return this.roleId;
			}
		}

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly string roleId;

		private readonly string error;
	}
}
