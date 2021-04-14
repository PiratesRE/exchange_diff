using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class LegacyUmUserException : LocalizedException
	{
		public LegacyUmUserException(string legacyDN) : base(Strings.LegacyUmUser(legacyDN))
		{
			this.legacyDN = legacyDN;
		}

		public LegacyUmUserException(string legacyDN, Exception innerException) : base(Strings.LegacyUmUser(legacyDN), innerException)
		{
			this.legacyDN = legacyDN;
		}

		protected LegacyUmUserException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.legacyDN = (string)info.GetValue("legacyDN", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("legacyDN", this.legacyDN);
		}

		public string LegacyDN
		{
			get
			{
				return this.legacyDN;
			}
		}

		private readonly string legacyDN;
	}
}
