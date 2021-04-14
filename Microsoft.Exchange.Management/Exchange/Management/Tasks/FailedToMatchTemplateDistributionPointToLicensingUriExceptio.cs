using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedToMatchTemplateDistributionPointToLicensingUriException : LocalizedException
	{
		public FailedToMatchTemplateDistributionPointToLicensingUriException(Guid templateGuid, Uri templateDp, Uri tpdDp) : base(Strings.FailedToMatchTemplateDistributionPointToLicensingUri(templateGuid, templateDp, tpdDp))
		{
			this.templateGuid = templateGuid;
			this.templateDp = templateDp;
			this.tpdDp = tpdDp;
		}

		public FailedToMatchTemplateDistributionPointToLicensingUriException(Guid templateGuid, Uri templateDp, Uri tpdDp, Exception innerException) : base(Strings.FailedToMatchTemplateDistributionPointToLicensingUri(templateGuid, templateDp, tpdDp), innerException)
		{
			this.templateGuid = templateGuid;
			this.templateDp = templateDp;
			this.tpdDp = tpdDp;
		}

		protected FailedToMatchTemplateDistributionPointToLicensingUriException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.templateGuid = (Guid)info.GetValue("templateGuid", typeof(Guid));
			this.templateDp = (Uri)info.GetValue("templateDp", typeof(Uri));
			this.tpdDp = (Uri)info.GetValue("tpdDp", typeof(Uri));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("templateGuid", this.templateGuid);
			info.AddValue("templateDp", this.templateDp);
			info.AddValue("tpdDp", this.tpdDp);
		}

		public Guid TemplateGuid
		{
			get
			{
				return this.templateGuid;
			}
		}

		public Uri TemplateDp
		{
			get
			{
				return this.templateDp;
			}
		}

		public Uri TpdDp
		{
			get
			{
				return this.tpdDp;
			}
		}

		private readonly Guid templateGuid;

		private readonly Uri templateDp;

		private readonly Uri tpdDp;
	}
}
