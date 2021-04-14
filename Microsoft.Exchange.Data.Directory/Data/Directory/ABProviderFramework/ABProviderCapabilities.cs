using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.ABProviderFramework
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ABProviderCapabilities
	{
		internal ABProviderCapabilities(ABProviderFlags flags)
		{
			if ((flags & (ABProviderFlags.HasGal | ABProviderFlags.CanBrowse)) != flags)
			{
				throw new ArgumentOutOfRangeException("Got an unknown flag", "flags");
			}
			bool flag = (flags & ABProviderFlags.CanBrowse) != ABProviderFlags.None;
			bool flag2 = (flags & ABProviderFlags.HasGal) != ABProviderFlags.None;
			if (flag && !flag2)
			{
				throw new ArgumentException("Unsupported combination: canBrowse && !hasGal", "flags");
			}
			this.flags = flags;
		}

		public override string ToString()
		{
			return this.flags.ToString();
		}

		public ABProviderFlags Flags
		{
			get
			{
				return this.flags;
			}
		}

		public bool HasGal
		{
			get
			{
				return this.CheckFlags(ABProviderFlags.HasGal);
			}
		}

		public bool CanBrowse
		{
			get
			{
				return this.CheckFlags(ABProviderFlags.CanBrowse);
			}
		}

		private bool CheckFlags(ABProviderFlags flags)
		{
			return (this.flags & flags) == flags;
		}

		private const int AllKnownFlags = 3;

		private readonly ABProviderFlags flags;
	}
}
