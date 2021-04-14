using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmLastServerTimeStampCorruptedException : AmCommonException
	{
		public AmLastServerTimeStampCorruptedException(string property, string corruptedValue) : base(ReplayStrings.AmLastServerTimeStampCorruptedException(property, corruptedValue))
		{
			this.property = property;
			this.corruptedValue = corruptedValue;
		}

		public AmLastServerTimeStampCorruptedException(string property, string corruptedValue, Exception innerException) : base(ReplayStrings.AmLastServerTimeStampCorruptedException(property, corruptedValue), innerException)
		{
			this.property = property;
			this.corruptedValue = corruptedValue;
		}

		protected AmLastServerTimeStampCorruptedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.property = (string)info.GetValue("property", typeof(string));
			this.corruptedValue = (string)info.GetValue("corruptedValue", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("property", this.property);
			info.AddValue("corruptedValue", this.corruptedValue);
		}

		public string Property
		{
			get
			{
				return this.property;
			}
		}

		public string CorruptedValue
		{
			get
			{
				return this.corruptedValue;
			}
		}

		private readonly string property;

		private readonly string corruptedValue;
	}
}
