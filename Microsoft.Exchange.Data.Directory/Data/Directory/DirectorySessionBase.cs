using System;
using System.Globalization;
using System.Net;
using System.Security.AccessControl;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Data.Directory
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal abstract class DirectorySessionBase
	{
		public abstract DirectoryBackendType DirectoryBackendType { get; }

		protected bool IsDirectoryBackendAD
		{
			get
			{
				return (byte)(this.DirectoryBackendType & DirectoryBackendType.AD) == 1;
			}
		}

		protected bool IsDirectoryBackendMServ
		{
			get
			{
				return (byte)(this.DirectoryBackendType & DirectoryBackendType.MServ) == 2;
			}
		}

		protected bool IsDirectoryBackendMbx
		{
			get
			{
				return (byte)(this.DirectoryBackendType & DirectoryBackendType.Mbx) == 4;
			}
		}

		public TimeSpan? ClientSideSearchTimeout
		{
			get
			{
				return this.clientSideSearchTimeout;
			}
			set
			{
				this.clientSideSearchTimeout = value;
			}
		}

		public ConfigScopes ConfigScope
		{
			get
			{
				return this.configScope;
			}
			protected set
			{
				this.configScope = value;
			}
		}

		public ConsistencyMode ConsistencyMode
		{
			get
			{
				return this.consistencyMode;
			}
		}

		public string DomainController
		{
			get
			{
				return this.domainController;
			}
			set
			{
				this.CheckDomainControllerParameterConsistency(value);
				this.domainController = value;
				if (value == null)
				{
					this.stickyDC = false;
				}
			}
		}

		public bool EnforceContainerizedScoping
		{
			get
			{
				return this.enforceContainerizedScoping;
			}
			set
			{
				this.enforceContainerizedScoping = value;
			}
		}

		public bool EnforceDefaultScope
		{
			get
			{
				return this.enforceDefaultScope;
			}
			set
			{
				this.enforceDefaultScope = value;
			}
		}

		public string LastUsedDc
		{
			get
			{
				return this.ServerSettings.LastUsedDc(this.SessionSettings.GetAccountOrResourceForestFqdn());
			}
		}

		public int Lcid
		{
			get
			{
				return this.lcid;
			}
			protected set
			{
				this.lcid = value;
			}
		}

		public string LinkResolutionServer
		{
			get
			{
				return this.linkResolutionServer;
			}
			set
			{
				this.linkResolutionServer = value;
			}
		}

		public bool LogSizeLimitExceededEvent
		{
			get
			{
				return this.logSizeLimitExceededEvent;
			}
			set
			{
				this.logSizeLimitExceededEvent = value;
			}
		}

		public NetworkCredential NetworkCredential
		{
			get
			{
				return this.networkCredential;
			}
		}

		public bool ReadOnly
		{
			get
			{
				return this.readOnly;
			}
		}

		public ADServerSettings ServerSettings
		{
			get
			{
				return this.SessionSettings.ServerSettings;
			}
		}

		public TimeSpan? ServerTimeout
		{
			get
			{
				return this.serverTimeout;
			}
			set
			{
				if (value != null && value <= TimeSpan.Zero)
				{
					throw new ArgumentOutOfRangeException("value", value, DirectoryStrings.ExceptionServerTimeoutNegative);
				}
				this.serverTimeout = value;
			}
		}

		public ADSessionSettings SessionSettings
		{
			get
			{
				return this.sessionSettings;
			}
		}

		public bool SkipRangedAttributes
		{
			get
			{
				return this.skipRangedAttributes;
			}
			set
			{
				this.skipRangedAttributes = value;
			}
		}

		public bool UseConfigNC
		{
			get
			{
				return this.useConfigNC;
			}
			set
			{
				this.useConfigNC = value;
			}
		}

		public bool UseGlobalCatalog
		{
			get
			{
				return this.useGlobalCatalog;
			}
			set
			{
				this.useGlobalCatalog = value;
			}
		}

		public string[] ExclusiveLdapAttributes { get; set; }

		public IActivityScope ActivityScope
		{
			get
			{
				return this.logContext.ActivityScope;
			}
			set
			{
				this.logContext.ActivityScope = value;
			}
		}

		public string CallerInfo
		{
			get
			{
				return this.logContext.GetCallerInformation();
			}
		}

		protected ADObjectId SearchRoot
		{
			get
			{
				return this.searchRoot;
			}
			set
			{
				this.searchRoot = value;
			}
		}

		protected DirectorySessionBase(bool useConfigNC, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings)
		{
			if (sessionSettings == null)
			{
				throw new ArgumentNullException("sessionSettings");
			}
			this.domainController = null;
			this.consistencyMode = consistencyMode;
			this.lcid = CultureInfo.CurrentCulture.LCID;
			this.useGlobalCatalog = false;
			this.enforceDefaultScope = true;
			this.useConfigNC = useConfigNC;
			this.readOnly = readOnly;
			this.networkCredential = networkCredential;
			this.sessionSettings = sessionSettings;
			this.enforceContainerizedScoping = false;
			this.configScope = sessionSettings.ConfigScopes;
		}

		public RawSecurityDescriptor ReadSecurityDescriptor(ADObjectId id)
		{
			SecurityDescriptor securityDescriptor = this.ReadSecurityDescriptorBlob(id);
			if (securityDescriptor == null)
			{
				return null;
			}
			return securityDescriptor.ToRawSecurityDescriptor();
		}

		public abstract SecurityDescriptor ReadSecurityDescriptorBlob(ADObjectId id);

		internal void SetCallerInfo(string callerFilePath, string memberName, int callerFileLine)
		{
			this.logContext.FilePath = callerFilePath;
			this.logContext.FileLine = callerFileLine;
			this.logContext.MemberName = memberName;
		}

		protected void CheckDomainControllerParameterConsistency(string dcName)
		{
			if (!string.IsNullOrEmpty(dcName) && this.SessionSettings.PartitionId != null)
			{
				string forestFQDN = this.SessionSettings.PartitionId.ForestFQDN;
				if (!ADServerSettings.IsServerNamePartitionSameAsPartitionId(dcName, forestFQDN))
				{
					throw new DomainControllerFromWrongDomainException(DirectoryStrings.WrongDCForCurrentPartition(dcName, forestFQDN));
				}
			}
		}

		protected string GetStackTraceLine(int skipFrames = 4)
		{
			string[] array = Environment.StackTrace.Split(new string[]
			{
				"at "
			}, StringSplitOptions.RemoveEmptyEntries);
			for (int i = skipFrames; i < array.Length; i++)
			{
				string text = array[i];
				if (!string.IsNullOrEmpty(text.Trim()) && !text.Contains("Session.") && !text.Contains("Reader"))
				{
					return text.Substring(0, text.Length - skipFrames).Replace(',', ';');
				}
			}
			return null;
		}

		protected ADRawEntry CreateAndInitializeADRawEntry(PropertyBag propertyBag)
		{
			ADRawEntry adrawEntry = new ADRawEntry((ADPropertyBag)propertyBag);
			adrawEntry.ResetChangeTracking(true);
			return adrawEntry;
		}

		protected abstract ADObject CreateAndInitializeObject<TResult>(ADPropertyBag propertyBag, ADRawEntry dummyInstance) where TResult : IConfigurable, new();

		protected readonly bool readOnly;

		protected TimeSpan? serverTimeout;

		protected TimeSpan? clientSideSearchTimeout;

		protected string domainController;

		protected string linkResolutionServer;

		protected int lcid;

		protected bool useConfigNC;

		protected bool useGlobalCatalog;

		protected bool enforceDefaultScope;

		protected bool logSizeLimitExceededEvent = true;

		[NonSerialized]
		protected NetworkCredential networkCredential;

		protected ConsistencyMode consistencyMode;

		protected ADObjectId searchRoot;

		protected ADSessionSettings sessionSettings;

		protected ConfigScopes configScope;

		protected bool skipRangedAttributes;

		protected bool enforceContainerizedScoping;

		protected bool isRehomed;

		protected bool stickyDC;

		private ADLogContext logContext = new ADLogContext();
	}
}
