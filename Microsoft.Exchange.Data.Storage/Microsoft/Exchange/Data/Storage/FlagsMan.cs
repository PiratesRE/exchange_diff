using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FlagsMan
	{
		public static bool IsAutoTagSet(object retentionFlagsObject)
		{
			if (!(retentionFlagsObject is int) && !(retentionFlagsObject is RetentionAndArchiveFlags))
			{
				return false;
			}
			RetentionAndArchiveFlags retentionAndArchiveFlags = (RetentionAndArchiveFlags)retentionFlagsObject;
			EnumValidator.ThrowIfInvalid<RetentionAndArchiveFlags>(retentionAndArchiveFlags, "retentionFlags");
			return (retentionAndArchiveFlags & RetentionAndArchiveFlags.Autotag) != RetentionAndArchiveFlags.None;
		}

		public static bool IsUserOverrideSet(object retentionFlagsObject)
		{
			if (!(retentionFlagsObject is int) && !(retentionFlagsObject is RetentionAndArchiveFlags))
			{
				return false;
			}
			RetentionAndArchiveFlags retentionAndArchiveFlags = (RetentionAndArchiveFlags)retentionFlagsObject;
			EnumValidator.ThrowIfInvalid<RetentionAndArchiveFlags>(retentionAndArchiveFlags, "retentionFlags");
			return (retentionAndArchiveFlags & RetentionAndArchiveFlags.UserOverride) != RetentionAndArchiveFlags.None;
		}

		public static bool IsExplicitSet(object retentionFlagsObject)
		{
			if (!(retentionFlagsObject is int) && !(retentionFlagsObject is RetentionAndArchiveFlags))
			{
				return false;
			}
			RetentionAndArchiveFlags retentionAndArchiveFlags = (RetentionAndArchiveFlags)retentionFlagsObject;
			EnumValidator.ThrowIfInvalid<RetentionAndArchiveFlags>(retentionAndArchiveFlags, "retentionFlags");
			return (retentionAndArchiveFlags & RetentionAndArchiveFlags.ExplicitTag) != RetentionAndArchiveFlags.None;
		}

		public static bool IsPersonalTagSet(object retentionFlagsObject)
		{
			if (!(retentionFlagsObject is int) && !(retentionFlagsObject is RetentionAndArchiveFlags))
			{
				return false;
			}
			RetentionAndArchiveFlags retentionAndArchiveFlags = (RetentionAndArchiveFlags)retentionFlagsObject;
			EnumValidator.ThrowIfInvalid<RetentionAndArchiveFlags>(retentionAndArchiveFlags, "retentionFlags");
			return (retentionAndArchiveFlags & RetentionAndArchiveFlags.PersonalTag) != RetentionAndArchiveFlags.None;
		}

		public static bool IsSystemDataSet(object retentionFlagsObject)
		{
			if (!(retentionFlagsObject is int) && !(retentionFlagsObject is RetentionAndArchiveFlags))
			{
				return false;
			}
			RetentionAndArchiveFlags retentionAndArchiveFlags = (RetentionAndArchiveFlags)retentionFlagsObject;
			EnumValidator.ThrowIfInvalid<RetentionAndArchiveFlags>(retentionAndArchiveFlags, "retentionFlags");
			return (retentionAndArchiveFlags & RetentionAndArchiveFlags.SystemData) != RetentionAndArchiveFlags.None;
		}

		public static bool IsExplicitArchiveSet(object retentionFlagsObject)
		{
			if (!(retentionFlagsObject is int) && !(retentionFlagsObject is RetentionAndArchiveFlags))
			{
				return false;
			}
			RetentionAndArchiveFlags retentionAndArchiveFlags = (RetentionAndArchiveFlags)retentionFlagsObject;
			EnumValidator.ThrowIfInvalid<RetentionAndArchiveFlags>(retentionAndArchiveFlags, "retentionFlags");
			return (retentionAndArchiveFlags & RetentionAndArchiveFlags.ExplictArchiveTag) != RetentionAndArchiveFlags.None;
		}

		public static bool DoesFolderNeedRescan(int flags)
		{
			EnumValidator.ThrowIfInvalid<RetentionAndArchiveFlags>((RetentionAndArchiveFlags)flags, "flags");
			return (flags & 384) != 0;
		}

		public static RetentionAndArchiveFlags ClearNeedRescan(RetentionAndArchiveFlags flags)
		{
			EnumValidator.ThrowIfInvalid<RetentionAndArchiveFlags>(flags, "flags");
			return flags & ~RetentionAndArchiveFlags.NeedsRescan;
		}

		public static int ClearPendingRescan(int flags)
		{
			return flags & -257;
		}

		public static int SetNeedRescan(int flags)
		{
			return flags | 128;
		}

		public static RetentionAndArchiveFlags SetPendingRescan(RetentionAndArchiveFlags flags)
		{
			EnumValidator.ThrowIfInvalid<RetentionAndArchiveFlags>(flags, "flags");
			return flags | RetentionAndArchiveFlags.PendingRescan;
		}

		public static int ClearAutoTag(int retentionFlags)
		{
			return retentionFlags & -5;
		}

		public static int ClearUserOverride(int retentionFlags)
		{
			return retentionFlags & -3;
		}

		public static int ClearExplicit(int retentionFlags)
		{
			return retentionFlags & -2;
		}

		public static int ClearPersonalTag(int retentionFlags)
		{
			return retentionFlags & -9;
		}

		public static int SetAutoTag(int? retentionFlags)
		{
			return (retentionFlags ?? 0) | 4;
		}

		public static RetentionAndArchiveFlags ClearAllRetentionFlags(RetentionAndArchiveFlags flags)
		{
			EnumValidator.ThrowIfInvalid<RetentionAndArchiveFlags>(flags, "flags");
			return flags & ~(RetentionAndArchiveFlags.ExplicitTag | RetentionAndArchiveFlags.UserOverride | RetentionAndArchiveFlags.Autotag | RetentionAndArchiveFlags.PersonalTag);
		}

		public static int SetExplicit(int retentionFlags)
		{
			return retentionFlags | 1;
		}

		public static int SetPersonalTag(int retentionFlags)
		{
			return retentionFlags | 8;
		}

		public static int SetSystemData(int retentionFlags)
		{
			return retentionFlags | 64;
		}

		public static RetentionAndArchiveFlags ClearAllArchiveFlags(RetentionAndArchiveFlags flags)
		{
			EnumValidator.ThrowIfInvalid<RetentionAndArchiveFlags>(flags, "flags");
			return flags & ~(RetentionAndArchiveFlags.ExplictArchiveTag | RetentionAndArchiveFlags.KeepInPlace);
		}

		public static RetentionAndArchiveFlags SetExplicitArchiveFlag(RetentionAndArchiveFlags flags)
		{
			EnumValidator.ThrowIfInvalid<RetentionAndArchiveFlags>(flags, "flags");
			return flags | RetentionAndArchiveFlags.ExplictArchiveTag;
		}

		public static RetentionAndArchiveFlags ClearExplicitArchiveFlag(RetentionAndArchiveFlags flags)
		{
			EnumValidator.ThrowIfInvalid<RetentionAndArchiveFlags>(flags, "flags");
			return flags & ~RetentionAndArchiveFlags.ExplictArchiveTag;
		}
	}
}
