using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ParameterErrorForInternetUsageException : LocalizedException
	{
		public ParameterErrorForInternetUsageException() : base(Strings.ParameterErrorForInternetUsage)
		{
		}

		public ParameterErrorForInternetUsageException(Exception innerException) : base(Strings.ParameterErrorForInternetUsage, innerException)
		{
		}

		protected ParameterErrorForInternetUsageException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
