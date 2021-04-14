using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Transport.RecipientAPI;

namespace Microsoft.Exchange.EdgeSync.Validation
{
	internal class RecipientValidator : ConfigValidator
	{
		public RecipientValidator(ReplicationTopology topology) : base(topology, "Recipients")
		{
			base.ConfigDirectoryPath = string.Empty;
			base.LdapQuery = Schema.Query.QueryAllSmtpRecipients;
			this.orgConfigRoot = base.OrgAdRootPath;
			base.OrgAdRootPath = DistinguishedName.RemoveLeafRelativeDistinguishedNames(base.Topology.LocalHub.DistinguishedName, 8);
			this.compareAttributes = this.PayloadAttributes;
		}

		protected override IDirectorySession DataSession
		{
			get
			{
				return base.Topology.RecipientSession;
			}
		}

		protected override string[] PayloadAttributes
		{
			get
			{
				if (this.payloadAttributesArray == null)
				{
					List<string> list = new List<string>(RecipientSchema.AttributeNames.Length - 1);
					foreach (string text in RecipientSchema.AttributeNames)
					{
						if (!text.Equals("msExchVersion", StringComparison.OrdinalIgnoreCase))
						{
							list.Add(text);
						}
					}
					this.payloadAttributesArray = list.ToArray();
				}
				return this.payloadAttributesArray;
			}
		}

		protected override string[] ReadAttributes
		{
			get
			{
				return RecipientValidator.readAttributes;
			}
		}

		protected override string ADSearchPath
		{
			get
			{
				return string.Empty;
			}
		}

		protected override string ADAMSearchPath
		{
			get
			{
				return "CN=Recipients,OU=MSExchangeGateway";
			}
		}

		protected override string ADAMLdapQuery
		{
			get
			{
				return "(proxyAddresses=*)";
			}
		}

		public override EdgeConfigStatus Validate(EdgeConnectionInfo subscription)
		{
			EdgeConfigStatus edgeConfigStatus = base.Validate(subscription);
			this.ValidateExchangeServerRecipient(edgeConfigStatus);
			return edgeConfigStatus;
		}

