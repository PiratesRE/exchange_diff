using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class IncrementalSyncIntervalRangeException : RecipientTaskException
	{
		public IncrementalSyncIntervalRangeException(int minDays, int maxDays) : base(Strings.ErrorIncrementalSyncIntervalRange(minDays, maxDays))
		{
			this.minDays = minDays;
			this.maxDays = maxDays;
		}

		public IncrementalSyncIntervalRangeException(int minDays, int maxDays, Exception innerException) : base(Strings.ErrorIncrementalSyncIntervalRange(minDays, maxDays), innerException)
		{
			this.minDays = minDays;
			this.maxDays = maxDays;
		}

		protected IncrementalSyncIntervalRangeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.minDays = (int)info.GetValue("minDays", typeof(int));
			this.maxDays = (int)info.GetValue("maxDays", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("minDays", this.minDays);
			info.AddValue("maxDays", this.maxDays);
		}

		public int MinDays
		{
			get
			{
				return this.minDays;
			}
		}

		public int MaxDays
		{
			get
			{
				return this.maxDays;
			}
		}

		private readonly int minDays;

		private readonly int maxDays;
	}
}
