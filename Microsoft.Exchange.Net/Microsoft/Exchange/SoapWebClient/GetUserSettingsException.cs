using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.SoapWebClient
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class GetUserSettingsException : LocalizedException
	{
		public GetUserSettingsException(LocalizedString message) : base(message)
		{
		}

		public GetUserSettingsException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected GetUserSettingsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
