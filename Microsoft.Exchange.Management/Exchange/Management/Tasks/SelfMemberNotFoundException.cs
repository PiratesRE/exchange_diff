using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SelfMemberNotFoundException : RecipientTaskException
	{
		public SelfMemberNotFoundException(string group) : base(Strings.SelfMemberNotFoundException(group))
		{
			this.group = group;
		}

		public SelfMemberNotFoundException(string group, Exception innerException) : base(Strings.SelfMemberNotFoundException(group), innerException)
		{
			this.group = group;
		}

		protected SelfMemberNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
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
