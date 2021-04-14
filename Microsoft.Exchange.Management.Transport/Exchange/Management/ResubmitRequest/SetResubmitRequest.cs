using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.Management.ResubmitRequest
{
	[Cmdlet("Set", "ResubmitRequest", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
	public sealed class SetResubmitRequest : SetObjectWithIdentityTaskBase<ResubmitRequestIdentityParameter, ResubmitRequest, ResubmitRequest>
	{
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false)]
		public ServerIdParameter Server
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

		[Parameter(Mandatory = true)]
		public bool Enabled
		{
			get
			{
				return (bool)base.Fields["Enabled"];
			}
			set
			{
				base.Fields["Enabled"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.SetResubmitRequestConfirmation;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return new ResubmitRequestDataProvider(this.Server, (this.Identity == null) ? null : this.Identity.Identity);
		}

		protected override void InternalValidate()
		{
			try
			{
				if (this.Server == null)
				{
					this.Server = new ServerIdParameter();
				}
				base.InternalValidate();
			}
			catch (RpcException rpcException)
			{
				GetResubmitRequest.ProcessRpcError(rpcException, this.Server.Fqdn, this);
			}
			catch (ResubmitRequestException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidArgument, null);
			}
			catch (LocalizedException exception2)
			{
				base.WriteError(exception2, ExchangeErrorCategory.Client, null);
			}
		}

		protected override void InternalProcessRecord()
		{
			this.DataObject.SetExchangeVersion(ExchangeObjectVersion.Exchange2010);
			if (this.Enabled)
			{
				this.DataObject.State = ResubmitRequestState.Running;
			}
			else
			{
				this.DataObject.State = ResubmitRequestState.Paused;
			}
			try
			{
				base.InternalProcessRecord();
			}
			catch (RpcException rpcException)
			{
				GetResubmitRequest.ProcessRpcError(rpcException, this.Server.Fqdn, this);
			}
			catch (ResubmitRequestException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidArgument, null);
			}
			catch (LocalizedException exception2)
			{
				base.WriteError(exception2, ExchangeErrorCategory.Client, null);
			}
		}
	}
}
