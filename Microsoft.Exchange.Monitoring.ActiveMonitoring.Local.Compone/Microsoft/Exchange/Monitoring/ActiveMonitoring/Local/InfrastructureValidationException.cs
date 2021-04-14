using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Local
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InfrastructureValidationException : LocalizedException
	{
		public InfrastructureValidationException(string error) : base(Strings.InfrastructureValidationError(error))
		{
			this.error = error;
		}

		public InfrastructureValidationException(string error, Exception innerException) : base(Strings.InfrastructureValidationError(error), innerException)
		{
			this.error = error;
		}

		protected InfrastructureValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
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
