using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class SettingsViolationException : LocalizedException
	{
		public SettingsViolationException(int statusCode) : base(Strings.RequestSettingsException(statusCode))
		{
			this.statusCode = statusCode;
		}

		public SettingsViolationException(int statusCode, Exception innerException) : base(Strings.RequestSettingsException(statusCode), innerException)
		{
			this.statusCode = statusCode;
		}

		protected SettingsViolationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.statusCode = (int)info.GetValue("statusCode", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("statusCode", this.statusCode);
		}

		public int StatusCode
		{
			get
			{
				return this.statusCode;
			}
		}

		private readonly int statusCode;
	}
}
