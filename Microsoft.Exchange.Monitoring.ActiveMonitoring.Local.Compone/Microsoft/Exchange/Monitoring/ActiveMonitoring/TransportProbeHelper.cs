using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.GlobalLocatorService;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Exchange.Hygiene.Data;
using Microsoft.Exchange.Hygiene.Data.Directory;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Transport;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring
{
	internal class TransportProbeHelper : ProbeDefinitionHelper
	{
		private XmlNode ExtensionNode
		{
			get
			{
				return this.extensionNode;
			}
		}

		private XmlNode MailFrom
		{
			get
			{
				return this.mailFrom;
			}
		}

		private XmlNode MailTo
		{
			get
			{
				return this.mailTo;
			}
		}

		private XmlNode AuthenticationAccount
		{
			get
			{
				return this.authenticationAccount;
			}
		}

		private XmlNode CheckMail
		{
			get
			{
				return this.checkMail;
			}
		}

		private bool PopulateMailFrom
		{
			get
			{
				return this.populateMailFrom;
			}
		}

		private bool PopulateMailTo
		{
			get
			{
				return this.populateMailTo;
			}
		}

		private bool PopulateBoth
		{
			get
			{
				return this.PopulateMailFrom && this.PopulateMailTo;
			}
		}

		private bool PopulateSenderPassword
		{
			get
			{
				return this.populateSenderPassword;
			}
		}

		private bool PopulateRecipientPassword
		{
			get
			{
				return this.populateRecipientPassword;
			}
		}

		private bool PopulateCheckMailCredential
		{
			get
			{
				return this.populateCheckMailCredential;
			}
		}

		private bool PopulateAuthenticationAccount
		{
			get
			{
				return this.populateAuthenticationAccount;
			}
		}

		private TransportProbeHelper.MonitoringMailboxType MonitoringMailbox
		{
			get
			{
				return this.monitoringMailbox;
			}
		}

		private bool CafeMailboxIsNeeded
		{
			get
			{
				return this.MonitoringMailbox == TransportProbeHelper.MonitoringMailboxType.Cafe || this.MonitoringMailbox == TransportProbeHelper.MonitoringMailboxType.Both || (this.MonitoringMailbox == TransportProbeHelper.MonitoringMailboxType.Either && DiscoveryContext.ExchangeInstalledRoles["Cafe"]);
			}
		}

		private bool BackendMailboxIsNeeded
		{
			get
			{
				return this.MonitoringMailbox == TransportProbeHelper.MonitoringMailboxType.Backend || this.MonitoringMailbox == TransportProbeHelper.MonitoringMailboxType.Both || (this.MonitoringMailbox == TransportProbeHelper.MonitoringMailboxType.Either && DiscoveryContext.ExchangeInstalledRoles["Mailbox"]);
			}
		}

		private TransportProbeHelper.SenderRecipientMatchType MatchType
		{
			get
			{
				return this.matchType;
			}
		}

		private string SenderFeatureTag
		{
			get
			{
				return this.senderFeatureTag;
			}
		}

		private string RecipientFeatureTag
		{
			get
			{
				return this.recipientFeatureTag;
			}
		}

		private ProbeOrganizationInfo SenderOrganizationInfo
		{
			get
			{
				if (this.senderOrganizationInfo == null)
				{
					if (string.IsNullOrWhiteSpace(this.SenderFeatureTag))
					{
						throw new XmlException("The required attribute SenderFeatureTag is missing.");
					}
					GlobalConfigSession globalConfigSession = new GlobalConfigSession();
					IEnumerable<ProbeOrganizationInfo> probeOrganizations = globalConfigSession.GetProbeOrganizations(this.SenderFeatureTag);
					if (probeOrganizations == null || probeOrganizations.Count<ProbeOrganizationInfo>() == 0)
					{
						throw new InvalidOperationException("Cannot find any test tenant with sender feature tag=" + this.SenderFeatureTag + ".");
					}
					this.senderOrganizationInfo = this.GetTenantOrg(probeOrganizations);
					if (this.senderOrganizationInfo == null)
					{
						throw new InvalidOperationException("Cannot find a test tenant with sender feature tag=" + this.SenderFeatureTag + ".");
					}
					this.senderTenantID = this.senderOrganizationInfo.ProbeOrganizationId.ObjectGuid;
					if (this.senderTenantID == Guid.Empty)
					{
						string message = "Failed to get TenantId using SenderFeatureTag.";
						WTFDiagnostics.TraceDebug(ExTraceGlobals.GenericHelperTracer, base.TraceContext, message, null, "SenderOrganizationInfo", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\TransportProbeHelper.cs", 453);
						throw new InvalidOperationException(message);
					}
				}
				return this.senderOrganizationInfo;
			}
		}

		private ProbeOrganizationInfo RecipientOrganizationInfo
		{
			get
			{
				if (this.recipientOrganizationInfo == null)
				{
					if (string.IsNullOrWhiteSpace(this.RecipientFeatureTag))
					{
						throw new XmlException("The required attribute RecipientFeatureTag is missing.");
					}
					GlobalConfigSession globalConfigSession = new GlobalConfigSession();
					IEnumerable<ProbeOrganizationInfo> probeOrganizations = globalConfigSession.GetProbeOrganizations(this.RecipientFeatureTag);
					if (probeOrganizations == null || probeOrganizations.Count<ProbeOrganizationInfo>() == 0)
					{
						throw new InvalidOperationException("Cannot find any test tenant with recipient feature tag=" + this.RecipientFeatureTag + ".");
					}
					this.recipientOrganizationInfo = this.GetTenantOrg(probeOrganizations);
					if (this.recipientOrganizationInfo == null)
					{
						throw new InvalidOperationException("Cannot find a test tenant with recipient feature tag=" + this.RecipientFeatureTag + ".");
					}
					this.recipientTenantID = this.recipientOrganizationInfo.ProbeOrganizationId.ObjectGuid;
					if (this.recipientTenantID == Guid.Empty)
					{
						string message = "Failed to get TenantId using RecipientFeatureTag.";
						WTFDiagnostics.TraceDebug(ExTraceGlobals.GenericHelperTracer, base.TraceContext, message, null, "RecipientOrganizationInfo", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\TransportProbeHelper.cs", 500);
						throw new InvalidOperationException(message);
					}
				}
				return this.recipientOrganizationInfo;
			}
		}

		private Guid SenderTenantID
		{
			get
			{
				if (this.senderTenantID == Guid.Empty && this.SenderOrganizationInfo != null)
				{
					this.senderTenantID = this.SenderOrganizationInfo.ProbeOrganizationId.ObjectGuid;
				}
				return this.senderTenantID;
			}
		}

		private Guid RecipientTenantID
		{
			get
			{
				if (this.recipientTenantID == Guid.Empty && this.RecipientOrganizationInfo != null)
				{
					this.recipientTenantID = this.RecipientOrganizationInfo.ProbeOrganizationId.ObjectGuid;
				}
				return this.recipientTenantID;
			}
		}

		private ITenantRecipientSession SenderTenantSession
		{
			get
			{
				if (this.senderTenantSession == null && this.SenderOrganizationInfo != null)
				{
					this.senderTenantSession = DirectorySessionFactory.Default.CreateTenantRecipientSession(null, null, CultureInfo.InvariantCulture.LCID, true, ConsistencyMode.IgnoreInvalid, null, ADSessionSettings.FromExternalDirectoryOrganizationId(this.SenderTenantID), 560, "SenderTenantSession", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\TransportProbeHelper.cs");
					if (this.senderTenantSession == null)
					{
						throw new InvalidOperationException("Cannot get sender TenantConfigurationSession.");
					}
				}
				return this.senderTenantSession;
			}
		}

		private ITenantRecipientSession RecipientTenantSession
		{
			get
			{
				if (this.recipientTenantSession == null && this.RecipientOrganizationInfo != null)
				{
					this.recipientTenantSession = DirectorySessionFactory.Default.CreateTenantRecipientSession(null, null, CultureInfo.InvariantCulture.LCID, true, ConsistencyMode.IgnoreInvalid, null, ADSessionSettings.FromExternalDirectoryOrganizationId(this.RecipientTenantID), 591, "RecipientTenantSession", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\TransportProbeHelper.cs");
					if (this.recipientTenantSession == null)
					{
						throw new InvalidOperationException("Cannot get recipient TenantConfigurationSession.");
					}
				}
				return this.recipientTenantSession;
			}
		}

		private IEnumerable<ADUser> SenderUsers
		{
			get
			{
				if (this.SenderOrganizationInfo != null)
				{
					this.senderUsers = this.SenderTenantSession.FindADUser(null, QueryScope.SubTree, null, null, int.MaxValue);
					if (this.senderUsers == null || this.senderUsers.Count<ADUser>() == 0)
					{
						throw new InvalidOperationException("Cannot get sender users.");
					}
				}
				return this.senderUsers;
			}
		}

		private IEnumerable<ADUser> RecipientUsers
		{
			get
			{
				if (this.RecipientOrganizationInfo != null)
				{
					this.recipientUsers = this.RecipientTenantSession.FindADUser(null, QueryScope.SubTree, null, null, int.MaxValue);
					if (this.recipientUsers == null || this.recipientUsers.Count<ADUser>() == 0)
					{
						throw new InvalidOperationException("Cannot get recipient users.");
					}
				}
				return this.recipientUsers;
			}
		}

		private string SenderPassword
		{
			get
			{
				SecureString loginPassword = this.SenderOrganizationInfo.LoginPassword;
				if (loginPassword == null)
				{
					throw new ArgumentException("The sender organization does not have a password specified.");
				}
				IntPtr ptr = Marshal.SecureStringToBSTR(loginPassword);
				string result;
				try
				{
					result = Marshal.PtrToStringBSTR(ptr);
				}
				finally
				{
					Marshal.FreeBSTR(ptr);
				}
				return result;
			}
		}

		private string RecipientPassword
		{
			get
			{
				SecureString loginPassword = this.RecipientOrganizationInfo.LoginPassword;
				if (loginPassword == null)
				{
					throw new ArgumentException("The recipient organization does not have a password specified.");
				}
				IntPtr ptr = Marshal.SecureStringToBSTR(loginPassword);
				string result;
				try
				{
					result = Marshal.PtrToStringBSTR(ptr);
				}
				finally
				{
					Marshal.FreeBSTR(ptr);
				}
				return result;
			}
		}

		private ICollection<MailboxDatabaseInfo> MailboxCollectionForBackend
		{
			get
			{
				if (this.mailboxCollectionForBackend == null)
				{
					LocalEndpointManager instance = LocalEndpointManager.Instance;
					if (instance.MailboxDatabaseEndpoint == null)
					{
						throw new InvalidOperationException("No mailbox database for Backend found on this server.");
					}
					this.mailboxCollectionForBackend = instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend;
				}
				return this.mailboxCollectionForBackend;
			}
		}

		private ICollection<MailboxDatabaseInfo> MailboxCollectionForCafe
		{
			get
			{
				if (this.mailboxCollectionForCafe == null)
				{
					LocalEndpointManager instance = LocalEndpointManager.Instance;
					if (instance.MailboxDatabaseEndpoint == null)
					{
						throw new InvalidOperationException("No mailbox database for Cafe found on this server.");
					}
					this.mailboxCollectionForCafe = instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForCafe;
				}
				return this.mailboxCollectionForCafe;
			}
		}

		private bool RecipientPasswordIsNeeded
		{
			get
			{
				return this.PopulateRecipientPassword || this.PopulateCheckMailCredential || this.PopulateAuthenticationAccount;
			}
		}

		private bool IsExTest
		{
			get
			{
				NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
				foreach (NetworkInterface networkInterface in allNetworkInterfaces)
				{
					IPInterfaceProperties ipproperties = networkInterface.GetIPProperties();
					if (!string.IsNullOrWhiteSpace(ipproperties.DnsSuffix) && ipproperties.DnsSuffix.ToLower().Contains("extest.microsoft.com"))
					{
						return true;
					}
				}
				return false;
			}
		}

		internal override List<ProbeDefinition> CreateDefinition()
		{
			List<ProbeDefinition> list = new List<ProbeDefinition>();
			if (!this.CheckEssentialInfo())
			{
				list.Add(base.CreateProbeDefinition());
				return list;
			}
			try
			{
				this.xmlNodeCollection = new List<XmlNode>();
				this.PopulateUsers();
				int num = this.xmlNodeCollection.Count;
				foreach (XmlNode xmlNode in this.xmlNodeCollection)
				{
					XmlElement xmlElement = (XmlElement)xmlNode;
					ProbeDefinition probeDefinition = base.CreateProbeDefinition(xmlElement);
					if (num > 1)
					{
						probeDefinition.Name = string.Format("{0}_{1}", probeDefinition.Name, num);
						num--;
					}
					list.Add(probeDefinition);
				}
			}
			catch (TransientDALException ex)
			{
				if (!this.IsExTest)
				{
					throw;
				}
				WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.GenericHelperTracer, base.TraceContext, "This topology does not support feature tags. {0}", ex.ToString(), null, "CreateDefinition", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\TransportProbeHelper.cs", 865);
			}
			return list;
		}

		private void PopulateUsers()
		{
			if (!this.PopulateMailFrom && !this.PopulateMailTo && !this.PopulateAuthenticationAccount)
			{
				this.xmlNodeCollection.Add(this.ExtensionNode);
				return;
			}
			string optionalXmlAttribute = DefinitionHelperBase.GetOptionalXmlAttribute<string>(this.MailFrom, "Select", "all", base.TraceContext);
			int num = (string.Compare(optionalXmlAttribute.ToLower(), "all", true) == 0) ? 0 : ((int)Convert.ChangeType(optionalXmlAttribute, typeof(int)));
			num = ((num < 0) ? 0 : num);
			optionalXmlAttribute = DefinitionHelperBase.GetOptionalXmlAttribute<string>(this.MailTo, "Select", "all", base.TraceContext);
			int num2 = (string.Compare(optionalXmlAttribute.ToLower(), "all", true) == 0) ? 0 : ((int)Convert.ChangeType(optionalXmlAttribute, typeof(int)));
			num2 = ((num2 < 0) ? 0 : num2);
			if (this.MonitoringMailbox != TransportProbeHelper.MonitoringMailboxType.None)
			{
				if (!this.CheckRoles())
				{
					return;
				}
				LocalEndpointManager instance = LocalEndpointManager.Instance;
				if (instance.MailboxDatabaseEndpoint == null)
				{
					WTFDiagnostics.TraceDebug(ExTraceGlobals.GenericHelperTracer, base.TraceContext, "TransportProbeHelper: No mailbox database found on this server", null, "PopulateUsers", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\TransportProbeHelper.cs", 909);
					return;
				}
				if (this.CafeMailboxIsNeeded)
				{
					this.PopulateMailboxes(this.MailboxCollectionForCafe, num, num2);
				}
				if (this.BackendMailboxIsNeeded)
				{
					this.PopulateMailboxes(this.MailboxCollectionForBackend, num, num2);
					return;
				}
			}
			else
			{
				IEnumerable<ADUser> enumerable = null;
				if (this.PopulateMailFrom)
				{
					enumerable = this.SelectUsers(this.SenderUsers, num);
				}
				IEnumerable<ADUser> enumerable2 = null;
				if (this.PopulateMailTo || this.PopulateAuthenticationAccount)
				{
					enumerable2 = this.SelectUsers(this.RecipientUsers, num2);
				}
				if (this.PopulateBoth)
				{
					this.InsertSenderAndRecipient(enumerable, enumerable2);
					return;
				}
				if (this.PopulateMailFrom)
				{
					this.InsertSender(enumerable);
					return;
				}
				if (this.PopulateMailTo)
				{
					this.InsertRecipient(enumerable2);
					return;
				}
				if (this.PopulateAuthenticationAccount)
				{
					this.InsertAuthenticationAccount(enumerable2);
				}
			}
		}

		private bool CheckRoles()
		{
			if (this.MonitoringMailbox == TransportProbeHelper.MonitoringMailboxType.Cafe && !DiscoveryContext.ExchangeInstalledRoles["Cafe"])
			{
				WTFDiagnostics.TraceDebug(ExTraceGlobals.GenericHelperTracer, base.TraceContext, "The Cafe role is not installed on this server.", null, "CheckRoles", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\TransportProbeHelper.cs", 976);
				return false;
			}
			if (this.MonitoringMailbox == TransportProbeHelper.MonitoringMailboxType.Backend && !DiscoveryContext.ExchangeInstalledRoles["Mailbox"])
			{
				WTFDiagnostics.TraceDebug(ExTraceGlobals.GenericHelperTracer, base.TraceContext, "The Mailbox role is not installed on this server.", null, "CheckRoles", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\TransportProbeHelper.cs", 982);
				return false;
			}
			if (this.MonitoringMailbox == TransportProbeHelper.MonitoringMailboxType.Both && (!DiscoveryContext.ExchangeInstalledRoles["Cafe"] || !DiscoveryContext.ExchangeInstalledRoles["Mailbox"]))
			{
				WTFDiagnostics.TraceDebug(ExTraceGlobals.GenericHelperTracer, base.TraceContext, "Either the Cafe or Mailbox role is not installed on this server.", null, "CheckRoles", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\TransportProbeHelper.cs", 989);
				return false;
			}
			if (this.MonitoringMailbox == TransportProbeHelper.MonitoringMailboxType.Either && !DiscoveryContext.ExchangeInstalledRoles["Cafe"] && !DiscoveryContext.ExchangeInstalledRoles["Mailbox"])
			{
				WTFDiagnostics.TraceDebug(ExTraceGlobals.GenericHelperTracer, base.TraceContext, "Neither the Cafe nor the Mailbox role is installed on this server.", null, "CheckRoles", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\TransportProbeHelper.cs", 996);
				return false;
			}
			return true;
		}

		private void PopulateMailboxes(ICollection<MailboxDatabaseInfo> mailboxes, int senderRequested, int recipientRequested)
		{
			IEnumerable<MailboxDatabaseInfo> enumerable = null;
			if (this.PopulateMailFrom)
			{
				enumerable = this.SelectUsers(mailboxes, senderRequested, this.PopulateSenderPassword);
			}
			IEnumerable<MailboxDatabaseInfo> enumerable2 = null;
			if (this.PopulateMailTo || this.PopulateAuthenticationAccount)
			{
				enumerable2 = this.SelectUsers(mailboxes, recipientRequested, this.RecipientPasswordIsNeeded);
			}
			if (this.PopulateBoth)
			{
				this.InsertSenderAndRecipient(enumerable, enumerable2);
				return;
			}
			if (this.PopulateMailFrom)
			{
				this.InsertSender(enumerable);
				return;
			}
			if (this.PopulateMailTo)
			{
				this.InsertRecipient(enumerable2);
				return;
			}
			if (this.PopulateAuthenticationAccount)
			{
				this.InsertAuthenticationAccount(enumerable2);
			}
		}

		private IEnumerable<MailboxDatabaseInfo> SelectUsers(IEnumerable<MailboxDatabaseInfo> mailboxes, int numberRequested, bool populatePassword)
		{
			IEnumerable<MailboxDatabaseInfo> enumerable;
			if (populatePassword)
			{
				enumerable = from m in mailboxes
				where !string.IsNullOrWhiteSpace(m.MonitoringAccount) && !string.IsNullOrWhiteSpace(m.MonitoringAccountDomain) && !string.IsNullOrWhiteSpace(m.MonitoringAccountPassword)
				select m;
			}
			else
			{
				enumerable = from m in mailboxes
				where !string.IsNullOrWhiteSpace(m.MonitoringAccount) && !string.IsNullOrWhiteSpace(m.MonitoringAccountDomain)
				select m;
			}
			if (enumerable == null || enumerable.Count<MailboxDatabaseInfo>() == 0)
			{
				string message = string.Format("Unable to populate users: # of users with email address {0}= 0.", populatePassword ? "and password " : string.Empty);
				WTFDiagnostics.TraceDebug(ExTraceGlobals.GenericHelperTracer, base.TraceContext, message, null, "SelectUsers", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\TransportProbeHelper.cs", 1076);
				throw new InvalidOperationException(message);
			}
			int count;
			int count2;
			this.GetRandomNumbers(enumerable.Count<MailboxDatabaseInfo>(), numberRequested, out count, out count2);
			return enumerable.Skip(count2).Take(count);
		}

		private IEnumerable<ADUser> SelectUsers(IEnumerable<ADUser> users, int numberRequested)
		{
			IEnumerable<ADUser> enumerable = users.Where(delegate(ADUser user)
			{
				SmtpAddress windowsLiveID = user.WindowsLiveID;
				return user.WindowsLiveID.IsValidAddress;
			});
			if (enumerable == null || enumerable.Count<ADUser>() == 0)
			{
				string message = "Unable to populate users: # of users with email address = 0.";
				WTFDiagnostics.TraceDebug(ExTraceGlobals.GenericHelperTracer, base.TraceContext, message, null, "SelectUsers", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\TransportProbeHelper.cs", 1103);
				throw new InvalidOperationException(message);
			}
			int count;
			int count2;
			this.GetRandomNumbers(enumerable.Count<ADUser>(), numberRequested, out count, out count2);
			return enumerable.Skip(count2).Take(count);
		}

		private void InsertSenderAndRecipient(IEnumerable<MailboxDatabaseInfo> senderSelected, IEnumerable<MailboxDatabaseInfo> recipientSelected)
		{
			if (senderSelected == null || recipientSelected == null)
			{
				WTFDiagnostics.TraceDebug(ExTraceGlobals.GenericHelperTracer, base.TraceContext, "Empty sender and/or recipient list", null, "InsertSenderAndRecipient", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\TransportProbeHelper.cs", 1126);
				return;
			}
			if (this.MatchType == TransportProbeHelper.SenderRecipientMatchType.OneToMany)
			{
				using (IEnumerator<MailboxDatabaseInfo> enumerator = senderSelected.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MailboxDatabaseInfo sender = enumerator.Current;
						foreach (MailboxDatabaseInfo recipient in recipientSelected)
						{
							XmlNode item = this.InsertSenderAndRecipient(sender, recipient);
							this.xmlNodeCollection.Add(item);
						}
					}
					return;
				}
			}
			if (this.MatchType == TransportProbeHelper.SenderRecipientMatchType.ToOneself)
			{
				using (IEnumerator<MailboxDatabaseInfo> enumerator3 = senderSelected.GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						MailboxDatabaseInfo mailboxDatabaseInfo = enumerator3.Current;
						XmlNode item2 = this.InsertSenderAndRecipient(mailboxDatabaseInfo, mailboxDatabaseInfo);
						this.xmlNodeCollection.Add(item2);
					}
					return;
				}
			}
			if (senderSelected.Count<MailboxDatabaseInfo>() >= recipientSelected.Count<MailboxDatabaseInfo>())
			{
				int num = 0;
				List<MailboxDatabaseInfo> list = recipientSelected.ToList<MailboxDatabaseInfo>();
				using (IEnumerator<MailboxDatabaseInfo> enumerator4 = senderSelected.GetEnumerator())
				{
					while (enumerator4.MoveNext())
					{
						MailboxDatabaseInfo sender2 = enumerator4.Current;
						XmlNode item3 = this.InsertSenderAndRecipient(sender2, list[num]);
						this.xmlNodeCollection.Add(item3);
						if (++num == list.Count)
						{
							num = 0;
						}
					}
					return;
				}
			}
			int num2 = 0;
			List<MailboxDatabaseInfo> list2 = senderSelected.ToList<MailboxDatabaseInfo>();
			foreach (MailboxDatabaseInfo recipient2 in recipientSelected)
			{
				XmlNode item4 = this.InsertSenderAndRecipient(list2[num2], recipient2);
				this.xmlNodeCollection.Add(item4);
				if (++num2 == list2.Count)
				{
					num2 = 0;
				}
			}
		}

		private void InsertSenderAndRecipient(IEnumerable<ADUser> senderSelected, IEnumerable<ADUser> recipientSelected)
		{
			if (senderSelected == null || recipientSelected == null)
			{
				WTFDiagnostics.TraceDebug(ExTraceGlobals.GenericHelperTracer, base.TraceContext, "Empty sender and/or recipient list", null, "InsertSenderAndRecipient", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\TransportProbeHelper.cs", 1199);
				return;
			}
			if (this.MatchType == TransportProbeHelper.SenderRecipientMatchType.OneToMany)
			{
				using (IEnumerator<ADUser> enumerator = senderSelected.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ADUser sender = enumerator.Current;
						foreach (ADUser recipient in recipientSelected)
						{
							XmlNode item = this.InsertSenderAndRecipient(sender, recipient);
							this.xmlNodeCollection.Add(item);
						}
					}
					return;
				}
			}
			if (this.MatchType == TransportProbeHelper.SenderRecipientMatchType.ToOneself)
			{
				using (IEnumerator<ADUser> enumerator3 = senderSelected.GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						ADUser aduser = enumerator3.Current;
						XmlNode item2 = this.InsertSenderAndRecipient(aduser, aduser);
						this.xmlNodeCollection.Add(item2);
					}
					return;
				}
			}
			if (senderSelected.Count<ADUser>() >= recipientSelected.Count<ADUser>())
			{
				int num = 0;
				List<ADUser> list = recipientSelected.ToList<ADUser>();
				using (IEnumerator<ADUser> enumerator4 = senderSelected.GetEnumerator())
				{
					while (enumerator4.MoveNext())
					{
						ADUser sender2 = enumerator4.Current;
						XmlNode item3 = this.InsertSenderAndRecipient(sender2, list[num]);
						this.xmlNodeCollection.Add(item3);
						if (++num == list.Count)
						{
							num = 0;
						}
					}
					return;
				}
			}
			int num2 = 0;
			List<ADUser> list2 = senderSelected.ToList<ADUser>();
			foreach (ADUser recipient2 in recipientSelected)
			{
				XmlNode item4 = this.InsertSenderAndRecipient(list2[num2], recipient2);
				this.xmlNodeCollection.Add(item4);
				if (++num2 == list2.Count)
				{
					num2 = 0;
				}
			}
		}

		private XmlNode InsertSenderAndRecipient(MailboxDatabaseInfo sender, MailboxDatabaseInfo recipient)
		{
			XmlNode xmlNode = this.ExtensionNode.CloneNode(true);
			string user = sender.MonitoringAccount + "@" + sender.MonitoringAccountDomain;
			string user2 = recipient.MonitoringAccount + "@" + recipient.MonitoringAccountDomain;
			this.InsertSenderUsername(xmlNode, user);
			this.InsertRecipientUsername(xmlNode, user2);
			this.InsertSenderPassword(xmlNode, sender.MonitoringAccountPassword);
			this.InsertRecipientPassword(xmlNode, recipient.MonitoringAccountPassword);
			this.InsertAuthenticationAccount(xmlNode, user2, recipient.MonitoringAccountPassword);
			this.InsertCheckMailCredentials(xmlNode, user2, recipient.MonitoringAccountPassword);
			this.InsertSenderTenantId(xmlNode, sender);
			this.InsertRecipientTenantId(xmlNode, recipient);
			return xmlNode;
		}

		private XmlNode InsertSenderAndRecipient(ADUser sender, ADUser recipient)
		{
			XmlNode xmlNode = this.ExtensionNode.CloneNode(true);
			this.InsertSenderUsername(xmlNode, sender.WindowsLiveID.ToString());
			this.InsertRecipientUsername(xmlNode, recipient.WindowsLiveID.ToString());
			if (this.PopulateSenderPassword)
			{
				this.InsertSenderPassword(xmlNode, this.SenderPassword);
			}
			string passwd = this.RecipientPasswordIsNeeded ? this.RecipientPassword : string.Empty;
			this.InsertRecipientPassword(xmlNode, passwd);
			this.InsertAuthenticationAccount(xmlNode, recipient.WindowsLiveID.ToString(), passwd);
			this.InsertCheckMailCredentials(xmlNode, recipient.WindowsLiveID.ToString(), passwd);
			this.InsertTenantId(xmlNode);
			return xmlNode;
		}

		private void InsertSender(IEnumerable<MailboxDatabaseInfo> selected)
		{
			if (selected == null)
			{
				WTFDiagnostics.TraceDebug(ExTraceGlobals.GenericHelperTracer, base.TraceContext, "Empty sender list", null, "InsertSender", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\TransportProbeHelper.cs", 1337);
				return;
			}
			foreach (MailboxDatabaseInfo mailboxDatabaseInfo in selected)
			{
				XmlNode xmlNode = this.ExtensionNode.CloneNode(true);
				string user = mailboxDatabaseInfo.MonitoringAccount + "@" + mailboxDatabaseInfo.MonitoringAccountDomain;
				this.InsertSenderUsername(xmlNode, user);
				this.InsertSenderPassword(xmlNode, mailboxDatabaseInfo.MonitoringAccountPassword);
				this.InsertSenderTenantId(xmlNode, mailboxDatabaseInfo);
				this.xmlNodeCollection.Add(xmlNode);
			}
		}

		private void InsertSender(IEnumerable<ADUser> selected)
		{
			if (selected == null)
			{
				WTFDiagnostics.TraceDebug(ExTraceGlobals.GenericHelperTracer, base.TraceContext, "Empty sender list", null, "InsertSender", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\TransportProbeHelper.cs", 1367);
				return;
			}
			foreach (ADUser aduser in selected)
			{
				XmlNode xmlNode = this.ExtensionNode.CloneNode(true);
				this.InsertSenderUsername(xmlNode, aduser.WindowsLiveID.ToString());
				if (this.PopulateSenderPassword)
				{
					this.InsertSenderPassword(xmlNode, this.SenderPassword);
				}
				this.InsertTenantId(xmlNode);
				this.xmlNodeCollection.Add(xmlNode);
			}
		}

		private void InsertRecipient(IEnumerable<MailboxDatabaseInfo> selected)
		{
			if (selected == null)
			{
				WTFDiagnostics.TraceDebug(ExTraceGlobals.GenericHelperTracer, base.TraceContext, "Empty recipient list", null, "InsertRecipient", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\TransportProbeHelper.cs", 1398);
				return;
			}
			foreach (MailboxDatabaseInfo mailboxDatabaseInfo in selected)
			{
				XmlNode xmlNode = this.ExtensionNode.CloneNode(true);
				string user = mailboxDatabaseInfo.MonitoringAccount + "@" + mailboxDatabaseInfo.MonitoringAccountDomain;
				this.InsertRecipientUsername(xmlNode, user);
				this.InsertRecipientPassword(xmlNode, mailboxDatabaseInfo.MonitoringAccountPassword);
				this.InsertAuthenticationAccount(xmlNode, user, mailboxDatabaseInfo.MonitoringAccountPassword);
				this.InsertCheckMailCredentials(xmlNode, user, mailboxDatabaseInfo.MonitoringAccountPassword);
				this.InsertRecipientTenantId(xmlNode, mailboxDatabaseInfo);
				this.xmlNodeCollection.Add(xmlNode);
			}
		}

		private void InsertRecipient(IEnumerable<ADUser> selected)
		{
			if (selected == null)
			{
				WTFDiagnostics.TraceDebug(ExTraceGlobals.GenericHelperTracer, base.TraceContext, "Empty recipient list", null, "InsertRecipient", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\TransportProbeHelper.cs", 1430);
				return;
			}
			foreach (ADUser aduser in selected)
			{
				XmlNode xmlNode = this.ExtensionNode.CloneNode(true);
				this.InsertRecipientUsername(xmlNode, aduser.WindowsLiveID.ToString());
				string passwd = this.RecipientPasswordIsNeeded ? this.RecipientPassword : string.Empty;
				this.InsertRecipientPassword(xmlNode, passwd);
				this.InsertAuthenticationAccount(xmlNode, aduser.WindowsLiveID.ToString(), passwd);
				this.InsertCheckMailCredentials(xmlNode, aduser.WindowsLiveID.ToString(), passwd);
				this.InsertTenantId(xmlNode);
				this.xmlNodeCollection.Add(xmlNode);
			}
		}

		private void InsertAuthenticationAccount(IEnumerable<MailboxDatabaseInfo> selected)
		{
			if (selected == null)
			{
				WTFDiagnostics.TraceDebug(ExTraceGlobals.GenericHelperTracer, base.TraceContext, "Empty recipient list", null, "InsertAuthenticationAccount", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\TransportProbeHelper.cs", 1462);
				return;
			}
			foreach (MailboxDatabaseInfo mailboxDatabaseInfo in selected)
			{
				XmlNode xmlNode = this.ExtensionNode.CloneNode(true);
				string user = mailboxDatabaseInfo.MonitoringAccount + "@" + mailboxDatabaseInfo.MonitoringAccountDomain;
				this.InsertAuthenticationAccount(xmlNode, user, mailboxDatabaseInfo.MonitoringAccountPassword);
				this.InsertRecipientTenantId(xmlNode, mailboxDatabaseInfo);
				this.xmlNodeCollection.Add(xmlNode);
			}
		}

		private void InsertAuthenticationAccount(IEnumerable<ADUser> selected)
		{
			if (selected == null)
			{
				WTFDiagnostics.TraceDebug(ExTraceGlobals.GenericHelperTracer, base.TraceContext, "Empty recipient list", null, "InsertAuthenticationAccount", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\TransportProbeHelper.cs", 1489);
				return;
			}
			foreach (ADUser aduser in selected)
			{
				XmlNode xmlNode = this.ExtensionNode.CloneNode(true);
				string passwd = this.RecipientPasswordIsNeeded ? this.RecipientPassword : string.Empty;
				this.InsertAuthenticationAccount(xmlNode, aduser.WindowsLiveID.ToString(), passwd);
				this.InsertTenantId(xmlNode);
				this.xmlNodeCollection.Add(xmlNode);
			}
		}

		private void InsertSenderUsername(XmlNode node, string user)
		{
			if (this.PopulateMailFrom)
			{
				XmlElement xmlElement = (node.SelectSingleNode("WorkContext/SendMail/MailFrom") as XmlElement) ?? (node.SelectSingleNode("WorkContext/MailFrom") as XmlElement);
				if (xmlElement != null)
				{
					string name = "Username";
					if (xmlElement.HasAttribute(name))
					{
						xmlElement.Attributes[name].Value = user;
						return;
					}
					xmlElement.SetAttribute(name, user);
				}
			}
		}

		private void InsertRecipientUsername(XmlNode node, string user)
		{
			if (this.PopulateMailTo)
			{
				XmlElement xmlElement = (node.SelectSingleNode("WorkContext/SendMail/MailTo") as XmlElement) ?? (node.SelectSingleNode("WorkContext/MailTo") as XmlElement);
				if (xmlElement != null)
				{
					string name = "Username";
					if (xmlElement.HasAttribute(name))
					{
						xmlElement.Attributes[name].Value = user;
						return;
					}
					xmlElement.SetAttribute(name, user);
				}
			}
		}

		private void InsertSenderPassword(XmlNode node, string passwd)
		{
			if (this.PopulateSenderPassword)
			{
				XmlElement xmlElement = (node.SelectSingleNode("WorkContext/SendMail/MailFrom") as XmlElement) ?? (node.SelectSingleNode("WorkContext/MailFrom") as XmlElement);
				if (xmlElement != null)
				{
					string name = "Password";
					if (xmlElement.HasAttribute(name))
					{
						xmlElement.Attributes[name].Value = passwd;
						return;
					}
					xmlElement.SetAttribute(name, passwd);
				}
			}
		}

		private void InsertRecipientPassword(XmlNode node, string passwd)
		{
			if (this.PopulateRecipientPassword)
			{
				XmlElement xmlElement = (node.SelectSingleNode("WorkContext/SendMail/MailTo") as XmlElement) ?? (node.SelectSingleNode("WorkContext/MailTo") as XmlElement);
				if (xmlElement != null)
				{
					string name = "Password";
					if (xmlElement.HasAttribute(name))
					{
						xmlElement.Attributes[name].Value = passwd;
						return;
					}
					xmlElement.SetAttribute(name, passwd);
				}
			}
		}

		private void InsertAuthenticationAccount(XmlNode node, string user, string passwd)
		{
			if (this.PopulateAuthenticationAccount)
			{
				XmlElement xmlElement = (node.SelectSingleNode("WorkContext/AuthenticationAccount") as XmlElement) ?? (node.SelectSingleNode("WorkContext/SendMail/AuthenticationAccount") as XmlElement);
				if (xmlElement != null)
				{
					string name = "Username";
					if (xmlElement.HasAttribute(name))
					{
						xmlElement.Attributes[name].Value = user;
					}
					else
					{
						xmlElement.SetAttribute(name, user);
					}
					string name2 = "Password";
					if (xmlElement.HasAttribute(name2))
					{
						xmlElement.Attributes[name2].Value = passwd;
						return;
					}
					xmlElement.SetAttribute(name2, passwd);
				}
			}
		}

		private void InsertCheckMailCredentials(XmlNode node, string user, string passwd)
		{
			if (this.PopulateCheckMailCredential)
			{
				XmlElement xmlElement = node.SelectSingleNode("WorkContext/CheckMail") as XmlElement;
				if (xmlElement != null)
				{
					string name = "Username";
					string name2 = "Password";
					if (xmlElement.HasAttribute(name))
					{
						xmlElement.Attributes[name].Value = user;
					}
					else
					{
						xmlElement.SetAttribute(name, user);
					}
					if (xmlElement.HasAttribute(name2))
					{
						xmlElement.Attributes[name2].Value = passwd;
						return;
					}
					xmlElement.SetAttribute(name2, passwd);
				}
			}
		}

		private void InsertTenantId(XmlNode node)
		{
			XmlElement xmlElement = (node.SelectSingleNode("WorkContext/SendMail") ?? node.SelectSingleNode("WorkContext")) as XmlElement;
			if (!string.IsNullOrWhiteSpace(this.SenderFeatureTag))
			{
				xmlElement.SetAttribute("SenderTenantID", this.SenderTenantID.ToString());
			}
			if (!string.IsNullOrWhiteSpace(this.RecipientFeatureTag))
			{
				xmlElement.SetAttribute("RecipientTenantID", this.RecipientTenantID.ToString());
			}
		}

		private void InsertSenderTenantId(XmlNode node, MailboxDatabaseInfo mailbox)
		{
			if (LocalEndpointManager.IsDataCenter)
			{
				XmlElement xmlElement = (node.SelectSingleNode("WorkContext/SendMail") ?? node.SelectSingleNode("WorkContext")) as XmlElement;
				xmlElement.SetAttribute("SenderTenantId", this.GetTenantId(mailbox));
			}
		}

		private void InsertRecipientTenantId(XmlNode node, MailboxDatabaseInfo mailbox)
		{
			if (LocalEndpointManager.IsDataCenter)
			{
				XmlElement xmlElement = (node.SelectSingleNode("WorkContext/SendMail") ?? node.SelectSingleNode("WorkContext")) as XmlElement;
				xmlElement.SetAttribute("RecipientTenantId", this.GetTenantId(mailbox));
			}
		}

		private string GetTenantId(MailboxDatabaseInfo mailbox)
		{
			Guid empty = Guid.Empty;
			StringBuilder stringBuilder = new StringBuilder();
			if (mailbox.MonitoringAccountOrganizationId != null)
			{
				if (MultiTenantTransport.TryGetExternalOrgId(mailbox.MonitoringAccountOrganizationId, out empty) != ADOperationResult.Success)
				{
					stringBuilder.AppendLine("Attempt to get ExternalOrgId using MonitoringAccountOrganizationId failed.");
				}
			}
			else
			{
				stringBuilder.AppendLine("Mailbox has null MonitoringAccountOrganizationId.");
			}
			if (empty == Guid.Empty)
			{
				stringBuilder.AppendLine("Could not get TenantId using MailboxDatabaseInfo.");
				WTFDiagnostics.TraceDebug(ExTraceGlobals.GenericHelperTracer, base.TraceContext, stringBuilder.ToString(), null, "GetTenantId", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\TransportProbeHelper.cs", 1772);
				throw new InvalidOperationException(stringBuilder.ToString());
			}
			return empty.ToString();
		}

		private void GetRandomNumbers(int total, int max, out int count, out int skip)
		{
			count = max;
			skip = 0;
			if (max == 0 || max >= total)
			{
				count = total;
				return;
			}
			skip = TransportProbeHelper.random.Next(0, total - max + 1);
		}

		private bool IsPopulateUserRequired(XmlNode node)
		{
			return node != null && string.IsNullOrWhiteSpace(node.InnerText) && string.IsNullOrWhiteSpace(DefinitionHelperBase.GetOptionalXmlAttribute<string>(node, "Username", string.Empty, base.TraceContext));
		}

		private bool IsPopulatePasswordRequired(XmlNode node)
		{
			if (node == null)
			{
				return false;
			}
			string text = "Password";
			return ((XmlElement)node).HasAttribute(text) && string.IsNullOrWhiteSpace(DefinitionHelperBase.GetOptionalXmlAttribute<string>(node, text, string.Empty, base.TraceContext));
		}

		private bool CheckEssentialInfo()
		{
			this.extensionNode = base.DefinitionNode.SelectSingleNode("ExtensionAttributes");
			if (this.ExtensionNode == null)
			{
				return false;
			}
			this.mailFrom = (this.ExtensionNode.SelectSingleNode("WorkContext/SendMail/MailFrom") ?? this.ExtensionNode.SelectSingleNode("WorkContext/MailFrom"));
			this.mailTo = (this.ExtensionNode.SelectSingleNode("WorkContext/SendMail/MailTo") ?? this.ExtensionNode.SelectSingleNode("WorkContext/MailTo"));
			this.authenticationAccount = (this.ExtensionNode.SelectSingleNode("WorkContext/AuthenticationAccount") ?? this.ExtensionNode.SelectSingleNode("WorkContext/SendMail/AuthenticationAccount"));
			this.checkMail = this.ExtensionNode.SelectSingleNode("WorkContext/CheckMail");
			this.monitoringMailbox = DefinitionHelperBase.GetOptionalXmlEnumAttribute<TransportProbeHelper.MonitoringMailboxType>(base.DiscoveryContext.ContextNode, "MonitoringMailbox", TransportProbeHelper.MonitoringMailboxType.None, base.TraceContext);
			this.matchType = DefinitionHelperBase.GetOptionalXmlEnumAttribute<TransportProbeHelper.SenderRecipientMatchType>(base.DiscoveryContext.ContextNode, "SenderRecipientMatchType", TransportProbeHelper.SenderRecipientMatchType.ToOneself, base.TraceContext);
			this.senderFeatureTag = DefinitionHelperBase.GetOptionalXmlAttribute<string>(base.DiscoveryContext.ContextNode, "SenderFeatureTag", string.Empty, base.TraceContext);
			this.recipientFeatureTag = DefinitionHelperBase.GetOptionalXmlAttribute<string>(base.DiscoveryContext.ContextNode, "RecipientFeatureTag", string.Empty, base.TraceContext);
			this.selectTenantBasedOnEnv = DefinitionHelperBase.GetOptionalXmlAttribute<bool>(base.DiscoveryContext.ContextNode, "SelectTenantBasedOnEnv", false, base.TraceContext);
			this.populateMailFrom = this.IsPopulateUserRequired(this.MailFrom);
			this.populateMailTo = this.IsPopulateUserRequired(this.MailTo);
			this.populateSenderPassword = (this.PopulateMailFrom && this.IsPopulatePasswordRequired(this.MailFrom));
			this.populateRecipientPassword = (this.PopulateMailTo && this.IsPopulatePasswordRequired(this.MailTo));
			this.populateAuthenticationAccount = this.IsPopulateUserRequired(this.AuthenticationAccount);
			this.populateCheckMailCredential = (this.PopulateMailTo && this.CheckMail != null);
			this.senderTenantSession = null;
			this.senderOrganizationInfo = null;
			this.senderUsers = null;
			this.senderTenantID = Guid.Empty;
			this.recipientTenantSession = null;
			this.recipientOrganizationInfo = null;
			this.recipientUsers = null;
			this.recipientTenantID = Guid.Empty;
			this.mailboxCollectionForBackend = null;
			this.mailboxCollectionForCafe = null;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("TypeName: " + base.DefinitionNode.Attributes["TypeName"]);
			stringBuilder.AppendLine("WorkContext:");
			stringBuilder.AppendLine(this.extensionNode.InnerXml);
			stringBuilder.AppendLine("SenderFeatureTag: " + this.senderFeatureTag);
			stringBuilder.AppendLine("RecipientFeatureTag: " + this.recipientFeatureTag);
			stringBuilder.AppendLine("SelectTenantBasedOnEnv: " + this.selectTenantBasedOnEnv.ToString());
			stringBuilder.AppendLine("MonitoringMailbox: " + this.monitoringMailbox.ToString());
			WTFDiagnostics.TraceDebug(ExTraceGlobals.GenericHelperTracer, base.TraceContext, stringBuilder.ToString(), null, "CheckEssentialInfo", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\TransportProbeHelper.cs", 1896);
			return true;
		}

		private ProbeOrganizationInfo GetTenantOrg(IEnumerable<ProbeOrganizationInfo> probeOrgs)
		{
			if (probeOrgs.Count<ProbeOrganizationInfo>() == 0)
			{
				return null;
			}
			IEnumerable<ProbeOrganizationInfo> source = probeOrgs;
			if (this.selectTenantBasedOnEnv)
			{
				if (FfoLocalEndpointManager.IsForefrontForOfficeDatacenter)
				{
					source = from o in probeOrgs
					where o.CustomerType == CustomerType.FilteringOnly
					select o;
				}
				else if (LocalEndpointManager.IsDataCenter)
				{
					source = from o in probeOrgs
					where o.CustomerType == CustomerType.Hosted
					select o;
				}
				if (source.Count<ProbeOrganizationInfo>() == 0)
				{
					source = probeOrgs;
				}
			}
			int count = TransportProbeHelper.random.Next(0, source.Count<ProbeOrganizationInfo>());
			return source.Skip(count).First<ProbeOrganizationInfo>();
		}

		private static Random random = new Random();

		private XmlNode extensionNode;

		private XmlNode mailFrom;

		private XmlNode mailTo;

		private XmlNode authenticationAccount;

		private XmlNode checkMail;

		private bool populateMailFrom;

		private bool populateMailTo;

		private bool populateAuthenticationAccount;

		private bool populateSenderPassword;

		private bool populateRecipientPassword;

		private bool populateCheckMailCredential;

		private TransportProbeHelper.MonitoringMailboxType monitoringMailbox;

		private TransportProbeHelper.SenderRecipientMatchType matchType;

		private List<XmlNode> xmlNodeCollection;

		private string senderFeatureTag;

		private string recipientFeatureTag;

		private ProbeOrganizationInfo senderOrganizationInfo;

		private ProbeOrganizationInfo recipientOrganizationInfo;

		private bool selectTenantBasedOnEnv;

		private ITenantRecipientSession senderTenantSession;

		private ITenantRecipientSession recipientTenantSession;

		private Guid senderTenantID;

		private Guid recipientTenantID;

		private IEnumerable<ADUser> senderUsers;

		private IEnumerable<ADUser> recipientUsers;

		private ICollection<MailboxDatabaseInfo> mailboxCollectionForBackend;

		private ICollection<MailboxDatabaseInfo> mailboxCollectionForCafe;

		private enum MonitoringMailboxType
		{
			None,
			Backend,
			Cafe,
			Both,
			Either
		}

		private enum SenderRecipientMatchType
		{
			OneToOne,
			OneToMany,
			ToOneself
		}
	}
}
