using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SelfMemberAlreadyExistsException : RecipientTaskException
	{
		public SelfMemberAlreadyExistsException(string group) : base(Strings.SelfMemberAlreadyExistsException(group))
		{
			this.group = group;
		}

		public SelfMemberAlreadyExistsException(string group, Exception innerException) : base(Strings.SelfMemberAlreadyExistsException(group), innerException)
		{
			this.group = group;
		}

		protected SelfMemberAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.group = (string)info.GetValue("group", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("group", this.group);
		}

		public string Group
		{
			get
			{
				return this.group;
			}
		}

		private readonly string group;
	}
}
