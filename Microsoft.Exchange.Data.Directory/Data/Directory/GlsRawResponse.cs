using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.GlobalLocatorService;
using schemas.microsoft.com.O365Filtering.GlobalLocatorService.Data;

namespace Microsoft.Exchange.Data.Directory
{
	internal class GlsRawResponse
	{
		internal string ResourceForest
		{
			get
			{
				return this.resourceForest ?? string.Empty;
			}
		}

		internal string AccountForest
		{
			get
			{
				return this.accountForest ?? string.Empty;
			}
		}

		internal string TenantId
		{
			get
			{
				return this.tenantId ?? string.Empty;
			}
		}

		internal string SmtpNextHopDomain
		{
			get
			{
				return this.smtpNextHopDomain ?? string.Empty;
			}
		}

		internal string TenantFlags
		{
			get
			{
				return this.tenantFlags ?? string.Empty;
			}
		}

		internal string TenantContainerCN
		{
			get
			{
				return this.tenantContainerCN ?? string.Empty;
			}
		}

		internal string DomainName
		{
			get
			{
				return this.domainName ?? string.Empty;
			}
		}

		internal string DomainInUse
		{
			get
			{
				return this.domainInUse ?? string.Empty;
			}
		}

		internal string DomainFlags
		{
			get
			{
				return this.domainFlags ?? string.Empty;
			}
		}

		internal string MSAUserNetID
		{
			get
			{
				return this.msaUserNetID ?? string.Empty;
			}
		}

		internal string MSAUserMemberName
		{
			get
			{
				return this.msaUserMemberName ?? string.Empty;
			}
		}

		internal void Populate(DomainInfo domainInfo)
		{
			if (domainInfo != null && domainInfo.Properties != null)
			{
				this.domainName = domainInfo.DomainName;
				foreach (KeyValuePair<string, string> keyValuePair in domainInfo.Properties)
				{
					if (keyValuePair.Key.Equals(DomainProperty.ExoDomainInUse.Name))
					{
						this.domainInUse = keyValuePair.Value;
					}
					else if (keyValuePair.Key.Equals(DomainProperty.ExoFlags.Name))
					{
						this.domainFlags = keyValuePair.Value;
					}
				}
			}
		}

		internal void Populate(TenantInfo tenantInfo)
		{
			if (tenantInfo != null && tenantInfo.Properties != null)
			{
				this.tenantId = tenantInfo.TenantId.ToString();
				foreach (KeyValuePair<string, string> keyValuePair in tenantInfo.Properties)
				{
					if (keyValuePair.Key.Equals(TenantProperty.EXOResourceForest.Name))
					{
						this.resourceForest = keyValuePair.Value;
					}
					else if (keyValuePair.Key.Equals(TenantProperty.EXOAccountForest.Name))
					{
						this.accountForest = keyValuePair.Value;
					}
					else if (keyValuePair.Key.Equals(TenantProperty.EXOTenantContainerCN.Name))
					{
						this.tenantContainerCN = keyValuePair.Value;
					}
					else if (keyValuePair.Key.Equals(TenantProperty.EXOSmtpNextHopDomain.Name))
					{
						this.smtpNextHopDomain = keyValuePair.Value;
					}
					else if (keyValuePair.Key.Equals(TenantProperty.EXOTenantFlags.Name))
					{
						this.tenantFlags = keyValuePair.Value;
					}
				}
			}
		}

		internal void Populate(UserInfo userInfo)
		{
			if (userInfo != null)
			{
				this.msaUserNetID = userInfo.UserKey;
				this.msaUserMemberName = userInfo.MSAUserName;
			}
		}

		internal void Populate(FindDomainResponse response)
		{
		}

		private string resourceForest;

		private string accountForest;

		private string tenantId;

		private string smtpNextHopDomain;

		private string tenantFlags;

		private string tenantContainerCN;

		private string domainName;

		private string domainInUse;

		private string domainFlags;

		private string msaUserNetID;

		private string msaUserMemberName;
	}
}
