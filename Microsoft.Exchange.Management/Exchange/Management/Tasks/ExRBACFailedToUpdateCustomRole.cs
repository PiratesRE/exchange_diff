using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ExRBACFailedToUpdateCustomRole : LocalizedException
	{
		public ExRBACFailedToUpdateCustomRole(string roleName, string targetCustomRoleName, string error) : base(Strings.ExRBACFailedToUpdateCustomRole(roleName, targetCustomRoleName, error))
		{
			this.roleName = roleName;
			this.targetCustomRoleName = targetCustomRoleName;
			this.error = error;
		}

		public ExRBACFailedToUpdateCustomRole(string roleName, string targetCustomRoleName, string error, Exception innerException) : base(Strings.ExRBACFailedToUpdateCustomRole(roleName, targetCustomRoleName, error), innerException)
		{
			this.roleName = roleName;
			this.targetCustomRoleName = targetCustomRoleName;
			this.error = error;
		}

		protected ExRBACFailedToUpdateCustomRole(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.roleName = (string)info.GetValue("roleName", typeof(string));
			this.targetCustomRoleName = (string)info.GetValue("targetCustomRoleName", typeof(string));
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("roleName", this.roleName);
			info.AddValue("targetCustomRoleName", this.targetCustomRoleName);
			info.AddValue("error", this.error);
		}

		public string RoleName
		{
			get
			{
				return this.roleName;
			}
		}

		public string TargetCustomRoleName
		{
			get
			{
				return this.targetCustomRoleName;
			}
		}

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly string roleName;

		private readonly string targetCustomRoleName;

		private readonly string error;
	}
}
