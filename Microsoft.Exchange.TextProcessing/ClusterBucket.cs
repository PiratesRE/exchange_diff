using System;
using System.Threading;

namespace Microsoft.Exchange.TextProcessing
{
	internal class ClusterBucket : IInitialize
	{
		public ClusterBucket()
		{
			this.Initialize();
		}

		public DateTime LastRefreshTime { get; set; }

		public int SizeIncoming
		{
			get
			{
				return this.sizeIncoming;
			}
			set
			{
				this.sizeIncoming = value;
			}
		}

		public int SizeOutgoing
		{
			get
			{
				return this.sizeOutgoing;
			}
			set
			{
				this.sizeOutgoing = value;
			}
		}

		public ActionEnum ActionMode { get; private set; }

		public ClusteringStatusEnum Status
		{
			get
			{
				return (ClusteringStatusEnum)this.status;
			}
		}

		public SmallCounterMap SenderDomains { get; private set; }

		public SmallCounterMap Senders { get; private set; }

		public SmallCounterMap RecipientDomains { get; private set; }

		public SmallCounterMap Subjects { get; private set; }

		public SmallCounterMap ClientIps { get; private set; }

		public SmallCounterMap ClientIp24s { get; private set; }

		public int SenCount
		{
			get
			{
				return this.senCount;
			}
			private set
			{
				this.senCount = value;
			}
		}

		public int HoneypotCount
		{
			get
			{
				return this.honeypotCount;
			}
			private set
			{
				this.honeypotCount = value;
			}
		}

		public int FnCount
		{
			get
			{
				return this.fnCount;
			}
			private set
			{
				this.fnCount = value;
			}
		}

		public int ThirdPartyCount
		{
			get
			{
				return this.thirdPartyCount;
			}
			private set
			{
				this.thirdPartyCount = value;
			}
		}

		public int SewrCount
		{
			get
			{
				return this.sewrCount;
			}
			private set
			{
				this.sewrCount = value;
			}
		}

		public int SpamCount
		{
			get
			{
				return this.spamCount;
			}
			private set
			{
				this.spamCount = value;
			}
		}

		public LshFingerprint Clusteroid
		{
			get
			{
				return this.clusteroid;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.clusteroid = value;
			}
		}

		public void Initialize()
		{
			this.LastRefreshTime = DateTime.UtcNow;
			this.clusteroid = null;
			this.ActionMode = ActionEnum.BelowThreshold;
			this.SenderDomains = new SmallCounterMap();
			this.Senders = new SmallCounterMap();
			this.RecipientDomains = new SmallCounterMap();
			this.Subjects = new SmallCounterMap();
			this.ClientIps = new SmallCounterMap();
			this.ClientIp24s = new SmallCounterMap();
			this.SenCount = 0;
			this.HoneypotCount = 0;
			this.FnCount = 0;
			this.ThirdPartyCount = 0;
			this.SizeIncoming = 0;
			this.SizeOutgoing = 0;
			this.status = 0;
			this.sewrCount = 0;
			this.spamCount = 0;
		}

		public int BucketSize(DirectionEnum direction)
		{
			if (direction == DirectionEnum.Incoming)
			{
				return this.sizeIncoming;
			}
			return this.sizeOutgoing;
		}

		public void Add(MailInfo mailInfo, ClusterContext clusterContext, out ClusterResult result)
		{
			if (mailInfo == null)
			{
				throw new ArgumentNullException("mailInfo");
			}
			if (mailInfo.RecipientsDomainHash == null || mailInfo.RecipientsDomainHash.Length == 0)
			{
				throw new ArgumentException("emailDirection is null or empty.");
			}
			if (clusterContext == null)
			{
				throw new ArgumentNullException("clusterContext");
			}
			this.IncrementAttributeCounter(mailInfo);
			this.IncrementFeedCounter(mailInfo);
			this.IncrementClusterSize(mailInfo);
			int[] propertyValues;
			this.Summarize(mailInfo, clusterContext, out propertyValues);
			result = new ClusterResult();
			this.SetClusterResult(mailInfo, clusterContext, propertyValues, result);
		}

		internal ClusteringStatusEnum CaculateSourceStatus(int[] propertyValues, ClusterContext clusterContext)
		{
			if (propertyValues == null)
			{
				throw new ArgumentNullException("propertyValues");
			}
			if (clusterContext == null)
			{
				throw new ArgumentNullException("clusterContext");
			}
			ClusteringStatusEnum clusteringStatusEnum = this.SourceType(propertyValues[0], clusterContext);
			ClusteringStatusEnum clusteringStatusEnum2 = this.SourceType(propertyValues[1], clusterContext);
			ClusteringStatusEnum clusteringStatusEnum3 = this.SourceType(propertyValues[2], clusterContext);
			ClusteringStatusEnum clusteringStatusEnum4 = this.SourceType(propertyValues[4], clusterContext);
			ClusteringStatusEnum clusteringStatusEnum5 = this.SourceType(propertyValues[5], clusterContext);
			ClusteringStatusEnum clusteringStatusEnum6 = clusteringStatusEnum | clusteringStatusEnum2 | clusteringStatusEnum3 | clusteringStatusEnum4 | clusteringStatusEnum5;
			if ((clusteringStatusEnum6 & ClusteringStatusEnum.SourceMask) == ClusteringStatusEnum.OneSource)
			{
				return ClusteringStatusEnum.OneSource;
			}
			if ((clusteringStatusEnum6 & ClusteringStatusEnum.SourceMask) == ClusteringStatusEnum.MultiSource)
			{
				return ClusteringStatusEnum.MultiSource;
			}
			if ((clusteringStatusEnum6 & ClusteringStatusEnum.SourceMask) == (ClusteringStatusEnum.OneSource | ClusteringStatusEnum.MultiSource))
			{
				return ClusteringStatusEnum.OneAndMultiSource;
			}
			return ClusteringStatusEnum.UkOneOrMultiSource;
		}

