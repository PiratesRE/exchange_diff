using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	internal class TransportTraceConfiguration : TraceConfigurationBase
	{
		public override void OnLoad()
		{
			this.filteredSubjects = TransportTraceConfiguration.GetFilterList(this.exTraceConfiguration, "Subject");
			this.filteredSenders = TransportTraceConfiguration.GetFilterList(this.exTraceConfiguration, "SenderSmtp");
			this.filteredRecipients = TransportTraceConfiguration.GetFilterList(this.exTraceConfiguration, "RecipientSmtp");
			this.filteredUsers = TransportTraceConfiguration.EncapsulateAddressList(TransportTraceConfiguration.GetFilterList(this.exTraceConfiguration, "UserDN"));
		}

		public List<string> FilteredSubjects
		{
			get
			{
				return this.filteredSubjects;
			}
		}

		public List<string> FilteredSenders
		{
			get
			{
				return this.filteredSenders;
			}
		}

		public List<string> FilteredRecipients
		{
			get
			{
				return this.filteredRecipients;
			}
		}

		public List<string> FilteredUsers
		{
			get
			{
				return this.filteredUsers;
			}
		}

		private static List<string> EncapsulateAddressList(List<string> legacyDNList)
		{
			List<string> list = new List<string>(legacyDNList.Count);
			foreach (string address in legacyDNList)
			{
				SmtpProxyAddress smtpProxyAddress;
				if (SmtpProxyAddress.TryEncapsulate("EX", address, Components.Configuration.FirstOrgAcceptedDomainTable.DefaultDomain.DomainName.Domain, out smtpProxyAddress))
				{
					list.Add(smtpProxyAddress.SmtpAddress);
				}
			}
			return list;
		}

		private static List<string> GetFilterList(ExTraceConfiguration configuration, string filterKey)
		{
			List<string> result;
			if (configuration.CustomParameters.TryGetValue(filterKey, out result))
			{
				return result;
			}
			return TransportTraceConfiguration.emptyList;
		}

		public const string SmtpSenderFilterKey = "SenderSmtp";

		public const string SmtpRecipientFilterKey = "RecipientSmtp";

		public const string UserDNFilterKey = "UserDN";

		public const string SubjectFilterKey = "Subject";

		private static List<string> emptyList = new List<string>();

		private List<string> filteredSubjects;

		private List<string> filteredSenders;

		private List<string> filteredRecipients;

		private List<string> filteredUsers;
	}
}
