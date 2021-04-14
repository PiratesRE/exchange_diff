using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ErrorInvalidNameOrDescriptionParametersException : LocalizedException
	{
		public ErrorInvalidNameOrDescriptionParametersException(string parameterName, int length, int maxLength) : base(Strings.ErrorInvalidNameOrDescriptionParameters(parameterName, length, maxLength))
		{
			this.parameterName = parameterName;
			this.length = length;
			this.maxLength = maxLength;
		}

		public ErrorInvalidNameOrDescriptionParametersException(string parameterName, int length, int maxLength, Exception innerException) : base(Strings.ErrorInvalidNameOrDescriptionParameters(parameterName, length, maxLength), innerException)
		{
			this.parameterName = parameterName;
			this.length = length;
			this.maxLength = maxLength;
		}

		protected ErrorInvalidNameOrDescriptionParametersException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.parameterName = (string)info.GetValue("parameterName", typeof(string));
			this.length = (int)info.GetValue("length", typeof(int));
			this.maxLength = (int)info.GetValue("maxLength", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("parameterName", this.parameterName);
			info.AddValue("length", this.length);
			info.AddValue("maxLength", this.maxLength);
		}

		public string ParameterName
		{
			get
			{
				return this.parameterName;
			}
		}

		public int Length
		{
			get
			{
				return this.length;
			}
		}

		public int MaxLength
		{
			get
			{
				return this.maxLength;
			}
		}

		private readonly string parameterName;

		private readonly int length;

		private readonly int maxLength;
	}
}
