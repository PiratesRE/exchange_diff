using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.QueueViewer;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.QueueViewer;

namespace Microsoft.Exchange.Management.QueueViewerTasks
{
	public abstract class MessageActionWithIdentity : MessageAction
	{
		[Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, ParameterSetName = "Identity", Position = 0)]
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

		protected ServerIdParameter Server
		{
			get
			{
				return (ServerIdParameter)base.Fields["Server"];
			}
			set
			{
				base.Fields["Server"] = value;
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			if (this.Identity != null)
			{
				this.Server = ServerIdParameter.Parse(this.Identity.QueueIdentity.Server);
			}
		}

		protected override LocalizedException GetLocalizedException(Exception ex)
		{
			if (ex is QueueViewerException)
			{
				return ErrorMapper.GetLocalizedException((ex as QueueViewerException).ErrorCode, this.Identity, this.Server);
			}
			if (ex is RpcException)
			{
				return ErrorMapper.GetLocalizedException((ex as RpcException).ErrorCode, null, this.Server);
			}
			return null;
		}
	}
}
