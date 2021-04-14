using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidUMServerStateOperationException : LocalizedException
	{
		public InvalidUMServerStateOperationException(string s) : base(Strings.InvalidUMServerStateOperationException(s))
		{
			this.s = s;
		}

		public InvalidUMServerStateOperationException(string s, Exception innerException) : base(Strings.InvalidUMServerStateOperationException(s), innerException)
		{
			this.s = s;
		}

		protected InvalidUMServerStateOperationException(SerializationInfo info, StreamingContext context) : base(info, context)
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
