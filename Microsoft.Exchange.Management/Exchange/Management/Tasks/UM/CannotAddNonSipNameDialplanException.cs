using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotAddNonSipNameDialplanException : LocalizedException
	{
		public CannotAddNonSipNameDialplanException(string s) : base(Strings.CannotAddNonSipNameDialplan(s))
		{
			this.s = s;
		}

		public CannotAddNonSipNameDialplanException(string s, Exception innerException) : base(Strings.CannotAddNonSipNameDialplan(s), innerException)
		{
			this.s = s;
		}

		protected CannotAddNonSipNameDialplanException(SerializationInfo info, StreamingContext context) : base(info, context)
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
