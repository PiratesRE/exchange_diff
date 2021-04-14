using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Migration
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MigrationUserLimitExceededException : LocalizedException
	{
		public MigrationUserLimitExceededException(int count) : base(Strings.MigrationUserLimitExceeded(count))
		{
			this.count = count;
		}

		public MigrationUserLimitExceededException(int count, Exception innerException) : base(Strings.MigrationUserLimitExceeded(count), innerException)
		{
			this.count = count;
		}

		protected MigrationUserLimitExceededException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.count = (int)info.GetValue("count", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("count", this.count);
		}

		public int Count
		{
			get
			{
				return this.count;
			}
		}

		private readonly int count;
	}
}
