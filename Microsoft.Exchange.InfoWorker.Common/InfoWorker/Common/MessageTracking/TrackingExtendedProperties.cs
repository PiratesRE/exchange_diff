using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	[Serializable]
	internal class TrackingExtendedProperties
	{
		internal TrackingExtendedProperties() : this(false, false, null, false, string.Empty, string.Empty, string.Empty, false)
		{
		}

		internal TrackingExtendedProperties(bool expandTree, bool searchAsRecip, TimeSpan? timeout, bool getAdditionalRecords, bool searchForModerationResult) : this(expandTree, searchAsRecip, timeout, getAdditionalRecords, string.Empty, string.Empty, string.Empty, searchForModerationResult)
		{
		}

		internal TrackingExtendedProperties(string messageTrackingReportId) : this(false, false, null, false, messageTrackingReportId, string.Empty, string.Empty, false)
		{
		}

		internal TrackingExtendedProperties(bool expandTree, bool searchAsRecip, TimeSpan? timeout, bool getAdditionalRecords, string messageTrackingReportId, string arbitrationMailboxAddress, string initMessageId, bool searchForModerationResult)
		{
			this.ExpandTree = expandTree;
			this.SearchAsRecip = searchAsRecip;
			this.Timeout = timeout;
			this.GetAdditionalRecords = getAdditionalRecords;
			this.MessageTrackingReportId = messageTrackingReportId;
			this.ArbitrationMailboxAddress = arbitrationMailboxAddress;
			this.InitMessageId = initMessageId;
			this.SearchForModerationResult = searchForModerationResult;
		}

		internal bool ExpandTree { get; private set; }

		internal bool SearchAsRecip { get; private set; }

		internal TimeSpan? Timeout { get; private set; }

		internal bool GetAdditionalRecords { get; private set; }

		internal bool SearchForModerationResult { get; private set; }

		internal string MessageTrackingReportId { get; set; }

		internal string ArbitrationMailboxAddress { get; private set; }

		internal string InitMessageId { get; private set; }

		internal static TrackingExtendedProperties CreateFromTrackingPropertyArray(TrackingPropertyType[] properties)
		{
			bool expandTree = false;
			bool searchAsRecip = false;
			bool searchForModerationResult = false;
			TimeSpan? timeout = null;
			bool getAdditionalRecords = false;
			string messageTrackingReportId = string.Empty;
			string arbitrationMailboxAddress = string.Empty;
			string initMessageId = string.Empty;
			if (properties == null)
			{
				return new TrackingExtendedProperties();
			}
			foreach (TrackingPropertyType trackingPropertyType in properties)
			{
				if ("ExpandTree".Equals(trackingPropertyType.Name, StringComparison.Ordinal))
				{
					expandTree = true;
				}
				else if ("SearchAsRecip".Equals(trackingPropertyType.Name, StringComparison.Ordinal))
				{
					searchAsRecip = true;
				}
				else if ("Timeout".Equals(trackingPropertyType.Name, StringComparison.Ordinal))
				{
					timeout = Parse.ParseFromMilliseconds(trackingPropertyType.Value);
				}
				else if ("GetAdditionalRecords".Equals(trackingPropertyType.Name, StringComparison.Ordinal))
				{
					getAdditionalRecords = true;
				}
				else if ("SearchForModerationResult".Equals(trackingPropertyType.Name, StringComparison.Ordinal))
				{
					searchForModerationResult = true;
				}
				else if ("MessageTrackingReportId".Equals(trackingPropertyType.Name, StringComparison.Ordinal))
				{
					messageTrackingReportId = trackingPropertyType.Value;
				}
				else if ("ArbitrationMailboxAddress".Equals(trackingPropertyType.Name, StringComparison.Ordinal))
				{
					arbitrationMailboxAddress = trackingPropertyType.Value;
				}
				else if ("InitiationMessageId".Equals(trackingPropertyType.Name, StringComparison.Ordinal))
				{
					initMessageId = trackingPropertyType.Value;
				}
			}
			return new TrackingExtendedProperties(expandTree, searchAsRecip, timeout, getAdditionalRecords, messageTrackingReportId, arbitrationMailboxAddress, initMessageId, searchForModerationResult);
		}

		internal TrackingPropertyType[] ToTrackingPropertyArray()
		{
			List<TrackingPropertyType> list = new List<TrackingPropertyType>(4);
			if (this.ExpandTree)
			{
				list.Add(new TrackingPropertyType
				{
					Name = "ExpandTree"
				});
			}
			if (this.SearchAsRecip)
			{
				list.Add(new TrackingPropertyType
				{
					Name = "SearchAsRecip"
				});
			}
			if (this.Timeout != null)
			{
				list.Add(new TrackingPropertyType
				{
					Name = "Timeout",
					Value = this.Timeout.Value.TotalMilliseconds.ToString(CultureInfo.InvariantCulture)
				});
			}
			if (this.GetAdditionalRecords)
			{
				list.Add(new TrackingPropertyType
				{
					Name = "GetAdditionalRecords"
				});
			}
			if (this.SearchForModerationResult)
			{
				list.Add(new TrackingPropertyType
				{
					Name = "SearchForModerationResult"
				});
			}
			if (!string.IsNullOrEmpty(this.MessageTrackingReportId))
			{
				list.Add(new TrackingPropertyType
				{
					Name = "MessageTrackingReportId",
					Value = this.MessageTrackingReportId
				});
			}
			if (!string.IsNullOrEmpty(this.ArbitrationMailboxAddress))
			{
				list.Add(new TrackingPropertyType
				{
					Name = "ArbitrationMailboxAddress",
					Value = this.ArbitrationMailboxAddress
				});
			}
			if (!string.IsNullOrEmpty(this.InitMessageId))
			{
				list.Add(new TrackingPropertyType
				{
					Name = "InitiationMessageId",
					Value = this.InitMessageId
				});
			}
			return list.ToArray();
		}
	}
}
