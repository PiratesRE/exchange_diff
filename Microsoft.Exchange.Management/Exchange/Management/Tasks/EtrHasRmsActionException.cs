using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class EtrHasRmsActionException : LocalizedException
	{
		public EtrHasRmsActionException(string ruleName) : base(Strings.EtrHasRmsAction(ruleName))
		{
			this.ruleName = ruleName;
		}

		public EtrHasRmsActionException(string ruleName, Exception innerException) : base(Strings.EtrHasRmsAction(ruleName), innerException)
		{
			this.ruleName = ruleName;
		}

		protected EtrHasRmsActionException(SerializationInfo info, StreamingContext context) : base(info, context)
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
