using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class RumInfo
	{
		private RumInfo() : this(RumType.None, null)
		{
			this.SendTime = null;
		}

		protected RumInfo(RumType type, ExDateTime? originalStartTime)
		{
			this.Type = type;
			this.occurrenceOriginalStartTime = originalStartTime;
		}

		public RumType Type { get; private set; }

		public ExDateTime? OccurrenceOriginalStartTime
		{
			get
			{
				return this.occurrenceOriginalStartTime;
			}
		}

		public bool IsSuccessfullySent
		{
			get
			{
				return this.SendTime != null;
			}
		}

		public ExDateTime? SendTime { get; internal set; }

		protected virtual void Merge(RumInfo infoToMerge)
		{
			if (infoToMerge == null)
			{
				throw new ArgumentNullException("infoToMerge");
			}
			if (!infoToMerge.IsNullOp)
			{
				if (this.OccurrenceOriginalStartTime == null)
				{
					if (infoToMerge.OccurrenceOriginalStartTime != null)
					{
						throw new ArgumentException("Cannot merge a master RUM info with an occurrence RUM info.", "infoToMerge");
					}
				}
				else
				{
					if (infoToMerge.OccurrenceOriginalStartTime == null)
					{
						throw new ArgumentException("Cannot merge an occurrence RUM info with a master RUM info.", "infoToMerge");
					}
					if (!this.OccurrenceOriginalStartTime.Equals(infoToMerge.OccurrenceOriginalStartTime))
					{
						throw new ArgumentOutOfRangeException("infoToMerge", "Two RUMs for different occurrences cannot be merged with each other.");
					}
				}
				if (this.IsNullOp)
				{
					throw new ArgumentException("Cannot merge a NullOp RUM info with a non-NullOp RUM info.", "infoToMerge");
				}
				if (this.Type != infoToMerge.Type)
				{
					throw new ArgumentException("Two RUMs of different types cannot be merged with each other.", "infoToMerge");
				}
			}
		}

		public static RumInfo Merge(RumInfo info1, RumInfo info2)
		{
			if (info1 == null)
			{
				throw new ArgumentNullException("info1");
			}
			if (info2 == null)
			{
				throw new ArgumentNullException("info2");
			}
			if (info1.IsNullOp)
			{
				return info2;
			}
			if (info2.IsNullOp)
			{
				return info1;
			}
			info1.Merge(info2);
			return info1;
		}

		public bool IsNullOp
		{
			get
			{
				return this.Type == RumType.None;
			}
		}

		public bool IsOccurrenceRum
		{
			get
			{
				return this.OccurrenceOriginalStartTime != null;
			}
		}

		private readonly ExDateTime? occurrenceOriginalStartTime;
	}
}
