using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmRoleChangedWhileOperationIsInProgressException : AmDbActionException
	{
		public AmRoleChangedWhileOperationIsInProgressException(string roleStart, string roleCurrent) : base(ReplayStrings.AmRoleChangedWhileOperationIsInProgress(roleStart, roleCurrent))
		{
			this.roleStart = roleStart;
			this.roleCurrent = roleCurrent;
		}

		public AmRoleChangedWhileOperationIsInProgressException(string roleStart, string roleCurrent, Exception innerException) : base(ReplayStrings.AmRoleChangedWhileOperationIsInProgress(roleStart, roleCurrent), innerException)
		{
			this.roleStart = roleStart;
			this.roleCurrent = roleCurrent;
		}

		protected AmRoleChangedWhileOperationIsInProgressException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.roleStart = (string)info.GetValue("roleStart", typeof(string));
			this.roleCurrent = (string)info.GetValue("roleCurrent", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("roleStart", this.roleStart);
			info.AddValue("roleCurrent", this.roleCurrent);
		}

		public string RoleStart
		{
			get
			{
				return this.roleStart;
			}
		}

		public string RoleCurrent
		{
			get
			{
				return this.roleCurrent;
			}
		}

		private readonly string roleStart;

		private readonly string roleCurrent;
	}
}
