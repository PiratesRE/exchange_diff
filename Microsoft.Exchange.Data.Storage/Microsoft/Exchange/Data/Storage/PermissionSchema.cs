using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PermissionSchema : Schema
	{
		public new static PermissionSchema Instance
		{
			get
			{
				if (PermissionSchema.instance == null)
				{
					PermissionSchema.instance = new PermissionSchema();
				}
				return PermissionSchema.instance;
			}
		}

		public static readonly StorePropertyDefinition MemberId = InternalSchema.MemberId;

		public static readonly StorePropertyDefinition MemberEntryId = InternalSchema.MemberEntryId;

		public static readonly StorePropertyDefinition MemberName = InternalSchema.MemberName;

		public static readonly StorePropertyDefinition MemberRights = InternalSchema.MemberRights;

		public static readonly StorePropertyDefinition MemberSecurityIdentifier = InternalSchema.MemberSecurityIdentifier;

		public static readonly StorePropertyDefinition MemberIsGroup = InternalSchema.MemberIsGroup;

		private static PermissionSchema instance = null;
	}
}
