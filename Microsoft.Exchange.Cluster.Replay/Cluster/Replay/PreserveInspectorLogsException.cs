using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class PreserveInspectorLogsException : TransientException
	{
		public PreserveInspectorLogsException(string errorText) : base(ReplayStrings.PreserveInspectorLogsError(errorText))
		{
			this.errorText = errorText;
		}

		public PreserveInspectorLogsException(string errorText, Exception innerException) : base(ReplayStrings.PreserveInspectorLogsError(errorText), innerException)
		{
			this.errorText = errorText;
		}

		protected PreserveInspectorLogsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.errorText = (string)info.GetValue("errorText", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("errorText", this.errorText);
		}

		public string ErrorText
		{
			get
			{
				return this.errorText;
			}
		}

		private readonly string errorText;
	}
}
