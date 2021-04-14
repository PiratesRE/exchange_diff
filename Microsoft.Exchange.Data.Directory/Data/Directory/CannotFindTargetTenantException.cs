using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotFindTargetTenantException : ADOperationException
	{
		public CannotFindTargetTenantException(string oldTenant, string newPartition, string guid) : base(DirectoryStrings.NoMatchingTenantInTargetPartition(oldTenant, newPartition, guid))
		{
			this.oldTenant = oldTenant;
			this.newPartition = newPartition;
			this.guid = guid;
		}

		public CannotFindTargetTenantException(string oldTenant, string newPartition, string guid, Exception innerException) : base(DirectoryStrings.NoMatchingTenantInTargetPartition(oldTenant, newPartition, guid), innerException)
		{
			this.oldTenant = oldTenant;
			this.newPartition = newPartition;
			this.guid = guid;
		}

		protected CannotFindTargetTenantException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.oldTenant = (string)info.GetValue("oldTenant", typeof(string));
			this.newPartition = (string)info.GetValue("newPartition", typeof(string));
			this.guid = (string)info.GetValue("guid", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("oldTenant", this.oldTenant);
			info.AddValue("newPartition", this.newPartition);
			info.AddValue("guid", this.guid);
		}

		public string OldTenant
		{
			get
			{
				return this.oldTenant;
			}
		}

		public string NewPartition
		{
			get
			{
				return this.newPartition;
			}
		}

		public string Guid
		{
			get
			{
				return this.guid;
			}
		}

		private readonly string oldTenant;

		private readonly string newPartition;

		private readonly string guid;
	}
}
