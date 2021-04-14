using System;
using System.Threading;

namespace Microsoft.Exchange.TextProcessing
{
	internal class ClusterBin
	{
		public ClusterBin()
		{
			this.currentBinData = new TimePeriodBigMap<ulong, ClusterBucket>();
		}

		public ClusterBin(ClusterContext clusterContext)
		{
			this.clusterContext = clusterContext;
			int internalStoreNumber = clusterContext.InternalStoreNumber;
			TimeSpan swapTime = new TimeSpan(0, clusterContext.SwapTimeInMinutes, 0);
			TimeSpan mergeTime = new TimeSpan(0, clusterContext.MergeTimeInMinutes, 0);
			TimeSpan cleanTime = new TimeSpan(0, clusterContext.CleanTimeInMinutes, 0);
			this.currentBinData = new TimePeriodBigMap<ulong, ClusterBucket>(internalStoreNumber, swapTime, mergeTime, cleanTime, 71993);
			if (clusterContext.UseBloomFilter)
			{
				this.NewBloomFilter(clusterContext.PowerIndexOf2, clusterContext.MaxCountValue, clusterContext.HashNumbers);
			}
		}

		internal TimePeriodBigMap<ulong, ClusterBucket> CurrentBinData
		{
			get
			{
				return this.currentBinData;
			}
		}

		internal ClusterContext ClusterContext
		{
			get
			{
				return this.clusterContext;
			}
			set
			{
				this.clusterContext = value;
			}
		}

		public void NewBloomFilter(int powerIndex, int maxBitsValue, int hashNumbers)
		{
			CountingBloomFilter<Bits4> value = new CountingBloomFilter<Bits4>(powerIndex, maxBitsValue, hashNumbers);
			Interlocked.CompareExchange<CountingBloomFilter<Bits4>>(ref this.bloomFilter, value, this.bloomFilter);
		}

		public void ProcessOneFeed(MailInfo mailInfo, out ClusterResult result)
		{
			result = null;
			if (mailInfo == null)
			{
				throw new ArgumentNullException("mailInfo");
			}
			if (mailInfo.RecipientsDomainHash == null || mailInfo.RecipientsDomainHash.Length == 0)
			{
				throw new ArgumentException("emailDirection is null or empty.");
			}
			if (mailInfo.RecipientsDomainHash.Length > this.clusterContext.MaxHashSetSize || mailInfo.RecipientsDomainHash.Length <= 0)
			{
				throw new ArgumentException("recipientsDomainHash is out of range.");
			}
			if (!this.Filtering(mailInfo))
			{
				return;
			}
			ulong num = mailInfo.Key;
			ClusterBucket bucket = this.GetBucket(mailInfo.DocumentTime, mailInfo.Key);
			for (int i = 0; i < 4; i++)
			{
				if (bucket.Clusteroid == null)
				{
					bucket.Clusteroid = mailInfo.Fingerprint;
					bucket.Add(mailInfo, this.ClusterContext, out result);
					return;
				}
				int num2;
				int num3;
				LshFingerprint.ComputeSimilarity(bucket.Clusteroid, mailInfo.Fingerprint, out num2, out num3, false, false);
				if (num2 >= this.clusterContext.LowSimilarityBoundInt)
				{
					bucket.Add(mailInfo, this.ClusterContext, out result);
					return;
				}
				num += 1UL;
				bucket = this.GetBucket(mailInfo.DocumentTime, num);
			}
		}

		internal ClusterBucket GetBucket(DateTime dateTime, ulong key)
		{
			return this.currentBinData.GetValue(dateTime, key);
		}

		private bool Filtering(MailInfo mailInfo)
		{
			if (this.bloomFilter != null)
			{
				bool flag = this.bloomFilter.Add(FnvHash.Fnv1A64(BitConverter.GetBytes(mailInfo.Key)), mailInfo.RecipientNumber);
				return this.IsSpamFeed(mailInfo) || flag;
			}
			return true;
		}

		private bool IsSpamFeed(MailInfo mailInfo)
		{
			return mailInfo.FnFeed || mailInfo.SenFeed || mailInfo.HoneypotFeed || mailInfo.ThirdPartyFeed || mailInfo.SewrFeed;
		}

		private readonly TimePeriodBigMap<ulong, ClusterBucket> currentBinData;

		private CountingBloomFilter<Bits4> bloomFilter;

		private ClusterContext clusterContext;
	}
}
