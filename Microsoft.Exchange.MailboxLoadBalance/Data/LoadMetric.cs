using System;
using System.Globalization;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.MailboxLoadBalance.Directory;

namespace Microsoft.Exchange.MailboxLoadBalance.Data
{
	[DataContract]
	internal class LoadMetric
	{
		public LoadMetric(string name, bool sizeMetric)
		{
			this.name = name;
			this.isSizeMetric = sizeMetric;
		}

		public LocalizedString FriendlyName
		{
			get
			{
				return new LocalizedString(this.Name);
			}
		}

		public virtual bool IsSize
		{
			get
			{
				return this.isSizeMetric;
			}
		}

		public virtual string Name
		{
			get
			{
				return this.name;
			}
		}

		public static bool operator ==(LoadMetric left, LoadMetric right)
		{
			return object.Equals(left, right);
		}

		public static bool operator !=(LoadMetric left, LoadMetric right)
		{
			return !object.Equals(left, right);
		}

		public override bool Equals(object obj)
		{
			if (object.ReferenceEquals(null, obj))
			{
				return false;
			}
			if (object.ReferenceEquals(this, obj))
			{
				return true;
			}
			LoadMetric left = obj as LoadMetric;
			return !(left == null) && this.Equals((LoadMetric)obj);
		}

		public override int GetHashCode()
		{
			if (this.name == null)
			{
				return 0;
			}
			return this.name.GetHashCode();
		}

		public virtual EntitySelector GetSelector(LoadContainer container, string constraintSetIdentity, long units)
		{
			return new NullEntitySelector();
		}

		public virtual long GetUnitsForMailbox(DirectoryMailbox mailbox)
		{
			return 0L;
		}

		public virtual long GetUnitsForSize(ByteQuantifiedSize size)
		{
			if (!this.IsSize)
			{
				throw new InvalidOperationException("Cannot get units for size on a non-size metric.");
			}
			return (long)size.ToBytes();
		}

		public virtual string GetValueString(long value)
		{
			string arg = this.IsSize ? this.ToByteQuantifiedSize(value).ToString() : value.ToString(CultureInfo.InvariantCulture);
			return string.Format("{0}={1}", this.Name, arg);
		}

		public virtual ByteQuantifiedSize ToByteQuantifiedSize(long value)
		{
			if (!this.isSizeMetric)
			{
				throw new InvalidOperationException("Cannot convert a non-size based metric to ByteQuantifiedSize.");
			}
			return this.CreateByteQuantifiedSizeFromValue((ulong)value);
		}

		public override string ToString()
		{
			return string.Format("LoadMetric::{0}", this.name);
		}

		protected virtual ByteQuantifiedSize CreateByteQuantifiedSizeFromValue(ulong value)
		{
			return ByteQuantifiedSize.FromBytes(value);
		}

		protected bool Equals(LoadMetric other)
		{
			return string.Equals(this.name, other.name);
		}

		[DataMember]
		private readonly bool isSizeMetric;

		[DataMember]
		private readonly string name;
	}
}
