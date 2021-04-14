using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotPopulateMetabaseInformationException : LocalizedException
	{
		public CannotPopulateMetabaseInformationException(string vDir, string errorMessage) : base(Strings.CasHealthFailedToPopulateFromMetabase(vDir, errorMessage))
		{
			this.vDir = vDir;
			this.errorMessage = errorMessage;
		}

		public CannotPopulateMetabaseInformationException(string vDir, string errorMessage, Exception innerException) : base(Strings.CasHealthFailedToPopulateFromMetabase(vDir, errorMessage), innerException)
		{
			this.vDir = vDir;
			this.errorMessage = errorMessage;
		}

		protected CannotPopulateMetabaseInformationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.vDir = (string)info.GetValue("vDir", typeof(string));
			this.errorMessage = (string)info.GetValue("errorMessage", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("vDir", this.vDir);
			info.AddValue("errorMessage", this.errorMessage);
		}

		public string VDir
		{
			get
			{
				return this.vDir;
			}
		}

		public string ErrorMessage
		{
			get
			{
				return this.errorMessage;
			}
		}

		private readonly string vDir;

		private readonly string errorMessage;
	}
}