		internal ClusteringStatusEnum CalculateSpamFeedStatus(int[] propertyValues, ClusterContext clusterContext)
		{
			if (propertyValues == null)
			{
				throw new ArgumentNullException("propertyValues");
			}
			if (clusterContext == null)
			{
				throw new ArgumentNullException("clusterContext");
			}
			ClusteringStatusEnum clusteringStatusEnum = ClusteringStatusEnum.None;
			if (propertyValues[9] > clusterContext.SenFeedSizeAbove)
			{
				clusteringStatusEnum |= ClusteringStatusEnum.SenFeed;
			}
			if (propertyValues[7] > clusterContext.FnFeedSizeAbove)
			{
				clusteringStatusEnum |= ClusteringStatusEnum.FnFeed;
			}
			if (propertyValues[8] > clusterContext.HoneypotFeedSizeAbove)
			{
				clusteringStatusEnum |= ClusteringStatusEnum.HoneypotFeed;
			}
			if (propertyValues[10] > clusterContext.ThirdPartyFeedSizeAbove)
			{
				clusteringStatusEnum |= ClusteringStatusEnum.ThirdPartyFeed;
			}
			if (propertyValues[11] > clusterContext.SewrFeedSizeAbove)
			{
				clusteringStatusEnum |= ClusteringStatusEnum.SewrFeed;
			}
			if (propertyValues[12] > clusterContext.SpamSizeAbove)
			{
				clusteringStatusEnum |= ClusteringStatusEnum.SpamVerdict;
			}
			return clusteringStatusEnum;
		}

		private ActionEnum ResolveActionMode(DirectionEnum emailDirection, int[] propertyValues, ClusterContext clusterContext)
		{
			ClusteringStatusEnum clusteringStatusEnum = this.Status;
			if ((clusteringStatusEnum & ClusteringStatusEnum.MultiSource) != ClusteringStatusEnum.MultiSource)
			{
				clusteringStatusEnum |= this.CaculateSourceStatus(propertyValues, clusterContext);
			}
			clusteringStatusEnum |= this.CalculateSpamFeedStatus(propertyValues, clusterContext);
			ClusteringStatusEnum clusteringStatusEnum2 = (ClusteringStatusEnum)Interlocked.CompareExchange(ref this.status, this.status | (int)clusteringStatusEnum, this.status);
			int num = this.BucketSize(emailDirection);
			if (propertyValues[3] <= clusterContext.NumberOfRecipientDomainAbove)
			{
				this.ActionMode = ActionEnum.BelowThreshold;
				return ActionEnum.BelowThreshold;
			}
			if (this.ActionMode == ActionEnum.BelowThreshold)
			{
				bool flag = ((this.Status & ClusteringStatusEnum.SpamFeedMask) != ClusteringStatusEnum.None && num > clusterContext.SpamFeedClusterSizeAbove) || ((this.Status & ClusteringStatusEnum.SpamVerdict) != ClusteringStatusEnum.None && num > clusterContext.SpamVerdictFeedClusterSizeAbove) || ((this.Status & ClusteringStatusEnum.OneSource) == ClusteringStatusEnum.OneSource && num > clusterContext.AllOneSourceClusterSizeAbove) || ((this.Status & ClusteringStatusEnum.OneAndMultiSource) == ClusteringStatusEnum.OneAndMultiSource && num > clusterContext.OneAndMultiSourceClusterSizeAbove) || ((this.Status & ClusteringStatusEnum.MultiSource) == ClusteringStatusEnum.MultiSource && num > clusterContext.AllMultiSourceClusterSizeAbove);
				if (flag)
				{
					this.ActionMode = ActionEnum.ReachedThreshold;
					return ActionEnum.ReachedThreshold;
				}
				return ActionEnum.BelowThreshold;
			}
			else
			{
				if (this.ActionMode == ActionEnum.ReachedThreshold)
				{
					this.ActionMode = ActionEnum.OverThreshold;
					return ActionEnum.OverThreshold;
				}
				if (this.ActionMode == ActionEnum.OverThreshold && (clusteringStatusEnum & ClusteringStatusEnum.SpamFeedMask) != ClusteringStatusEnum.None && (clusteringStatusEnum2 & ClusteringStatusEnum.SpamFeedMask) == ClusteringStatusEnum.None)
				{
					this.ActionMode = ActionEnum.ReachedThreshold;
					return ActionEnum.ReachedThreshold;
				}
				return this.ActionMode;
			}
		}