		public EdgeConfigStatus ValidateOneRecipient(EdgeConnectionInfo subscription, string proxyAddressToVerify)
		{
			ADObjectId adobjectId = null;
			string[] attributeNames = RecipientSchema.AttributeNames;
			string[] array = RecipientValidator.readAttributes;
			int num = attributeNames.Length;
			int num2 = array.Length;
			string[] array2 = new string[num + num2];
			attributeNames.CopyTo(array2, 0);
			array.CopyTo(array2, num);
			Connection connection = null;
			EdgeConfigStatus result;
			try
			{
				connection = new Connection(this.DataSession.GetReadConnection(null, ref adobjectId));
				List<ExSearchResultEntry> list = new List<ExSearchResultEntry>();
				foreach (ExSearchResultEntry item in connection.PagedScan(null, "(proxyAddresses=" + proxyAddressToVerify + ")", SearchScope.Subtree, array2))
				{
					list.Add(item);
				}
				if (list.Count == 0)
				{
					result = new RecipientConfigStatus(SyncStatus.NotSynchronized, "Recipient doesn't exist in source Active Directory");
				}
				else if (list.Count > 1)
				{
					RecipientConfigStatus recipientConfigStatus = new RecipientConfigStatus(SyncStatus.NotSynchronized, "More than one recipient found in source Active Directory and may cause NDR on Edge server. RecipientStatus.ConflictObjects contains relevant entries.");
					foreach (ExSearchResultEntry exSearchResultEntry in list)
					{
						recipientConfigStatus.ConflictObjects.Add(new ADObjectId(exSearchResultEntry.DistinguishedName));
					}
					result = recipientConfigStatus;
				}
				else
				{
					string hashedFormWithPrefix = this.proxyAddressHasher.GetHashedFormWithPrefix(proxyAddressToVerify.Substring(5));
					List<ExSearchResultEntry> list2 = new List<ExSearchResultEntry>();
					foreach (ExSearchResultEntry item2 in subscription.EdgeConnection.PagedScan(this.ADAMSearchPath, "(proxyAddresses=" + hashedFormWithPrefix + ")", SearchScope.Subtree, array2))
					{
						list2.Add(item2);
					}
					if (list2.Count > 1)
					{
						RecipientConfigStatus recipientConfigStatus2 = new RecipientConfigStatus(SyncStatus.NotSynchronized, "More than one recipient found in target Edge Server and may cause NDR on Edge server. RecipientStatus.ConflictObjects contains relevant entries.");
						foreach (ExSearchResultEntry exSearchResultEntry2 in list2)
						{
							recipientConfigStatus2.ConflictObjects.Add(new ADObjectId(exSearchResultEntry2.DistinguishedName));
						}
						result = recipientConfigStatus2;
					}
					else
					{
						ExSearchResultEntry exSearchResultEntry3 = list[0];
						DirectoryAttribute directoryAttribute = exSearchResultEntry3.Attributes["objectGUID"];
						Guid guid = new Guid((byte[])directoryAttribute.GetValues(typeof(byte[]))[0]);
						string absolutePath = "cn=" + guid.ToString() + "," + this.ADAMSearchPath;
						ExSearchResultEntry exSearchResultEntry4 = subscription.EdgeConnection.ReadObjectEntry(absolutePath, array2);
						if (exSearchResultEntry4 == null)
						{
							result = new RecipientConfigStatus(SyncStatus.NotSynchronized, "Recipient doesn't exist in target Edge Server and may cause NDR on Edge server")
							{
								OrgOnlyObjects = 
								{
									new ADObjectId(exSearchResultEntry3.DistinguishedName)
								}
							};
						}
						else if (!this.CompareAttributes(exSearchResultEntry4, exSearchResultEntry3, attributeNames))
						{
							result = new RecipientConfigStatus(SyncStatus.NotSynchronized, "Recipient exists in target Edge Server but attributes are not synchronized")
							{
								ConflictObjects = 
								{
									new ADObjectId(exSearchResultEntry3.DistinguishedName)
								}
							};
						}
						else
						{
							DirectoryAttribute directoryAttribute2 = null;
							bool flag = false;
							if (exSearchResultEntry4.Attributes.TryGetValue("msExchRequireAuthToSendTo", out directoryAttribute2) && directoryAttribute2 != null && directoryAttribute2.Count > 0 && bool.TryParse((string)directoryAttribute2[0], out flag) && flag)
							{
								result = new RecipientConfigStatus(SyncStatus.Synchronized, "Recipient requires sender authentication and this may cause NDR on Edge server. RecipientStatus.ConflictObjects contains the recipient object in source Active Directory")
								{
									ConflictObjects = 
									{
										new ADObjectId(exSearchResultEntry3.DistinguishedName)
									}
								};
							}
							else
							{
								result = new RecipientConfigStatus(SyncStatus.Synchronized, null);
							}
						}
					}
				}
			}
			catch (ExDirectoryException ex)
			{
				result = new RecipientConfigStatus(SyncStatus.DirectoryError, ex.Message);
			}
			catch (ADTransientException ex2)
			{
				result = new RecipientConfigStatus(SyncStatus.DirectoryError, ex2.Message);
			}
			catch (ADOperationException ex3)
			{
				result = new RecipientConfigStatus(SyncStatus.DirectoryError, ex3.Message);
			}
			finally
			{
				if (connection != null)
				{
					connection.Dispose();
					connection = null;
				}
			}
			return result;
		}

		protected override string GetADRelativePath(ExSearchResultEntry searchEntry)
		{
			Guid guid = new Guid((byte[])searchEntry.Attributes["objectGUID"][0]);
			return string.Format("cn={0}", guid.ToString().ToLower());
		}

		protected override bool CompareAttributes(ExSearchResultEntry edgeEntry, ExSearchResultEntry hubEntry, string[] copyAttributes)
		{
			this.HashProxyAddresses(hubEntry);
			return base.CompareAttributes(edgeEntry, hubEntry, this.compareAttributes);
		}

