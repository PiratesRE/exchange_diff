using System;
using System.Linq;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal sealed class DispatchOptions
	{
		internal DispatchOptions()
		{
			this.isMonitoringContext = false;
			this.isMapiHttp = false;
			this.isExchangeSubsystem = false;
		}

		internal DispatchOptions(string protocolSequence, AuxiliaryData auxiliaryData)
		{
			this.isMonitoringContext = DispatchOptions.IsInMonitoringContext(auxiliaryData);
			this.isMapiHttp = "MapiHttp".Equals(protocolSequence);
			this.isExchangeSubsystem = DispatchOptions.IsExchangeSubsystem(auxiliaryData);
		}

		internal bool AppendMonitoringAuxiliaryBlock
		{
			get
			{
				return this.isMonitoringContext;
			}
		}

		internal bool DoNotRethrowExceptionsOnFailure
		{
			get
			{
				return !this.isMapiHttp && this.isMonitoringContext;
			}
		}

		internal bool EnforceMicrodelays
		{
			get
			{
				return !this.isMonitoringContext;
			}
		}

		internal bool UseRandomAdditionalRetryDelay
		{
			get
			{
				return !this.isMonitoringContext && !this.isExchangeSubsystem;
			}
		}

		private static bool IsInMonitoringContext(AuxiliaryData auxiliaryData)
		{
			SetMonitoringContextAuxiliaryBlock setMonitoringContextAuxiliaryBlock = auxiliaryData.Input.OfType<SetMonitoringContextAuxiliaryBlock>().FirstOrDefault<SetMonitoringContextAuxiliaryBlock>();
			return setMonitoringContextAuxiliaryBlock != null;
		}

		private static bool IsExchangeSubsystem(AuxiliaryData auxiliaryData)
		{
			PerfClientInfoAuxiliaryBlock perfClientInfoAuxiliaryBlock = auxiliaryData.Input.OfType<PerfClientInfoAuxiliaryBlock>().FirstOrDefault<PerfClientInfoAuxiliaryBlock>();
			return perfClientInfoAuxiliaryBlock != null && perfClientInfoAuxiliaryBlock.ClientMode == ClientMode.ExchangeServer;
		}

		private const string MapiHttpProtocolSequence = "MapiHttp";

		private readonly bool isMonitoringContext;

		private readonly bool isMapiHttp;

		private readonly bool isExchangeSubsystem;
	}
}
