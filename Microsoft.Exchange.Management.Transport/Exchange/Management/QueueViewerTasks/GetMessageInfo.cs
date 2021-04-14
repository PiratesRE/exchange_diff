using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.QueueViewer;

namespace Microsoft.Exchange.Management.QueueViewerTasks
{
	[Cmdlet("Get", "Message", DefaultParameterSetName = "Filter")]
	public sealed class GetMessageInfo : RpcPagedGetObjectTask<ExtensibleMessageInfo>
	{
		public GetMessageInfo()
		{
			this.IncludeRecipientInfo = new SwitchParameter(true);
		}

		[Parameter(ParameterSetName = "Identity", Position = 0)]
		[ValidateNotNullOrEmpty]
		public MessageIdentity Identity
		{
			get
			{
				return (MessageIdentity)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(ParameterSetName = "Queue", ValueFromPipeline = true)]
		public QueueIdentity Queue
		{
			get
			{
				return (QueueIdentity)base.Fields["Queue"];
			}
			set
			{
				base.Fields["Queue"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IncludeRecipientInfo
		{
			get
			{
				return base.IncludeDetails;
			}
			set
			{
				base.IncludeDetails = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IncludeComponentLatencyInfo
		{
			get
			{
				return base.IncludeLatencyInfo;
			}
			set
			{
				base.IncludeLatencyInfo = value;
			}
		}

		internal override ObjectSchema FilterableObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<ExtensibleMessageInfoSchema>();
			}
		}

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			if (base.NeedSuppressingPiiData && base.ExchangeRunspaceConfig != null)
			{
				base.ExchangeRunspaceConfig.EnablePiiMap = true;
			}
		}

		protected override void InternalValidate()
		{
			this.InitializeInnerFilter<ExtensibleMessageInfo>(ExtensibleMessageInfoSchema.Identity, ExtensibleMessageInfoSchema.Queue);
			base.InternalValidate();
		}

		internal override void InitializeInnerFilter<Object>(QueueViewerPropertyDefinition<Object> messageIdentity, QueueViewerPropertyDefinition<Object> queueIdentity)
		{
			if (this.Identity != null)
			{
				if (this.Identity.IsFullySpecified)
				{
					this.innerFilter = new ComparisonFilter(ComparisonOperator.Equal, messageIdentity, this.Identity);
				}
				else
				{
					this.innerFilter = new TextFilter(messageIdentity, this.Identity.ToString(), MatchOptions.FullString, MatchFlags.Default);
				}
				base.Server = ServerIdParameter.Parse(this.Identity.QueueIdentity.Server);
				return;
			}
			if (this.Queue != null)
			{
				if (this.Queue.IsFullySpecified)
				{
					this.innerFilter = new ComparisonFilter(ComparisonOperator.Equal, queueIdentity, this.Queue);
				}
				else
				{
					this.innerFilter = new TextFilter(queueIdentity, this.Queue.ToString(), MatchOptions.FullString, MatchFlags.Default);
				}
				base.Server = ServerIdParameter.Parse(this.Queue.Server);
			}
		}
	}
}
