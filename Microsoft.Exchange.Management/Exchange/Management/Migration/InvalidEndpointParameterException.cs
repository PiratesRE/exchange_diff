using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Migration
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidEndpointParameterException : LocalizedException
	{
		public InvalidEndpointParameterException(string parameterName, string type, LocalizedString reason) : base(Strings.ErrorInvalidEndpointParameter(parameterName, type, reason))
		{
			this.parameterName = parameterName;
			this.type = type;
			this.reason = reason;
		}

		public InvalidEndpointParameterException(string parameterName, string type, LocalizedString reason, Exception innerException) : base(Strings.ErrorInvalidEndpointParameter(parameterName, type, reason), innerException)
		{
			this.parameterName = parameterName;
			this.type = type;
			this.reason = reason;
		}

		protected InvalidEndpointParameterException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.parameterName = (string)info.GetValue("parameterName", typeof(string));
			this.type = (string)info.GetValue("type", typeof(string));
			this.reason = (LocalizedString)info.GetValue("reason", typeof(LocalizedString));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("parameterName", this.parameterName);
			info.AddValue("type", this.type);
			info.AddValue("reason", this.reason);
		}

		public string ParameterName
		{
			get
			{
				return this.parameterName;
			}
		}

		public string Type
		{
			get
			{
				return this.type;
			}
		}

		public LocalizedString Reason
		{
			get
			{
				return this.reason;
			}
		}

		private readonly string parameterName;

		private readonly string type;

		private readonly LocalizedString reason;
	}
}
