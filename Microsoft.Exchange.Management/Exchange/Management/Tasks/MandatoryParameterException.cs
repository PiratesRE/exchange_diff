using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MandatoryParameterException : LocalizedException
	{
		public MandatoryParameterException(string parameter) : base(Strings.ExceptionMandatoryParameter(parameter))
		{
			this.parameter = parameter;
		}

		public MandatoryParameterException(string parameter, Exception innerException) : base(Strings.ExceptionMandatoryParameter(parameter), innerException)
		{
			this.parameter = parameter;
		}

		protected MandatoryParameterException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.parameter = (string)info.GetValue("parameter", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("parameter", this.parameter);
		}

		public string Parameter
		{
			get
			{
				return this.parameter;
			}
		}

		private readonly string parameter;
	}
}
