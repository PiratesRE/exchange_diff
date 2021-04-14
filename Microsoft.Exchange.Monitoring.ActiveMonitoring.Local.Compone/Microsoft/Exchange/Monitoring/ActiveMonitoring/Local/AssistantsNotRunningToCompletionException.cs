using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Local
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AssistantsNotRunningToCompletionException : LocalizedException
	{
		public AssistantsNotRunningToCompletionException(string error) : base(Strings.AssistantsNotRunningToCompletionError(error))
		{
			this.error = error;
		}

		public AssistantsNotRunningToCompletionException(string error, Exception innerException) : base(Strings.AssistantsNotRunningToCompletionError(error), innerException)
		{
			this.error = error;
		}

		protected AssistantsNotRunningToCompletionException(SerializationInfo info, StreamingContext context) : base(info, context)
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
