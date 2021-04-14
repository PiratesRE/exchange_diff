using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Globalization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.EdgeSync.Datacenter;

namespace Microsoft.Exchange.EdgeSync.Ehf
{
	internal class EhfADAdapter
	{
		public virtual IEnumerable<ExSearchResultEntry> PagedScan(string baseDN, string query, params string[] attributes)
		{
			if (this.connection == null)
			{
				throw new InvalidOperationException("AD connection has not been initialized");
			}
			return this.connection.PagedScan(baseDN, query, attributes);
		}

		public virtual ExSearchResultEntry ReadObjectEntry(string distinguishedName, bool readDeleted, params string[] attrNames)
		{
			if (this.connection == null)
			{
				throw new InvalidOperationException("AD connection has not been initialized");
			}
			return this.connection.ReadObjectEntry(distinguishedName, readDeleted, attrNames);
		}

		public virtual ExSearchResultEntry ReadObjectEntry(Guid objectGuid, bool readDeleted, params string[] attrNames)
		{
			return this.ReadObjectEntry(EhfADAdapter.CreateDNForObjectGuid(objectGuid), readDeleted, attrNames);
		}

		public virtual void SetAttributeValue(Guid objectGuid, string attrName, object value)
		{
			this.SetAttributeValue(EhfADAdapter.CreateDNForObjectGuid(objectGuid), attrName, value);
		}

		public void SetAttributeValue(string dn, string attrName, object value)
		{
			if (this.connection == null)
			{
				throw new InvalidOperationException("AD connection has not been initialized");
			}
			if (value != null)
			{
				ModifyRequest request = new ModifyRequest(dn, DirectoryAttributeOperation.Replace, attrName, new object[]
				{
					value
				});
				this.connection.SendRequest(request);
				return;
			}
			ModifyRequest request2 = new ModifyRequest(dn, DirectoryAttributeOperation.Replace, attrName, new object[0]);
			this.connection.SendRequest(request2);
		}

		public virtual void SetAttributeValues(Guid objectGuid, IEnumerable<KeyValuePair<string, object>> attributes)
		{
			if (this.connection == null)
			{
				throw new InvalidOperationException("AD connection has not been initialized");
			}
			ModifyRequest modifyRequest = new ModifyRequest();
			modifyRequest.DistinguishedName = EhfADAdapter.CreateDNForObjectGuid(objectGuid);
			foreach (KeyValuePair<string, object> keyValuePair in attributes)
			{
				DirectoryAttributeModification directoryAttributeModification = new DirectoryAttributeModification();
				directoryAttributeModification.Operation = DirectoryAttributeOperation.Replace;
				directoryAttributeModification.Name = keyValuePair.Key;
				if (keyValuePair.Value != null)
				{
					string text = keyValuePair.Value as string;
					if (text != null)
					{
						directoryAttributeModification.Add(text);
					}
					else
					{
						byte[] array = keyValuePair.Value as byte[];
						if (array == null)
						{
							throw new ArgumentException("Value of the attribute should be of type String or Byte[]", "attributesAndValues");
						}
						directoryAttributeModification.Add(array);
					}
				}
				modifyRequest.Modifications.Add(directoryAttributeModification);
			}
			this.connection.SendRequest(modifyRequest);
		}

		public virtual void SetAttributeValues(Guid objectGuid, IEnumerable<KeyValuePair<string, List<byte[]>>> attributes)
		{
			if (this.connection == null)
			{
				throw new InvalidOperationException("AD connection has not been initialized");
			}
			ModifyRequest modifyRequest = new ModifyRequest();
			modifyRequest.DistinguishedName = EhfADAdapter.CreateDNForObjectGuid(objectGuid);
			foreach (KeyValuePair<string, List<byte[]>> keyValuePair in attributes)
			{
				DirectoryAttributeModification directoryAttributeModification = new DirectoryAttributeModification();
				directoryAttributeModification.Operation = DirectoryAttributeOperation.Replace;
				directoryAttributeModification.Name = keyValuePair.Key;
				if (keyValuePair.Value != null && keyValuePair.Value.Count > 0)
				{
					foreach (byte[] value in keyValuePair.Value)
					{
						directoryAttributeModification.Add(value);
					}
				}
				modifyRequest.Modifications.Add(directoryAttributeModification);
			}
			this.connection.SendRequest(modifyRequest);
		}

		public virtual void SetAttributeValues(Guid objectGuid, string attrName, object[] values)
		{
			if (this.connection == null)
			{
				throw new InvalidOperationException("AD connection has not been initialized");
			}
			string distinguishedName = EhfADAdapter.CreateDNForObjectGuid(objectGuid);
			ModifyRequest request;
			if (values != null && values.Length > 0)
			{
				request = new ModifyRequest(distinguishedName, DirectoryAttributeOperation.Replace, attrName, values);
			}
			else
			{
				request = new ModifyRequest(distinguishedName, DirectoryAttributeOperation.Replace, attrName, new object[0]);
			}
			this.connection.SendRequest(request);
		}

		public virtual void SetConnection(Connection connection)
		{
			this.connection = connection;
		}

		public virtual EhfADAdapter GetConfigADAdapter(EdgeSyncDiag diagSession, out Exception exception)
		{
			exception = null;
			IConfigurationSession configSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(this.connection.Fqdn, true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 339, "GetConfigADAdapter", "f:\\15.00.1497\\sources\\dev\\EdgeSync\\src\\EHF\\EhfADAdapter.cs");
			ADObjectId rootId = null;
			PooledLdapConnection pooledLdapConnection = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				pooledLdapConnection = configSession.GetReadConnection(configSession.DomainController, ref rootId);
			}, 3);
			if (adoperationResult.Succeeded)
			{
				EhfADAdapter ehfADAdapter = new EhfADAdapter();
				ehfADAdapter.SetConnection(new Connection(pooledLdapConnection));
				return ehfADAdapter;
			}
			exception = adoperationResult.Exception;
			return null;
		}

		private static string CreateDNForObjectGuid(Guid objectGuid)
		{
			return string.Format(CultureInfo.InvariantCulture, "<GUID={0}>", new object[]
			{
				objectGuid.ToString("D")
			});
		}

		private Connection connection;
	}
}
