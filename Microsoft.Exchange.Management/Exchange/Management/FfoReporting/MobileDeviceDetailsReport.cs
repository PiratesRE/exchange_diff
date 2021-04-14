using System;
using System.Data.Services.Common;
using Microsoft.Exchange.Management.FfoReporting.Common;

namespace Microsoft.Exchange.Management.FfoReporting
{
	[DataServiceKey("DeviceId")]
	[Serializable]
	public sealed class MobileDeviceDetailsReport : FfoReportObject
	{
		[DalConversion("DefaultSerializer", "DeviceId", new string[]
		{

		})]
		[Redact]
		public Guid DeviceId { get; set; }

		[Redact]
		[DalConversion("DefaultSerializer", "EASId", new string[]
		{

		})]
		public string EASId { get; set; }

		[DalConversion("DefaultSerializer", "IntuneId", new string[]
		{

		})]
		[Redact]
		public Guid IntuneId { get; set; }

		[DalConversion("DefaultSerializer", "User", new string[]
		{

		})]
		[Redact]
		public string User { get; set; }

		[Redact]
		[DalConversion("DefaultSerializer", "DeviceName", new string[]
		{

		})]
		public string DeviceName { get; set; }

		[DalConversion("DefaultSerializer", "DeviceModel", new string[]
		{

		})]
		public string DeviceModel { get; set; }

		[DalConversion("DefaultSerializer", "DeviceType", new string[]
		{

		})]
		public string DeviceType { get; set; }

		[DalConversion("DefaultSerializer", "FirstSyncTime", new string[]
		{

		})]
		public DateTime FirstSyncTime { get; set; }

		[DalConversion("DefaultSerializer", "LastSyncTime", new string[]
		{

		})]
		public DateTime LastSyncTime { get; set; }

		[DalConversion("DefaultSerializer", "IMEI", new string[]
		{

		})]
		[Redact]
		public string IMEI { get; set; }

		[Redact]
		[DalConversion("DefaultSerializer", "PhoneNumber", new string[]
		{

		})]
		public string PhoneNumber { get; set; }

		[DalConversion("DefaultSerializer", "MobileNetwork", new string[]
		{

		})]
		[Redact]
		public string MobileNetwork { get; set; }

		[DalConversion("DefaultSerializer", "EASVersion", new string[]
		{

		})]
		public string EASVersion { get; set; }

		[DalConversion("DefaultSerializer", "UserAgent", new string[]
		{

		})]
		public string UserAgent { get; set; }

		[DalConversion("DefaultSerializer", "DeviceLanguage", new string[]
		{

		})]
		public string DeviceLanguage { get; set; }

		[DalConversion("DefaultSerializer", "DeletedTime", new string[]
		{

		})]
		public DateTime? DeletedTime { get; set; }

		[DalConversion("DefaultSerializer", "Platform", new string[]
		{

		})]
		public string Platform { get; set; }

		[DalConversion("DefaultSerializer", "AccessState", new string[]
		{

		})]
		public int? AccessState { get; set; }

		[DalConversion("DefaultSerializer", "AccessStateRason", new string[]
		{

		})]
		public int? AccessStateReason { get; set; }

		[Redact]
		[DalConversion("DefaultSerializer", "AccessSetBy", new string[]
		{

		})]
		public string AccessSetBy { get; set; }

		[Redact]
		[DalConversion("DefaultSerializer", "PolicyApplied", new string[]
		{

		})]
		public string PolicyApplied { get; set; }

		public string Manufacturer { get; set; }

		[Redact]
		public string SerialNumber { get; set; }

		[ODataInput("StartDate")]
		public DateTime? StartDate { get; set; }

		[ODataInput("EndDate")]
		public DateTime? EndDate { get; set; }
	}
}
