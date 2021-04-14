using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Migration
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CutoverMigrationNotAllowedException : LocalizedException
	{
		public CutoverMigrationNotAllowedException(string tenantName) : base(Strings.CutoverMigrationNotAllowed(tenantName))
		{
			this.tenantName = tenantName;
		}

		public CutoverMigrationNotAllowedException(string tenantName, Exception innerException) : base(Strings.CutoverMigrationNotAllowed(tenantName), innerException)
		{
			this.tenantName = tenantName;
		}

		protected CutoverMigrationNotAllowedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.tenantName = (string)info.GetValue("tenantName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("tenantName", this.tenantName);
		}

		public string TenantName
		{
			get
			{
				return this.tenantName;
			}
		}

		private readonly string tenantName;
	}
}
