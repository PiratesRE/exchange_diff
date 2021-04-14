using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InvalidTenantGuidException : LocalizedException
	{
		public InvalidTenantGuidException(Guid tenantGuid) : base(Strings.InvalidTenantGuidException(tenantGuid))
		{
			this.tenantGuid = tenantGuid;
		}

		public InvalidTenantGuidException(Guid tenantGuid, Exception innerException) : base(Strings.InvalidTenantGuidException(tenantGuid), innerException)
		{
			this.tenantGuid = tenantGuid;
		}

		protected InvalidTenantGuidException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.tenantGuid = (Guid)info.GetValue("tenantGuid", typeof(Guid));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("tenantGuid", this.tenantGuid);
		}

		public Guid TenantGuid
		{
			get
			{
				return this.tenantGuid;
			}
		}

		private readonly Guid tenantGuid;
	}
}
