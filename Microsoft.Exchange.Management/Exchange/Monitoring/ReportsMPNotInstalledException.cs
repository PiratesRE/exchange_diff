using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReportsMPNotInstalledException : LocalizedException
	{
		public ReportsMPNotInstalledException() : base(Strings.ReportsMPNotInstalled)
		{
		}

		public ReportsMPNotInstalledException(Exception innerException) : base(Strings.ReportsMPNotInstalled, innerException)
		{
		}

		protected ReportsMPNotInstalledException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
