using System;
using System.ComponentModel;
using System.Management.Automation;
using System.Net;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public abstract class AddIPListEntry<T> : NewTaskBase<T> where T : IPListEntry, new()
	{
		protected override IConfigDataProvider CreateSession()
		{
			if (this.server == null)
			{
				throw new InvalidOperationException("target RPC server (this.server) must be initialized prior to calling CreateSession().");
			}
			return new IPListDataProvider(this.server.Name);
		}

		[Parameter(Mandatory = true, ParameterSetName = "IPRange")]
		public IPRange IPRange
		{
			get
			{
				T dataObject = this.DataObject;
				return dataObject.IPRange;
			}
			set
			{
				T dataObject = this.DataObject;
				dataObject.IPRange = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "IPAddress")]
		public IPAddress IPAddress
		{
			get
			{
				T dataObject = this.DataObject;
				return dataObject.IPRange.UpperBound;
			}
			set
			{
				T dataObject = this.DataObject;
				dataObject.IPRange = IPRange.CreateSingleAddress(value);
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime ExpirationTime
		{
			get
			{
				T dataObject = this.DataObject;
				return dataObject.ExpirationTime;
			}
			set
			{
				T dataObject = this.DataObject;
				dataObject.ExpirationTime = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Comment
		{
			get
			{
				T dataObject = this.DataObject;
				return dataObject.Comment;
			}
			set
			{
				T dataObject = this.DataObject;
				dataObject.Comment = value;
			}
		}

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
			IConfigurationSession session = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 168, "InternalBeginProcessing", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\MessageHygiene\\IPAllowBlockListEntries\\AddIPListEntry.cs");
			this.server = (Server)base.GetDataObject<Server>(this.serverId, session, null, new LocalizedString?(Strings.ErrorServerNotFound(this.serverId.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(this.serverId.ToString())));
			if (this.server == null || !IPListEntryUtils.IsSupportedRole(this.server))
			{
				base.WriteError(new LocalizedException(Strings.ErrorInvalidServerRole((this.server != null) ? this.server.Name : Environment.MachineName)), ErrorCategory.InvalidOperation, null);
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (!base.HasErrors)
			{
				T dataObject = this.DataObject;
				if (dataObject.HasExpired)
				{
					base.WriteError(new LocalizedException(Strings.ExpirationTimeTooSoon(this.ExpirationTime)), ErrorCategory.InvalidArgument, this.DataObject);
				}
			}
			TaskLogger.LogExit();
		}

		private ServerIdParameter serverId;

		private Server server;
	}
}
