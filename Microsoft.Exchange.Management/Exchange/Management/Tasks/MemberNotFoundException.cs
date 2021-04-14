using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MemberNotFoundException : RecipientTaskException
	{
		public MemberNotFoundException(string id, string group) : base(Strings.MemberNotFoundException(id, group))
		{
			this.id = id;
			this.group = group;
		}

		public MemberNotFoundException(string id, string group, Exception innerException) : base(Strings.MemberNotFoundException(id, group), innerException)
		{
			this.id = id;
			this.group = group;
		}

		protected MemberNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.id = (string)info.GetValue("id", typeof(string));
			this.group = (string)info.GetValue("group", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("id", this.id);
			info.AddValue("group", this.group);
		}

		public string Id
		{
			get
			{
				return this.id;
			}
		}

		public string Group
		{
			get
			{
				return this.group;
			}
		}

		private readonly string id;

		private readonly string group;
	}
}
