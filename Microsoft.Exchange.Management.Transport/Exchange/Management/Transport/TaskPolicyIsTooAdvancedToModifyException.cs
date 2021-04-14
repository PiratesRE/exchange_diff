using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Transport
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class TaskPolicyIsTooAdvancedToModifyException : TaskObjectIsTooAdvancedException
	{
		public TaskPolicyIsTooAdvancedToModifyException(string identity) : base(Strings.ErrorTaskPolicyIsTooAdvancedToModify(identity))
		{
			this.identity = identity;
		}

		public TaskPolicyIsTooAdvancedToModifyException(string identity, Exception innerException) : base(Strings.ErrorTaskPolicyIsTooAdvancedToModify(identity), innerException)
		{
			this.identity = identity;
		}

		protected TaskPolicyIsTooAdvancedToModifyException(SerializationInfo info, StreamingContext context) : base(info, context)
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
