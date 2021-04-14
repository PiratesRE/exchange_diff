using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	[Serializable]
	internal class TenantInboundConnectorBucket : ConfigurablePropertyBag, ISerializable
	{
		public TenantInboundConnectorBucket()
		{
			this.TenantInboundConnectors = new Dictionary<string, TenantInboundConnector>(StringComparer.InvariantCultureIgnoreCase);
		}

		public TenantInboundConnectorBucket(string key)
		{
			this.BucketKey = key;
			this.TenantInboundConnectors = new Dictionary<string, TenantInboundConnector>(StringComparer.InvariantCultureIgnoreCase);
		}

		public TenantInboundConnectorBucket(SerializationInfo info, StreamingContext context)
		{
			this.BucketKey = (string)info.GetValue(TenantInboundConnectorBucket.BucketKeyProp.Name, typeof(string));
			this.TenantInboundConnectors = (Dictionary<string, TenantInboundConnector>)info.GetValue(TenantInboundConnectorBucket.TenantInboundConnectorsProp.Name, typeof(Dictionary<string, TenantInboundConnector>));
			this.WhenChangedUTC = (DateTime?)info.GetValue(TenantInboundConnectorBucket.WhenChangedProp.Name, typeof(DateTime?));
		}

		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this.BucketKey);
			}
		}

		public string BucketKey
		{
			get
			{
				return this[TenantInboundConnectorBucket.BucketKeyProp] as string;
			}
			set
			{
				this[TenantInboundConnectorBucket.BucketKeyProp] = value;
			}
		}

		public Dictionary<string, TenantInboundConnector> TenantInboundConnectors
		{
			get
			{
				return this[TenantInboundConnectorBucket.TenantInboundConnectorsProp] as Dictionary<string, TenantInboundConnector>;
			}
			set
			{
				this[TenantInboundConnectorBucket.TenantInboundConnectorsProp] = value;
			}
		}

		public DateTime? WhenChangedUTC
		{
			get
			{
				return (DateTime?)this[TenantInboundConnectorBucket.WhenChangedProp];
			}
			set
			{
				this[TenantInboundConnectorBucket.WhenChangedProp] = value;
			}
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue(TenantInboundConnectorBucket.BucketKeyProp.Name, this.BucketKey);
			info.AddValue(TenantInboundConnectorBucket.TenantInboundConnectorsProp.Name, this.TenantInboundConnectors);
			info.AddValue(TenantInboundConnectorBucket.WhenChangedProp.Name, this.WhenChangedUTC);
		}

		public static IEnumerable<string> BucketizeConnector(TenantInboundConnector connector)
		{
			if (connector == null)
			{
				throw new ArgumentNullException("connector");
			}
			if (connector.SenderIPAddresses != null)
			{
				foreach (string ipKey in TenantInboundConnectorBucket.FindAllVariationsOfKeysBasedOnSenderIPAddresses(connector.SenderIPAddresses))
				{
					yield return ipKey;
				}
			}
			if (connector.TlsSenderCertificateName != null)
			{
				SmtpDomainWithSubdomains domain = connector.TlsSenderCertificateName.TlsCertificateName as SmtpDomainWithSubdomains;
				if (domain != null)
				{
					yield return domain.Address;
				}
				else
				{
					yield return connector.TlsSenderCertificateName.ToString();
				}
			}
			yield break;
		}

		internal static string GetPrimaryKeyForTenantInboundConnector(TenantInboundConnector connector)
		{
			return string.Format("{0}:{1}", connector.OrganizationalUnitRoot.ObjectGuid, connector.Name.ToLower());
		}

		internal void AddEntriesToBucket(IEnumerable<TenantInboundConnector> entriesToAdd)
		{
			foreach (TenantInboundConnector tenantInboundConnector in entriesToAdd)
			{
				string primaryKeyForTenantInboundConnector = TenantInboundConnectorBucket.GetPrimaryKeyForTenantInboundConnector(tenantInboundConnector);
				this.TenantInboundConnectors[primaryKeyForTenantInboundConnector] = tenantInboundConnector;
				this.UpdateWhenChanged(tenantInboundConnector);
			}
		}

		internal void RemoveEntriesFromBucket(IEnumerable<TenantInboundConnector> entriesToRemove)
		{
			foreach (TenantInboundConnector connector in entriesToRemove)
			{
				string primaryKeyForTenantInboundConnector = TenantInboundConnectorBucket.GetPrimaryKeyForTenantInboundConnector(connector);
				this.TenantInboundConnectors.Remove(primaryKeyForTenantInboundConnector);
				this.UpdateWhenChanged(connector);
			}
		}

		private void UpdateWhenChanged(TenantInboundConnector connector)
		{
			if (this.WhenChangedUTC == null || (connector.WhenChangedUTC != null && this.WhenChangedUTC < connector.WhenChangedUTC))
			{
				this.WhenChangedUTC = connector.WhenChangedUTC;
			}
		}

		internal static IEnumerable<string> FindAllVariationsOfKeysBasedOnSenderIPAddresses(IEnumerable<IPRange> ipRanges)
		{
			return ipRanges.SelectMany((IPRange ip) => DalHelper.GetIPsFromIPRange(ip));
		}

		private static readonly HygienePropertyDefinition BucketKeyProp = new HygienePropertyDefinition("BucketKey", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		private static readonly HygienePropertyDefinition TenantInboundConnectorsProp = new HygienePropertyDefinition("TenantInboundConnectors", typeof(IDictionary<string, TenantInboundConnector>));

		private static readonly HygienePropertyDefinition WhenChangedProp = DalHelper.WhenChangedProp;
	}
}
