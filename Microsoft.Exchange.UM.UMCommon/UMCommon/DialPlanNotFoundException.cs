using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DialPlanNotFoundException : LocalizedException
	{
		public DialPlanNotFoundException(string s) : base(Strings.DialPlanNotFound(s))
		{
			this.s = s;
		}

		public DialPlanNotFoundException(string s, Exception innerException) : base(Strings.DialPlanNotFound(s), innerException)
		{
			this.s = s;
		}

		protected DialPlanNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.s = (string)info.GetValue("s", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("s", this.s);
		}

		public string S
		{
			get
			{
				return this.s;
			}
		}

		private readonly string s;
	}
}
