using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ParameterLengthExceededPermanentException : MailboxReplicationPermanentException
	{
		public ParameterLengthExceededPermanentException(string parameterName, int maxValueLength) : base(Strings.ErrorMaxParameterLengthExceeded(parameterName, maxValueLength))
		{
			this.parameterName = parameterName;
			this.maxValueLength = maxValueLength;
		}

		public ParameterLengthExceededPermanentException(string parameterName, int maxValueLength, Exception innerException) : base(Strings.ErrorMaxParameterLengthExceeded(parameterName, maxValueLength), innerException)
		{
			this.parameterName = parameterName;
			this.maxValueLength = maxValueLength;
		}

		protected ParameterLengthExceededPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.parameterName = (string)info.GetValue("parameterName", typeof(string));
			this.maxValueLength = (int)info.GetValue("maxValueLength", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("parameterName", this.parameterName);
			info.AddValue("maxValueLength", this.maxValueLength);
		}

		public string ParameterName
		{
			get
			{
				return this.parameterName;
			}
		}

		public int MaxValueLength
		{
			get
			{
				return this.maxValueLength;
			}
		}

		private readonly string parameterName;

		private readonly int maxValueLength;
	}
}
