using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Rpc.IPFilter;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	public abstract class IPListEntry : IConfigurable
	{
		internal static T NewIPListEntry<T>(IPFilterRange range) where T : IConfigurable, new()
		{
			IPListEntry iplistEntry = (IPListEntry)((object)((default(T) == null) ? Activator.CreateInstance<T>() : default(T)));
			IPRange.Format format = (IPRange.Format)(range.Flags & 15);
			IPListEntryType iplistEntryType = (IPListEntryType)(range.Flags >> 4 & 15);
			IPListEntry.AuthorType authorType = (IPListEntry.AuthorType)(range.Flags >> 8 & 15);
			iplistEntry.identity = new IPListEntryIdentity(range.Identity);
			ulong high;
			ulong low;
			range.GetLowerBound(out high, out low);
			IPvxAddress startAddress = new IPvxAddress(high, low);
			range.GetUpperBound(out high, out low);
			IPvxAddress endAddress = new IPvxAddress(high, low);
			iplistEntry.range = new IPRange(startAddress, endAddress, format);
			iplistEntry.expirationTimeUtc = range.ExpiresOn.ToUniversalTime();
			iplistEntry.isMachineGenerated = (authorType == IPListEntry.AuthorType.Automatic);
			iplistEntry.listType = iplistEntryType;
			iplistEntry.comment = range.Comment;
			return (T)((object)iplistEntry);
		}

		protected IPListEntry()
		{
			this.identity = null;
			this.range = null;
			this.expirationTimeUtc = DateTime.MaxValue;
			this.isMachineGenerated = false;
			this.listType = this.ListType;
			this.isValid = true;
			this.comment = string.Empty;
		}

		public abstract IPListEntryType ListType { get; }

		public IPRange IPRange
		{
			get
			{
				return this.range;
			}
			set
			{
				this.range = value;
			}
		}

		public DateTime ExpirationTime
		{
			get
			{
				return this.expirationTimeUtc.ToLocalTime();
			}
			set
			{
				this.expirationTimeUtc = value.ToUniversalTime();
			}
		}

		public string Comment
		{
			get
			{
				return this.comment;
			}
			set
			{
				this.comment = value;
			}
		}

		public bool IsMachineGenerated
		{
			get
			{
				return this.isMachineGenerated;
			}
		}

		public bool HasExpired
		{
			get
			{
				return this.expirationTimeUtc < DateTime.UtcNow;
			}
		}

		public ObjectId Identity
		{
			get
			{
				return this.identity;
			}
			internal set
			{
				this.identity = (IPListEntryIdentity)value;
			}
		}

		public bool IsValid
		{
			get
			{
				return this.isValid;
			}
		}

		public ObjectState ObjectState
		{
			get
			{
				return ObjectState.Unchanged;
			}
		}

		internal IPFilterRange ToIPFilterRange()
		{
			IPFilterRange ipfilterRange = new IPFilterRange();
			ipfilterRange.ExpiresOn = this.expirationTimeUtc;
			IPvxAddress pvxAddress = this.IPRange.LowerBound;
			ipfilterRange.SetLowerBound((ulong)(pvxAddress >> 64), (ulong)pvxAddress);
			pvxAddress = this.IPRange.UpperBound;
			ipfilterRange.SetUpperBound((ulong)(pvxAddress >> 64), (ulong)pvxAddress);
			ipfilterRange.Flags = (int)this.IPRange.RangeFormat;
			ipfilterRange.Comment = this.comment;
			return ipfilterRange;
		}

		public ValidationError[] Validate()
		{
			List<ValidationError> list = new List<ValidationError>();
			if (this.listType != this.ListType)
			{
				list.Add(new PropertyValidationError(Strings.IPListEntryTypeMismatch, null, this.ListType));
			}
			this.isValid = (list.Count == 0);
			return list.ToArray();
		}

		public void CopyChangesFrom(IConfigurable source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			IPListEntry iplistEntry = source as IPListEntry;
			if (iplistEntry == null)
			{
				throw new ArgumentException(DirectoryStrings.ExceptionCopyChangesForIncompatibleTypes(source.GetType(), base.GetType()), "source");
			}
			if (iplistEntry.identity != null)
			{
				this.identity = new IPListEntryIdentity(iplistEntry.identity.Index);
			}
			if (iplistEntry.range != null)
			{
				this.range = new IPRange(new IPvxAddress(iplistEntry.range.LowerBound), new IPvxAddress(iplistEntry.range.UpperBound), iplistEntry.range.RangeFormat);
			}
			this.expirationTimeUtc = iplistEntry.expirationTimeUtc;
			this.isMachineGenerated = iplistEntry.IsMachineGenerated;
			this.listType = iplistEntry.ListType;
			this.isValid = iplistEntry.IsValid;
			this.comment = iplistEntry.Comment;
		}

		public void ResetChangeTracking()
		{
			throw new NotSupportedException("IPListEntry.CopyChangesFrom");
		}

		private IPListEntryIdentity identity;

		private IPRange range;

		private DateTime expirationTimeUtc;

		private bool isMachineGenerated;

		private IPListEntryType listType;

		private bool isValid;

		private string comment;

		private enum AuthorType
		{
			Undefined,
			Manual,
			Automatic
		}

		internal struct FieldNames
		{
			public const string IPAddress = "IPAddress";
		}
	}
}
