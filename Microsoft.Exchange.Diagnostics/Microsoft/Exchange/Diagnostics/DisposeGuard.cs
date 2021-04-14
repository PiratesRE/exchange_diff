using System;
using System.Diagnostics;

namespace Microsoft.Exchange.Diagnostics
{
	[DebuggerNonUserCode]
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct DisposeGuard : IDisposable
	{
		public T Add<T>(T disposable) where T : class, IDisposable
		{
			this.CheckDisposed();
			if (disposable == null)
			{
				return disposable;
			}
			bool flag = false;
			try
			{
				this.AddGuardedObject(disposable);
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					DisposeGuard.DisposeIfPresent(disposable);
				}
			}
			return disposable;
		}

		public void Success()
		{
			this.CheckDisposed();
			this.SlotCount = 0;
		}

		internal static void DisposeIfPresent(IDisposable disposable)
		{
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}

		private static IDisposable Swap(ref IDisposable fixedSlot, IDisposable newValue)
		{
			IDisposable result = fixedSlot;
			fixedSlot = newValue;
			return result;
		}

		private void CheckDisposed()
		{
			if (this.slotCount == 65535)
			{
				throw new ObjectDisposedException(base.GetType().ToString() + " has already been disposed.");
			}
		}

		private ushort SlotCount
		{
			get
			{
				return this.slotCount;
			}
			set
			{
				if (value >= 4 && value > this.slotCount)
				{
					if (this.overflowSlots == null)
					{
						this.overflowSlots = new IDisposable[Math.Max((int)(value - 4), 4)];
					}
					if ((int)value > 4 + this.overflowSlots.Length)
					{
						Array.Resize<IDisposable>(ref this.overflowSlots, Math.Max((int)value, this.overflowSlots.Length * 2));
					}
				}
				this.slotCount = value;
			}
		}

		private void AddGuardedObject(IDisposable disposable)
		{
			this.SlotCount += 1;
			this.SwapSlot((int)(this.SlotCount - 1), disposable);
		}

		private void RemoveLastGuardedObject()
		{
			DisposeGuard.DisposeIfPresent(this.SwapSlot((int)(this.SlotCount - 1), null));
			this.SlotCount -= 1;
		}

		private IDisposable SwapSlot(int index, IDisposable newValue)
		{
			switch (index)
			{
			case 0:
				return DisposeGuard.Swap(ref this.fixedSlot0, newValue);
			case 1:
				return DisposeGuard.Swap(ref this.fixedSlot1, newValue);
			case 2:
				return DisposeGuard.Swap(ref this.fixedSlot2, newValue);
			case 3:
				return DisposeGuard.Swap(ref this.fixedSlot3, newValue);
			default:
				return DisposeGuard.Swap(ref this.overflowSlots[index - 4], newValue);
			}
		}

		public void Dispose()
		{
			while (this.SlotCount > 0)
			{
				this.RemoveLastGuardedObject();
			}
			this.slotCount = ushort.MaxValue;
		}

		private const int FixedSlotCapacity = 4;

		private const int InitialOverflowSlotCapacity = 4;

		private ushort slotCount;

		private IDisposable fixedSlot0;

		private IDisposable fixedSlot1;

		private IDisposable fixedSlot2;

		private IDisposable fixedSlot3;

		private IDisposable[] overflowSlots;
	}
}
