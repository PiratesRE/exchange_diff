using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.LatencyDetection;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class TaskPerformanceData
	{
		public static PerformanceDataProvider CmdletInvoked
		{
			get
			{
				if (TaskPerformanceData.cmdletInvoked == null)
				{
					TaskPerformanceData.cmdletInvoked = new PerformanceDataProvider("Cmdlet Invoked");
				}
				return TaskPerformanceData.cmdletInvoked;
			}
		}

		public static PerformanceDataProvider BeginProcessingInvoked
		{
			get
			{
				if (TaskPerformanceData.beginProcessingInvoked == null)
				{
					TaskPerformanceData.beginProcessingInvoked = new PerformanceDataProvider("BeginProcessing Invoked");
				}
				return TaskPerformanceData.beginProcessingInvoked;
			}
		}

		public static PerformanceDataProvider ProcessRecordInvoked
		{
			get
			{
				if (TaskPerformanceData.processRecordInvoked == null)
				{
					TaskPerformanceData.processRecordInvoked = new PerformanceDataProvider("ProcessRecord Invoked");
				}
				return TaskPerformanceData.processRecordInvoked;
			}
		}

		public static PerformanceDataProvider EndProcessingInvoked
		{
			get
			{
				if (TaskPerformanceData.endProcessingInvoked == null)
				{
					TaskPerformanceData.endProcessingInvoked = new PerformanceDataProvider("EndProcessing Invoked");
				}
				return TaskPerformanceData.endProcessingInvoked;
			}
		}

		public static PerformanceDataProvider InternalValidate
		{
			get
			{
				if (TaskPerformanceData.internalValidate == null)
				{
					TaskPerformanceData.internalValidate = new PerformanceDataProvider("InternalValidate");
				}
				return TaskPerformanceData.internalValidate;
			}
		}

		public static PerformanceDataProvider InternalStateReset
		{
			get
			{
				if (TaskPerformanceData.internalStateReset == null)
				{
					TaskPerformanceData.internalStateReset = new PerformanceDataProvider("InternalStateReset");
				}
				return TaskPerformanceData.internalStateReset;
			}
		}

		public static PerformanceDataProvider InternalProcessRecord
		{
			get
			{
				if (TaskPerformanceData.internalProcessRecord == null)
				{
					TaskPerformanceData.internalProcessRecord = new PerformanceDataProvider("InternalProcessRecord");
				}
				return TaskPerformanceData.internalProcessRecord;
			}
		}

		public static PerformanceDataProvider SaveInitial
		{
			get
			{
				if (TaskPerformanceData.saveInitial == null)
				{
					TaskPerformanceData.saveInitial = new PerformanceDataProvider("SaveInitial");
				}
				return TaskPerformanceData.saveInitial;
			}
		}

		public static PerformanceDataProvider ReadUpdated
		{
			get
			{
				if (TaskPerformanceData.readUpdated == null)
				{
					TaskPerformanceData.readUpdated = new PerformanceDataProvider("ReadUpdated");
				}
				return TaskPerformanceData.readUpdated;
			}
		}

		public static PerformanceDataProvider SaveResult
		{
			get
			{
				if (TaskPerformanceData.saveResult == null)
				{
					TaskPerformanceData.saveResult = new PerformanceDataProvider("SaveResult");
				}
				return TaskPerformanceData.saveResult;
			}
		}

		public static PerformanceDataProvider ReadResult
		{
			get
			{
				if (TaskPerformanceData.readResult == null)
				{
					TaskPerformanceData.readResult = new PerformanceDataProvider("ReadResult");
				}
				return TaskPerformanceData.readResult;
			}
		}

		public static PerformanceDataProvider WriteResult
		{
			get
			{
				if (TaskPerformanceData.writeResult == null)
				{
					TaskPerformanceData.writeResult = new PerformanceDataProvider("WriteResult");
				}
				return TaskPerformanceData.writeResult;
			}
		}

		public static PerformanceDataProvider WindowsLiveIdProvisioningHandlerForNew
		{
			get
			{
				if (TaskPerformanceData.windowsLiveIdProvisioningHandlerForNew == null)
				{
					TaskPerformanceData.windowsLiveIdProvisioningHandlerForNew = new PerformanceDataProvider("WindowsLiveIdProvisioningHandlerForNew");
				}
				return TaskPerformanceData.windowsLiveIdProvisioningHandlerForNew;
			}
		}

		public static PerformanceDataProvider MailboxProvisioningHandler
		{
			get
			{
				if (TaskPerformanceData.mailboxProvisioningHandler == null)
				{
					TaskPerformanceData.mailboxProvisioningHandler = new PerformanceDataProvider("MailboxProvisioningHandler");
				}
				return TaskPerformanceData.mailboxProvisioningHandler;
			}
		}

		public static PerformanceDataProvider AdminLogProvisioningHandler
		{
			get
			{
				if (TaskPerformanceData.adminLogProvisioningHandler == null)
				{
					TaskPerformanceData.adminLogProvisioningHandler = new PerformanceDataProvider("AdminLogProvisioningHandler");
				}
				return TaskPerformanceData.adminLogProvisioningHandler;
			}
		}

		public static PerformanceDataProvider OtherProvisioningHandlers
		{
			get
			{
				if (TaskPerformanceData.otherProvisioningHandlers == null)
				{
					TaskPerformanceData.otherProvisioningHandlers = new PerformanceDataProvider("OtherProvisioningHandlers");
				}
				return TaskPerformanceData.otherProvisioningHandlers;
			}
		}

		[ThreadStatic]
		private static PerformanceDataProvider cmdletInvoked;

		[ThreadStatic]
		private static PerformanceDataProvider beginProcessingInvoked;

		[ThreadStatic]
		private static PerformanceDataProvider processRecordInvoked;

		[ThreadStatic]
		private static PerformanceDataProvider endProcessingInvoked;

		[ThreadStatic]
		private static PerformanceDataProvider internalValidate;

		[ThreadStatic]
		private static PerformanceDataProvider internalStateReset;

		[ThreadStatic]
		private static PerformanceDataProvider internalProcessRecord;

		[ThreadStatic]
		private static PerformanceDataProvider saveInitial;

		[ThreadStatic]
		private static PerformanceDataProvider readUpdated;

		[ThreadStatic]
		private static PerformanceDataProvider saveResult;

		[ThreadStatic]
		private static PerformanceDataProvider readResult;

		[ThreadStatic]
		private static PerformanceDataProvider writeResult;

		[ThreadStatic]
		private static PerformanceDataProvider windowsLiveIdProvisioningHandlerForNew;

		[ThreadStatic]
		private static PerformanceDataProvider mailboxProvisioningHandler;

		[ThreadStatic]
		private static PerformanceDataProvider adminLogProvisioningHandler;

		[ThreadStatic]
		private static PerformanceDataProvider otherProvisioningHandlers;
	}
}
