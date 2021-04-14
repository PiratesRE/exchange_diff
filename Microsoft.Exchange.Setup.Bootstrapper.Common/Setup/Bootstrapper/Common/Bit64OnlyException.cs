using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Setup.Bootstrapper.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class Bit64OnlyException : LocalizedException
	{
		public Bit64OnlyException() : base(Strings.Bit64Only)
		{
		}

		public Bit64OnlyException(Exception innerException) : base(Strings.Bit64Only, innerException)
		{
		}

		protected Bit64OnlyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
