using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class UnableToMoveUCSMigratedMailboxToDownlevelException : RecipientTaskException
	{
		public UnableToMoveUCSMigratedMailboxToDownlevelException(string name) : base(Strings.UnableToMoveUCSMigratedMailboxToDownlevelError(name))
		{
			this.name = name;
		}

		public UnableToMoveUCSMigratedMailboxToDownlevelException(string name, Exception innerException) : base(Strings.UnableToMoveUCSMigratedMailboxToDownlevelError(name), innerException)
		{
			this.name = name;
		}

		protected UnableToMoveUCSMigratedMailboxToDownlevelException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.name = (string)info.GetValue("name", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("name", this.name);
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		private readonly string name;
	}
}
