using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class EitherSenderOrRmsOnlineParametersMustBeSpecifiedException : LocalizedException
	{
		public EitherSenderOrRmsOnlineParametersMustBeSpecifiedException() : base(Strings.EitherSenderOrRmsOnlineParametersMustBeSpecified)
		{
		}

		public EitherSenderOrRmsOnlineParametersMustBeSpecifiedException(Exception innerException) : base(Strings.EitherSenderOrRmsOnlineParametersMustBeSpecified, innerException)
		{
		}

		protected EitherSenderOrRmsOnlineParametersMustBeSpecifiedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
