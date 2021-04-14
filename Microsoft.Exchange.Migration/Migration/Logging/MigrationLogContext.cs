using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration.Logging
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MigrationLogContext
	{
		private MigrationLogContext()
		{
		}

		public static MigrationLogContext Current
		{
			get
			{
				MigrationLogContext result;
				if ((result = MigrationLogContext.context) == null)
				{
					result = (MigrationLogContext.context = new MigrationLogContext());
				}
				return result;
			}
		}

		public string Source
		{
			get
			{
				if (string.IsNullOrEmpty(this.source))
				{
					return "Default";
				}
				return this.source;
			}
			set
			{
				this.source = value;
				this.isDirty = true;
			}
		}

		public ADObjectId Organization
		{
			get
			{
				return this.org;
			}
			set
			{
				this.org = value;
				this.isDirty = true;
			}
		}

		public MigrationJob Job
		{
			get
			{
				return this.job;
			}
			set
			{
				this.job = value;
				this.isDirty = true;
			}
		}

		public string JobItem
		{
			get
			{
				return this.jobItem;
			}
			set
			{
				this.jobItem = value;
				this.isDirty = true;
			}
		}

		public static void Clear()
		{
			MigrationLogContext.context = null;
		}

		public override string ToString()
		{
			if (!this.isDirty)
			{
				return this.cachedTostring;
			}
			this.isDirty = false;
			string[] value = new string[]
			{
				(this.org != null) ? this.org.Name : string.Empty,
				(this.job != null) ? (this.job.JobId.ToString() + "," + this.job.JobName) : string.Empty,
				(this.jobItem != null) ? this.jobItem : string.Empty
			};
			this.cachedTostring = string.Join(",", value);
			return this.cachedTostring;
		}

		public override int GetHashCode()
		{
			return ((this.org == null) ? 0 : this.org.GetHashCode()) + ((this.job == null) ? 0 : this.job.GetHashCode()) + ((this.jobItem == null) ? 0 : this.jobItem.GetHashCode());
		}

		internal const string DefaultSeparator = ",";

		private const string DefaultSource = "Default";

		[ThreadStatic]
		private static MigrationLogContext context;

		private string source;

		private ADObjectId org;

		private MigrationJob job;

		private string jobItem;

		private string cachedTostring = string.Empty;

		private bool isDirty = true;
	}
}
