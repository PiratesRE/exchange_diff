using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public abstract class PagedGetObjectTask<T> : Task where T : IConfigurable
	{
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ParameterSetName = "Filter")]
		public string Filter
		{
			get
			{
				return (string)base.Fields["Filter"];
			}
			set
			{
				this.innerFilter = new MonadFilter(value, this, this.FilterableObjectSchema).InnerFilter;
				base.Fields["Filter"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public T BookmarkObject
		{
			get
			{
				return (T)((object)base.Fields["BookmarkObject"]);
			}
			set
			{
				base.Fields["BookmarkObject"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int BookmarkIndex
		{
			get
			{
				return (int)base.Fields["BookmarkIndex"];
			}
			set
			{
				base.Fields["BookmarkIndex"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IncludeBookmark
		{
			get
			{
				return (bool)base.Fields["IncludeBookmark"];
			}
			set
			{
				base.Fields["IncludeBookmark"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int> ResultSize
		{
			get
			{
				return (Unlimited<int>)base.Fields["ResultSize"];
			}
			set
			{
				base.Fields["ResultSize"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool SearchForward
		{
			get
			{
				return (bool)base.Fields["SearchForward"];
			}
			set
			{
				base.Fields["SearchForward"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public QueueViewerSortOrderEntry[] SortOrder
		{
			get
			{
				return (QueueViewerSortOrderEntry[])base.Fields["SortOrder"];
			}
			set
			{
				base.Fields["SortOrder"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ReturnPageInfo
		{
			get
			{
				return (bool)base.Fields["ReturnPageInfo"];
			}
			set
			{
				base.Fields["ReturnPageInfo"] = value;
			}
		}

		internal abstract ObjectSchema FilterableObjectSchema { get; }

		protected override ITaskModuleFactory CreateTaskModuleFactory()
		{
			return new GetTaskBaseModuleFactory();
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			if (base.Fields["BookmarkIndex"] == null)
			{
				this.BookmarkIndex = 0;
			}
			if (this.BookmarkIndex > 0 && this.BookmarkObject != null)
			{
				base.ThrowTerminatingError(new InvalidOperationException(Strings.MutuallyExclusiveArguments("BookmarkIndex", "BookmarkObject")), ErrorCategory.InvalidArgument, null);
			}
			if (base.Fields["ResultSize"] == null)
			{
				this.ResultSize = 1000;
			}
			if (!this.ResultSize.IsUnlimited && this.ResultSize.Value < 0)
			{
				base.ThrowTerminatingError(new ArgumentOutOfRangeException("ResultSize", this.ResultSize, string.Empty), ErrorCategory.InvalidArgument, null);
			}
			if (base.Fields["SearchForward"] == null)
			{
				this.SearchForward = true;
			}
			if (base.Fields["SortOrder"] == null)
			{
				this.SortOrder = null;
			}
			if (base.Fields["IncludeBookmark"] == null)
			{
				this.IncludeBookmark = true;
			}
			if (base.Fields["ReturnPageInfo"] == null)
			{
				this.ReturnPageInfo = false;
			}
		}

		protected virtual void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter(new object[]
			{
				dataObject.Identity,
				dataObject
			});
			base.WriteObject(dataObject);
			TaskLogger.LogExit();
		}

		protected QueryFilter innerFilter;
	}
}
