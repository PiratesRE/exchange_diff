using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DialPlanNotFoundException : LocalizedException
	{
		public DialPlanNotFoundException(string s) : base(Strings.ExceptionDialPlanNotFound(s))
		{
			this.s = s;
		}

		public DialPlanNotFoundException(string s, Exception innerException) : base(Strings.ExceptionDialPlanNotFound(s), innerException)
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
