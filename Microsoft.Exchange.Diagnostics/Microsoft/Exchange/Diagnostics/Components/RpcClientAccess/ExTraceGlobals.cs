using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.RpcClientAccess
{
	public static class ExTraceGlobals
	{
		public static Trace RpcRawBufferTracer
		{
			get
			{
				if (ExTraceGlobals.rpcRawBufferTracer == null)
				{
					ExTraceGlobals.rpcRawBufferTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.rpcRawBufferTracer;
			}
		}

		public static Trace FailedRopTracer
		{
			get
			{
				if (ExTraceGlobals.failedRopTracer == null)
				{
					ExTraceGlobals.failedRopTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.failedRopTracer;
			}
		}

		public static Trace RopLevelExceptionTracer
		{
			get
			{
				if (ExTraceGlobals.ropLevelExceptionTracer == null)
				{
					ExTraceGlobals.ropLevelExceptionTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.ropLevelExceptionTracer;
			}
		}

		public static Trace NotImplementedTracer
		{
			get
			{
				if (ExTraceGlobals.notImplementedTracer == null)
				{
					ExTraceGlobals.notImplementedTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.notImplementedTracer;
			}
		}

		public static Trace NotificationHandlerTracer
		{
			get
			{
				if (ExTraceGlobals.notificationHandlerTracer == null)
				{
					ExTraceGlobals.notificationHandlerTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.notificationHandlerTracer;
			}
		}

		public static Trace NotificationDeliveryTracer
		{
			get
			{
				if (ExTraceGlobals.notificationDeliveryTracer == null)
				{
					ExTraceGlobals.notificationDeliveryTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.notificationDeliveryTracer;
			}
		}

		public static Trace AttachmentTracer
		{
			get
			{
				if (ExTraceGlobals.attachmentTracer == null)
				{
					ExTraceGlobals.attachmentTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.attachmentTracer;
			}
		}

		public static Trace MessageTracer
		{
			get
			{
				if (ExTraceGlobals.messageTracer == null)
				{
					ExTraceGlobals.messageTracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.messageTracer;
			}
		}

		public static Trace FailedRpcTracer
		{
			get
			{
				if (ExTraceGlobals.failedRpcTracer == null)
				{
					ExTraceGlobals.failedRpcTracer = new Trace(ExTraceGlobals.componentGuid, 8);
				}
				return ExTraceGlobals.failedRpcTracer;
			}
		}

		public static Trace ClientThrottledTracer
		{
			get
			{
				if (ExTraceGlobals.clientThrottledTracer == null)
				{
					ExTraceGlobals.clientThrottledTracer = new Trace(ExTraceGlobals.componentGuid, 9);
				}
				return ExTraceGlobals.clientThrottledTracer;
			}
		}

		public static Trace ConnectRpcTracer
		{
			get
			{
				if (ExTraceGlobals.connectRpcTracer == null)
				{
					ExTraceGlobals.connectRpcTracer = new Trace(ExTraceGlobals.componentGuid, 10);
				}
				return ExTraceGlobals.connectRpcTracer;
			}
		}

		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 11);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		public static Trace UnhandledExceptionTracer
		{
			get
			{
				if (ExTraceGlobals.unhandledExceptionTracer == null)
				{
					ExTraceGlobals.unhandledExceptionTracer = new Trace(ExTraceGlobals.componentGuid, 12);
				}
				return ExTraceGlobals.unhandledExceptionTracer;
			}
		}

		public static Trace AsyncRpcTracer
		{
			get
			{
				if (ExTraceGlobals.asyncRpcTracer == null)
				{
					ExTraceGlobals.asyncRpcTracer = new Trace(ExTraceGlobals.componentGuid, 13);
				}
				return ExTraceGlobals.asyncRpcTracer;
			}
		}

		public static Trace AccessControlTracer
		{
			get
			{
				if (ExTraceGlobals.accessControlTracer == null)
				{
					ExTraceGlobals.accessControlTracer = new Trace(ExTraceGlobals.componentGuid, 14);
				}
				return ExTraceGlobals.accessControlTracer;
			}
		}

		public static Trace AsyncRopHandlerTracer
		{
			get
			{
				if (ExTraceGlobals.asyncRopHandlerTracer == null)
				{
					ExTraceGlobals.asyncRopHandlerTracer = new Trace(ExTraceGlobals.componentGuid, 15);
				}
				return ExTraceGlobals.asyncRopHandlerTracer;
			}
		}

		public static Trace ConnectXropTracer
		{
			get
			{
				if (ExTraceGlobals.connectXropTracer == null)
				{
					ExTraceGlobals.connectXropTracer = new Trace(ExTraceGlobals.componentGuid, 16);
				}
				return ExTraceGlobals.connectXropTracer;
			}
		}

		public static Trace FailedXropTracer
		{
			get
			{
				if (ExTraceGlobals.failedXropTracer == null)
				{
					ExTraceGlobals.failedXropTracer = new Trace(ExTraceGlobals.componentGuid, 17);
				}
				return ExTraceGlobals.failedXropTracer;
			}
		}

		public static Trace AvailabilityTracer
		{
			get
			{
				if (ExTraceGlobals.availabilityTracer == null)
				{
					ExTraceGlobals.availabilityTracer = new Trace(ExTraceGlobals.componentGuid, 18);
				}
				return ExTraceGlobals.availabilityTracer;
			}
		}

		public static Trace LogonTracer
		{
			get
			{
				if (ExTraceGlobals.logonTracer == null)
				{
					ExTraceGlobals.logonTracer = new Trace(ExTraceGlobals.componentGuid, 19);
				}
				return ExTraceGlobals.logonTracer;
			}
		}

		public static Trace FolderTracer
		{
			get
			{
				if (ExTraceGlobals.folderTracer == null)
				{
					ExTraceGlobals.folderTracer = new Trace(ExTraceGlobals.componentGuid, 20);
				}
				return ExTraceGlobals.folderTracer;
			}
		}

		public static Trace ExchangeAsyncDispatchTracer
		{
			get
			{
				if (ExTraceGlobals.exchangeAsyncDispatchTracer == null)
				{
					ExTraceGlobals.exchangeAsyncDispatchTracer = new Trace(ExTraceGlobals.componentGuid, 21);
				}
				return ExTraceGlobals.exchangeAsyncDispatchTracer;
			}
		}

		public static Trace ExchangeDispatchTracer
		{
			get
			{
				if (ExTraceGlobals.exchangeDispatchTracer == null)
				{
					ExTraceGlobals.exchangeDispatchTracer = new Trace(ExTraceGlobals.componentGuid, 22);
				}
				return ExTraceGlobals.exchangeDispatchTracer;
			}
		}

		public static Trace DispatchTaskTracer
		{
			get
			{
				if (ExTraceGlobals.dispatchTaskTracer == null)
				{
					ExTraceGlobals.dispatchTaskTracer = new Trace(ExTraceGlobals.componentGuid, 23);
				}
				return ExTraceGlobals.dispatchTaskTracer;
			}
		}

		public static Trace RpcHttpConnectionRegistrationAsyncDispatchTracer
		{
			get
			{
				if (ExTraceGlobals.rpcHttpConnectionRegistrationAsyncDispatchTracer == null)
				{
					ExTraceGlobals.rpcHttpConnectionRegistrationAsyncDispatchTracer = new Trace(ExTraceGlobals.componentGuid, 24);
				}
				return ExTraceGlobals.rpcHttpConnectionRegistrationAsyncDispatchTracer;
			}
		}

		private static Guid componentGuid = new Guid("E5EC0B19-2F45-4b2f-8B2B-4B0473209669");

		private static Trace rpcRawBufferTracer = null;

		private static Trace failedRopTracer = null;

		private static Trace ropLevelExceptionTracer = null;

		private static Trace notImplementedTracer = null;

		private static Trace notificationHandlerTracer = null;

		private static Trace notificationDeliveryTracer = null;

		private static Trace attachmentTracer = null;

		private static Trace messageTracer = null;

		private static Trace failedRpcTracer = null;

		private static Trace clientThrottledTracer = null;

		private static Trace connectRpcTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;

		private static Trace unhandledExceptionTracer = null;

		private static Trace asyncRpcTracer = null;

		private static Trace accessControlTracer = null;

		private static Trace asyncRopHandlerTracer = null;

		private static Trace connectXropTracer = null;

		private static Trace failedXropTracer = null;

		private static Trace availabilityTracer = null;

		private static Trace logonTracer = null;

		private static Trace folderTracer = null;

		private static Trace exchangeAsyncDispatchTracer = null;

		private static Trace exchangeDispatchTracer = null;

		private static Trace dispatchTaskTracer = null;

		private static Trace rpcHttpConnectionRegistrationAsyncDispatchTracer = null;
	}
}
