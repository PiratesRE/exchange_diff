using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Local
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AssistantsActiveDatabaseException : LocalizedException
	{
		public AssistantsActiveDatabaseException(string error) : base(Strings.AssistantsActiveDatabaseError(error))
		{
			this.error = error;
		}

		public AssistantsActiveDatabaseException(string error, Exception innerException) : base(Strings.AssistantsActiveDatabaseError(error), innerException)
		{
			this.error = error;
		}

		protected AssistantsActiveDatabaseException(SerializationInfo info, StreamingContext context) : base(info, context)
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
