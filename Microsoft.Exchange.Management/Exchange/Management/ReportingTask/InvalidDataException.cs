using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ReportingTask
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidDataException : ReportingException
	{
		public InvalidDataException(string error) : base(Strings.InvalidDataException(error))
		{
			this.error = error;
		}

		public InvalidDataException(string error, Exception innerException) : base(Strings.InvalidDataException(error), innerException)
		{
			this.error = error;
		}

		protected InvalidDataException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("error", this.error);
		}

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly string error;
	}
}
