using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PerUserDataCollector : BaseObject
	{
		internal PerUserDataCollector(int maxSize)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				if (maxSize < 2)
				{
					throw new BufferTooSmallException();
				}
				this.maxSize = maxSize;
				this.perUserDataEntries = new List<PerUserData>();
				this.writer = new CountWriter();
				this.writer.WriteUInt16(0);
				disposeGuard.Success();
			}
		}

		internal PerUserDataCollector(int maxSize, PerUserData[] perUserDataEntries) : this(maxSize)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				foreach (PerUserData perUserData in perUserDataEntries)
				{
					if (!this.TryAddPerUserData(perUserData))
					{
						break;
					}
				}
				disposeGuard.Success();
			}
		}

		internal int MaxSize
		{
			get
			{
				return this.maxSize;
			}
		}

		internal List<PerUserData> PerUserDataEntries
		{
			get
			{
				return this.perUserDataEntries;
			}
		}

		public bool TryAddPerUserData(PerUserData perUserData)
		{
			base.CheckDisposed();
			if (perUserData == null)
			{
				throw new ArgumentNullException("perUserData");
			}
			perUserData.Serialize(this.writer);
			if (this.writer.Position > (long)this.maxSize)
			{
				return false;
			}
			this.perUserDataEntries.Add(perUserData);
			return true;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder("PerUserDataCollector Collector:");
			stringBuilder.AppendLine("\nMaxSize: " + this.MaxSize);
			stringBuilder.AppendLine("Number of PerUserData entries: " + this.PerUserDataEntries.Count);
			return stringBuilder.ToString();
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<PerUserDataCollector>(this);
		}

		protected override void InternalDispose()
		{
			Util.DisposeIfPresent(this.writer);
			base.InternalDispose();
		}

		private readonly int maxSize;

		private readonly CountWriter writer;

		private List<PerUserData> perUserDataEntries;
	}
}
