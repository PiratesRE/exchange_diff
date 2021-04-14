using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Core
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NoValidDomainNameExistsInDomainSettingsException : LocalizedException
	{
		public NoValidDomainNameExistsInDomainSettingsException() : base(CoreStrings.NoValidDomainNameExistsInDomainSettings)
		{
		}

		public NoValidDomainNameExistsInDomainSettingsException(Exception innerException) : base(CoreStrings.NoValidDomainNameExistsInDomainSettings, innerException)
		{
		}

		protected NoValidDomainNameExistsInDomainSettingsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
