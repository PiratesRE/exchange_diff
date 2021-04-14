using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ReportingTask
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidQueryException : ReportingException
	{
		public InvalidQueryException(int number) : base(Strings.InvalidQueryException(number))
		{
			this.number = number;
		}

		public InvalidQueryException(int number, Exception innerException) : base(Strings.InvalidQueryException(number), innerException)
		{
			this.number = number;
		}

		protected InvalidQueryException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.number = (int)info.GetValue("number", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("number", this.number);
		}

		public int Number
		{
			get
			{
				return this.number;
			}
		}

		private readonly int number;
	}
}
