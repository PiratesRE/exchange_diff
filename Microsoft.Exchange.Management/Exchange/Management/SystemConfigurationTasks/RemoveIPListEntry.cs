﻿using System;
using System.ComponentModel;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public abstract class RemoveIPListEntry<T> : RemoveTaskBase<IPListEntryIdentity, T> where T : IPListEntry, new()
	{
		[Parameter(Mandatory = false)]
		public ServerIdParameter Server
		{
			get
			{
				return this.serverId;
			}
			set
			{
				this.serverId = value;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			if (this.server == null)
			{
				throw new InvalidOperationException("target RPC server (this.server) must be initialized prior to calling CreateSession().");
			}
			return new IPListDataProvider(this.server.Name);
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || typeof(RpcException).IsInstanceOfType(exception) || typeof(RpcClientException).IsInstanceOfType(exception) || typeof(Win32Exception).IsInstanceOfType(exception);
		}

		protected override void TranslateException(ref Exception e, out ErrorCategory category)
		{
			category = (ErrorCategory)1001;
			e = RpcClientException.TranslateRpcException(e);
			if (typeof(RpcClientException).IsInstanceOfType(e))
			{
				e = new LocalizedException(Strings.ConnectionToIPListRPCEndpointFailed((this.server != null) ? this.server.Name : Environment.MachineName), e);
				return;
			}
			if (typeof(Win32Exception).IsInstanceOfType(e))
			{
				e = new LocalizedException(Strings.GenericError(e.Message), e);
				return;
			}
			base.TranslateException(ref e, out category);
		}

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			if (this.serverId == null)
			{
				this.serverId = new ServerIdParameter();
			}
			IConfigurationSession session = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 121, "InternalBeginProcessing", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\MessageHygiene\\IPAllowBlockListEntries\\RemoveIPListEntry.cs");
			this.server = (Server)base.GetDataObject<Server>(this.serverId, session, null, new LocalizedString?(Strings.ErrorServerNotFound(this.serverId.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(this.serverId.ToString())));
			if (this.server == null || !IPListEntryUtils.IsSupportedRole(this.server))
			{
				base.WriteError(new LocalizedException(Strings.ErrorInvalidServerRole((this.server != null) ? this.server.Name : Environment.MachineName)), ErrorCategory.InvalidOperation, null);
			}
		}

		private ServerIdParameter serverId;

		private Server server;
	}
}
