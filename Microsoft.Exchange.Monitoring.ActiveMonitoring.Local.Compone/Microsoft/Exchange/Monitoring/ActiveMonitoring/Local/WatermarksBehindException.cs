using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Local
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class WatermarksBehindException : LocalizedException
	{
		public WatermarksBehindException(string database) : base(Strings.WatermarksBehind(database))
		{
			this.database = database;
		}

		public WatermarksBehindException(string database, Exception innerException) : base(Strings.WatermarksBehind(database), innerException)
		{
			this.database = database;
		}

		protected WatermarksBehindException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.database = (string)info.GetValue("database", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("database", this.database);
		}

		public string Database
		{
			get
			{
				return this.database;
			}
		}

		private readonly string database;
	}
}
