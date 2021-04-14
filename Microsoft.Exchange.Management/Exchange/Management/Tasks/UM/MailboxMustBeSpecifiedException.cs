using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MailboxMustBeSpecifiedException : LocalizedException
	{
		public MailboxMustBeSpecifiedException(string parameter) : base(Strings.MailboxMustBeSpecifiedException(parameter))
		{
			this.parameter = parameter;
		}

		public MailboxMustBeSpecifiedException(string parameter, Exception innerException) : base(Strings.MailboxMustBeSpecifiedException(parameter), innerException)
		{
			this.parameter = parameter;
		}

		protected MailboxMustBeSpecifiedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.parameter = (string)info.GetValue("parameter", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("parameter", this.parameter);
		}

		public string Parameter
		{
			get
			{
				return this.parameter;
			}
		}

		private readonly string parameter;
	}
}
