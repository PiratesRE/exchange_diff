using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Migration
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidLegacyExchangeDnValueException : LocalizedException
	{
		public InvalidLegacyExchangeDnValueException(string parameterName) : base(Strings.InvalidLegacyExchangeDnParameterValue(parameterName))
		{
			this.parameterName = parameterName;
		}

		public InvalidLegacyExchangeDnValueException(string parameterName, Exception innerException) : base(Strings.InvalidLegacyExchangeDnParameterValue(parameterName), innerException)
		{
			this.parameterName = parameterName;
		}

		protected InvalidLegacyExchangeDnValueException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.parameterName = (string)info.GetValue("parameterName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("parameterName", this.parameterName);
		}

		public string ParameterName
		{
			get
			{
				return this.parameterName;
			}
		}

		private readonly string parameterName;
	}
}
