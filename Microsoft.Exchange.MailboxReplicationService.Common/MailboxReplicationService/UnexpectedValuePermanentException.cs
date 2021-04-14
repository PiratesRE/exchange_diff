using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnexpectedValuePermanentException : MailboxReplicationPermanentException
	{
		public UnexpectedValuePermanentException(string value, string parameterName) : base(MrsStrings.UnexpectedValue(value, parameterName))
		{
			this.value = value;
			this.parameterName = parameterName;
		}

		public UnexpectedValuePermanentException(string value, string parameterName, Exception innerException) : base(MrsStrings.UnexpectedValue(value, parameterName), innerException)
		{
			this.value = value;
			this.parameterName = parameterName;
		}

		protected UnexpectedValuePermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.value = (string)info.GetValue("value", typeof(string));
			this.parameterName = (string)info.GetValue("parameterName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("value", this.value);
			info.AddValue("parameterName", this.parameterName);
		}

		public string Value
		{
			get
			{
				return this.value;
			}
		}

		public string ParameterName
		{
			get
			{
				return this.parameterName;
			}
		}

		private readonly string value;

		private readonly string parameterName;
	}
}
