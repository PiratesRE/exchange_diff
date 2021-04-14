using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ReportingTask
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DatabaseException : ReportingException
	{
		public DatabaseException(int number, string error) : base(Strings.DatabaseException(number, error))
		{
			this.number = number;
			this.error = error;
		}

		public DatabaseException(int number, string error, Exception innerException) : base(Strings.DatabaseException(number, error), innerException)
		{
			this.number = number;
			this.error = error;
		}

		protected DatabaseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.number = (int)info.GetValue("number", typeof(int));
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("number", this.number);
			info.AddValue("error", this.error);
		}

		public int Number
		{
			get
			{
				return this.number;
			}
		}

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly int number;

		private readonly string error;
	}
}
