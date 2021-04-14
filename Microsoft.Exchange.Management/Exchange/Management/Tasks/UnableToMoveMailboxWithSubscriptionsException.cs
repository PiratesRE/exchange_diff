using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class UnableToMoveMailboxWithSubscriptionsException : RecipientTaskException
	{
		public UnableToMoveMailboxWithSubscriptionsException(string name) : base(Strings.UnableToMoveMailboxWithSubscriptions(name))
		{
			this.name = name;
		}

		public UnableToMoveMailboxWithSubscriptionsException(string name, Exception innerException) : base(Strings.UnableToMoveMailboxWithSubscriptions(name), innerException)
		{
			this.name = name;
		}

		protected UnableToMoveMailboxWithSubscriptionsException(SerializationInfo info, StreamingContext context) : base(info, context)
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
