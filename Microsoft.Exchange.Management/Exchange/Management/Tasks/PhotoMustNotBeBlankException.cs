using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class PhotoMustNotBeBlankException : LocalizedException
	{
		public PhotoMustNotBeBlankException() : base(Strings.PhotoMustNotBeBlank)
		{
		}

		public PhotoMustNotBeBlankException(Exception innerException) : base(Strings.PhotoMustNotBeBlank, innerException)
		{
		}

		protected PhotoMustNotBeBlankException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
