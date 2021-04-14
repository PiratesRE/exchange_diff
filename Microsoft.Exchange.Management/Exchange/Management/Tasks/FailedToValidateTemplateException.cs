using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.RightsManagementServices.Core;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedToValidateTemplateException : LocalizedException
	{
		public FailedToValidateTemplateException(Guid templateId, WellKnownErrorCode eCode) : base(Strings.FailedToValidateTemplate(templateId, eCode))
		{
			this.templateId = templateId;
			this.eCode = eCode;
		}

		public FailedToValidateTemplateException(Guid templateId, WellKnownErrorCode eCode, Exception innerException) : base(Strings.FailedToValidateTemplate(templateId, eCode), innerException)
		{
			this.templateId = templateId;
			this.eCode = eCode;
		}

		protected FailedToValidateTemplateException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.templateId = (Guid)info.GetValue("templateId", typeof(Guid));
			this.eCode = (WellKnownErrorCode)info.GetValue("eCode", typeof(WellKnownErrorCode));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("templateId", this.templateId);
			info.AddValue("eCode", this.eCode);
		}

		public Guid TemplateId
		{
			get
			{
				return this.templateId;
			}
		}

		public WellKnownErrorCode ECode
		{
			get
			{
				return this.eCode;
			}
		}

		private readonly Guid templateId;

		private readonly WellKnownErrorCode eCode;
	}
}