		private void HashProxyAddresses(ExSearchResultEntry sourceEntry)
		{
			DirectoryAttribute directoryAttribute = sourceEntry.Attributes["proxyAddresses"];
			List<string> list = new List<string>();
			for (int i = 0; i < directoryAttribute.Count; i++)
			{
				string text = directoryAttribute[i] as string;
				if (text.StartsWith("sh:", StringComparison.OrdinalIgnoreCase))
				{
					return;
				}
				if (text.StartsWith("smtp:", StringComparison.OrdinalIgnoreCase))
				{
					string smtpAddress = text.Substring(5);
					string hashedFormWithPrefix = this.proxyAddressHasher.GetHashedFormWithPrefix(smtpAddress);
					list.Add(hashedFormWithPrefix);
				}
			}
			DirectoryAttribute value = new DirectoryAttribute(directoryAttribute.Name, list.ToArray());
			sourceEntry.Attributes["proxyAddresses"] = value;
		}

		private void ValidateExchangeServerRecipient(EdgeConfigStatus status)
		{
			Connection connection = null;
			if (status.SyncStatus != SyncStatus.NotSynchronized && status.SyncStatus != SyncStatus.Synchronized)
			{
				return;
			}
			try
			{
				ADObjectId adobjectId = null;
				string absolutePath = "CN=" + ADMicrosoftExchangeRecipient.DefaultName + ",CN=Transport Settings," + this.orgConfigRoot;
				string absolutePath2 = "CN=" + ADMicrosoftExchangeRecipient.DefaultName + ",CN=Transport Settings," + base.AdamRootPath;
				connection = new Connection(this.DataSession.GetReadConnection(null, ref adobjectId));
				ExSearchResultEntry exSearchResultEntry = connection.ReadObjectEntry(absolutePath, Schema.ExchangeRecipient.PayloadAttributes);
				ExSearchResultEntry exSearchResultEntry2 = base.CurrentEdgeConnection.EdgeConnection.ReadObjectEntry(absolutePath2, Schema.ExchangeRecipient.PayloadAttributes);
				if (exSearchResultEntry != exSearchResultEntry2)
				{
					if (exSearchResultEntry == null)
					{
						status.SyncStatus = SyncStatus.NotSynchronized;
						if (base.MaxReportedLength.IsUnlimited || (ulong)base.MaxReportedLength.Value > (ulong)((long)status.EdgeOnlyObjects.Count))
						{
							status.EdgeOnlyObjects.Add(new ADObjectId(exSearchResultEntry2.DistinguishedName));
						}
					}
					else if (exSearchResultEntry2 == null)
					{
						status.SyncStatus = SyncStatus.NotSynchronized;
						if (base.MaxReportedLength.IsUnlimited || (ulong)base.MaxReportedLength.Value > (ulong)((long)status.OrgOnlyObjects.Count))
						{
							status.OrgOnlyObjects.Add(new ADObjectId(exSearchResultEntry.DistinguishedName));
						}
					}
					else
					{
						if (!base.CompareAttributes(exSearchResultEntry2, exSearchResultEntry, Schema.ExchangeRecipient.PayloadAttributes))
						{
							status.SyncStatus = SyncStatus.NotSynchronized;
							if (base.MaxReportedLength.IsUnlimited || (ulong)base.MaxReportedLength.Value > (ulong)((long)status.ConflictObjects.Count))
							{
								status.ConflictObjects.Add(new ADObjectId(exSearchResultEntry.DistinguishedName));
							}
						}
						if (status.SyncStatus == SyncStatus.Synchronized)
						{
							status.SynchronizedObjects += 1U;
						}
					}
				}
			}
			catch (ExDirectoryException)
			{
				status.SyncStatus = SyncStatus.DirectoryError;
			}
			finally
			{
				if (connection != null)
				{
					connection.Dispose();
					connection = null;
				}
			}
		}

		private static readonly string[] readAttributes = new string[]
		{
			"objectGUID"
		};

		private readonly ProxyAddressHasher proxyAddressHasher = new ProxyAddressHasher();

		private string[] compareAttributes;

		private string orgConfigRoot;

		private string[] payloadAttributesArray;
	}
}
