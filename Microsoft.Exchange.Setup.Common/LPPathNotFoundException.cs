using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Setup.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class LPPathNotFoundException : LocalizedException
	{
		public LPPathNotFoundException() : base(Strings.LanguagePackPathNotFoundError)
		{
		}

		public LPPathNotFoundException(Exception innerException) : base(Strings.LanguagePackPathNotFoundError, innerException)
		{
		}

		protected LPPathNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
