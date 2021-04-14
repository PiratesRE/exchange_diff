using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.SoapWebClient.EWS;
using Microsoft.Exchange.UM.TroubleshootingTool.Shared;

namespace Microsoft.Exchange.UM.UMCommon
{
	[DataContract(Name = "CDRData", Namespace = "http://schemas.microsoft.com/version1/CDRData")]
	[XmlType(TypeName = "CDRDataType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class CDRData
	{
		[DataMember(Name = "CallStartTime")]
		[XmlElement]
		public DateTime CallStartTime { get; set; }

		[XmlElement]
		[DataMember(Name = "CallType")]
		public string CallType { get; set; }

		[DataMember(Name = "CallIdentity")]
		[XmlElement]
		public string CallIdentity { get; set; }

		[XmlElement]
		[DataMember(Name = "ParentCallIdentity")]
		public string ParentCallIdentity { get; set; }

		[DataMember(Name = "UMServerName")]
		[XmlElement]
		public string UMServerName { get; set; }

		[XmlElement]
		[DataMember(Name = "DialPlanGuid")]
		public Guid DialPlanGuid { get; set; }

		[DataMember(Name = "DialPlanName")]
		[XmlElement]
		public string DialPlanName { get; set; }

		[DataMember(Name = "CallDuration")]
		[XmlElement]
		public int CallDuration { get; set; }

		[DataMember(Name = "IPGatewayAddress")]
		[XmlElement]
		public string IPGatewayAddress { get; set; }

		[XmlElement]
		[DataMember(Name = "IPGatewayName")]
		public string IPGatewayName { get; set; }

		[DataMember(Name = "GatewayGuid")]
		[XmlElement]
		public Guid GatewayGuid { get; set; }

		[DataMember(Name = "CalledPhoneNumber")]
		[XmlElement]
		public string CalledPhoneNumber { get; set; }

		[XmlElement]
		[DataMember(Name = "CallerPhoneNumber")]
		public string CallerPhoneNumber { get; set; }

		[DataMember(Name = "OfferResult")]
		[XmlElement]
		public string OfferResult { get; set; }

		[XmlElement]
		[DataMember(Name = "DropCallReason")]
		public string DropCallReason { get; set; }

		[DataMember(Name = "ReasonForCall")]
		[XmlElement]
		public string ReasonForCall { get; set; }

		[DataMember(Name = "TransferredNumber")]
		[XmlElement]
		public string TransferredNumber { get; set; }

		[DataMember(Name = "DialedString")]
		[XmlElement]
		public string DialedString { get; set; }

		[DataMember(Name = "CallerMailboxAlias")]
		[XmlElement]
		public string CallerMailboxAlias { get; set; }

		[XmlElement]
		[DataMember(Name = "CalleeMailboxAlias")]
		public string CalleeMailboxAlias { get; set; }

		[DataMember(Name = "CallerLegacyExchangeDN")]
		[XmlElement]
		public string CallerLegacyExchangeDN { get; set; }

		[DataMember(Name = "CalleeLegacyExchangeDN")]
		[XmlElement]
		public string CalleeLegacyExchangeDN { get; set; }

		[DataMember(Name = "AutoAttendantName")]
		[XmlElement]
		public string AutoAttendantName { get; set; }

		[DataMember(Name = "EDiscoveryUserObjectGuid")]
		[XmlIgnore]
		public Guid EDiscoveryUserObjectGuid { get; set; }

		[DataMember(Name = "AudioQualityMetrics")]
		[XmlElement]
		public AudioQuality AudioQualityMetrics { get; set; }

		[DataMember(Name = "TenantGuid")]
		[XmlIgnore]
		public Guid TenantGuid { get; set; }

		[XmlElement]
		public DateTime CreationTime { get; set; }

		public CDRData() : this(default(DateTime), null, null, null, null, Guid.Empty, null, 0, null, null, Guid.Empty, null, null, null, null, null, null, null, null, null, null, null, null, Guid.Empty, AudioQuality.CreateDefaultAudioQuality(), Guid.Empty)
		{
		}

		public CDRData(DateTime callStartTime, string callType, string callId, string parentCallIdentity, string umServerName, Guid dialPlanGuid, string dialPlanName, int callDuration, string ipGatewayAddress, string ipGatewayName, Guid gatewayGuid, string calledPhoneNumber, string callerPhoneNumber, string offerResult, string dropCallReason, string reasonForCall, string transferredNumber, string dialedString, string callerMailboxAlias, string calleeMailboxAlias, string callerLegacyExchangeDN, string calleeLegacyExchangeDN, string autoAttendantName, Guid ediscoveryUserObjectGuid, AudioQuality audioQualitymetrics, Guid tenantGuid)
		{
			this.CallStartTime = callStartTime;
			this.CallType = callType;
			this.CallIdentity = callId;
			this.ParentCallIdentity = parentCallIdentity;
			this.UMServerName = umServerName;
			this.DialPlanGuid = dialPlanGuid;
			this.DialPlanName = dialPlanName;
			this.CallDuration = callDuration;
			this.IPGatewayAddress = ipGatewayAddress;
			this.IPGatewayName = ipGatewayName;
			this.GatewayGuid = gatewayGuid;
			this.CalledPhoneNumber = calledPhoneNumber;
			this.CallerPhoneNumber = callerPhoneNumber;
			this.OfferResult = offerResult;
			this.DropCallReason = dropCallReason;
			this.ReasonForCall = reasonForCall;
			this.TransferredNumber = transferredNumber;
			this.DialedString = dialedString;
			this.CallerMailboxAlias = callerMailboxAlias;
			this.CalleeMailboxAlias = calleeMailboxAlias;
			this.CallerLegacyExchangeDN = callerLegacyExchangeDN;
			this.CalleeLegacyExchangeDN = calleeLegacyExchangeDN;
			this.AutoAttendantName = autoAttendantName;
			this.EDiscoveryUserObjectGuid = ediscoveryUserObjectGuid;
			this.AudioQualityMetrics = audioQualitymetrics;
			this.TenantGuid = tenantGuid;
		}

		public CDRDataType ToCDRDataType()
		{
			return new CDRDataType
			{
				CallStartTime = this.CallStartTime,
				CallType = this.CallType,
				CallIdentity = this.CallIdentity,
				ParentCallIdentity = this.ParentCallIdentity,
				UMServerName = this.UMServerName,
				DialPlanGuid = this.DialPlanGuid.ToString(),
				DialPlanName = this.DialPlanName,
				CallDuration = this.CallDuration,
				IPGatewayAddress = this.IPGatewayAddress,
				IPGatewayName = this.IPGatewayName,
				GatewayGuid = this.GatewayGuid.ToString(),
				CalledPhoneNumber = this.CalledPhoneNumber,
				CallerPhoneNumber = this.CallerPhoneNumber,
				OfferResult = this.OfferResult,
				DropCallReason = this.DropCallReason,
				ReasonForCall = this.ReasonForCall,
				TransferredNumber = this.TransferredNumber,
				DialedString = this.DialedString,
				CallerMailboxAlias = this.CallerMailboxAlias,
				CalleeMailboxAlias = this.CalleeMailboxAlias,
				CallerLegacyExchangeDN = this.CallerLegacyExchangeDN,
				CalleeLegacyExchangeDN = this.CalleeLegacyExchangeDN,
				AutoAttendantName = this.AutoAttendantName,
				AudioQualityMetrics = new AudioQualityType(),
				AudioQualityMetrics = 
				{
					AudioCodec = this.AudioQualityMetrics.AudioCodec,
					BurstDensity = this.AudioQualityMetrics.BurstDensity,
					BurstDuration = this.AudioQualityMetrics.BurstDuration,
					Jitter = this.AudioQualityMetrics.Jitter,
					NMOS = this.AudioQualityMetrics.NMOS,
					NMOSDegradation = this.AudioQualityMetrics.NMOSDegradation,
					NMOSDegradationJitter = this.AudioQualityMetrics.NMOSDegradationJitter,
					NMOSDegradationPacketLoss = this.AudioQualityMetrics.NMOSDegradationPacketLoss,
					PacketLoss = this.AudioQualityMetrics.PacketLoss,
					RoundTrip = this.AudioQualityMetrics.RoundTrip
				}
			};
		}

		public CDRData(CDRDataType ewsType)
		{
			this.CallStartTime = ewsType.CallStartTime;
			this.CallType = ewsType.CallType;
			this.CallIdentity = ewsType.CallIdentity;
			this.ParentCallIdentity = ewsType.ParentCallIdentity;
			this.UMServerName = ewsType.UMServerName;
			this.DialPlanGuid = new Guid(ewsType.DialPlanGuid);
			this.DialPlanName = ewsType.DialPlanName;
			this.CallDuration = ewsType.CallDuration;
			this.IPGatewayAddress = ewsType.IPGatewayAddress;
			this.IPGatewayName = ewsType.IPGatewayName;
			this.GatewayGuid = new Guid(ewsType.GatewayGuid);
			this.CalledPhoneNumber = ewsType.CalledPhoneNumber;
			this.CallerPhoneNumber = ewsType.CallerPhoneNumber;
			this.OfferResult = ewsType.OfferResult;
			this.DropCallReason = ewsType.DropCallReason;
			this.ReasonForCall = ewsType.ReasonForCall;
			this.TransferredNumber = ewsType.TransferredNumber;
			this.DialedString = ewsType.DialedString;
			this.CallerMailboxAlias = ewsType.CallerMailboxAlias;
			this.CalleeMailboxAlias = ewsType.CalleeMailboxAlias;
			this.CallerLegacyExchangeDN = ewsType.CallerLegacyExchangeDN;
			this.CalleeLegacyExchangeDN = ewsType.CalleeLegacyExchangeDN;
			this.AutoAttendantName = ewsType.AutoAttendantName;
			this.AudioQualityMetrics = AudioQuality.CreateDefaultAudioQuality();
			this.AudioQualityMetrics.AudioCodec = ewsType.AudioQualityMetrics.AudioCodec;
			this.AudioQualityMetrics.BurstDensity = ewsType.AudioQualityMetrics.BurstDensity;
			this.AudioQualityMetrics.BurstDuration = ewsType.AudioQualityMetrics.BurstDuration;
			this.AudioQualityMetrics.Jitter = ewsType.AudioQualityMetrics.Jitter;
			this.AudioQualityMetrics.NMOS = ewsType.AudioQualityMetrics.NMOS;
			this.AudioQualityMetrics.NMOSDegradation = ewsType.AudioQualityMetrics.NMOSDegradation;
			this.AudioQualityMetrics.NMOSDegradationJitter = ewsType.AudioQualityMetrics.NMOSDegradationJitter;
			this.AudioQualityMetrics.NMOSDegradationPacketLoss = ewsType.AudioQualityMetrics.NMOSDegradationPacketLoss;
			this.AudioQualityMetrics.PacketLoss = ewsType.AudioQualityMetrics.PacketLoss;
			this.AudioQualityMetrics.RoundTrip = ewsType.AudioQualityMetrics.RoundTrip;
			this.TenantGuid = Guid.Empty;
			this.EDiscoveryUserObjectGuid = Guid.Empty;
		}
	}
}
