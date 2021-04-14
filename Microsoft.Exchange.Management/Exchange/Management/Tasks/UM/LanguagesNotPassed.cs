using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class LanguagesNotPassed : LocalizedException
	{
		public LanguagesNotPassed() : base(Strings.LanguagesNotPassed)
		{
		}

		public LanguagesNotPassed(Exception innerException) : base(Strings.LanguagesNotPassed, innerException)
		{
		}

		protected LanguagesNotPassed(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
