using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MissingParameterException : LocalizedException
	{
		public MissingParameterException(string parameter) : base(Strings.messageMissingParameterException(parameter))
		{
			this.parameter = parameter;
		}

		public MissingParameterException(string parameter, Exception innerException) : base(Strings.messageMissingParameterException(parameter), innerException)
		{
			this.parameter = parameter;
		}

		protected MissingParameterException(SerializationInfo info, StreamingContext context) : base(info, context)
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
