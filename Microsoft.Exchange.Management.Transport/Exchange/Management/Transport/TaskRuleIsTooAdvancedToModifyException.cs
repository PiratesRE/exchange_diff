using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Transport
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class TaskRuleIsTooAdvancedToModifyException : TaskObjectIsTooAdvancedException
	{
		public TaskRuleIsTooAdvancedToModifyException(string identity) : base(Strings.ErrorTaskRuleIsTooAdvancedToModify(identity))
		{
			this.identity = identity;
		}

		public TaskRuleIsTooAdvancedToModifyException(string identity, Exception innerException) : base(Strings.ErrorTaskRuleIsTooAdvancedToModify(identity), innerException)
		{
			this.identity = identity;
		}

		protected TaskRuleIsTooAdvancedToModifyException(SerializationInfo info, StreamingContext context) : base(info, context)
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
