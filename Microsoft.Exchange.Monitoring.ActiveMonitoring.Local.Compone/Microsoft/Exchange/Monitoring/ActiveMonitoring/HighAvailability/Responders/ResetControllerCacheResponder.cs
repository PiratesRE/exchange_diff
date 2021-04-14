using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability.Responders
{
	public class ResetControllerCacheResponder : ResponderWorkItem
	{
		public static ResponderDefinition CreateDefinition(string name, string serviceName, string alertTypeId, string alertMask, string targetResource, ServiceHealthStatus targetHealthState, string throttleGroupName = null)
		{
			return new ResponderDefinition
			{
				AssemblyPath = Assembly.GetExecutingAssembly().Location,
				TypeName = typeof(ResetControllerCacheResponder).FullName,
				Name = name,
				ServiceName = serviceName,
				AlertTypeId = alertTypeId,
				AlertMask = alertMask,
				TargetResource = targetResource,
				TargetHealthState = targetHealthState,
				RecurrenceIntervalSeconds = 300,
				WaitIntervalSeconds = 30,
				TimeoutSeconds = 150,
				MaxRetryAttempts = 2,
				Enabled = true
			};
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			HpAcuCliWrapper.ControllerStatusSimple[] array = HpAcuCliWrapper.ListAllControllers();
			WorkItemResult result = base.Result;
			string format = "Controllers discovered = {0}";
			object arg;
			if (array != null)
			{
				arg = string.Join(",", from o in array
				select o.Name);
			}
			else
			{
				arg = "NULL";
			}
			result.StateAttribute1 = string.Format(format, arg);
			HpAcuCliWrapper.ControllerStatusSimple controllerStatusSimple = (from o in array
			where o.Name.Contains("Smart Array P")
			select o).FirstOrDefault<HpAcuCliWrapper.ControllerStatusSimple>();
			if (controllerStatusSimple == null || string.IsNullOrEmpty(controllerStatusSimple.Name))
			{
				base.Result.StateAttribute2 = "Controller chosen is NULL. No action taken";
				return;
			}
			base.Result.StateAttribute2 = string.Format("Controller chosen = {0} Slot={1} Status={2} Cache={3} Battery={4}", new object[]
			{
				controllerStatusSimple.Name,
				controllerStatusSimple.SlotNumber,
				controllerStatusSimple.Status,
				controllerStatusSimple.CacheStatus,
				controllerStatusSimple.BatteryStatus
			});
			if (!ResetControllerCacheResponder.CheckControllerStatus(controllerStatusSimple))
			{
				base.Result.StateAttribute3 = "Controller not in good shape. No action taken";
				return;
			}
			HpAcuCliWrapper.ModifyCommandResult modifyCommandResult = HpAcuCliWrapper.ModifyLogicalDriveArrayAccelerator(controllerStatusSimple.SlotNumber, false);
			base.Result.StateAttribute3 = string.Format("Step 1/6: Turn off ArrayAccelerator on Logical Drives = {0}, Error = {1}", modifyCommandResult.Success, modifyCommandResult.ErrorMessage);
			if (!modifyCommandResult.Success)
			{
				throw new HighAvailabilityMAResponderException("Unable to turn off Array Accelerator!");
			}
			modifyCommandResult = HpAcuCliWrapper.ModifyLogicalDriveArrayAccelerator(controllerStatusSimple.SlotNumber, true);
			base.Result.StateAttribute3 = string.Format("Step 2/6: Turn on ArrayAccelerator on Logical Drives = {0}, Error = {1}", modifyCommandResult.Success, modifyCommandResult.ErrorMessage);
			if (!modifyCommandResult.Success)
			{
				throw new HighAvailabilityMAResponderException("Unable to turn on Array Accelerator!");
			}
			modifyCommandResult = HpAcuCliWrapper.ModifyLogicalDriveCaching(controllerStatusSimple.SlotNumber, false);
			base.Result.StateAttribute3 = string.Format("Step 3/6: Turn off Caching on Logical Drives = {0}, Error = {1}", modifyCommandResult.Success, modifyCommandResult.ErrorMessage);
			if (!modifyCommandResult.Success)
			{
				throw new HighAvailabilityMAResponderException("Unable to turn off Caching!");
			}
			modifyCommandResult = HpAcuCliWrapper.ModifyLogicalDriveCaching(controllerStatusSimple.SlotNumber, true);
			base.Result.StateAttribute3 = string.Format("Step 4/6: Turn on Caching on Logical Drives = {0}, Error = {1}", modifyCommandResult.Success, modifyCommandResult.ErrorMessage);
			if (!modifyCommandResult.Success)
			{
				throw new HighAvailabilityMAResponderException("Unable to turn on Caching!");
			}
			modifyCommandResult = HpAcuCliWrapper.ResetCacheRatio(controllerStatusSimple.SlotNumber);
			base.Result.StateAttribute3 = string.Format("Step 5/6: Reset Cache Ratio = {0}, Error = {1}", modifyCommandResult.Success, modifyCommandResult.ErrorMessage);
			if (!modifyCommandResult.Success)
			{
				throw new HighAvailabilityMAResponderException("Unable to Reset Cache Ratio!");
			}
			modifyCommandResult = HpAcuCliWrapper.ResetNoBatteryWriteCache(controllerStatusSimple.SlotNumber);
			base.Result.StateAttribute3 = string.Format("Step 6/6: Reset NoBatteryWriteCache Policy = {0}, Error = {1}", modifyCommandResult.Success, modifyCommandResult.ErrorMessage);
			if (!modifyCommandResult.Success)
			{
				throw new HighAvailabilityMAResponderException("Unable to Reset NoBatteryWriteCache Policy!");
			}
			Thread.Sleep(TimeSpan.FromSeconds(15.0));
			array = HpAcuCliWrapper.ListAllControllers();
			HpAcuCliWrapper.ControllerStatusSimple controllerStatusSimple2 = (from o in array
			where o.Name.Contains("Smart Array P")
			select o).FirstOrDefault<HpAcuCliWrapper.ControllerStatusSimple>();
			base.Result.StateAttribute2 = string.Format("END Controller status = {0} Slot={1} Status={2} Cache={3} Battery={4}", new object[]
			{
				controllerStatusSimple2.Name,
				controllerStatusSimple2.SlotNumber,
				controllerStatusSimple2.Status,
				controllerStatusSimple2.CacheStatus,
				controllerStatusSimple2.BatteryStatus
			});
			if (!ResetControllerCacheResponder.CheckControllerStatus(controllerStatusSimple2))
			{
				throw new HighAvailabilityMAResponderException("Controller status is not in as good shape as before!");
			}
			HighAvailabilityUtility.LogEvent("Microsoft-Exchange-HighAvailability/Operational", "Microsoft-Exchange-HighAvailability", 388L, 7, EventLogEntryType.Information, new object[]
			{
				Environment.MachineName
			});
		}

		private static bool CheckControllerStatus(HpAcuCliWrapper.ControllerStatusSimple controller)
		{
			return controller != null && controller.BatteryStatus.Equals("OK", StringComparison.OrdinalIgnoreCase) && controller.Status.Equals("OK", StringComparison.OrdinalIgnoreCase);
		}

		private const string LogName = "Microsoft-Exchange-HighAvailability/Operational";

		private const string LogSource = "Microsoft-Exchange-HighAvailability";

		private const int TaskId = 7;

		private const long SuccessEventId = 388L;
	}
}
