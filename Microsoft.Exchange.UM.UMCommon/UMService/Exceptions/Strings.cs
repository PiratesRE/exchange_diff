using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.UM.UMService.Exceptions
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(254072865U, "WPFatalError");
			Strings.stringIDs.Add(2561408674U, "ConnectionManagerRestartFailure");
			Strings.stringIDs.Add(538835726U, "WorkerProcessStartTimeout");
			Strings.stringIDs.Add(519842198U, "WorkerProcessStartFailed");
			Strings.stringIDs.Add(3373645747U, "DialPlanObjectInvalid");
			Strings.stringIDs.Add(1549735078U, "ReadyThreadStartFailed");
			Strings.stringIDs.Add(214038602U, "FatalKeyFailed");
			Strings.stringIDs.Add(1422214618U, "ServiceName");
			Strings.stringIDs.Add(824705909U, "ResetKeyFailed");
			Strings.stringIDs.Add(4289272219U, "ReadyKeyFailed");
			Strings.stringIDs.Add(88115702U, "ResetThreadStartFailed");
			Strings.stringIDs.Add(2514021662U, "AssignProcessToJobObjectFailed");
			Strings.stringIDs.Add(3790914792U, "StopKeyFailed");
			Strings.stringIDs.Add(509941095U, "WorkerProcessManagerInitFailed");
			Strings.stringIDs.Add(3452399285U, "Server");
			Strings.stringIDs.Add(729817298U, "WorkerProcessRestartFailure");
		}

		public static LocalizedString UMServiceHeartbeatException(string extraInfo)
		{
			return new LocalizedString("UMServiceHeartbeatException", Strings.ResourceManager, new object[]
			{
				extraInfo
			});
		}

		public static LocalizedString UMServiceSetJobObjectFailed(string name, string win32Message)
		{
			return new LocalizedString("UMServiceSetJobObjectFailed", Strings.ResourceManager, new object[]
			{
				name,
				win32Message
			});
		}

		public static LocalizedString WPFatalError
		{
			get
			{
				return new LocalizedString("WPFatalError", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConnectionManagerRestartFailure
		{
			get
			{
				return new LocalizedString("ConnectionManagerRestartFailure", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UMServiceControlChannelException(int port, string errorMessage)
		{
			return new LocalizedString("UMServiceControlChannelException", Strings.ResourceManager, new object[]
			{
				port,
				errorMessage
			});
		}

		public static LocalizedString WorkerProcessStartTimeout
		{
			get
			{
				return new LocalizedString("WorkerProcessStartTimeout", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WorkerProcessStartFailed
		{
			get
			{
				return new LocalizedString("WorkerProcessStartFailed", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UMWorkerStartTimeoutException(int seconds)
		{
			return new LocalizedString("UMWorkerStartTimeoutException", Strings.ResourceManager, new object[]
			{
				seconds
			});
		}

		public static LocalizedString DialPlanObjectInvalid
		{
			get
			{
				return new LocalizedString("DialPlanObjectInvalid", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReadyThreadStartFailed
		{
			get
			{
				return new LocalizedString("ReadyThreadStartFailed", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FatalKeyFailed
		{
			get
			{
				return new LocalizedString("FatalKeyFailed", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidWorkerProcessPath(string path)
		{
			return new LocalizedString("InvalidWorkerProcessPath", Strings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString ServiceName
		{
			get
			{
				return new LocalizedString("ServiceName", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UMServiceCreateJobObjectFailed(string name, string win32Message)
		{
			return new LocalizedString("UMServiceCreateJobObjectFailed", Strings.ResourceManager, new object[]
			{
				name,
				win32Message
			});
		}

		public static LocalizedString ResetKeyFailed
		{
			get
			{
				return new LocalizedString("ResetKeyFailed", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TLSEndPointInitializationFailure(string msg)
		{
			return new LocalizedString("TLSEndPointInitializationFailure", Strings.ResourceManager, new object[]
			{
				msg
			});
		}

		public static LocalizedString UMWorkerProcessStartFailed(string message)
		{
			return new LocalizedString("UMWorkerProcessStartFailed", Strings.ResourceManager, new object[]
			{
				message
			});
		}

		public static LocalizedString ReadyKeyFailed
		{
			get
			{
				return new LocalizedString("ReadyKeyFailed", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UMWorkerProcessExceededMaxThrashCount(int count)
		{
			return new LocalizedString("UMWorkerProcessExceededMaxThrashCount", Strings.ResourceManager, new object[]
			{
				count
			});
		}

		public static LocalizedString ResetThreadStartFailed
		{
			get
			{
				return new LocalizedString("ResetThreadStartFailed", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SipPortsUnavailable(int port1, int port2)
		{
			return new LocalizedString("SipPortsUnavailable", Strings.ResourceManager, new object[]
			{
				port1,
				port2
			});
		}

		public static LocalizedString AssignProcessToJobObjectFailed
		{
			get
			{
				return new LocalizedString("AssignProcessToJobObjectFailed", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StopKeyFailed
		{
			get
			{
				return new LocalizedString("StopKeyFailed", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UMInvalidWorkerProcessPath(string path)
		{
			return new LocalizedString("UMInvalidWorkerProcessPath", Strings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString FailedToCreateVoiceMailFilePath(string path)
		{
			return new LocalizedString("FailedToCreateVoiceMailFilePath", Strings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString WorkerProcessManagerInitFailed
		{
			get
			{
				return new LocalizedString("WorkerProcessManagerInitFailed", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Server
		{
			get
			{
				return new LocalizedString("Server", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WorkerProcessRestartFailure
		{
			get
			{
				return new LocalizedString("WorkerProcessRestartFailure", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(16);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.UM.UMService.Exceptions.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			WPFatalError = 254072865U,
			ConnectionManagerRestartFailure = 2561408674U,
			WorkerProcessStartTimeout = 538835726U,
			WorkerProcessStartFailed = 519842198U,
			DialPlanObjectInvalid = 3373645747U,
			ReadyThreadStartFailed = 1549735078U,
			FatalKeyFailed = 214038602U,
			ServiceName = 1422214618U,
			ResetKeyFailed = 824705909U,
			ReadyKeyFailed = 4289272219U,
			ResetThreadStartFailed = 88115702U,
			AssignProcessToJobObjectFailed = 2514021662U,
			StopKeyFailed = 3790914792U,
			WorkerProcessManagerInitFailed = 509941095U,
			Server = 3452399285U,
			WorkerProcessRestartFailure = 729817298U
		}

		private enum ParamIDs
		{
			UMServiceHeartbeatException,
			UMServiceSetJobObjectFailed,
			UMServiceControlChannelException,
			UMWorkerStartTimeoutException,
			InvalidWorkerProcessPath,
			UMServiceCreateJobObjectFailed,
			TLSEndPointInitializationFailure,
			UMWorkerProcessStartFailed,
			UMWorkerProcessExceededMaxThrashCount,
			SipPortsUnavailable,
			UMInvalidWorkerProcessPath,
			FailedToCreateVoiceMailFilePath
		}
	}
}
