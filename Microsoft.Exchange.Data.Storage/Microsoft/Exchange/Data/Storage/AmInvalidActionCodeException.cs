using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Storage
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmInvalidActionCodeException : AmServerException
	{
		public AmInvalidActionCodeException(int actionCode, string member, string value) : base(ServerStrings.AmInvalidActionCodeException(actionCode, member, value))
		{
			this.actionCode = actionCode;
			this.member = member;
			this.value = value;
		}

		public AmInvalidActionCodeException(int actionCode, string member, string value, Exception innerException) : base(ServerStrings.AmInvalidActionCodeException(actionCode, member, value), innerException)
		{
			this.actionCode = actionCode;
			this.member = member;
			this.value = value;
		}

		protected AmInvalidActionCodeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.actionCode = (int)info.GetValue("actionCode", typeof(int));
			this.member = (string)info.GetValue("member", typeof(string));
			this.value = (string)info.GetValue("value", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("actionCode", this.actionCode);
			info.AddValue("member", this.member);
			info.AddValue("value", this.value);
		}

		public int ActionCode
		{
			get
			{
				return this.actionCode;
			}
		}

		public string Member
		{
			get
			{
				return this.member;
			}
		}

		public string Value
		{
			get
			{
				return this.value;
			}
		}

		private readonly int actionCode;

		private readonly string member;

		private readonly string value;
	}
}
