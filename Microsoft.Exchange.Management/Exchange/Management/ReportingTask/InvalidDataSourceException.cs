using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ReportingTask
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidDataSourceException : ReportingException
	{
		public InvalidDataSourceException(int number) : base(Strings.InvalidDataSourceException(number))
		{
			this.number = number;
		}

		public InvalidDataSourceException(int number, Exception innerException) : base(Strings.InvalidDataSourceException(number), innerException)
		{
			this.number = number;
		}

		protected InvalidDataSourceException(SerializationInfo info, StreamingContext context) : base(info, context)
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
