using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ExchangeSettingsEnableUsageException : ExchangeSettingsException
	{
		public ExchangeSettingsEnableUsageException() : base(Strings.ExchangeSettingsEnableUsage)
		{
		}

		public ExchangeSettingsEnableUsageException(Exception innerException) : base(Strings.ExchangeSettingsEnableUsage, innerException)
		{
		}

		protected ExchangeSettingsEnableUsageException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
