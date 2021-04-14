using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Transport
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InvalidSensitiveInformationParameterNameException : InvalidContentContainsSensitiveInformationException
	{
		public InvalidSensitiveInformationParameterNameException(string invalidParameter) : base(Strings.InvalidSensitiveInformationParameterName(invalidParameter))
		{
			this.invalidParameter = invalidParameter;
		}

		public InvalidSensitiveInformationParameterNameException(string invalidParameter, Exception innerException) : base(Strings.InvalidSensitiveInformationParameterName(invalidParameter), innerException)
		{
			this.invalidParameter = invalidParameter;
		}

		protected InvalidSensitiveInformationParameterNameException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.invalidParameter = (string)info.GetValue("invalidParameter", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("invalidParameter", this.invalidParameter);
		}

		public string InvalidParameter
		{
			get
			{
				return this.invalidParameter;
			}
		}

		private readonly string invalidParameter;
	}
}
