using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal sealed class NspiBindDispatchTask : NspiStateDispatchTask
	{
		public NspiBindDispatchTask(CancelableAsyncCallback asyncCallback, object asyncState, ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding, NspiContext context, NspiBindFlags flags, NspiState state, Guid? serverGuid) : base(asyncCallback, asyncState, protocolRequestInfo, context, state)
		{
			this.clientBinding = clientBinding;
			this.flags = flags;
			this.serverGuid = serverGuid;
		}

		public NspiStatus End(out Guid? guid, out int contextHandle)
		{
			base.CheckDisposed();
			bool flag = false;
			NspiStatus status;
			try
			{
				base.CheckCompletion();
				guid = this.serverGuid;
				if (base.Status == NspiStatus.Success)
				{
					contextHandle = base.ContextHandle;
				}
				else
				{
					contextHandle = 0;
				}
				flag = true;
				status = base.Status;
			}
			finally
			{
				if (!flag || base.Status != NspiStatus.Success)
				{
					base.IsContextRundown = true;
				}
			}
			return status;
		}

		protected override string TaskName
		{
			get
			{
				return "NspiBind";
			}
		}

		protected override void InternalDebugTrace()
		{
			NspiDispatchTask.NspiTracer.TraceDebug<string, NspiBindFlags, string>((long)base.ContextHandle, "{0} params: flags={1}, guid={2}", this.TaskName, this.flags, (this.serverGuid != null) ? this.serverGuid.Value.ToString() : "<null>");
		}

		protected override void InternalTaskExecute()
		{
			base.InternalTaskExecute();
			this.serverGuid = new Guid?(base.Context.Guid);
			base.Context.ProtocolLogSession[ProtocolLog.Field.Authentication] = this.clientBinding.AuthenticationType.ToString();
			if (!base.Context.IsAnonymous && Configuration.EncryptionRequired && !this.clientBinding.IsEncrypted)
			{
				NspiDispatchTask.NspiTracer.TraceError((long)base.ContextHandle, "Encrypted connection is required.");
				throw new NspiException(NspiStatus.NotSupported, "EncryptionRequired");
			}
			base.NspiContextCallWrapper("Bind", () => base.Context.Bind(this.flags, base.NspiState));
			if (base.Status == NspiStatus.Success)
			{
				try
				{
					base.Context.Budget.StartConnection("NspiBindDispatchTask");
				}
				catch (OverBudgetException)
				{
					NspiDispatchTask.NspiTracer.TraceDebug<string>((long)base.ContextHandle, "Too many connections for user {0}", base.Context.UserIdentity);
					throw new NspiException(NspiStatus.GeneralFailure, "TooManyConnections");
				}
			}
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<NspiBindDispatchTask>(this);
		}

		private const string Name = "NspiBind";

		private readonly ClientBinding clientBinding;

		private readonly NspiBindFlags flags;

		private Guid? serverGuid = null;
	}
}
