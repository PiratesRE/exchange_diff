using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Threading;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.PowerShell.HostingTools;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal class CmdletProxyDataReader : IEnumerable<PSObject>, IEnumerable, IDisposable
	{
		private void AssertReaderIsOpen()
		{
			if (this.IsClosed)
			{
				throw new InvalidOperationException("Reader is already closed.");
			}
		}

		public CmdletProxyDataReader(RunspaceMediator runspaceMediator, PSCommand cmd, Task.TaskWarningLoggingDelegate writeWarning)
		{
			this.runspaceProxy = new RunspaceProxy(runspaceMediator, true);
			bool flag = false;
			try
			{
				this.shellProxy = new PowerShellProxy(this.runspaceProxy, cmd);
				this.writeWarning = writeWarning;
				this.asyncResult = (PowerShellAsyncResult<PSObject>)this.shellProxy.BeginInvoke(null, null);
				PSDataCollection<PSObject> output = this.asyncResult.Output;
				this.shellProxy.PowerShell.InvocationStateChanged += this.PipelineStateChanged;
				output.DataAdded += this.PipelineDataAdded;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					this.Close();
				}
			}
		}

		private bool Read()
		{
			this.AssertReaderIsOpen();
			this.currentRecord = null;
			PSDataCollection<PSObject> output = this.asyncResult.Output;
			if (this.WaitOne(output))
			{
				this.currentRecord = output[0];
				output.RemoveAt(0);
			}
			if (this.currentRecord == null && this.shellProxy.Errors != null && this.shellProxy.Errors.Count != 0 && this.shellProxy.Errors[0].Exception != null)
			{
				try
				{
					throw this.shellProxy.Errors[0].Exception;
				}
				finally
				{
					this.Close();
				}
			}
			if (this.shellProxy != null && this.shellProxy.Warnings != null && this.shellProxy.Warnings.Count != 0)
			{
				foreach (WarningRecord warningRecord in this.shellProxy.Warnings)
				{
					this.writeWarning(new LocalizedString(warningRecord.Message));
				}
			}
			if (this.currentRecord == null)
			{
				this.Close();
			}
			return null != this.currentRecord;
		}

		public void Close()
		{
			if (!this.IsClosed)
			{
				try
				{
					if (this.shellProxy != null && this.asyncResult != null)
					{
						this.shellProxy.EndInvoke(this.asyncResult);
					}
				}
				finally
				{
					if (this.shellProxy != null)
					{
						this.shellProxy.PowerShell.InvocationStateChanged -= this.PipelineStateChanged;
					}
					if (this.asyncResult != null)
					{
						PSDataCollection<PSObject> output = this.asyncResult.Output;
						if (output != null)
						{
							output.DataAdded -= this.PipelineDataAdded;
						}
					}
					this.UnblockWaitOne();
					this.isClosed = true;
					if (this.runspaceProxy != null)
					{
						this.runspaceProxy.Dispose();
						this.runspaceProxy = null;
					}
				}
			}
		}

		public bool IsClosed
		{
			get
			{
				return this.isClosed;
			}
		}

		public PSObject CurrentRecord
		{
			get
			{
				return this.currentRecord;
			}
		}

		public IEnumerator<PSObject> GetEnumerator()
		{
			while (this.Read())
			{
				yield return this.CurrentRecord;
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.Close();
			}
		}

		protected virtual int GetMaxTimeoutMinutes()
		{
			return 15;
		}

		~CmdletProxyDataReader()
		{
			this.Dispose(false);
		}

		private void PipelineDataAdded(object sender, DataAddedEventArgs e)
		{
			this.UnblockWaitOne();
		}

		private void PipelineStateChanged(object sender, PSInvocationStateChangedEventArgs e)
		{
			if (e.InvocationStateInfo.State == PSInvocationState.Failed || e.InvocationStateInfo.State == PSInvocationState.Stopped || e.InvocationStateInfo.State == PSInvocationState.Completed)
			{
				this.UnblockWaitOne();
			}
		}

		private bool WaitOne(PSDataCollection<PSObject> output)
		{
			lock (this.syncObject)
			{
				ExDateTime now = ExDateTime.Now;
				while (output.IsOpen && output.Count == 0 && (this.shellProxy.PowerShell.InvocationStateInfo.State == PSInvocationState.NotStarted || this.shellProxy.PowerShell.InvocationStateInfo.State == PSInvocationState.Running))
				{
					if (!Monitor.Wait(this.syncObject, TimeSpan.FromMinutes((double)this.GetMaxTimeoutMinutes())))
					{
						ExDateTime now2 = ExDateTime.Now;
						throw new TimeoutException(Strings.PowerShellTimeout((int)(now2 - now).TotalMinutes));
					}
				}
			}
			return output.Count > 0;
		}

		private void UnblockWaitOne()
		{
			lock (this.syncObject)
			{
				Monitor.PulseAll(this.syncObject);
			}
		}

		private const int MaxWaitMinutes = 15;

		private PowerShellProxy shellProxy;

		private PowerShellAsyncResult<PSObject> asyncResult;

		private RunspaceProxy runspaceProxy;

		private PSObject currentRecord;

		private bool isClosed;

		private object syncObject = new object();

		private readonly Task.TaskWarningLoggingDelegate writeWarning;
	}
}
