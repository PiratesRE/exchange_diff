using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.QueueViewer;
using Microsoft.Exchange.Transport.QueueViewer;

namespace Microsoft.Exchange.Management.QueueViewerTasks
{
	public abstract class MessageActionWithFilter : MessageActionWithIdentity
	{
		[Parameter(Mandatory = true, ParameterSetName = "Filter")]
		[ValidateNotNullOrEmpty]
		public string Filter
		{
			get
			{
				return (string)base.Fields["Filter"];
			}
			set
			{
				this.InitializeInnerFilter<ExtensibleMessageInfoSchema>(value, ObjectSchema.GetInstance<ExtensibleMessageInfoSchema>());
				base.Fields["Filter"] = value;
			}
		}

		[Parameter(ParameterSetName = "Filter", ValueFromPipeline = true)]
		public new ServerIdParameter Server
		{
			get
			{
				return base.Server;
			}
			set
			{
				base.Server = value;
			}
		}

		internal void InitializeInnerFilter<Schema>(string filterString, Schema messageInfoSchema) where Schema : PagedObjectSchema
		{
			QueryFilter filter = new MonadFilter(filterString, this, messageInfoSchema).InnerFilter;
			this.innerFilter = DateTimeConverter.ConvertQueryFilter(filter);
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			if (base.Identity == null && this.Server == null)
			{
				this.Server = ServerIdParameter.Parse("localhost");
			}
			if (this.Filter != null && !VersionedQueueViewerClient.UsePropertyBagBasedAPI((string)this.Server))
			{
				this.InitializeInnerFilter<MessageInfoSchema>(this.Filter, ObjectSchema.GetInstance<MessageInfoSchema>());
			}
		}

		protected QueryFilter innerFilter;
	}
}
