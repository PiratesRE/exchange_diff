using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	internal class AdminAuditLogSearchException : LocalizedException
	{
		public AdminAuditLogSearchException(LocalizedString message) : base(message)
		{
		}

		public AdminAuditLogSearchException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		public AdminAuditLogSearchException(LocalizedString message, AdminAuditLogSearch searchCriteria) : base(message)
		{
			this.searchCriteria = searchCriteria;
		}

		public AdminAuditLogSearchException(LocalizedString message, Exception innerException, AdminAuditLogSearch searchCriteria) : base(message, innerException)
		{
			this.searchCriteria = searchCriteria;
		}

		public AdminAuditLogSearchException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context)
		{
		}

		public override string ToString()
		{
			return base.ToString() + Strings.AdminAuditLogSearchCriteria((this.searchCriteria == null) ? string.Empty : this.searchCriteria.ToString());
		}

		private AdminAuditLogSearch searchCriteria;
	}
}
