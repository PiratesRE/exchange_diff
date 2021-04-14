using System;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Configuration
{
	[KnownType(typeof(UMSettingsData))]
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[DataContract]
	[KnownType(typeof(OwaFlightConfigData))]
	[KnownType(typeof(OwaConfigurationBaseData))]
	[KnownType(typeof(SmimeSettingsData))]
	[KnownType(typeof(MobileDevicePolicyData))]
	[KnownType(typeof(PolicyTipsData))]
	[KnownType(typeof(OwaAttachmentPolicyData))]
	[KnownType(typeof(WacConfigData))]
	[KnownType(typeof(OwaHelpUrlData))]
	[KnownType(typeof(OwaOrgConfigData))]
	internal abstract class SerializableDataBase
	{
		public override bool Equals(object other)
		{
			return this.InternalEquals(other);
		}

		public override int GetHashCode()
		{
			return this.InternalGetHashCode();
		}

		protected abstract bool InternalEquals(object other);

		protected abstract int InternalGetHashCode();

		protected static bool ArrayContentsEquals<T>(T[] s1, T[] s2)
		{
			return object.ReferenceEquals(s1, s2) || (s1 != null && s2 != null && s1.SequenceEqual(s2));
		}

		protected static int ArrayContentsHash<T>(T[] a1)
		{
			int num = 17;
			if (a1 != null)
			{
				foreach (T t in a1)
				{
					if (t != null)
					{
						num = (num * 397 ^ t.GetHashCode());
					}
				}
			}
			return num;
		}
	}
}
