using System;
using System.Management.Automation;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Configuration.Common.LocStrings;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public abstract class ConfigureService : ManageServiceBase
	{
		internal ServiceActionType FirstFailureActionType
		{
			get
			{
				return this.firstFailureActionType;
			}
			set
			{
				this.firstFailureActionType = value;
			}
		}

		internal ServiceActionType SecondFailureActionType
		{
			get
			{
				return this.secondFailureActionType;
			}
			set
			{
				this.secondFailureActionType = value;
			}
		}

		internal ServiceActionType AllOtherFailuresActionType
		{
			get
			{
				return this.allOtherFailuresActionType;
			}
			set
			{
				this.allOtherFailuresActionType = value;
			}
		}

		protected abstract string Name { get; }

		protected uint FirstFailureActionDelay
		{
			get
			{
				return this.firstFailureActionDelay;
			}
			set
			{
				this.firstFailureActionDelay = value;
			}
		}

		protected uint SecondFailureActionDelay
		{
			get
			{
				return this.secondFailureActionDelay;
			}
			set
			{
				this.secondFailureActionDelay = value;
			}
		}

		protected uint AllOtherFailuresActionDelay
		{
			get
			{
				return this.allOtherFailuresActionDelay;
			}
			set
			{
				this.allOtherFailuresActionDelay = value;
			}
		}

		protected uint FailureResetPeriod
		{
			get
			{
				return this.failureResetPeriod;
			}
			set
			{
				this.failureResetPeriod = value;
			}
		}

		protected bool FailureActionsFlag
		{
			get
			{
				return this.failureActionsFlag;
			}
			set
			{
				this.failureActionsFlag = value;
			}
		}

		protected void ConfigureFailureActions()
		{
			base.DoNativeServiceTask(this.Name, ServiceAccessFlags.AllAccess, delegate(IntPtr service)
			{
				IntPtr intPtr = IntPtr.Zero;
				TaskLogger.Trace("Configuring failure actions...", new object[0]);
				try
				{
					ServiceFailureActions serviceFailureActions = default(ServiceFailureActions);
					serviceFailureActions.resetPeriod = this.FailureResetPeriod;
					serviceFailureActions.rebootMessage = null;
					serviceFailureActions.command = null;
					serviceFailureActions.actionCount = 3U;
					int num = Marshal.SizeOf(typeof(ServiceAction));
					intPtr = Marshal.AllocHGlobal((int)((long)num * (long)((ulong)serviceFailureActions.actionCount)));
					serviceFailureActions.actions = intPtr;
					this.ConfigureFailureAction(intPtr, num, 0, this.firstFailureActionType, this.firstFailureActionDelay);
					this.ConfigureFailureAction(intPtr, num, 1, this.secondFailureActionType, this.secondFailureActionDelay);
					this.ConfigureFailureAction(intPtr, num, 2, this.allOtherFailuresActionType, this.allOtherFailuresActionDelay);
					if (!NativeMethods.ChangeServiceConfig2(service, ServiceConfigInfoLevels.FailureActions, ref serviceFailureActions))
					{
						base.WriteError(TaskWin32Exception.FromErrorCodeAndVerbose(Marshal.GetLastWin32Error(), Strings.ErrorChangeServiceConfig2(this.Name)), ErrorCategory.WriteError, null);
					}
				}
				finally
				{
					if (IntPtr.Zero != intPtr)
					{
						Marshal.FreeHGlobal(intPtr);
					}
				}
			});
		}

		protected void ConfigureServiceSidType()
		{
			base.DoNativeServiceTask(this.Name, ServiceAccessFlags.AllAccess, delegate(IntPtr service)
			{
				ServiceSidActions serviceSidActions = default(ServiceSidActions);
				serviceSidActions.serviceSidType = 0U;
				if (!NativeMethods.ChangeServiceConfig2(service, ServiceConfigInfoLevels.ServiceSid, ref serviceSidActions))
				{
					base.WriteError(TaskWin32Exception.FromErrorCodeAndVerbose(Marshal.GetLastWin32Error(), Strings.ErrorChangeServiceConfig2(this.Name)), ErrorCategory.WriteError, null);
				}
			});
		}

		protected void ConfigureFailureActionsFlag()
		{
			base.DoNativeServiceTask(this.Name, ServiceAccessFlags.AllAccess, delegate(IntPtr service)
			{
				TaskLogger.Trace("Configuring failure actions flag...", new object[0]);
				ServiceFailureActionsFlag serviceFailureActionsFlag = default(ServiceFailureActionsFlag);
				serviceFailureActionsFlag.fFailureActionsOnNonCrashFailures = this.failureActionsFlag;
				if (!NativeMethods.ChangeServiceConfig2(service, ServiceConfigInfoLevels.FailureActionsFlag, ref serviceFailureActionsFlag))
				{
					base.WriteError(TaskWin32Exception.FromErrorCodeAndVerbose(Marshal.GetLastWin32Error(), Strings.ErrorChangeServiceConfig2(this.Name)), ErrorCategory.WriteError, null);
				}
			});
		}

		private void ConfigureFailureAction(IntPtr actions, int serviceActionDataSize, int index, ServiceActionType failureActionType, uint failureActionDelay)
		{
			Marshal.WriteInt32(actions, index * serviceActionDataSize + (int)Marshal.OffsetOf(typeof(ServiceAction), "actionType"), (int)failureActionType);
			Marshal.WriteInt32(actions, index * serviceActionDataSize + (int)Marshal.OffsetOf(typeof(ServiceAction), "delay"), (int)failureActionDelay);
		}

		private const uint failureActionTryCount = 3U;

		private uint firstFailureActionDelay;

		private ServiceActionType firstFailureActionType;

		private uint secondFailureActionDelay;

		private ServiceActionType secondFailureActionType;

		private uint allOtherFailuresActionDelay;

		private ServiceActionType allOtherFailuresActionType;

		private uint failureResetPeriod;

		private bool failureActionsFlag;
	}
}
