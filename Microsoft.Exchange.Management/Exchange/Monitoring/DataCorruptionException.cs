using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DataCorruptionException : LocalizedException
	{
		public DataCorruptionException(string dataSource, string violation) : base(Strings.messageDataCorruptionException(dataSource, violation))
		{
			this.dataSource = dataSource;
			this.violation = violation;
		}

		public DataCorruptionException(string dataSource, string violation, Exception innerException) : base(Strings.messageDataCorruptionException(dataSource, violation), innerException)
		{
			this.dataSource = dataSource;
			this.violation = violation;
		}

		protected DataCorruptionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dataSource = (string)info.GetValue("dataSource", typeof(string));
			this.violation = (string)info.GetValue("violation", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dataSource", this.dataSource);
			info.AddValue("violation", this.violation);
		}

		public string DataSource
		{
			get
			{
				return this.dataSource;
			}
		}

		public string Violation
		{
			get
			{
				return this.violation;
			}
		}

		private readonly string dataSource;

		private readonly string violation;
	}
}
