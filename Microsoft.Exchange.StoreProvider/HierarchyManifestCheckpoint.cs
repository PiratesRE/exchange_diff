using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class HierarchyManifestCheckpoint
	{
		public HierarchyManifestCheckpoint(MapiStore mapiStore, byte[] stateIdsetGiven, byte[] stateCnsetSeen, SafeExExportHierManifestExHandle iExchangeExportHierManifestEx, int maxUncoalescedCount)
		{
			this.mapiStore = mapiStore;
			this.iExchangeExportHierManifestEx = iExchangeExportHierManifestEx;
			this.maxUncoalescedCount = maxUncoalescedCount;
			this.idsGiven = new List<long>(maxUncoalescedCount);
			this.cnsSeen = new List<long>(maxUncoalescedCount);
			this.idsDeleted = new List<long>(maxUncoalescedCount);
			this.checkpointStateIdsetGiven = HierarchyManifestCheckpoint.DuplicateArray(stateIdsetGiven);
			this.checkpointStateCnsetSeen = HierarchyManifestCheckpoint.DuplicateArray(stateCnsetSeen);
		}

		public void CnSeen(long cn)
		{
			this.cnsSeen.Add(cn);
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

		public void Checkpoint(out byte[] stateIdsetGiven, out byte[] stateCnsetSeen)
		{
			if (this.IsCoalesced)
			{
				stateIdsetGiven = HierarchyManifestCheckpoint.DuplicateArray(this.checkpointStateIdsetGiven);
				stateCnsetSeen = HierarchyManifestCheckpoint.DuplicateArray(this.checkpointStateCnsetSeen);
				return;
			}
			SafeExMemoryHandle safeExMemoryHandle = null;
			SafeExMemoryHandle safeExMemoryHandle2 = null;
			try
			{
				int num = 0;
				int num2 = 0;
				int num3 = this.iExchangeExportHierManifestEx.Checkpoint(this.checkpointStateIdsetGiven, this.checkpointStateIdsetGiven.Length, this.checkpointStateCnsetSeen, this.checkpointStateCnsetSeen.Length, HierarchyManifestCheckpoint.ToLengthPrefixedArray(this.idsGiven), HierarchyManifestCheckpoint.ToLengthPrefixedArray(this.cnsSeen), HierarchyManifestCheckpoint.ToLengthPrefixedArray(this.idsDeleted), out safeExMemoryHandle, out num, out safeExMemoryHandle2, out num2);
				if (num3 != 0)
				{
					MapiExceptionHelper.ThrowIfError("Unable to checkpoint ICS state.", num3, this.iExchangeExportHierManifestEx, this.mapiStore.LastLowLevelException);
				}
				byte[] array = Array<byte>.Empty;
				if (num > 0 && safeExMemoryHandle != null)
				{
					array = new byte[num];
					Marshal.Copy(safeExMemoryHandle.DangerousGetHandle(), array, 0, num);
				}
				byte[] array2 = Array<byte>.Empty;
				if (num2 > 0 && safeExMemoryHandle2 != null)
				{
					array2 = new byte[num2];
					Marshal.Copy(safeExMemoryHandle2.DangerousGetHandle(), array2, 0, num2);
				}
				stateIdsetGiven = HierarchyManifestCheckpoint.DuplicateArray(array);
				stateCnsetSeen = HierarchyManifestCheckpoint.DuplicateArray(array2);
				this.checkpointStateIdsetGiven = array;
				this.checkpointStateCnsetSeen = array2;
				this.idsGiven.Clear();
				this.cnsSeen.Clear();
				this.idsDeleted.Clear();
			}
			finally
			{
				if (safeExMemoryHandle != null)
				{
					safeExMemoryHandle.Dispose();
				}
				if (safeExMemoryHandle2 != null)
				{
					safeExMemoryHandle2.Dispose();
				}
			}
		}

		private static byte[] DuplicateArray(byte[] byteArray)
		{
			byte[] array = Array<byte>.Empty;
			if (byteArray != null && byteArray.Length > 0)
			{
				array = new byte[byteArray.Length];
				Array.Copy(byteArray, 0, array, 0, byteArray.Length);
			}
			return array;
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
				return this.idsDeleted.Count + this.idsGiven.Count + this.cnsSeen.Count;
			}
		}

		private void CheckpointIfNeeded()
		{
			if (this.CoalesceCount >= this.maxUncoalescedCount)
			{
				byte[] array;
				byte[] array2;
				this.Checkpoint(out array, out array2);
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

		private readonly List<long> idsDeleted;

		private readonly int maxUncoalescedCount;

		private readonly SafeExExportHierManifestExHandle iExchangeExportHierManifestEx;

		private byte[] checkpointStateIdsetGiven;

		private byte[] checkpointStateCnsetSeen;
	}
}
