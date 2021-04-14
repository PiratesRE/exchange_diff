using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Local
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ParseDiagnosticsStringException : LocalizedException
	{
		public ParseDiagnosticsStringException(string error) : base(Strings.ParseDiagnosticsStringError(error))
		{
			this.error = error;
		}

		public ParseDiagnosticsStringException(string error, Exception innerException) : base(Strings.ParseDiagnosticsStringError(error), innerException)
		{
			this.error = error;
		}

		protected ParseDiagnosticsStringException(SerializationInfo info, StreamingContext context) : base(info, context)
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
