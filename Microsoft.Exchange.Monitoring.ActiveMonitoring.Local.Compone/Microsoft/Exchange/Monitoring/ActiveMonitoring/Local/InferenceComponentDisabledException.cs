using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Local
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InferenceComponentDisabledException : LocalizedException
	{
		public InferenceComponentDisabledException(string details) : base(Strings.InferenceComponentDisabled(details))
		{
			this.details = details;
		}

		public InferenceComponentDisabledException(string details, Exception innerException) : base(Strings.InferenceComponentDisabled(details), innerException)
		{
			this.details = details;
		}

		protected InferenceComponentDisabledException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.details = (string)info.GetValue("details", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("details", this.details);
		}

		public string Details
		{
			get
			{
				return this.details;
			}
		}

		private readonly string details;
	}
}
