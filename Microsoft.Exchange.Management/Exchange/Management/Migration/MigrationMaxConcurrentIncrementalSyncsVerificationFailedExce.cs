using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Migration
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MigrationMaxConcurrentIncrementalSyncsVerificationFailedException : LocalizedException
	{
		public MigrationMaxConcurrentIncrementalSyncsVerificationFailedException(Unlimited<int> value, Unlimited<int> maxValue) : base(Strings.MigrationMaxConcurrentIncrementalSyncsVerificationFailed(value, maxValue))
		{
			this.value = value;
			this.maxValue = maxValue;
		}

		public MigrationMaxConcurrentIncrementalSyncsVerificationFailedException(Unlimited<int> value, Unlimited<int> maxValue, Exception innerException) : base(Strings.MigrationMaxConcurrentIncrementalSyncsVerificationFailed(value, maxValue), innerException)
		{
			this.value = value;
			this.maxValue = maxValue;
		}

		protected MigrationMaxConcurrentIncrementalSyncsVerificationFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.value = (Unlimited<int>)info.GetValue("value", typeof(Unlimited<int>));
			this.maxValue = (Unlimited<int>)info.GetValue("maxValue", typeof(Unlimited<int>));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("value", this.value);
			info.AddValue("maxValue", this.maxValue);
		}

		public Unlimited<int> Value
		{
			get
			{
				return this.value;
			}
		}

		public Unlimited<int> MaxValue
		{
			get
			{
				return this.maxValue;
			}
		}

		private readonly Unlimited<int> value;

		private readonly Unlimited<int> maxValue;
	}
}
