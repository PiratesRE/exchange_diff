using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.ServiceProcess;
using System.Threading;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Cluster.Replay
{
	[InstallerType(typeof(ServiceProcessInstaller))]
	internal abstract class ReplayServiceBase : Component
	{
		public ReplayServiceBase()
		{
			this.acceptedCommands = 1;
			this.AutoLog = true;
			this.ServiceName = "";
		}

		[DefaultValue(true)]
		public bool AutoLog
		{
			get
			{
				return this.autoLog;
			}
			set
			{
				this.autoLog = value;
			}
		}

		[ComVisible(false)]
		public int ExitCode
		{
			get
			{
				return this.status.win32ExitCode;
			}
			set
			{
				this.status.win32ExitCode = value;
			}
		}

		[DefaultValue(false)]
		public bool CanHandlePowerEvent
		{
			get
			{
				return (this.acceptedCommands & 64) != 0;
			}
			set
			{
				if (this.commandPropsFrozen)
				{
					throw new InvalidOperationException(ReplayStrings.CannotChangeProperties);
				}
				if (value)
				{
					this.acceptedCommands |= 64;
					return;
				}
				this.acceptedCommands &= -65;
			}
		}

		[DefaultValue(false)]
		[ComVisible(false)]
		public bool CanHandleSessionChangeEvent
		{
			get
			{
				return (this.acceptedCommands & 128) != 0;
			}
			set
			{
				if (this.commandPropsFrozen)
				{
					throw new InvalidOperationException(ReplayStrings.CannotChangeProperties);
				}
				if (value)
				{
					this.acceptedCommands |= 128;
					return;
				}
				this.acceptedCommands &= -129;
			}
		}

		[DefaultValue(false)]
		public bool CanPauseAndContinue
		{
			get
			{
				return (this.acceptedCommands & 2) != 0;
			}
			set
			{
				if (this.commandPropsFrozen)
				{
					throw new InvalidOperationException(ReplayStrings.CannotChangeProperties);
				}
				if (value)
				{
					this.acceptedCommands |= 2;
					return;
				}
				this.acceptedCommands &= -3;
			}
		}

		[DefaultValue(false)]
		public bool CanShutdown
		{
			get
			{
				return (this.acceptedCommands & 4) != 0;
			}
			set
			{
				if (this.commandPropsFrozen)
				{
					throw new InvalidOperationException(ReplayStrings.CannotChangeProperties);
				}
				if (value)
				{
					this.acceptedCommands |= 4;
					return;
				}
				this.acceptedCommands &= -5;
			}
		}

		[DefaultValue(true)]
		public bool CanStop
		{
			get
			{
				return (this.acceptedCommands & 1) != 0;
			}
			set
			{
				if (this.commandPropsFrozen)
				{
					throw new InvalidOperationException(ReplayStrings.CannotChangeProperties);
				}
				if (value)
				{
					this.acceptedCommands |= 1;
					return;
				}
				this.acceptedCommands &= -2;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual EventLog EventLog
		{
			get
			{
				if (this.eventLog == null)
				{
					this.eventLog = new EventLog();
					this.eventLog.Source = this.ServiceName;
					this.eventLog.Log = "Application";
				}
				return this.eventLog;
			}
		}

		[TypeConverter("System.Diagnostics.Design.StringValueConverter, System.Design")]
		public string ServiceName
		{
			get
			{
				return this.serviceName;
			}
			set
			{
				if (this.nameFrozen)
				{
					throw new InvalidOperationException(ReplayStrings.CannotChangeName);
				}
				if (value != "" && !ReplayServiceBase.ValidServiceName(value))
				{
					throw new ArgumentException(ReplayStrings.ServiceName(value, 80.ToString(CultureInfo.CurrentCulture)));
				}
				this.serviceName = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected IntPtr ServiceHandle
		{
			get
			{
				new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
				return this.statusHandle;
			}
		}

		protected virtual TimeSpan StartTimeout
		{
			get
			{
				return this.DefaultTimeout;
			}
		}

		protected virtual TimeSpan StopTimeout
		{
			get
			{
				return this.DefaultTimeout;
			}
		}

		protected virtual TimeSpan PauseTimeout
		{
			get
			{
				return this.DefaultTimeout;
			}
		}

		protected virtual TimeSpan ContinueTimeout
		{
			get
			{
				return this.DefaultTimeout;
			}
		}

		protected virtual TimeSpan CustomCommandTimeout
		{
			get
			{
				return this.DefaultTimeout;
			}
		}

		public static void Run(ReplayServiceBase service)
		{
			if (service == null)
			{
				throw new ArgumentException(ReplayStrings.NoServices);
			}
			ReplayServiceBase.Run(new ReplayServiceBase[]
			{
				service
			});
		}

		public static void Run(ReplayServiceBase[] services)
		{
			if (services == null || services.Length == 0)
			{
				throw new ArgumentException(ReplayStrings.NoServices);
			}
			IntPtr intPtr = Marshal.AllocHGlobal((IntPtr)((services.Length + 1) * Marshal.SizeOf(typeof(NativeMethods.SERVICE_TABLE_ENTRY))));
			NativeMethods.SERVICE_TABLE_ENTRY[] array = new NativeMethods.SERVICE_TABLE_ENTRY[services.Length];
			bool multipleServices = services.Length > 1;
			IntPtr ptr = (IntPtr)0;
			for (int i = 0; i < services.Length; i++)
			{
				services[i].Initialize(multipleServices);
				array[i] = services[i].GetEntry();
				ptr = (IntPtr)((long)intPtr + (long)(Marshal.SizeOf(typeof(NativeMethods.SERVICE_TABLE_ENTRY)) * i));
				Marshal.StructureToPtr(array[i], ptr, true);
			}
			NativeMethods.SERVICE_TABLE_ENTRY service_TABLE_ENTRY = new NativeMethods.SERVICE_TABLE_ENTRY();
			service_TABLE_ENTRY.callback = null;
			service_TABLE_ENTRY.name = (IntPtr)0;
			ptr = (IntPtr)((long)intPtr + (long)(Marshal.SizeOf(typeof(NativeMethods.SERVICE_TABLE_ENTRY)) * services.Length));
			Marshal.StructureToPtr(service_TABLE_ENTRY, ptr, true);
			bool flag = NativeMethods.StartServiceCtrlDispatcher(intPtr);
			string text = "";
			if (!flag)
			{
				text = new Win32Exception().Message;
				string text2 = ReplayStrings.CantStartFromCommandLine;
				if (Environment.UserInteractive)
				{
					string title = ReplayStrings.CantStartFromCommandLineTitle;
					ReplayServiceBase.LateBoundMessageBoxShow(text2, title);
				}
				else
				{
					Console.WriteLine(text2);
				}
			}
			foreach (ReplayServiceBase replayServiceBase in services)
			{
				replayServiceBase.Dispose();
				if (!flag)
				{
					ReplayEventLogConstants.Tuple_StartFailed.LogEvent(null, new object[]
					{
						text
					});
				}
			}
		}

		public static void RunAsConsole(ReplayServiceBase service)
		{
			Console.WriteLine("Starting...");
			service.OnStartInternal(null);
			Console.WriteLine("Started. Type ENTER to stop.");
			Console.ReadLine();
			Console.WriteLine("Stopping...");
			service.OnStopInternal();
			Console.WriteLine("Stopped");
		}

		[ComVisible(false)]
		public unsafe void RequestAdditionalTime(int milliseconds)
		{
			fixed (NativeMethods.SERVICE_STATUS* ptr = &this.status)
			{
				if (this.status.currentState != 5 && this.status.currentState != 2 && this.status.currentState != 3 && this.status.currentState != 6)
				{
					throw new InvalidOperationException(ReplayStrings.NotInPendingState);
				}
				this.status.waitHint = milliseconds;
				this.status.checkPoint = this.status.checkPoint + 1;
				NativeMethods.SetServiceStatus(this.statusHandle, ptr);
			}
		}

		public void Stop()
		{
			this.DeferredStop();
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[ComVisible(false)]
		public unsafe void ServiceMainCallback(int argCount, IntPtr argPointer)
		{
			fixed (NativeMethods.SERVICE_STATUS* ptr = &this.status)
			{
				string[] array = null;
				if (argCount > 0)
				{
					char** ptr2 = (char**)argPointer.ToPointer();
					array = new string[argCount - 1];
					for (int i = 0; i < array.Length; i++)
					{
						ptr2 += sizeof(char*) / sizeof(char*);
						array[i] = Marshal.PtrToStringUni((IntPtr)(*(IntPtr*)ptr2));
					}
				}
				if (!this.initialized)
				{
					this.isServiceHosted = true;
					this.Initialize(true);
				}
				if (Environment.OSVersion.Version.Major >= 5)
				{
					this.statusHandle = NativeMethods.RegisterServiceCtrlHandlerEx(this.ServiceName, this.commandCallbackEx, (IntPtr)0);
				}
				else
				{
					this.statusHandle = NativeMethods.RegisterServiceCtrlHandler(this.ServiceName, this.commandCallback);
				}
				this.nameFrozen = true;
				if (this.statusHandle == (IntPtr)0)
				{
					string message = new Win32Exception().Message;
					ReplayEventLogConstants.Tuple_StartFailed.LogEvent(null, new object[]
					{
						message
					});
				}
				this.status.controlsAccepted = this.acceptedCommands;
				this.commandPropsFrozen = true;
				if ((this.status.controlsAccepted & 1) != 0)
				{
					this.status.controlsAccepted = (this.status.controlsAccepted | 256);
				}
				if (Environment.OSVersion.Version.Major < 5)
				{
					this.status.controlsAccepted = (this.status.controlsAccepted & -65);
				}
				this.status.currentState = 2;
				if (NativeMethods.SetServiceStatus(this.statusHandle, ptr))
				{
					this.startCompletedSignal = new ManualResetEvent(false);
					ThreadPool.QueueUserWorkItem(new WaitCallback(this.ServiceQueuedMainCallback), array);
					this.startCompletedSignal.WaitOne();
					if (!NativeMethods.SetServiceStatus(this.statusHandle, ptr))
					{
						ReplayEventLogConstants.Tuple_StartFailed.LogEvent(null, new object[]
						{
							new Win32Exception().Message
						});
						this.status.currentState = 1;
						NativeMethods.SetServiceStatus(this.statusHandle, ptr);
					}
					ptr = null;
				}
			}
		}

		public void ExRequestAdditionalTime(int milliseconds)
		{
			if (!Environment.UserInteractive)
			{
				this.RequestAdditionalTime(milliseconds);
			}
		}

		internal static bool ValidServiceName(string serviceName)
		{
			if (serviceName == null)
			{
				return false;
			}
			if (serviceName.Length > 80 || serviceName.Length == 0)
			{
				return false;
			}
			foreach (char c in serviceName.ToCharArray())
			{
				if (c == '\\' || c == '/')
				{
					return false;
				}
			}
			return true;
		}

		protected override void Dispose(bool disposing)
		{
			if (this.handleName != (IntPtr)0)
			{
				Marshal.FreeHGlobal(this.handleName);
				this.handleName = (IntPtr)0;
			}
			this.nameFrozen = false;
			this.commandPropsFrozen = false;
			this.disposed = true;
			base.Dispose(disposing);
		}

		protected virtual bool OnPowerEvent(PowerBroadcastStatus powerStatus)
		{
			return true;
		}

		protected virtual void OnSessionChange(SessionChangeDescription changeDescription)
		{
		}

		protected virtual void OnShutdown()
		{
		}

		protected virtual void OnPreShutdown()
		{
		}

		protected void OnStart(string[] args)
		{
			this.SendWatsonReportOnTimeout("OnStart", this.StartTimeout, new TimerCallback(this.OnStartTimeoutHandler), delegate
			{
				Dependencies.Watson.SendReportOnUnhandledException(delegate
				{
					this.OnStartInternal(args);
				});
			});
		}

		protected void OnStop()
		{
			this.SendWatsonReportOnTimeout("OnStop", this.StopTimeout, new TimerCallback(this.OnStopTimeoutHandler), delegate
			{
				Dependencies.Watson.SendReportOnUnhandledException(delegate
				{
					this.OnStopInternal();
				});
			});
		}

		protected void OnPause()
		{
			this.SendWatsonReportOnTimeout("OnPause", this.PauseTimeout, new TimerCallback(this.OnStartTimeoutHandler), delegate
			{
				Dependencies.Watson.SendReportOnUnhandledException(delegate
				{
					this.OnPauseInternal();
				});
			});
		}

		protected void OnContinue()
		{
			this.SendWatsonReportOnTimeout("OnContinue", this.ContinueTimeout, new TimerCallback(this.OnStartTimeoutHandler), delegate
			{
				Dependencies.Watson.SendReportOnUnhandledException(delegate
				{
					this.OnContinueInternal();
				});
			});
		}

		protected void OnCustomCommand(int command)
		{
			this.SendWatsonReportOnTimeout("OnCustomCommand", this.CustomCommandTimeout, new TimerCallback(this.OnStartTimeoutHandler), delegate
			{
				Dependencies.Watson.SendReportOnUnhandledException(delegate
				{
					this.OnCustomCommandInternal(command);
				});
			});
		}

		protected abstract void OnStartInternal(string[] args);

		protected abstract void OnStopInternal();

		protected virtual void OnPauseInternal()
		{
		}

		protected virtual void OnContinueInternal()
		{
		}

		protected virtual void OnCommandTimeout()
		{
		}

		protected virtual void OnCustomCommandInternal(int command)
		{
		}

		private static void LateBoundMessageBoxShow(string message, string title)
		{
			int value = 1572864;
			Type type = Type.GetType("System.Windows.Forms.MessageBox, System.Windows.Forms");
			Type type2 = Type.GetType("System.Windows.Forms.MessageBoxButtons, System.Windows.Forms");
			Type type3 = Type.GetType("System.Windows.Forms.MessageBoxIcon, System.Windows.Forms");
			Type type4 = Type.GetType("System.Windows.Forms.MessageBoxDefaultButton, System.Windows.Forms");
			Type type5 = Type.GetType("System.Windows.Forms.MessageBoxOptions, System.Windows.Forms");
			type.InvokeMember("Show", BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, null, null, new object[]
			{
				message,
				title,
				Enum.ToObject(type2, 0),
				Enum.ToObject(type3, 0),
				Enum.ToObject(type4, 0),
				Enum.ToObject(type5, value)
			}, CultureInfo.InvariantCulture);
		}

		private unsafe void DeferredContinue()
		{
			fixed (NativeMethods.SERVICE_STATUS* ptr = &this.status)
			{
				try
				{
					this.OnContinue();
					ReplayEventLogConstants.Tuple_ContinueSuccessful.LogEvent(null, new object[0]);
					this.status.currentState = 4;
				}
				catch (Exception ex)
				{
					this.status.currentState = 7;
					ReplayEventLogConstants.Tuple_ContinueFailed.LogEvent(string.Empty, new object[]
					{
						ex.ToString()
					});
					throw;
				}
				finally
				{
					NativeMethods.SetServiceStatus(this.statusHandle, ptr);
				}
			}
		}

		private void DeferredCustomCommand(int command)
		{
			try
			{
				this.OnCustomCommand(command);
				ReplayEventLogConstants.Tuple_CommandSuccessful.LogEvent(null, new object[0]);
			}
			catch (Exception ex)
			{
				ReplayEventLogConstants.Tuple_CommandFailed.LogEvent(string.Empty, new object[]
				{
					ex.ToString()
				});
				throw;
			}
		}

		private unsafe void DeferredPause()
		{
			fixed (NativeMethods.SERVICE_STATUS* ptr = &this.status)
			{
				try
				{
					this.OnPause();
					ReplayEventLogConstants.Tuple_PauseSuccessful.LogEvent(null, new object[0]);
					this.status.currentState = 7;
				}
				catch (Exception ex)
				{
					this.status.currentState = 4;
					ReplayEventLogConstants.Tuple_PauseFailed.LogEvent(string.Empty, new object[]
					{
						ex.ToString()
					});
					throw;
				}
				finally
				{
					NativeMethods.SetServiceStatus(this.statusHandle, ptr);
				}
			}
		}

		private void DeferredPowerEvent(int eventType, IntPtr eventData)
		{
			try
			{
				this.OnPowerEvent((PowerBroadcastStatus)eventType);
				ReplayEventLogConstants.Tuple_PowerEventOK.LogEvent(null, new object[0]);
			}
			catch (Exception ex)
			{
				ReplayEventLogConstants.Tuple_PowerEventFailed.LogEvent(string.Empty, new object[]
				{
					ex.ToString()
				});
				throw;
			}
		}

		private void DeferredSessionChange(int eventType, IntPtr eventData)
		{
			try
			{
				NativeMethods.WTSSESSION_NOTIFICATION structure = new NativeMethods.WTSSESSION_NOTIFICATION();
				Marshal.PtrToStructure(eventData, structure);
			}
			catch (Exception ex)
			{
				ReplayEventLogConstants.Tuple_SessionChangeFailed.LogEvent(ex.ToString(), new object[0]);
				throw;
			}
		}

		private unsafe void DeferredStop()
		{
			fixed (NativeMethods.SERVICE_STATUS* ptr = &this.status)
			{
				int currentState = this.status.currentState;
				this.status.checkPoint = 0;
				this.status.waitHint = 0;
				this.status.currentState = 3;
				NativeMethods.SetServiceStatus(this.statusHandle, ptr);
				try
				{
					this.OnStop();
					this.status.currentState = 1;
					NativeMethods.SetServiceStatus(this.statusHandle, ptr);
				}
				catch (Exception ex)
				{
					this.status.currentState = currentState;
					NativeMethods.SetServiceStatus(this.statusHandle, ptr);
					ReplayEventLogConstants.Tuple_StopFailed.LogEvent(string.Empty, new object[]
					{
						ex.ToString()
					});
					throw;
				}
				if (this.isServiceHosted)
				{
					try
					{
						AppDomain.Unload(AppDomain.CurrentDomain);
					}
					catch (CannotUnloadAppDomainException ex2)
					{
						ReplayEventLogConstants.Tuple_FailedToUnloadAppDomain.LogEvent(string.Empty, new object[]
						{
							AppDomain.CurrentDomain.FriendlyName,
							ex2.Message
						});
					}
				}
			}
		}

		private unsafe void DeferredShutdown()
		{
			fixed (NativeMethods.SERVICE_STATUS* ptr = &this.status)
			{
				int currentState = this.status.currentState;
				this.status.checkPoint = 0;
				this.status.waitHint = 0;
				this.status.currentState = 3;
				NativeMethods.SetServiceStatus(this.statusHandle, ptr);
				try
				{
					this.OnShutdown();
					ReplayEventLogConstants.Tuple_ShutdownOK.LogEvent(null, new object[0]);
					this.status.currentState = 1;
					NativeMethods.SetServiceStatus(this.statusHandle, ptr);
				}
				catch (Exception ex)
				{
					this.status.currentState = currentState;
					NativeMethods.SetServiceStatus(this.statusHandle, ptr);
					ReplayEventLogConstants.Tuple_ShutdownFailed.LogEvent(string.Empty, new object[]
					{
						ex.ToString()
					});
					throw;
				}
			}
		}

		private unsafe void DeferredPreShutdown()
		{
			fixed (NativeMethods.SERVICE_STATUS* ptr = &this.status)
			{
				this.status.checkPoint = 0;
				this.status.waitHint = 0;
				this.status.currentState = 3;
				NativeMethods.SetServiceStatus(this.statusHandle, ptr);
				try
				{
					this.OnPreShutdown();
					this.status.currentState = 1;
					NativeMethods.SetServiceStatus(this.statusHandle, ptr);
				}
				catch (Exception ex)
				{
					ReplayEventLogConstants.Tuple_PreShutdownFailed.LogEvent(string.Empty, new object[]
					{
						ex.ToString()
					});
					throw;
				}
			}
		}

		private void Initialize(bool multipleServices)
		{
			if (!this.initialized)
			{
				if (this.disposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				if (!multipleServices)
				{
					this.status.serviceType = 16;
				}
				else
				{
					this.status.serviceType = 32;
				}
				this.status.currentState = 2;
				this.status.controlsAccepted = 0;
				this.status.win32ExitCode = 0;
				this.status.serviceSpecificExitCode = 0;
				this.status.checkPoint = 0;
				this.status.waitHint = 0;
				this.mainCallback = new NativeMethods.ServiceMainCallback(this.ServiceMainCallback);
				this.commandCallback = new NativeMethods.ServiceControlCallback(this.ServiceCommandCallback);
				this.commandCallbackEx = new NativeMethods.ServiceControlCallbackEx(this.ServiceCommandCallbackEx);
				this.handleName = Marshal.StringToHGlobalUni(this.ServiceName);
				this.initialized = true;
			}
		}

		private NativeMethods.SERVICE_TABLE_ENTRY GetEntry()
		{
			NativeMethods.SERVICE_TABLE_ENTRY service_TABLE_ENTRY = new NativeMethods.SERVICE_TABLE_ENTRY();
			this.nameFrozen = true;
			service_TABLE_ENTRY.callback = this.mainCallback;
			service_TABLE_ENTRY.name = this.handleName;
			return service_TABLE_ENTRY;
		}

		private int ServiceCommandCallbackEx(int command, int eventType, IntPtr eventData, IntPtr eventContext)
		{
			int result = 0;
			switch (command)
			{
			case 13:
			{
				ReplayServiceBase.DeferredHandlerDelegateAdvanced deferredHandlerDelegateAdvanced = new ReplayServiceBase.DeferredHandlerDelegateAdvanced(this.DeferredPowerEvent);
				deferredHandlerDelegateAdvanced.BeginInvoke(eventType, eventData, null, null);
				break;
			}
			case 14:
			{
				ReplayServiceBase.DeferredHandlerDelegateAdvanced deferredHandlerDelegateAdvanced2 = new ReplayServiceBase.DeferredHandlerDelegateAdvanced(this.DeferredSessionChange);
				deferredHandlerDelegateAdvanced2.BeginInvoke(eventType, eventData, null, null);
				break;
			}
			default:
				this.ServiceCommandCallback(command);
				break;
			}
			return result;
		}

		private unsafe void ServiceCommandCallback(int command)
		{
			fixed (NativeMethods.SERVICE_STATUS* ptr = &this.status)
			{
				if (command == 4)
				{
					NativeMethods.SetServiceStatus(this.statusHandle, ptr);
				}
				else if (this.status.currentState != 5 && this.status.currentState != 2 && this.status.currentState != 3 && this.status.currentState != 6)
				{
					switch (command)
					{
					case 1:
					{
						int currentState = this.status.currentState;
						if (this.status.currentState == 7 || this.status.currentState == 4)
						{
							this.status.currentState = 3;
							NativeMethods.SetServiceStatus(this.statusHandle, ptr);
							this.status.currentState = currentState;
							ReplayServiceBase.DeferredHandlerDelegate deferredHandlerDelegate = new ReplayServiceBase.DeferredHandlerDelegate(this.DeferredStop);
							deferredHandlerDelegate.BeginInvoke(null, null);
							goto IL_1D4;
						}
						goto IL_1D4;
					}
					case 2:
						if (this.status.currentState == 4)
						{
							this.status.currentState = 6;
							NativeMethods.SetServiceStatus(this.statusHandle, ptr);
							ReplayServiceBase.DeferredHandlerDelegate deferredHandlerDelegate2 = new ReplayServiceBase.DeferredHandlerDelegate(this.DeferredPause);
							deferredHandlerDelegate2.BeginInvoke(null, null);
							goto IL_1D4;
						}
						goto IL_1D4;
					case 3:
						if (this.status.currentState == 7)
						{
							this.status.currentState = 5;
							NativeMethods.SetServiceStatus(this.statusHandle, ptr);
							ReplayServiceBase.DeferredHandlerDelegate deferredHandlerDelegate3 = new ReplayServiceBase.DeferredHandlerDelegate(this.DeferredContinue);
							deferredHandlerDelegate3.BeginInvoke(null, null);
							goto IL_1D4;
						}
						goto IL_1D4;
					case 4:
						break;
					case 5:
					{
						ReplayServiceBase.DeferredHandlerDelegate deferredHandlerDelegate4 = new ReplayServiceBase.DeferredHandlerDelegate(this.DeferredShutdown);
						deferredHandlerDelegate4.BeginInvoke(null, null);
						goto IL_1D4;
					}
					default:
						if (command == 15)
						{
							ReplayServiceBase.DeferredHandlerDelegate deferredHandlerDelegate5 = new ReplayServiceBase.DeferredHandlerDelegate(this.DeferredPreShutdown);
							deferredHandlerDelegate5.BeginInvoke(null, null);
							goto IL_1D4;
						}
						break;
					}
					ReplayServiceBase.DeferredHandlerDelegateCommand deferredHandlerDelegateCommand = new ReplayServiceBase.DeferredHandlerDelegateCommand(this.DeferredCustomCommand);
					deferredHandlerDelegateCommand.BeginInvoke(command, null, null);
				}
				IL_1D4:;
			}
		}

		private void ServiceQueuedMainCallback(object state)
		{
			string[] args = (string[])state;
			try
			{
				this.OnStart(args);
				this.status.checkPoint = 0;
				this.status.waitHint = 0;
				this.status.currentState = 4;
			}
			catch (Exception ex)
			{
				ReplayEventLogConstants.Tuple_StartFailed.LogEvent(null, new object[]
				{
					ex.ToString()
				});
				this.status.currentState = 1;
			}
			this.startCompletedSignal.Set();
		}

		private void SendWatsonReportOnTimeout(string caller, TimeSpan timeout, TimerCallback timeoutHandler, ReplayServiceBase.UnderTimeoutDelegate underTimeoutDelegate)
		{
			string state = string.Concat(new object[]
			{
				caller,
				" started on thread ",
				Environment.CurrentManagedThreadId,
				" at ",
				ExDateTime.Now.ToString(),
				" but did not complete in alloted time of ",
				timeout.ToString()
			});
			using (new Timer(timeoutHandler, state, timeout, TimeSpan.Zero))
			{
				underTimeoutDelegate();
			}
		}

		private void OnStartTimeoutHandler(object state)
		{
			if (!Debugger.IsAttached)
			{
				this.OnCommandTimeout();
				ReplayServiceBase.ServiceTimeoutException ex = new ReplayServiceBase.ServiceTimeoutException(state as string);
				Dependencies.Watson.SendReport(ex);
				throw ex;
			}
		}

		private void OnStopTimeoutHandler(object state)
		{
			if (!Debugger.IsAttached)
			{
				this.OnCommandTimeout();
				Action action = delegate()
				{
					this.CauseWatson1(state as string);
				};
				this.HavePossibleHungComponentInvoke(action);
				action();
			}
		}

		private void CauseWatson1(string state)
		{
			ReplayServiceBase.ServiceTimeoutException ex = new ReplayServiceBase.ServiceTimeoutException(state);
			Dependencies.Watson.SendReport(ex);
			throw ex;
		}

		protected virtual void HavePossibleHungComponentInvoke(Action toInvoke)
		{
		}

		public const int MaxNameLength = 80;

		private readonly TimeSpan DefaultTimeout = TimeSpan.FromMinutes(10.0);

		private NativeMethods.SERVICE_STATUS status = default(NativeMethods.SERVICE_STATUS);

		private IntPtr statusHandle;

		private NativeMethods.ServiceControlCallback commandCallback;

		private NativeMethods.ServiceControlCallbackEx commandCallbackEx;

		private NativeMethods.ServiceMainCallback mainCallback;

		private IntPtr handleName;

		private ManualResetEvent startCompletedSignal;

		private int acceptedCommands;

		private bool autoLog;

		private string serviceName;

		private EventLog eventLog;

		private bool nameFrozen;

		private bool commandPropsFrozen;

		private bool disposed;

		private bool initialized;

		private bool isServiceHosted;

		private delegate void DeferredHandlerDelegate();

		private delegate void DeferredHandlerDelegateCommand(int command);

		private delegate void DeferredHandlerDelegateAdvanced(int eventType, IntPtr eventData);

		private delegate void UnderTimeoutDelegate();

		private class ServiceTimeoutException : Exception
		{
			public ServiceTimeoutException(string message) : base(message)
			{
			}
		}
	}
}
