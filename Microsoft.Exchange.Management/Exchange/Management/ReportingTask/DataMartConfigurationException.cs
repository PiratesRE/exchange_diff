using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.ReportingTask
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DataMartConfigurationException : ReportingException
	{
		public DataMartConfigurationException(LocalizedString message) : base(message)
		{
		}

		public DataMartConfigurationException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected DataMartConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
