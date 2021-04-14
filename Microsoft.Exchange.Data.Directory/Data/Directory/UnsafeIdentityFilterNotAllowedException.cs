using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnsafeIdentityFilterNotAllowedException : DataSourceOperationException
	{
		public UnsafeIdentityFilterNotAllowedException(string filter, string orgId) : base(DirectoryStrings.ErrorUnsafeIdentityFilterNotAllowed(filter, orgId))
		{
			this.filter = filter;
			this.orgId = orgId;
		}

		public UnsafeIdentityFilterNotAllowedException(string filter, string orgId, Exception innerException) : base(DirectoryStrings.ErrorUnsafeIdentityFilterNotAllowed(filter, orgId), innerException)
		{
			this.filter = filter;
			this.orgId = orgId;
		}

		protected UnsafeIdentityFilterNotAllowedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.filter = (string)info.GetValue("filter", typeof(string));
			this.orgId = (string)info.GetValue("orgId", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("filter", this.filter);
			info.AddValue("orgId", this.orgId);
		}

		public string Filter
		{
			get
			{
				return this.filter;
			}
		}

		public string OrgId
		{
			get
			{
				return this.orgId;
			}
		}

		private readonly string filter;

		private readonly string orgId;
	}
}
