using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data.Directory.EventLog;

namespace Microsoft.Exchange.Data.Directory.ProvisioningCache
{
	internal class DiagnosticActivity : Activity
	{
		public DiagnosticActivity(ProvisioningCache cache, string pipeName) : base(cache)
		{
			if (string.IsNullOrWhiteSpace(pipeName))
			{
				throw new ArgumentException("pipeName is null or empty.");
			}
			this.pipeName = pipeName;
			this.serverStream = new NamedPipeServerStream(pipeName, PipeDirection.InOut, 4);
		}

		public override string Name
		{
			get
			{
				return "ProvisioningCache Diagnostic command receiver";
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.serverStream != null)
			{
				this.serverStream.Close();
				this.serverStream.Dispose();
				this.serverStream = null;
			}
		}

		protected override void InternalExecute()
		{
			Globals.LogEvent(DirectoryEventLogConstants.Tuple_PCStartingToReceiveDiagnosticCommand, this.pipeName, new object[]
			{
				this.pipeName
			});
			try
			{
				while (!base.GotStopSignalFromTestCode)
				{
					try
					{
						this.isWaitForConnection = true;
						this.serverStream.WaitForConnection();
						this.ProcessClientAccept();
					}
					catch (InvalidOperationException ex)
					{
						Globals.LogEvent(DirectoryEventLogConstants.Tuple_PCFailedToReceiveDiagnosticCommand, this.pipeName, new object[]
						{
							this.pipeName,
							ex.ToString()
						});
						throw ex;
					}
					catch (IOException ex2)
					{
						Globals.LogEvent(DirectoryEventLogConstants.Tuple_PCFailedToReceiveDiagnosticCommand, this.pipeName, new object[]
						{
							this.pipeName,
							ex2.ToString()
						});
						throw ex2;
					}
					finally
					{
						this.isWaitForConnection = false;
					}
				}
			}
			finally
			{
				this.InternalDispose(true);
			}
		}

		private void ProcessDiagnosticCommand(DiagnosticCommand command, PipeStream stream)
		{
			if (command.Command == DiagnosticType.Reset)
			{
				if (command.IsGlobalCacheEntryCommand)
				{
					base.ProvisioningCache.RemoveGlobalDatas(command.CacheEntries);
				}
				else if (command.Organizations == null)
				{
					base.ProvisioningCache.ResetOrganizationData();
				}
				else
				{
					foreach (Guid orgId in command.Organizations)
					{
						base.ProvisioningCache.RemoveOrganizationDatas(orgId, command.CacheEntries);
					}
				}
				byte[] bytes = Encoding.UTF8.GetBytes("Successfully reset the provisioning cache data");
				stream.Write(bytes, 0, bytes.Length);
				return;
			}
			if (command.Command == DiagnosticType.Dump)
			{
				foreach (CachedEntryObject cachedEntryObject in base.ProvisioningCache.DumpCachedEntries(command.Organizations, command.CacheEntries))
				{
					byte[] array = cachedEntryObject.ToSendMessage();
					stream.Write(array, 0, array.Length);
				}
			}
		}

		private void ProcessClientAccept()
		{
			byte[] array = new byte[1000];
			Array.Clear(array, 0, array.Length);
			try
			{
				if (!base.GotStopSignalFromTestCode)
				{
					int bufLen = this.serverStream.Read(array, 0, array.Length);
					Exception ex;
					DiagnosticCommand command = DiagnosticCommand.TryFromReceivedData(array, bufLen, out ex);
					if (ex != null)
					{
						Globals.LogEvent(DirectoryEventLogConstants.Tuple_PCInvalidDiagnosticCommandReceived, this.pipeName, new object[]
						{
							this.pipeName,
							Encoding.UTF8.GetString(array, 0, array.Length),
							ex.ToString()
						});
					}
					else
					{
						this.ProcessDiagnosticCommand(command, this.serverStream);
						this.serverStream.Flush();
					}
				}
			}
			catch (ArgumentOutOfRangeException ex2)
			{
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_PCFailedToReceiveClientDiagnosticDommand, this.pipeName, new object[]
				{
					this.pipeName,
					ex2.ToString()
				});
				throw ex2;
			}
			catch (IOException ex3)
			{
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_PCFailedToReceiveClientDiagnosticDommand, this.pipeName, new object[]
				{
					this.pipeName,
					ex3.ToString()
				});
				throw ex3;
			}
			catch (ObjectDisposedException ex4)
			{
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_PCFailedToReceiveClientDiagnosticDommand, this.pipeName, new object[]
				{
					this.pipeName,
					ex4.ToString()
				});
				throw ex4;
			}
			catch (InvalidOperationException ex5)
			{
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_PCFailedToReceiveClientDiagnosticDommand, this.pipeName, new object[]
				{
					this.pipeName,
					ex5.ToString()
				});
				throw ex5;
			}
			catch (NotSupportedException ex6)
			{
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_PCFailedToReceiveClientDiagnosticDommand, this.pipeName, new object[]
				{
					this.pipeName,
					ex6.ToString()
				});
				throw ex6;
			}
			finally
			{
				this.serverStream.Disconnect();
			}
		}

		internal override void StopExecute()
		{
			base.StopExecute();
			if (base.GotStopSignalFromTestCode && this.isWaitForConnection)
			{
				while (this.isWaitForConnection)
				{
					using (NamedPipeClientStream namedPipeClientStream = new NamedPipeClientStream(".", this.pipeName, PipeDirection.InOut))
					{
						try
						{
							byte[] array = new DiagnosticCommand(DiagnosticType.Dump, Guid.NewGuid()).ToSendMessage();
							namedPipeClientStream.Connect();
							namedPipeClientStream.Write(array, 0, array.Length);
							namedPipeClientStream.Flush();
						}
						catch
						{
						}
						Thread.Sleep(10000);
					}
				}
				base.AsyncThread.Join();
			}
		}

		private readonly string pipeName;

		private NamedPipeServerStream serverStream;

		private bool isWaitForConnection;
	}
}
