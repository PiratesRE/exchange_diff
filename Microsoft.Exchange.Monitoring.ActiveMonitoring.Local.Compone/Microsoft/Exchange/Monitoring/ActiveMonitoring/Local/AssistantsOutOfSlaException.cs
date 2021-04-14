using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Local
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AssistantsOutOfSlaException : LocalizedException
	{
		public AssistantsOutOfSlaException(string error) : base(Strings.AssistantsOutOfSlaError(error))
		{
			this.error = error;
		}

		public AssistantsOutOfSlaException(string error, Exception innerException) : base(Strings.AssistantsOutOfSlaError(error), innerException)
		{
			this.error = error;
		}

		protected AssistantsOutOfSlaException(SerializationInfo info, StreamingContext context) : base(info, context)
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
