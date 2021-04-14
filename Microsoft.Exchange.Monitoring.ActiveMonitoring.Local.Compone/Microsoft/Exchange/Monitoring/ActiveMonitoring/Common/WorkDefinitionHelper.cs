using System;
using System.Reflection;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal static class WorkDefinitionHelper
	{
		public static ProbeDefinition CreateProbeDefinition(string probeName, Type probeType, string targetResource, string serviceName, TimeSpan recurrenceInterval, bool enabled)
		{
			return new ProbeDefinition
			{
				AssemblyPath = probeType.Assembly.Location,
				TypeName = probeType.FullName,
				Name = probeName,
				RecurrenceIntervalSeconds = (int)recurrenceInterval.TotalSeconds,
				TimeoutSeconds = (int)recurrenceInterval.TotalSeconds,
				MaxRetryAttempts = 3,
				TargetResource = targetResource,
				ServiceName = serviceName,
				Enabled = enabled
			};
		}

		public static ResponderDefinition CreateRestartResponderDefinition(string responderName, Type responderType, string windowsServiceName, string componentName, string alertMask, string alertTypeId, ServiceHealthStatus targetHealthState, bool enabled)
		{
			ResponderDefinition responderDefinition = new ResponderDefinition();
			responderDefinition.AssemblyPath = responderType.Assembly.Location;
			responderDefinition.Name = responderName;
			responderDefinition.TypeName = responderType.FullName;
			responderDefinition.ServiceName = componentName;
			responderDefinition.Attributes["WindowsServiceName"] = windowsServiceName;
			responderDefinition.AlertTypeId = alertTypeId;
			responderDefinition.AlertMask = alertMask;
			responderDefinition.TargetHealthState = targetHealthState;
			responderDefinition.TargetResource = componentName;
			responderDefinition.Enabled = enabled;
			responderDefinition.RecurrenceIntervalSeconds = (int)TimeSpan.FromMinutes(2.0).TotalSeconds;
			responderDefinition.TimeoutSeconds = (int)TimeSpan.FromMinutes(2.0).TotalSeconds;
			responderDefinition.WaitIntervalSeconds = (int)TimeSpan.FromMinutes(2.0).TotalSeconds;
			return responderDefinition;
		}

		private const int MaxRetryAttempts = 3;

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;
	}
}
