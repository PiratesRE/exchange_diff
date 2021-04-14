using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ExRBACSaveRemoveRoleEntry : LocalizedException
	{
		public ExRBACSaveRemoveRoleEntry(string entryName, string roleId, string error) : base(Strings.ExRBACSaveRemoveRoleEntry(entryName, roleId, error))
		{
			this.entryName = entryName;
			this.roleId = roleId;
			this.error = error;
		}

		public ExRBACSaveRemoveRoleEntry(string entryName, string roleId, string error, Exception innerException) : base(Strings.ExRBACSaveRemoveRoleEntry(entryName, roleId, error), innerException)
		{
			this.entryName = entryName;
			this.roleId = roleId;
			this.error = error;
		}

		protected ExRBACSaveRemoveRoleEntry(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.entryName = (string)info.GetValue("entryName", typeof(string));
			this.roleId = (string)info.GetValue("roleId", typeof(string));
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("entryName", this.entryName);
			info.AddValue("roleId", this.roleId);
			info.AddValue("error", this.error);
		}

		public string EntryName
		{
			get
			{
				return this.entryName;
			}
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

		private readonly string entryName;

		private readonly string roleId;

		private readonly string error;
	}
}
