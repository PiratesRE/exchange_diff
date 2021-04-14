using System;
using System.ComponentModel;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.MailSubmission;

namespace Microsoft.Exchange.Management.ResubmitRequest
{
	[Cmdlet("Get", "ResubmitRequest", DefaultParameterSetName = "Identity")]
	public sealed class GetResubmitRequest : GetObjectWithIdentityTaskBase<ResubmitRequestIdentityParameter, ResubmitRequest>
	{
		[Parameter(Mandatory = false, ValueFromPipeline = true)]
		[ValidateNotNullOrEmpty]
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

		protected override IConfigDataProvider CreateSession()
		{
			return new ResubmitRequestDataProvider(this.Server, (this.Identity == null) ? null : this.Identity.Identity);
		}

		protected override void InternalProcessRecord()
		{
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

		internal static void ProcessRpcError(RpcException rpcException, string server, Task task)
		{
			LocalizedException exception;
			ErrorCategory category;
			if (rpcException.ErrorCode == MailSubmissionServiceRpcClient.EndpointNotRegistered)
			{
				exception = new LocalizedException(Strings.TransportRpcNotRegistered(server));
				category = ErrorCategory.ResourceUnavailable;
			}
			else if (rpcException.ErrorCode == MailSubmissionServiceRpcClient.ServerUnavailable)
			{
				exception = new LocalizedException(Strings.TransportRpcUnavailable(server));
				category = ErrorCategory.InvalidOperation;
			}
			else
			{
				Win32Exception ex = new Win32Exception(rpcException.ErrorCode);
				exception = new LocalizedException(Strings.ResubmitRequestGenericRpcError(ex.Message), ex);
				category = ErrorCategory.InvalidOperation;
			}
			task.WriteError(exception, category, null);
		}
	}
}