		private void Summarize(MailInfo mailInfo, ClusterContext clusterContext, out int[] propertyValues)
		{
			propertyValues = new int[ClusterBucket.StatusEnumLength];
			propertyValues[0] = this.Subjects.NumberOfMajorSource(clusterContext.NearOneSourcePercentageAbove);
			propertyValues[1] = this.SenderDomains.NumberOfMajorSource(clusterContext.NearOneSourcePercentageAbove);
			propertyValues[2] = this.Senders.NumberOfMajorSource(clusterContext.NearOneSourcePercentageAbove);
			propertyValues[3] = this.RecipientDomains.NumberOfMajorSource(clusterContext.NearOneSourcePercentageAbove);
			propertyValues[4] = this.ClientIps.NumberOfMajorSource(clusterContext.NearOneSourcePercentageAbove);
			propertyValues[5] = this.ClientIp24s.NumberOfMajorSource(clusterContext.NearOneSourcePercentageAbove);
			propertyValues[6] = this.BucketSize(mailInfo.EmailDirection);
			propertyValues[7] = this.fnCount;
			propertyValues[8] = this.honeypotCount;
			propertyValues[9] = this.senCount;
			propertyValues[10] = this.thirdPartyCount;
			propertyValues[11] = this.sewrCount;
			propertyValues[12] = this.spamCount;
		}

		private ClusteringStatusEnum SourceType(int sourceCount, ClusterContext clusterContext)
		{
			if (sourceCount == 1)
			{
				return ClusteringStatusEnum.OneSource;
			}
			if (sourceCount <= clusterContext.NumberofSourcesMadeOfMajorSourcesAbove)
			{
				return ClusteringStatusEnum.UkOneOrMultiSource;
			}
			return ClusteringStatusEnum.MultiSource;
		}

		private void IncrementAttributeCounter(MailInfo mailInfo)
		{
			this.SenderDomains.Increment((long)mailInfo.SenderDomainHash);
			this.Senders.Increment((long)mailInfo.SenderHash);
			this.Subjects.Increment((long)mailInfo.SubjectHash);
			this.ClientIps.Increment((long)mailInfo.ClientIpHash);
			this.ClientIp24s.Increment((long)mailInfo.ClientIp24Hash);
			if (this.RecipientDomains.CounterNumbers <= 20)
			{
				foreach (ulong key in mailInfo.RecipientsDomainHash)
				{
					if (this.RecipientDomains.CounterNumbers <= 20)
					{
						this.RecipientDomains.Increment((long)key);
					}
				}
			}
		}

		private void IncrementFeedCounter(MailInfo mailInfo)
		{
			if (mailInfo.SenFeed)
			{
				Interlocked.Add(ref this.senCount, mailInfo.RecipientNumber);
			}
			if (mailInfo.FnFeed)
			{
				Interlocked.Add(ref this.fnCount, mailInfo.RecipientNumber);
			}
			if (mailInfo.HoneypotFeed)
			{
				Interlocked.Add(ref this.honeypotCount, mailInfo.RecipientNumber);
			}
			if (mailInfo.ThirdPartyFeed)
			{
				Interlocked.Add(ref this.thirdPartyCount, mailInfo.RecipientNumber);
			}
			if (mailInfo.SewrFeed)
			{
				Interlocked.Add(ref this.sewrCount, mailInfo.RecipientNumber);
			}
			if (mailInfo.SpamVerdict)
			{
				Interlocked.Add(ref this.spamCount, mailInfo.RecipientNumber);
			}
		}

		private void IncrementClusterSize(MailInfo mailInfo)
		{
			if (!mailInfo.SpamVerdict)
			{
				if (mailInfo.EmailDirection == DirectionEnum.Incoming)
				{
					Interlocked.Add(ref this.sizeIncoming, mailInfo.RecipientNumber);
					return;
				}
				Interlocked.Add(ref this.sizeOutgoing, mailInfo.RecipientNumber);
			}
		}

		private void SetClusterResult(MailInfo mailInfo, ClusterContext clusterContext, int[] propertyValues, ClusterResult result)
		{
			result.ActionMode = this.ResolveActionMode(mailInfo.EmailDirection, propertyValues, clusterContext);
			result.Status = this.Status;
			result.PropertyValues = propertyValues;
			if (result.ActionMode != ActionEnum.BelowThreshold)
			{
				result.Clusteroid = this.Clusteroid;
				result.StartTimeStamp = this.LastRefreshTime;
			}
		}

		private static readonly int StatusEnumLength = Enum.GetNames(typeof(ClusterPropertyEnum)).Length;

		private LshFingerprint clusteroid;

		private int senCount;

		private int honeypotCount;

		private int fnCount;

		private int thirdPartyCount;

		private int sewrCount;

		private int spamCount;

		private int sizeIncoming;

		private int sizeOutgoing;

		private int status;
	}
}
