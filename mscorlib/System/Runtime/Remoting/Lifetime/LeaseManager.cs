using System;
using System.Collections;
using System.Diagnostics;
using System.Security;
using System.Threading;

namespace System.Runtime.Remoting.Lifetime
{
	internal class LeaseManager
	{
		internal static bool IsInitialized()
		{
			DomainSpecificRemotingData remotingData = Thread.GetDomain().RemotingData;
			LeaseManager leaseManager = remotingData.LeaseManager;
			return leaseManager != null;
		}

		[SecurityCritical]
		internal static LeaseManager GetLeaseManager(TimeSpan pollTime)
		{
			DomainSpecificRemotingData remotingData = Thread.GetDomain().RemotingData;
			LeaseManager leaseManager = remotingData.LeaseManager;
			if (leaseManager == null)
			{
				DomainSpecificRemotingData obj = remotingData;
				lock (obj)
				{
					if (remotingData.LeaseManager == null)
					{
						remotingData.LeaseManager = new LeaseManager(pollTime);
					}
					leaseManager = remotingData.LeaseManager;
				}
			}
			return leaseManager;
		}

		internal static LeaseManager GetLeaseManager()
		{
			DomainSpecificRemotingData remotingData = Thread.GetDomain().RemotingData;
			return remotingData.LeaseManager;
		}

		[SecurityCritical]
		private LeaseManager(TimeSpan pollTime)
		{
			this.pollTime = pollTime;
			this.leaseTimeAnalyzerDelegate = new TimerCallback(this.LeaseTimeAnalyzer);
			this.waitHandle = new AutoResetEvent(false);
			this.leaseTimer = new Timer(this.leaseTimeAnalyzerDelegate, null, -1, -1);
			this.leaseTimer.Change((int)pollTime.TotalMilliseconds, -1);
		}

		internal void ChangePollTime(TimeSpan pollTime)
		{
			this.pollTime = pollTime;
		}

		internal void ActivateLease(Lease lease)
		{
			Hashtable obj = this.leaseToTimeTable;
			lock (obj)
			{
				this.leaseToTimeTable[lease] = lease.leaseTime;
			}
		}

		internal void DeleteLease(Lease lease)
		{
			Hashtable obj = this.leaseToTimeTable;
			lock (obj)
			{
				this.leaseToTimeTable.Remove(lease);
			}
		}

		[Conditional("_LOGGING")]
		internal void DumpLeases(Lease[] leases)
		{
			for (int i = 0; i < leases.Length; i++)
			{
			}
		}

		internal ILease GetLease(MarshalByRefObject obj)
		{
			bool flag = true;
			Identity identity = MarshalByRefObject.GetIdentity(obj, out flag);
			if (identity == null)
			{
				return null;
			}
			return identity.Lease;
		}

		internal void ChangedLeaseTime(Lease lease, DateTime newTime)
		{
			Hashtable obj = this.leaseToTimeTable;
			lock (obj)
			{
				this.leaseToTimeTable[lease] = newTime;
			}
		}

		internal void RegisterSponsorCall(Lease lease, object sponsorId, TimeSpan sponsorshipTimeOut)
		{
			Hashtable obj = this.sponsorTable;
			lock (obj)
			{
				DateTime sponsorWaitTime = DateTime.UtcNow.Add(sponsorshipTimeOut);
				this.sponsorTable[sponsorId] = new LeaseManager.SponsorInfo(lease, sponsorId, sponsorWaitTime);
			}
		}

		internal void DeleteSponsor(object sponsorId)
		{
			Hashtable obj = this.sponsorTable;
			lock (obj)
			{
				this.sponsorTable.Remove(sponsorId);
			}
		}

		[SecurityCritical]
		private void LeaseTimeAnalyzer(object state)
		{
			DateTime utcNow = DateTime.UtcNow;
			Hashtable obj = this.leaseToTimeTable;
			lock (obj)
			{
				IDictionaryEnumerator enumerator = this.leaseToTimeTable.GetEnumerator();
				while (enumerator.MoveNext())
				{
					DateTime dateTime = (DateTime)enumerator.Value;
					Lease value = (Lease)enumerator.Key;
					if (dateTime.CompareTo(utcNow) < 0)
					{
						this.tempObjects.Add(value);
					}
				}
				for (int i = 0; i < this.tempObjects.Count; i++)
				{
					Lease key = (Lease)this.tempObjects[i];
					this.leaseToTimeTable.Remove(key);
				}
			}
			for (int j = 0; j < this.tempObjects.Count; j++)
			{
				Lease lease = (Lease)this.tempObjects[j];
				if (lease != null)
				{
					lease.LeaseExpired(utcNow);
				}
			}
			this.tempObjects.Clear();
			Hashtable obj2 = this.sponsorTable;
			lock (obj2)
			{
				IDictionaryEnumerator enumerator2 = this.sponsorTable.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					object key2 = enumerator2.Key;
					LeaseManager.SponsorInfo sponsorInfo = (LeaseManager.SponsorInfo)enumerator2.Value;
					if (sponsorInfo.sponsorWaitTime.CompareTo(utcNow) < 0)
					{
						this.tempObjects.Add(sponsorInfo);
					}
				}
				for (int k = 0; k < this.tempObjects.Count; k++)
				{
					LeaseManager.SponsorInfo sponsorInfo2 = (LeaseManager.SponsorInfo)this.tempObjects[k];
					this.sponsorTable.Remove(sponsorInfo2.sponsorId);
				}
			}
			for (int l = 0; l < this.tempObjects.Count; l++)
			{
				LeaseManager.SponsorInfo sponsorInfo3 = (LeaseManager.SponsorInfo)this.tempObjects[l];
				if (sponsorInfo3 != null && sponsorInfo3.lease != null)
				{
					sponsorInfo3.lease.SponsorTimeout(sponsorInfo3.sponsorId);
					this.tempObjects[l] = null;
				}
			}
			this.tempObjects.Clear();
			this.leaseTimer.Change((int)this.pollTime.TotalMilliseconds, -1);
		}

		private Hashtable leaseToTimeTable = new Hashtable();

		private Hashtable sponsorTable = new Hashtable();

		private TimeSpan pollTime;

		private AutoResetEvent waitHandle;

		private TimerCallback leaseTimeAnalyzerDelegate;

		private volatile Timer leaseTimer;

		private ArrayList tempObjects = new ArrayList(10);

		internal class SponsorInfo
		{
			internal SponsorInfo(Lease lease, object sponsorId, DateTime sponsorWaitTime)
			{
				this.lease = lease;
				this.sponsorId = sponsorId;
				this.sponsorWaitTime = sponsorWaitTime;
			}

			internal Lease lease;

			internal object sponsorId;

			internal DateTime sponsorWaitTime;
		}
	}
}
