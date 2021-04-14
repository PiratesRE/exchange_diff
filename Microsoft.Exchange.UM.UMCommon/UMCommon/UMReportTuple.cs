using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.UM.UMCommon
{
	[DataContract(Name = "UMReportTuple", Namespace = "http://schemas.microsoft.com/v1.0/UMReportAggregatedData")]
	internal class UMReportTuple : IEquatable<UMReportTuple>
	{
		[DataMember(Name = "DialPlanGuid")]
		public Guid DialPlanGuid { get; private set; }

		[DataMember(Name = "GatewayGuid")]
		public Guid GatewayGuid { get; private set; }

		public UMReportTuple() : this(Guid.Empty, Guid.Empty)
		{
		}

		public UMReportTuple(Guid dpGuid, Guid gwGuid)
		{
			this.DialPlanGuid = dpGuid;
			this.GatewayGuid = gwGuid;
		}

		public static bool operator ==(UMReportTuple tuple1, UMReportTuple tuple2)
		{
			return tuple1.Equals(tuple2);
		}

		public static bool operator !=(UMReportTuple tuple1, UMReportTuple tuple2)
		{
			return !tuple1.Equals(tuple2);
		}

		public static UMReportTuple[] GetTuplesToAddInReport(CDRData cdrData)
		{
			if (cdrData.DialPlanGuid == Guid.Empty)
			{
				throw new InvalidArgumentException("cdrData.DialPlanGuid cannot be empty.");
			}
			List<UMReportTuple> list = new List<UMReportTuple>
			{
				new UMReportTuple(),
				new UMReportTuple(cdrData.DialPlanGuid, Guid.Empty)
			};
			if (cdrData.GatewayGuid != Guid.Empty)
			{
				list.Add(new UMReportTuple(Guid.Empty, cdrData.GatewayGuid));
				list.Add(new UMReportTuple(cdrData.DialPlanGuid, cdrData.GatewayGuid));
			}
			return list.ToArray();
		}

		public bool Equals(UMReportTuple other)
		{
			return this.DialPlanGuid == this.DialPlanGuid && this.GatewayGuid == other.GatewayGuid;
		}

		public override bool Equals(object other)
		{
			if (other == null)
			{
				return base.Equals(other);
			}
			UMReportTuple umreportTuple = other as UMReportTuple;
			if (umreportTuple == null)
			{
				throw new InvalidCastException("Comparison of only UMReportTuple type is allowed.");
			}
			return this.Equals(umreportTuple);
		}

		public override int GetHashCode()
		{
			return this.DialPlanGuid.GetHashCode() ^ this.GatewayGuid.GetHashCode();
		}

		public bool ShouldRemoveFromReport(OrganizationId orgId)
		{
			IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromOrganizationId(orgId);
			return (this.DialPlanGuid != Guid.Empty && iadsystemConfigurationLookup.GetDialPlanFromId(new ADObjectId(this.DialPlanGuid)) == null) || (this.GatewayGuid != Guid.Empty && iadsystemConfigurationLookup.GetIPGatewayFromId(new ADObjectId(this.GatewayGuid)) == null);
		}
	}
}
