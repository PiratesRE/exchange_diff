using System;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Transport.RightsManagement
{
	internal sealed class RmsLatencyTracker : IRmsLatencyTracker
	{
		public RmsLatencyTracker(LatencyTracker latencyTracker)
		{
			this.latencyTracker = latencyTracker;
		}

		public void BeginTrackRmsLatency(RmsOperationType operation)
		{
			LatencyComponent latencyComponent = RmsLatencyTracker.GetLatencyComponent(operation);
			if (latencyComponent != LatencyComponent.None)
			{
				LatencyTracker.BeginTrackLatency(latencyComponent, this.latencyTracker);
			}
		}

		public void EndTrackRmsLatency(RmsOperationType operation)
		{
			LatencyComponent latencyComponent = RmsLatencyTracker.GetLatencyComponent(operation);
			if (latencyComponent != LatencyComponent.None)
			{
				LatencyTracker.EndTrackLatency(latencyComponent, this.latencyTracker);
			}
		}

		public void EndAndBeginTrackRmsLatency(RmsOperationType endOperation, RmsOperationType beginOperation)
		{
			LatencyComponent latencyComponent = RmsLatencyTracker.GetLatencyComponent(endOperation);
			LatencyComponent latencyComponent2 = RmsLatencyTracker.GetLatencyComponent(beginOperation);
			if (latencyComponent2 != LatencyComponent.None && latencyComponent != LatencyComponent.None)
			{
				LatencyTracker.EndAndBeginTrackLatency(latencyComponent, latencyComponent2, this.latencyTracker);
				return;
			}
			if (latencyComponent != LatencyComponent.None)
			{
				LatencyTracker.EndTrackLatency(latencyComponent, this.latencyTracker);
				return;
			}
			if (latencyComponent2 != LatencyComponent.None)
			{
				LatencyTracker.BeginTrackLatency(latencyComponent2, this.latencyTracker);
			}
		}

		private static LatencyComponent GetLatencyComponent(RmsOperationType operation)
		{
			switch (operation)
			{
			case RmsOperationType.AcquireLicense:
				return LatencyComponent.RmsAcquireLicense;
			case RmsOperationType.AcquireTemplates:
				return LatencyComponent.RmsAcquireTemplates;
			case RmsOperationType.AcquireTemplateInfo:
				return LatencyComponent.RmsAcquireTemplateInfo;
			case RmsOperationType.AcquireServerBoxRac:
				return LatencyComponent.RmsAcquireServerBoxRac;
			case RmsOperationType.AcquireClc:
				return LatencyComponent.RmsAcquireClc;
			case RmsOperationType.AcquirePrelicense:
				return LatencyComponent.RmsAcquirePrelicense;
			case RmsOperationType.FindServiceLocations:
				return LatencyComponent.RmsFindServiceLocation;
			case RmsOperationType.AcquireCertificationMexData:
				return LatencyComponent.RmsAcquireCertificationMexData;
			case RmsOperationType.AcquireServerLicensingMexData:
				return LatencyComponent.RmsAcquireServerLicensingMexData;
			case RmsOperationType.AcquireB2BRac:
				return LatencyComponent.RmsAcquireB2BRac;
			case RmsOperationType.AcquireB2BLicense:
				return LatencyComponent.RmsAcquireB2BLicense;
			case RmsOperationType.RequestDelegationToken:
				return LatencyComponent.RmsRequestDelegationToken;
			default:
				return LatencyComponent.None;
			}
		}

		private LatencyTracker latencyTracker;
	}
}
