using System;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AnchorService
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class CacheEntryBase : IDiagnosableObject
	{
		public CacheEntryBase(AnchorContext context, ADUser user)
		{
			this.Context = context;
			this.ObjectId = user.ObjectId;
			this.OrganizationId = (user.OrganizationId ?? OrganizationId.ForestWideOrgId);
			this.UserPrincipalName = user.UserPrincipalName;
			this.LastSyncTime = ExDateTime.MinValue;
			this.ADProvider = new AnchorADProvider(this.Context, this.OrganizationId, null);
		}

		public ADObjectId ObjectId { get; protected set; }

		public OrganizationId OrganizationId { get; protected set; }

		public string UserPrincipalName { get; protected set; }

		public abstract bool IsLocal { get; }

		public abstract bool IsActive { get; }

		public abstract int UniqueEntryCount { get; }

		public ExDateTime LastSyncTime { get; private set; }

		public bool IsStale
		{
			get
			{
				return this.LastSyncTime + this.StalenessInterval < ExDateTime.UtcNow;
			}
		}

		public Exception ServiceException { get; internal set; }

		public IAnchorADProvider ADProvider { get; protected set; }

		string IDiagnosableObject.HashableIdentity
		{
			get
			{
				return this.UserPrincipalName;
			}
		}

		private protected AnchorContext Context { protected get; private set; }

		private TimeSpan StalenessInterval
		{
			get
			{
				TimeSpan config = this.Context.Config.GetConfig<TimeSpan>("ScannerTimeDelay");
				TimeSpan timeSpan = TimeSpan.FromSeconds((double)((int)(this.Context.Config.GetConfig<TimeSpan>("ScannerInitialTimeDelay").TotalSeconds / 2.0)));
				if (!(timeSpan != TimeSpan.Zero) || !(timeSpan < config))
				{
					return config;
				}
				return timeSpan;
			}
		}

		public virtual bool Sync()
		{
			this.LastSyncTime = ExDateTime.UtcNow;
			return true;
		}

		public abstract void Activate();

		public abstract void Deactivate();

		public virtual bool Validate()
		{
			if (this.IsStale && !this.Sync())
			{
				this.Context.Logger.Log(MigrationEventType.Error, "CacheEntry {0} couldn't sync", new object[]
				{
					this
				});
				return false;
			}
			if (!this.IsActive)
			{
				this.Context.Logger.Log(MigrationEventType.Error, "CacheEntry {0} isn't active", new object[]
				{
					this
				});
				return false;
			}
			if (!this.IsLocal)
			{
				this.Context.Logger.Log(MigrationEventType.Error, "CacheEntry {0} doesn't exist on the local server", new object[]
				{
					this.OrganizationId
				});
				return false;
			}
			int config = this.Context.Config.GetConfig<int>("MaximumCacheEntryCountPerOrganization");
			if (config >= 0)
			{
				int uniqueEntryCount = this.UniqueEntryCount;
				if (uniqueEntryCount > config)
				{
					string text = string.Format("max cache entry count {0} for organization {1} exists in {2} multiple locations", config, this.OrganizationId, uniqueEntryCount);
					this.Context.Logger.LogEvent(MigrationEventType.Error, new string[]
					{
						text
					});
					return false;
				}
			}
			return true;
		}

		public string GetDiagnosticComponentName()
		{
			return "CacheEntryBase";
		}

		public XElement GetDiagnosticInfo(string argument)
		{
			XElement xelement = new XElement(this.GetDiagnosticComponentName());
			xelement.Add(new XElement("objectId", this.ObjectId));
			xelement.Add(new XElement("organizationId", this.OrganizationId));
			xelement.Add(new XElement("userPrincipalName", this.UserPrincipalName));
			return xelement;
		}

		public override string ToString()
		{
			return this.UserPrincipalName;
		}
	}
}
