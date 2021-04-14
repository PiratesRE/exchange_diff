using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Migration
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MigrationMaxConcurrentMigrationsVerificationFailedException : LocalizedException
	{
		public MigrationMaxConcurrentMigrationsVerificationFailedException(int value, int minValue, int maxValue) : base(Strings.MigrationMaxConcurrentMigrationsVerificationFailed(value, minValue, maxValue))
		{
			this.value = value;
			this.minValue = minValue;
			this.maxValue = maxValue;
		}

		public MigrationMaxConcurrentMigrationsVerificationFailedException(int value, int minValue, int maxValue, Exception innerException) : base(Strings.MigrationMaxConcurrentMigrationsVerificationFailed(value, minValue, maxValue), innerException)
		{
			this.value = value;
			this.minValue = minValue;
			this.maxValue = maxValue;
		}

		protected MigrationMaxConcurrentMigrationsVerificationFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.value = (int)info.GetValue("value", typeof(int));
			this.minValue = (int)info.GetValue("minValue", typeof(int));
			this.maxValue = (int)info.GetValue("maxValue", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("value", this.value);
			info.AddValue("minValue", this.minValue);
			info.AddValue("maxValue", this.maxValue);
		}

		public int Value
		{
			get
			{
				return this.value;
			}
		}

		public int MinValue
		{
			get
			{
				return this.minValue;
			}
		}

		public int MaxValue
		{
			get
			{
				return this.maxValue;
			}
		}

		private readonly int value;

		private readonly int minValue;

		private readonly int maxValue;
	}
}
