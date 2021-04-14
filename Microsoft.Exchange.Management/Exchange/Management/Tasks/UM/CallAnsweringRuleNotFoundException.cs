using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CallAnsweringRuleNotFoundException : LocalizedException
	{
		public CallAnsweringRuleNotFoundException(string identity) : base(Strings.CallAnsweringRuleNotFoundException(identity))
		{
			this.identity = identity;
		}

		public CallAnsweringRuleNotFoundException(string identity, Exception innerException) : base(Strings.CallAnsweringRuleNotFoundException(identity), innerException)
		{
			this.identity = identity;
		}

		protected CallAnsweringRuleNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.identity = (string)info.GetValue("identity", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("identity", this.identity);
		}

		public string Identity
		{
			get
			{
				return this.identity;
			}
		}

		private readonly string identity;
	}
}
