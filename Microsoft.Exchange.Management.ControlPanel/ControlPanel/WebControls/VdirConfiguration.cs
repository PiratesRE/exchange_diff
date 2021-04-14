using System;
using System.Collections.Generic;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	internal class VdirConfiguration
	{
		private VdirConfiguration(Guid vDirObjectId)
		{
			this.vDirObjectId = vDirObjectId;
			this.expireTime = DateTime.MinValue;
		}

		internal static VdirConfiguration Instance
		{
			get
			{
				VdirConfiguration vdirConfiguration;
				try
				{
					VdirConfiguration.rwLock.AcquireReaderLock(-1);
					Guid vdirObjectGuid = VdirConfiguration.GetVdirObjectGuid();
					if (!VdirConfiguration.instances.ContainsKey(vdirObjectGuid))
					{
						LockCookie lockCookie = VdirConfiguration.rwLock.UpgradeToWriterLock(-1);
						try
						{
							if (!VdirConfiguration.instances.ContainsKey(vdirObjectGuid))
							{
								VdirConfiguration.instances.Add(vdirObjectGuid, new VdirConfiguration(vdirObjectGuid));
							}
						}
						finally
						{
							VdirConfiguration.rwLock.DowngradeFromWriterLock(ref lockCookie);
						}
					}
					vdirConfiguration = VdirConfiguration.instances[vdirObjectGuid];
				}
				finally
				{
					try
					{
						VdirConfiguration.rwLock.ReleaseReaderLock();
					}
					catch (ApplicationException)
					{
					}
				}
				if (vdirConfiguration.IsExpired)
				{
					lock (vdirConfiguration.snycObj)
					{
						vdirConfiguration.Renew();
					}
				}
				return vdirConfiguration;
			}
		}

		private static Guid GetVdirObjectGuid()
		{
			Guid empty = Guid.Empty;
			if (HttpContext.Current != null)
			{
				string text = HttpContext.Current.Request.Headers["X-vDirObjectId"];
				if (!string.IsNullOrEmpty(text))
				{
					Guid.TryParse(text, out empty);
				}
			}
			return empty;
		}

		private bool IsExpired
		{
			get
			{
				return DateTime.UtcNow > this.expireTime;
			}
		}

		internal bool WindowsAuthenticationEnabled
		{
			get
			{
				return this.IsAuthenticationMethodEnabled(AuthenticationMethodFlags.WindowsIntegrated);
			}
		}

		internal bool BasicAuthenticationEnabled
		{
			get
			{
				return this.IsAuthenticationMethodEnabled(AuthenticationMethodFlags.Basic);
			}
		}

		internal bool DigestAuthenticationEnabled
		{
			get
			{
				return this.IsAuthenticationMethodEnabled(AuthenticationMethodFlags.Digest);
			}
		}

		internal bool FormBasedAuthenticationEnabled
		{
			get
			{
				return this.IsAuthenticationMethodEnabled(AuthenticationMethodFlags.Fba);
			}
		}

		internal bool LiveIdAuthenticationEnabled
		{
			get
			{
				return this.IsAuthenticationMethodEnabled(AuthenticationMethodFlags.LiveIdFba);
			}
		}

		internal bool AdfsAuthenticationEnabled
		{
			get
			{
				return this.IsAuthenticationMethodEnabled(AuthenticationMethodFlags.Adfs);
			}
		}

		internal bool IsAuthenticationMethodEnabled(AuthenticationMethodFlags flag)
		{
			return (this.authenticationMethods & flag) > AuthenticationMethodFlags.None;
		}

		private void Renew()
		{
			this.expireTime = DateTime.UtcNow.Add(VdirConfiguration.expirationPeriod);
			ITopologyConfigurationSession session = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 222, "Renew", "f:\\15.00.1497\\sources\\dev\\admin\\src\\ecp\\WebControls\\VdirConfiguration.cs");
			ADRawEntry virtualDirectoryObject = Utility.GetVirtualDirectoryObject(this.vDirObjectId, session, this.propertyDefinitions);
			if (virtualDirectoryObject != null)
			{
				this.authenticationMethods = (AuthenticationMethodFlags)virtualDirectoryObject[ADVirtualDirectorySchema.InternalAuthenticationMethodFlags];
			}
		}

		private const string VDirObjectIdHeaderName = "X-vDirObjectId";

		private readonly PropertyDefinition[] propertyDefinitions = new PropertyDefinition[]
		{
			ADVirtualDirectorySchema.InternalAuthenticationMethodFlags
		};

		private static Dictionary<Guid, VdirConfiguration> instances = new Dictionary<Guid, VdirConfiguration>();

		private static ReaderWriterLock rwLock = new ReaderWriterLock();

		private static TimeSpan expirationPeriod = new TimeSpan(0, 3, 0, 0);

		private readonly Guid vDirObjectId;

		private DateTime expireTime;

		private object snycObj = new object();

		private AuthenticationMethodFlags authenticationMethods;
	}
}
