using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DlpPolicyScriptExecutionException : LocalizedException
	{
		public DlpPolicyScriptExecutionException(string error) : base(Strings.DlpPolicyScriptExecutionError(error))
		{
			this.error = error;
		}

		public DlpPolicyScriptExecutionException(string error, Exception innerException) : base(Strings.DlpPolicyScriptExecutionError(error), innerException)
		{
			this.error = error;
		}

		protected DlpPolicyScriptExecutionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("error", this.error);
		}

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly string error;
	}
}
