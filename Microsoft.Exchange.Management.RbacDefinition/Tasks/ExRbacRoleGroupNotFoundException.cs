using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.RbacDefinition;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ExRbacRoleGroupNotFoundException : LocalizedException
	{
		public ExRbacRoleGroupNotFoundException(Guid guid, string groupName) : base(Strings.ExRbacRoleGroupNotFoundException(guid, groupName))
		{
			this.guid = guid;
			this.groupName = groupName;
		}

		public ExRbacRoleGroupNotFoundException(Guid guid, string groupName, Exception innerException) : base(Strings.ExRbacRoleGroupNotFoundException(guid, groupName), innerException)
		{
			this.guid = guid;
			this.groupName = groupName;
		}

		protected ExRbacRoleGroupNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.guid = (Guid)info.GetValue("guid", typeof(Guid));
			this.groupName = (string)info.GetValue("groupName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("guid", this.guid);
			info.AddValue("groupName", this.groupName);
		}

		public Guid Guid
		{
			get
			{
				return this.guid;
			}
		}

		public string GroupName
		{
			get
			{
				return this.groupName;
			}
		}

		private readonly Guid guid;

		private readonly string groupName;
	}
}
