using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Migration
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MigrationMaxConcurrentConnectionsVerificationFailedException : LocalizedException
	{
		public MigrationMaxConcurrentConnectionsVerificationFailedException(string value, string minValue, string maxValue) : base(Strings.MigrationMaxConcurrentConnectionsVerificationFailed(value, minValue, maxValue))
		{
			this.value = value;
			this.minValue = minValue;
			this.maxValue = maxValue;
		}

		public MigrationMaxConcurrentConnectionsVerificationFailedException(string value, string minValue, string maxValue, Exception innerException) : base(Strings.MigrationMaxConcurrentConnectionsVerificationFailed(value, minValue, maxValue), innerException)
		{
			this.value = value;
			this.minValue = minValue;
			this.maxValue = maxValue;
		}

		protected MigrationMaxConcurrentConnectionsVerificationFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.value = (string)info.GetValue("value", typeof(string));
			this.minValue = (string)info.GetValue("minValue", typeof(string));
			this.maxValue = (string)info.GetValue("maxValue", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("value", this.value);
			info.AddValue("minValue", this.minValue);
			info.AddValue("maxValue", this.maxValue);
		}

		public string Value
		{
			get
			{
				return this.value;
			}
		}

		public string MinValue
		{
			get
			{
				return this.minValue;
			}
		}

		public string MaxValue
		{
			get
			{
				return this.maxValue;
			}
		}

		private readonly string value;

		private readonly string minValue;

		private readonly string maxValue;
	}
}
