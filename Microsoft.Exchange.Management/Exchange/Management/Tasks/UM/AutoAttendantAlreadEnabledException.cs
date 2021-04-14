using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AutoAttendantAlreadEnabledException : LocalizedException
	{
		public AutoAttendantAlreadEnabledException(string s) : base(Strings.AutoAttendantAlreadEnabledException(s))
		{
			this.s = s;
		}

		public AutoAttendantAlreadEnabledException(string s, Exception innerException) : base(Strings.AutoAttendantAlreadEnabledException(s), innerException)
		{
			this.s = s;
		}

		protected AutoAttendantAlreadEnabledException(SerializationInfo info, StreamingContext context) : base(info, context)
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
