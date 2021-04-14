using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Text;
using System.Web;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.TroubleshootingTool.Shared;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.CrossServerMailboxAccess;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Cmdlet("Export", "UMCallDataRecord", SupportsShouldProcess = true)]
	public sealed class ExportUMCallDataRecord : UMReportsTaskBase<MailboxIdParameter>
	{
		private new MailboxIdParameter Identity
		{
			get
			{
				return base.Identity;
			}
			set
			{
				base.Identity = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false)]
		public UMDialPlanIdParameter UMDialPlan
		{
			get
			{
				return (UMDialPlanIdParameter)base.Fields["UMDialPlan"];
			}
			set
			{
				base.Fields["UMDialPlan"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public UMIPGatewayIdParameter UMIPGateway
		{
			get
			{
				return (UMIPGatewayIdParameter)base.Fields["UMIPGateway"];
			}
			set
			{
				base.Fields["UMIPGateway"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true)]
		public ExDateTime Date
		{
			get
			{
				return (ExDateTime)base.Fields["Date"];
			}
			set
			{
				base.Fields["Date"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true)]
		public Stream ClientStream { get; set; }

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageExportUMCallDataRecord(this.Date.ToString());
			}
		}

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			base.ValidateCommonParamsAndSetOrg(this.UMDialPlan, this.UMIPGateway, out this.dialPlanGuid, out this.gatewayGuid, out this.dialPlanName, out this.gatewayName);
		}

		protected override void ProcessMailbox()
		{
			try
			{
				ExDateTime exDateTime = this.Date.ToUtc();
				ExDateTime startDateTime = new ExDateTime(ExTimeZone.UtcTimeZone, exDateTime.Year, exDateTime.Month, exDateTime.Day);
				ExDateTime endDateTime = startDateTime.AddDays(1.0);
				StreamWriter streamWriter = new StreamWriter(this.ClientStream, Encoding.UTF8);
				streamWriter.WriteCsvLine(this.csvRow.Keys);
				using (IUMCallDataRecordStorage umcallDataRecordsAcessor = InterServerMailboxAccessor.GetUMCallDataRecordsAcessor(this.DataObject))
				{
					int num = 0;
					int numberOfRecordsToRead = 5000;
					if (Utils.RunningInTestMode)
					{
						numberOfRecordsToRead = 1;
					}
					bool flag;
					do
					{
						flag = false;
						CDRData[] umcallDataRecords = umcallDataRecordsAcessor.GetUMCallDataRecords(startDateTime.Subtract(this.TimeDelta), endDateTime.Add(this.TimeDelta), num, numberOfRecordsToRead);
						if (umcallDataRecords != null && umcallDataRecords.Length > 0)
						{
							num += umcallDataRecords.Length;
							this.WriteToStream(umcallDataRecords, streamWriter, startDateTime, endDateTime);
							flag = true;
						}
						streamWriter.Flush();
					}
					while (flag);
				}
			}
			catch (ArgumentException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidArgument, null);
			}
			catch (ObjectDisposedException exception2)
			{
				base.WriteError(exception2, ErrorCategory.InvalidArgument, null);
			}
			catch (IOException exception3)
			{
				base.WriteError(exception3, ErrorCategory.WriteError, null);
			}
			catch (UnableToFindUMReportDataException exception4)
			{
				base.WriteError(exception4, ErrorCategory.ReadError, null);
			}
			catch (StorageTransientException exception5)
			{
				base.WriteError(exception5, ErrorCategory.ReadError, null);
			}
			catch (StoragePermanentException exception6)
			{
				base.WriteError(exception6, ErrorCategory.ReadError, null);
			}
			catch (HttpException exception7)
			{
				base.WriteError(exception7, ErrorCategory.WriteError, null);
			}
			catch (CDROperationException exception8)
			{
				base.WriteError(exception8, ErrorCategory.ReadError, null);
			}
			catch (EWSUMMailboxAccessException exception9)
			{
				base.WriteError(exception9, ErrorCategory.ReadError, null);
			}
		}

		private void WriteToStream(CDRData[] cdrDataArray, StreamWriter writer, ExDateTime startDateTime, ExDateTime endDateTime)
		{
			foreach (CDRData cdrdata in cdrDataArray)
			{
				if (!(cdrdata.CallStartTime < startDateTime.UniversalTime) && !(cdrdata.CallStartTime >= endDateTime.UniversalTime) && (!(this.dialPlanGuid != Guid.Empty) || !(cdrdata.DialPlanGuid != this.dialPlanGuid)) && (!(this.gatewayGuid != Guid.Empty) || !(cdrdata.GatewayGuid != this.gatewayGuid)))
				{
					this.FillOrCleanCSVRow(cdrdata, false);
					writer.WriteCsvLine(this.csvRow.Values);
					this.FillOrCleanCSVRow(cdrdata, true);
				}
			}
		}

		private void FillOrCleanCSVRow(CDRData cdrData, bool clean)
		{
			this.csvRow["CallStartTime"] = (clean ? string.Empty : cdrData.CallStartTime.ToString("u"));
			this.csvRow["CallType"] = (clean ? string.Empty : Utils.CheckString(cdrData.CallType));
			this.csvRow["CallIdentity"] = (clean ? string.Empty : Utils.CheckString(cdrData.CallIdentity));
			this.csvRow["ParentCallIdentity"] = (clean ? string.Empty : Utils.CheckString(cdrData.ParentCallIdentity));
			if (!CommonConstants.UseDataCenterLogging)
			{
				this.csvRow["UMServerName"] = (clean ? string.Empty : Utils.CheckString(cdrData.UMServerName));
			}
			this.csvRow["DialPlanName"] = (clean ? string.Empty : Utils.CheckString(cdrData.DialPlanName));
			this.csvRow["CallDuration"] = (clean ? string.Empty : TimeSpan.FromSeconds((double)cdrData.CallDuration).ToString());
			this.csvRow["IPGatewayAddress"] = (clean ? string.Empty : Utils.CheckString(cdrData.IPGatewayAddress));
			this.csvRow["IPGatewayName"] = (clean ? string.Empty : Utils.CheckString(cdrData.IPGatewayName));
			this.csvRow["CalledPhoneNumber"] = (clean ? string.Empty : Utils.CheckString(cdrData.CalledPhoneNumber));
			this.csvRow["CallerPhoneNumber"] = (clean ? string.Empty : Utils.CheckString(cdrData.CallerPhoneNumber));
			this.csvRow["OfferResult"] = (clean ? string.Empty : Utils.CheckString(cdrData.OfferResult));
			this.csvRow["DropCallReason"] = (clean ? string.Empty : Utils.CheckString(cdrData.DropCallReason));
			this.csvRow["ReasonForCall"] = (clean ? string.Empty : Utils.CheckString(cdrData.ReasonForCall));
			this.csvRow["TransferredNumber"] = (clean ? string.Empty : Utils.CheckString(cdrData.TransferredNumber));
			this.csvRow["DialedString"] = (clean ? string.Empty : Utils.CheckString(cdrData.DialedString));
			this.csvRow["CallerMailboxAlias"] = (clean ? string.Empty : Utils.CheckString(cdrData.CallerMailboxAlias));
			this.csvRow["CalleeMailboxAlias"] = (clean ? string.Empty : Utils.CheckString(cdrData.CalleeMailboxAlias));
			this.csvRow["AutoAttendantName"] = (clean ? string.Empty : Utils.CheckString(cdrData.AutoAttendantName));
			this.csvRow["NMOS"] = (clean ? string.Empty : this.CheckAudioMetricString(cdrData.AudioQualityMetrics.NMOS));
			this.csvRow["NMOSDegradation"] = (clean ? string.Empty : this.CheckAudioMetricString(cdrData.AudioQualityMetrics.NMOSDegradation));
			this.csvRow["NMOSDegradationPacketLoss"] = (clean ? string.Empty : this.CheckAudioMetricString(cdrData.AudioQualityMetrics.NMOSDegradationPacketLoss));
			this.csvRow["NMOSDegradationJitter"] = (clean ? string.Empty : this.CheckAudioMetricString(cdrData.AudioQualityMetrics.NMOSDegradationJitter));
			this.csvRow["Jitter"] = (clean ? string.Empty : this.CheckAudioMetricString(cdrData.AudioQualityMetrics.Jitter));
			this.csvRow["PacketLoss"] = (clean ? string.Empty : this.CheckAudioMetricString(cdrData.AudioQualityMetrics.PacketLoss));
			this.csvRow["RoundTrip"] = (clean ? string.Empty : this.CheckAudioMetricString(cdrData.AudioQualityMetrics.RoundTrip));
			this.csvRow["BurstDensity"] = (clean ? string.Empty : this.CheckAudioMetricString(cdrData.AudioQualityMetrics.BurstDensity));
			this.csvRow["BurstDuration"] = (clean ? string.Empty : this.CheckAudioMetricString(cdrData.AudioQualityMetrics.BurstDuration));
			this.csvRow["AudioCodec"] = (clean ? string.Empty : Utils.CheckString(cdrData.AudioQualityMetrics.AudioCodec));
		}

		private string CheckAudioMetricString(float metric)
		{
			if (metric == AudioQuality.UnknownValue)
			{
				return string.Empty;
			}
			return metric.ToString("F1");
		}

		private const string FixedFormatString = "F1";

		private const string UTCFormatString = "u";

		private const int ChunkSize = 5000;

		private readonly TimeSpan TimeDelta = TimeSpan.FromHours(1.0);

		private Dictionary<string, string> csvRow = new Dictionary<string, string>
		{
			{
				"CallStartTime",
				string.Empty
			},
			{
				"CallType",
				string.Empty
			},
			{
				"CallIdentity",
				string.Empty
			},
			{
				"ParentCallIdentity",
				string.Empty
			},
			{
				"UMServerName",
				string.Empty
			},
			{
				"DialPlanName",
				string.Empty
			},
			{
				"CallDuration",
				string.Empty
			},
			{
				"IPGatewayAddress",
				string.Empty
			},
			{
				"IPGatewayName",
				string.Empty
			},
			{
				"CalledPhoneNumber",
				string.Empty
			},
			{
				"CallerPhoneNumber",
				string.Empty
			},
			{
				"OfferResult",
				string.Empty
			},
			{
				"DropCallReason",
				string.Empty
			},
			{
				"ReasonForCall",
				string.Empty
			},
			{
				"TransferredNumber",
				string.Empty
			},
			{
				"DialedString",
				string.Empty
			},
			{
				"CallerMailboxAlias",
				string.Empty
			},
			{
				"CalleeMailboxAlias",
				string.Empty
			},
			{
				"AutoAttendantName",
				string.Empty
			},
			{
				"NMOS",
				string.Empty
			},
			{
				"NMOSDegradation",
				string.Empty
			},
			{
				"NMOSDegradationJitter",
				string.Empty
			},
			{
				"NMOSDegradationPacketLoss",
				string.Empty
			},
			{
				"Jitter",
				string.Empty
			},
			{
				"PacketLoss",
				string.Empty
			},
			{
				"RoundTrip",
				string.Empty
			},
			{
				"BurstDensity",
				string.Empty
			},
			{
				"BurstDuration",
				string.Empty
			},
			{
				"AudioCodec",
				string.Empty
			}
		};

		private Guid dialPlanGuid;

		private Guid gatewayGuid;

		private string dialPlanName;

		private string gatewayName;

		internal abstract class CSVHeaders
		{
			public const string CallStartTime = "CallStartTime";

			public const string CallType = "CallType";

			public const string CallIdentity = "CallIdentity";

			public const string ParentCallIdentity = "ParentCallIdentity";

			public const string UMServerName = "UMServerName";

			public const string DialPlanName = "DialPlanName";

			public const string CallDuration = "CallDuration";

			public const string IPGatewayAddress = "IPGatewayAddress";

			public const string IPGatewayName = "IPGatewayName";

			public const string CalledPhoneNumber = "CalledPhoneNumber";

			public const string CallerPhoneNumber = "CallerPhoneNumber";

			public const string OfferResult = "OfferResult";

			public const string DropCallReason = "DropCallReason";

			public const string ReasonForCall = "ReasonForCall";

			public const string TransferredNumber = "TransferredNumber";

			public const string DialedString = "DialedString";

			public const string CallerMailboxAlias = "CallerMailboxAlias";

			public const string CalleeMailboxAlias = "CalleeMailboxAlias";

			public const string AutoAttendantName = "AutoAttendantName";

			public const string NMOS = "NMOS";

			public const string NMOSDegradation = "NMOSDegradation";

			public const string NMOSDegradationPacketLoss = "NMOSDegradationPacketLoss";

			public const string NMOSDegradationJitter = "NMOSDegradationJitter";

			public const string Jitter = "Jitter";

			public const string PacketLoss = "PacketLoss";

			public const string RoundTrip = "RoundTrip";

			public const string BurstDensity = "BurstDensity";

			public const string BurstDuration = "BurstDuration";

			public const string AudioCodec = "AudioCodec";
		}
	}
}
