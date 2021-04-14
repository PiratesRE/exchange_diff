using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.DirectoryServices;
using System.Globalization;
using System.Management.Automation;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Monitoring;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Test", "DataCenterDKMAccess", SupportsShouldProcess = true)]
	public sealed class TestDataCenterDKMAccess : Task
	{
		[Parameter(Mandatory = false)]
		public bool MonitoringContext
		{
			get
			{
				return (bool)(base.Fields["MonitoringContext"] ?? false);
			}
			set
			{
				base.Fields["MonitoringContext"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return TestDataCenterDKMAccess.confirmationMessage;
			}
		}

		private IEnumerable<Tuple<string, Dictionary<SecurityIdentifier, ActiveDirectoryRights>>> ExpectedAccessRights
		{
			get
			{
				Dictionary<SecurityIdentifier, ActiveDirectoryRights> dictionary = new Dictionary<SecurityIdentifier, ActiveDirectoryRights>
				{
					{
						(SecurityIdentifier)InitializeDkmDatacenter.ExchangeTrustedSubsystemAccount.Translate(typeof(SecurityIdentifier)),
						ActiveDirectoryRights.ReadControl | ActiveDirectoryRights.WriteOwner | ActiveDirectoryRights.CreateChild | ActiveDirectoryRights.ListChildren | ActiveDirectoryRights.Self | ActiveDirectoryRights.ReadProperty | ActiveDirectoryRights.WriteProperty | ActiveDirectoryRights.ListObject
					},
					{
						(SecurityIdentifier)InitializeDkmDatacenter.ExchangeServersAccount.Translate(typeof(SecurityIdentifier)),
						ActiveDirectoryRights.ReadControl | ActiveDirectoryRights.WriteOwner | ActiveDirectoryRights.CreateChild | ActiveDirectoryRights.ListChildren | ActiveDirectoryRights.Self | ActiveDirectoryRights.ReadProperty | ActiveDirectoryRights.WriteProperty | ActiveDirectoryRights.ListObject
					},
					{
						new SecurityIdentifier(WellKnownSidType.NetworkServiceSid, null),
						ActiveDirectoryRights.ReadControl | ActiveDirectoryRights.WriteOwner | ActiveDirectoryRights.CreateChild | ActiveDirectoryRights.ListChildren | ActiveDirectoryRights.Self | ActiveDirectoryRights.ReadProperty | ActiveDirectoryRights.WriteProperty | ActiveDirectoryRights.ListObject
					},
					{
						(SecurityIdentifier)InitializeDkmDatacenter.DomainAdminsAccount.Translate(typeof(SecurityIdentifier)),
						ActiveDirectoryRights.GenericAll
					},
					{
						new SecurityIdentifier(WellKnownSidType.EnterpriseControllersSid, null),
						ActiveDirectoryRights.GenericAll
					},
					{
						new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null),
						ActiveDirectoryRights.GenericAll
					}
				};
				TestDataCenterDKMAccess.TryAddSidToReadWriteAcl(dictionary, TestDataCenterDKMAccess.ExchangeAllRemoteResourceForestsAccount, out this.earrfNotExistsWarning);
				return new List<Tuple<string, Dictionary<SecurityIdentifier, ActiveDirectoryRights>>>
				{
					Tuple.Create<string, Dictionary<SecurityIdentifier, ActiveDirectoryRights>>("Microsoft Exchange DKM", dictionary),
					Tuple.Create<string, Dictionary<SecurityIdentifier, ActiveDirectoryRights>>("Microsoft Exchange Diagnostics DKM", new Dictionary<SecurityIdentifier, ActiveDirectoryRights>
					{
						{
							(SecurityIdentifier)InitializeDkmDatacenter.EdsServersAccount.Translate(typeof(SecurityIdentifier)),
							ActiveDirectoryRights.ReadControl | ActiveDirectoryRights.WriteOwner | ActiveDirectoryRights.CreateChild | ActiveDirectoryRights.ListChildren | ActiveDirectoryRights.Self | ActiveDirectoryRights.ReadProperty | ActiveDirectoryRights.WriteProperty | ActiveDirectoryRights.ListObject
						},
						{
							(SecurityIdentifier)InitializeDkmDatacenter.DomainAdminsAccount.Translate(typeof(SecurityIdentifier)),
							ActiveDirectoryRights.GenericAll
						},
						{
							new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null),
							ActiveDirectoryRights.GenericAll
						}
					})
				};
			}
		}

		protected override void InternalProcessRecord()
		{
			bool flag = true;
			StringBuilder stringBuilder = new StringBuilder();
			ADDomain addomain = ADForest.GetLocalForest().FindRootDomain(true);
			if (addomain == null)
			{
				flag = false;
				stringBuilder.AppendLine("Failed to read root domain");
			}
			else
			{
				IRootOrganizationRecipientSession session = TestDataCenterDKMAccess.CreateAdSession();
				foreach (Tuple<string, Dictionary<SecurityIdentifier, ActiveDirectoryRights>> tuple in this.ExpectedAccessRights)
				{
					flag &= TestDataCenterDKMAccess.VerifyDkmObjectPermissions(session, tuple.Item1, addomain.Id.ToDNString(), tuple.Item2, stringBuilder);
				}
			}
			if (flag)
			{
				stringBuilder.Append("DKM has correct ACL settings");
			}
			this.ReportDkmAclStatus(flag, stringBuilder);
		}

		private static IRootOrganizationRecipientSession CreateAdSession()
		{
			return DirectorySessionFactory.Default.CreateRootOrgRecipientSession(null, null, CultureInfo.InvariantCulture.LCID, true, ConsistencyMode.PartiallyConsistent, null, ADSessionSettings.FromRootOrgScopeSet(), 238, "CreateAdSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\rms\\TestDataCenterDKMAccess.cs");
		}

		private static bool VerifyDkmObjectPermissions(IRootOrganizationRecipientSession session, string dkmContainerName, string rootDomain, Dictionary<SecurityIdentifier, ActiveDirectoryRights> expectedRights, StringBuilder detailStatus)
		{
			IEnumerable<ADRawEntry> enumerable = TestDataCenterDKMAccess.ReadDkmAdObjects(session, dkmContainerName, rootDomain, detailStatus);
			return enumerable != null && TestDataCenterDKMAccess.CheckPermissionsOnDkmObjects(enumerable, session, expectedRights, detailStatus);
		}

		private static bool CheckPermissionsOnDkmObjects(IEnumerable<ADRawEntry> dkmObjects, IRootOrganizationRecipientSession session, Dictionary<SecurityIdentifier, ActiveDirectoryRights> expectedAccessRights, StringBuilder detailStatus)
		{
			bool result = true;
			foreach (ADRawEntry adrawEntry in dkmObjects)
			{
				RawSecurityDescriptor rawSecurityDescriptor;
				ActiveDirectorySecurity activeDirectorySecurity = PermissionTaskHelper.ReadAdSecurityDescriptor(adrawEntry, session, null, out rawSecurityDescriptor);
				if (activeDirectorySecurity == null)
				{
					result = false;
					detailStatus.AppendFormat("Failed to read security descriptor for DKM object {0}. Examine the ACL settings on DKM objects.\r\n", adrawEntry.Id.DistinguishedName);
				}
				else
				{
					AuthorizationRuleCollection accessRules = activeDirectorySecurity.GetAccessRules(true, true, typeof(SecurityIdentifier));
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.AppendLine(string.Format("Object DN: {0}\r\n", adrawEntry.Id.DistinguishedName));
					bool flag = false;
					Dictionary<SecurityIdentifier, ActiveDirectoryRights> dictionary = new Dictionary<SecurityIdentifier, ActiveDirectoryRights>();
					foreach (object obj in accessRules)
					{
						ActiveDirectoryAccessRule activeDirectoryAccessRule = (ActiveDirectoryAccessRule)obj;
						try
						{
							if (!expectedAccessRights.ContainsKey((SecurityIdentifier)activeDirectoryAccessRule.IdentityReference))
							{
								int num = AuthzAuthorization.CheckGenericPermission((SecurityIdentifier)activeDirectoryAccessRule.IdentityReference, rawSecurityDescriptor, AccessMask.MaximumAllowed);
								if (num != 0)
								{
									stringBuilder.AppendFormat("Unexpected ACE with Identity: {0}, Rights: {1}\r\n\r\n", TestDataCenterDKMAccess.AccountNameFromSid(activeDirectoryAccessRule.IdentityReference.ToString()), (ActiveDirectoryRights)num);
									result = false;
									flag = true;
								}
							}
							else
							{
								dictionary[(SecurityIdentifier)activeDirectoryAccessRule.IdentityReference] = (ActiveDirectoryRights)AuthzAuthorization.CheckGenericPermission((SecurityIdentifier)activeDirectoryAccessRule.IdentityReference, rawSecurityDescriptor, AccessMask.MaximumAllowed);
							}
						}
						catch (Win32Exception ex)
						{
							stringBuilder.AppendFormat("Failed to check ACL for Identity: {0} with Win32Exception {1} and ErrorCode {2}\r\n", TestDataCenterDKMAccess.AccountNameFromSid(activeDirectoryAccessRule.IdentityReference.ToString()), ex.Message, ex.ErrorCode);
							result = false;
							flag = true;
						}
					}
					Dictionary<SecurityIdentifier, ActiveDirectoryRights> dictionary2 = new Dictionary<SecurityIdentifier, ActiveDirectoryRights>(expectedAccessRights);
					foreach (KeyValuePair<SecurityIdentifier, ActiveDirectoryRights> keyValuePair in dictionary)
					{
						if (dictionary2[keyValuePair.Key] != keyValuePair.Value)
						{
							stringBuilder.AppendFormat("Wrong rights in ACE for Identity {0}\r\nExpected Rights: {1}\r\nActual Rights: {2}\r\n\r\n", TestDataCenterDKMAccess.AccountNameFromSid(keyValuePair.Key.ToString()), dictionary2[keyValuePair.Key], keyValuePair.Value);
							result = false;
							flag = true;
						}
						dictionary2.Remove(keyValuePair.Key);
					}
					if (dictionary2.Count > 0)
					{
						foreach (KeyValuePair<SecurityIdentifier, ActiveDirectoryRights> keyValuePair2 in dictionary2)
						{
							stringBuilder.AppendFormat("Missing expected ACE for Identity {0}\r\nExpected Rights: {1}\r\n\r\n", TestDataCenterDKMAccess.AccountNameFromSid(keyValuePair2.Key.ToString()), keyValuePair2.Value);
							result = false;
							flag = true;
						}
					}
					if (flag)
					{
						detailStatus.AppendLine(stringBuilder.ToString());
					}
				}
			}
			return result;
		}

		private static IEnumerable<ADRawEntry> ReadDkmAdObjects(IRootOrganizationRecipientSession session, string dkmContainerName, string rootDomain, StringBuilder detailStatus)
		{
			string dkmContainerDN = TestDataCenterDKMAccess.CreateDkmContainerDN(dkmContainerName, rootDomain);
			ADRawEntry[] dkmObjects = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				dkmObjects = session.Find(new ADObjectId(dkmContainerDN), QueryScope.SubTree, new CustomLdapFilter("(objectClass=contact)"), null, -1, new ADPropertyDefinition[]
				{
					ADObjectSchema.Name
				});
			});
			if (!adoperationResult.Succeeded)
			{
				detailStatus.AppendFormat("Failed to read DKM objects under DN {0} with exception {1}", dkmContainerDN, (adoperationResult.Exception == null) ? "N/A" : adoperationResult.Exception.Message);
				return null;
			}
			if (dkmObjects.Length == 0)
			{
				detailStatus.AppendFormat("Failed to find any DKM objects under DN {0}. Examine the ACL settings on DKM objects to ensure the Exchange Servers group is allowed.", dkmContainerDN);
				return null;
			}
			return dkmObjects;
		}

		private static string AccountNameFromSid(string sid)
		{
			try
			{
				return new SecurityIdentifier(sid).Translate(typeof(NTAccount)).ToString();
			}
			catch (IdentityNotMappedException)
			{
			}
			catch (SystemException)
			{
			}
			return sid;
		}

		private static string CreateDkmContainerDN(string dkmContainerName, string rootDomain)
		{
			return string.Format("CN={0},{1},{2},{3}", new object[]
			{
				dkmContainerName,
				"CN=Distributed KeyMan",
				"CN=Microsoft,CN=Program Data",
				rootDomain
			});
		}

		private static bool TryAddSidToReadWriteAcl(Dictionary<SecurityIdentifier, ActiveDirectoryRights> acl, NTAccount account, out string errorMessage)
		{
			errorMessage = string.Empty;
			try
			{
				SecurityIdentifier key = (SecurityIdentifier)account.Translate(typeof(SecurityIdentifier));
				acl.Add(key, ActiveDirectoryRights.ReadControl | ActiveDirectoryRights.WriteOwner | ActiveDirectoryRights.CreateChild | ActiveDirectoryRights.ListChildren | ActiveDirectoryRights.Self | ActiveDirectoryRights.ReadProperty | ActiveDirectoryRights.WriteProperty | ActiveDirectoryRights.ListObject);
			}
			catch (IdentityNotMappedException ex)
			{
				errorMessage = string.Format("\r\nCannot find account {0} to check ACL. Error: {1}\r\n", account.ToString(), ex.Message);
				return false;
			}
			catch (SystemException ex2)
			{
				errorMessage = string.Format("\r\nCannot check ACL for account {0}. Error: {1}\r\n", account.ToString(), ex2.Message);
				return false;
			}
			return true;
		}

		private void ReportDkmAclStatus(bool aclStatusCorrect, StringBuilder detailStatus)
		{
			if (!string.IsNullOrEmpty(this.earrfNotExistsWarning))
			{
				detailStatus.Append(this.earrfNotExistsWarning);
			}
			if (this.MonitoringContext)
			{
				MonitoringEvent item = new MonitoringEvent("MSExchange Monitoring DataCenterDKMAccess", aclStatusCorrect ? 1000 : 1001, aclStatusCorrect ? EventTypeEnumeration.Information : EventTypeEnumeration.Error, detailStatus.ToString());
				this.monitoringData.Events.Add(item);
				base.WriteObject(this.monitoringData);
				return;
			}
			TestDataCenterDKMAccessResult sendToPipeline = new TestDataCenterDKMAccessResult(aclStatusCorrect, detailStatus.ToString());
			base.WriteObject(sendToPipeline);
		}

		public const int Success = 1000;

		public const int Failed = 1001;

		private const ActiveDirectoryRights ReadWriteDkmKeysRights = ActiveDirectoryRights.ReadControl | ActiveDirectoryRights.WriteOwner | ActiveDirectoryRights.CreateChild | ActiveDirectoryRights.ListChildren | ActiveDirectoryRights.Self | ActiveDirectoryRights.ReadProperty | ActiveDirectoryRights.WriteProperty | ActiveDirectoryRights.ListObject;

		private const string CmdletNoun = "DataCenterDKMAccess";

		private const string CmdletMonitoringEventSource = "MSExchange Monitoring DataCenterDKMAccess";

		private static readonly LocalizedString confirmationMessage = new LocalizedString("Testing Datacenter DKM Access");

		private static readonly NTAccount ExchangeAllRemoteResourceForestsAccount = new NTAccount("Exchange All Remote Resource Forests");

		private readonly MonitoringData monitoringData = new MonitoringData();

		private string earrfNotExistsWarning;
	}
}
