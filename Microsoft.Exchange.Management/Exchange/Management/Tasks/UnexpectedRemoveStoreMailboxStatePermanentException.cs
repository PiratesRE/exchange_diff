using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnexpectedRemoveStoreMailboxStatePermanentException : LocalizedException
	{
		public UnexpectedRemoveStoreMailboxStatePermanentException(string identity, string state, string argument) : base(Strings.ErrorUnexpectedRemoveStoreMailboxState(identity, state, argument))
		{
			this.identity = identity;
			this.state = state;
			this.argument = argument;
		}

		public UnexpectedRemoveStoreMailboxStatePermanentException(string identity, string state, string argument, Exception innerException) : base(Strings.ErrorUnexpectedRemoveStoreMailboxState(identity, state, argument), innerException)
		{
			this.identity = identity;
			this.state = state;
			this.argument = argument;
		}

		protected UnexpectedRemoveStoreMailboxStatePermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.identity = (string)info.GetValue("identity", typeof(string));
			this.state = (string)info.GetValue("state", typeof(string));
			this.argument = (string)info.GetValue("argument", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("identity", this.identity);
			info.AddValue("state", this.state);
			info.AddValue("argument", this.argument);
		}

		public string Identity
		{
			get
			{
				return this.identity;
			}
		}

		public string State
		{
			get
			{
				return this.state;
			}
		}

		public string Argument
		{
			get
			{
				return this.argument;
			}
		}

		private readonly string identity;

		private readonly string state;

		private readonly string argument;
	}
}
