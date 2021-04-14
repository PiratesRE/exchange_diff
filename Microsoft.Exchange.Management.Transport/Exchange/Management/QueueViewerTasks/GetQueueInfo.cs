using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.QueueViewer;

namespace Microsoft.Exchange.Management.QueueViewerTasks
{
	[Cmdlet("Get", "Queue", DefaultParameterSetName = "Filter")]
	public sealed class GetQueueInfo : RpcPagedGetObjectTask<ExtensibleQueueInfo>
	{
		[ValidateNotNullOrEmpty]
		[Parameter(ParameterSetName = "Identity", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public QueueIdentity Identity
		{
			get
			{
				return (QueueIdentity)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public QueueViewerIncludesAndExcludes Include
		{
			get
			{
				return (QueueViewerIncludesAndExcludes)base.Fields["Include"];
			}
			set
			{
				base.Fields["Include"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public QueueViewerIncludesAndExcludes Exclude
		{
			get
			{
				return (QueueViewerIncludesAndExcludes)base.Fields["Exclude"];
			}
			set
			{
				base.Fields["Exclude"] = value;
			}
		}

		internal override ObjectSchema FilterableObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<ExtensibleQueueInfoSchema>();
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			this.InitializeInnerFilter<ExtensibleQueueInfo>(null, ExtensibleQueueInfoSchema.Identity);
			base.InternalValidate();
			if (base.HasErrors)
			{
				TaskLogger.LogExit();
				return;
			}
			TaskLogger.LogExit();
			this.ValidateIncludeExclude();
		}

		internal override void InitializeInnerFilter<Object>(QueueViewerPropertyDefinition<Object> messageIdentity, QueueViewerPropertyDefinition<Object> queueIdentity)
		{
			if (this.Identity != null)
			{
				if (this.Identity.IsFullySpecified)
				{
					this.innerFilter = new ComparisonFilter(ComparisonOperator.Equal, queueIdentity, this.Identity);
				}
				else
				{
					this.innerFilter = new TextFilter(queueIdentity, this.Identity.ToString(), MatchOptions.FullString, MatchFlags.Default);
				}
				base.Server = ServerIdParameter.Parse(this.Identity.Server);
			}
		}

		private void ValidateIncludeExclude()
		{
			QueueViewerIncludesAndExcludes exclude = this.Exclude;
			QueueViewerIncludesAndExcludes include = this.Include;
			if (exclude == null && include == null)
			{
				return;
			}
			if (this.Identity != null)
			{
				base.Server = ServerIdParameter.Parse(this.Identity.Server);
			}
			string filter;
			LocalizedString localizedString;
			if (QueueViewerIncludesAndExcludes.ComposeFilterString(base.Filter, exclude, include, out filter, out localizedString))
			{
				base.Filter = filter;
				return;
			}
			base.WriteError(new LocalizedException(localizedString), ErrorCategory.InvalidData, base.Filter);
		}
	}
}
