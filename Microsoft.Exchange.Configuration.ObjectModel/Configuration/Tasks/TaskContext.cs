using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public class TaskContext
	{
		internal Guid UniqueId { get; private set; }

		internal TaskInvocationInfo InvocationInfo { get; set; }

		internal ExchangeRunspaceConfiguration ExchangeRunspaceConfig { get; set; }

		internal ICommandShell CommandShell { get; set; }

		internal TaskUserInfo UserInfo { get; set; }

		internal ScopeSet ScopeSet { get; set; }

		internal ISessionState SessionState { get; set; }

		internal TaskStage Stage { get; set; }

		internal int CurrentObjectIndex { get; set; }

		internal TaskErrorInfo ErrorInfo { get; private set; }

		internal bool ShouldTerminateCmdletExecution { get; set; }

		internal IDictionary<string, object> Items { get; private set; }

		internal bool WasCancelled { get; set; }

		internal bool WasStopped { get; set; }

		internal bool ObjectWrittenToPipeline { get; set; }

		internal ADServerSettings ServerSettingsAfterFailOver { get; set; }

		internal bool CanBypassRBACScope
		{
			get
			{
				return this.ExchangeRunspaceConfig == null || (this.InvocationInfo.IsInternalOrigin && this.IsScriptInUserRole);
			}
		}

		private bool IsScriptInUserRole
		{
			get
			{
				return this.ExchangeRunspaceConfig.IsScriptInUserRole(this.InvocationInfo.ScriptName) || this.ExchangeRunspaceConfig.IsScriptInUserRole(this.InvocationInfo.RootScriptName);
			}
		}

		internal TaskContext(ICommandShell commandShell)
		{
			this.CommandShell = commandShell;
			this.UniqueId = Guid.NewGuid();
			this.CurrentObjectIndex = -1;
			this.Items = new Dictionary<string, object>();
			this.ErrorInfo = new TaskErrorInfo();
		}

		internal bool TryGetItem<T>(string key, ref T value)
		{
			object obj;
			if (this.Items.TryGetValue(key, out obj))
			{
				value = (T)((object)obj);
				return true;
			}
			return false;
		}

		public void Reset()
		{
			if (this.ErrorInfo != null)
			{
				this.ErrorInfo.ResetErrorInfo();
			}
			this.WasCancelled = false;
			this.ServerSettingsAfterFailOver = null;
		}
	}
}
