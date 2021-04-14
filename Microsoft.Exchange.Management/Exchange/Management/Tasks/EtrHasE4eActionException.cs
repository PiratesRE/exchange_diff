using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class EtrHasE4eActionException : LocalizedException
	{
		public EtrHasE4eActionException(string ruleName) : base(Strings.EtrHasE4eAction(ruleName))
		{
			this.ruleName = ruleName;
		}

		public EtrHasE4eActionException(string ruleName, Exception innerException) : base(Strings.EtrHasE4eAction(ruleName), innerException)
		{
			this.ruleName = ruleName;
		}

		protected EtrHasE4eActionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.ruleName = (string)info.GetValue("ruleName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ruleName", this.ruleName);
		}

		public string RuleName
		{
			get
			{
				return this.ruleName;
			}
		}

		private readonly string ruleName;
	}
}
