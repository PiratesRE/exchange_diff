using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MicrosoftExchangeContainerNotFoundException : LocalizedException
	{
		public MicrosoftExchangeContainerNotFoundException() : base(Strings.MicrosoftExchangeContainerNotFoundException)
		{
		}

		public MicrosoftExchangeContainerNotFoundException(Exception innerException) : base(Strings.MicrosoftExchangeContainerNotFoundException, innerException)
		{
		}

		protected MicrosoftExchangeContainerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
