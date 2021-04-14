using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ManifestCheckpoint
	{
		public ManifestCheckpoint(MapiStore mapiStore, Stream checkpointState, IExExportManifest iExchangeExportManifest, int maxUncoalescedCount)
		{
			this.mapiStore = mapiStore;
			this.iExchangeExportManifest = iExchangeExportManifest;
			this.maxUncoalescedCount = maxUncoalescedCount;
			this.idsGiven = new List<long>(maxUncoalescedCount);
			this.cnsSeen = new List<long>(maxUncoalescedCount);
			this.cnsSeenAssociated = new List<long>(maxUncoalescedCount);
			this.idsDeleted = new List<long>(maxUncoalescedCount);
			this.cnsRead = new List<long>(maxUncoalescedCount);
			this.checkpointState = checkpointState;
		}

		public void CnSeen(bool isAssociated, long cn)
		{
			if (isAssociated)
			{
				this.cnsSeenAssociated.Add(cn);
			}
			else
			{
				this.cnsSeen.Add(cn);
			}
			this.CheckpointIfNeeded();
		}

		public void IdGiven(long id)
		{
			this.idsGiven.Add(id);
			this.CheckpointIfNeeded();
		}

		public void IdDeleted(long id)
		{
			this.idsDeleted.Add(id);
			this.CheckpointIfNeeded();
		}

		public void CnRead(long cn)
		{
			if (cn != 0L)
			{
				this.cnsRead.Add(cn);
				this.CheckpointIfNeeded();
			}
		}

		public void Checkpoint()
		{
			this.checkpointState.Seek(0L, SeekOrigin.Begin);
			if (this.IsCoalesced)
			{
				return;
			}
			MapiIStream iStream = null;
			if (this.checkpointState != null)
			{
				iStream = new MapiIStream(this.checkpointState);
			}
			int num = this.iExchangeExportManifest.Checkpoint(iStream, false, ManifestCheckpoint.ToLengthPrefixedArray(this.idsGiven), ManifestCheckpoint.ToLengthPrefixedArray(this.cnsSeen), ManifestCheckpoint.ToLengthPrefixedArray(this.cnsSeenAssociated), ManifestCheckpoint.ToLengthPrefixedArray(this.idsDeleted), ManifestCheckpoint.ToLengthPrefixedArray(this.cnsRead));
			if (num != 0)
			{
				MapiExceptionHelper.ThrowIfError("Unable to checkpoint ICS state.", num, this.iExchangeExportManifest, this.mapiStore.LastLowLevelException);
			}
			this.checkpointState.Seek(0L, SeekOrigin.Begin);
			this.idsGiven.Clear();
			this.cnsSeen.Clear();
			this.cnsSeenAssociated.Clear();
			this.idsDeleted.Clear();
			this.cnsRead.Clear();
		}

		private bool IsCoalesced
		{
			get
			{
				return this.CoalesceCount == 0;
			}
		}

		private int CoalesceCount
		{
			get
			{
				return this.idsDeleted.Count + this.idsGiven.Count + this.cnsSeen.Count + this.cnsSeenAssociated.Count + this.cnsRead.Count;
			}
		}

		private void CheckpointIfNeeded()
		{
			if (this.CoalesceCount >= this.maxUncoalescedCount)
			{
				this.Checkpoint();
			}
		}

		private static long[] ToLengthPrefixedArray(List<long> list)
		{
			long[] array = new long[list.Count + 1];
			array[0] = (long)list.Count;
			list.CopyTo(array, 1);
			return array;
		}

		private readonly MapiStore mapiStore;

		private readonly List<long> idsGiven;

		private readonly List<long> cnsSeen;

		private readonly List<long> cnsSeenAssociated;

		private readonly List<long> idsDeleted;

		private readonly List<long> cnsRead;

		private readonly int maxUncoalescedCount;

		private readonly IExExportManifest iExchangeExportManifest;

		private readonly Stream checkpointState;
	}
}
