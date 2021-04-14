using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class OrganizationProperties
	{
		public bool SkipToUAndParentalControlCheck
		{
			get
			{
				return this.skipToUAndParentalControlCheck;
			}
		}

		public bool IsTenantAccessBlocked
		{
			get
			{
				return this.isTenantAccessBlocked;
			}
			internal set
			{
				this.isTenantAccessBlocked = value;
			}
		}

		public bool HideAdminAccessWarning
		{
			get
			{
				return this.hideAdminAccessWarning;
			}
			internal set
			{
				this.hideAdminAccessWarning = value;
			}
		}

		public bool ShowAdminAccessWarning
		{
			get
			{
				return this.showAdminAccessWarning;
			}
			internal set
			{
				this.showAdminAccessWarning = value;
			}
		}

		public bool ActivityBasedAuthenticationTimeoutEnabled
		{
			get
			{
				return this.activityBasedAuthenticationTimeoutEnabled;
			}
			internal set
			{
				this.activityBasedAuthenticationTimeoutEnabled = value;
			}
		}

		public bool ActivityBasedAuthenticationTimeoutWithSingleSignOnEnabled
		{
			get
			{
				return this.activityBasedAuthenticationTimeoutWithSingleSignOnEnabled;
			}
			internal set
			{
				this.activityBasedAuthenticationTimeoutWithSingleSignOnEnabled = value;
			}
		}

		public EnhancedTimeSpan ActivityBasedAuthenticationTimeoutInterval
		{
			get
			{
				return this.activityBasedAuthenticationTimeoutInterval;
			}
			internal set
			{
				this.activityBasedAuthenticationTimeoutInterval = value;
			}
		}

		public string ServicePlan
		{
			get
			{
				return this.servicePlan;
			}
		}

		public bool IsLicensingEnforced
		{
			get
			{
				return this.isLicensingEnforced;
			}
			internal set
			{
				this.isLicensingEnforced = value;
			}
		}

		internal OrganizationProperties(bool skipToUAndParentalControlCheck, string servicePlan)
		{
			this.skipToUAndParentalControlCheck = skipToUAndParentalControlCheck;
			this.servicePlan = servicePlan;
			this.propertyBag = new Dictionary<Type, object>();
			this.readerWriterLock = new ReaderWriterLock();
		}

		public void SetValue<T>(T value)
		{
			try
			{
				this.readerWriterLock.AcquireWriterLock(-1);
				this.propertyBag[typeof(T)] = value;
			}
			finally
			{
				try
				{
					this.readerWriterLock.ReleaseWriterLock();
				}
				catch (ApplicationException)
				{
				}
			}
		}

		public bool TryGetValue<T>(out T value)
		{
			bool result;
			try
			{
				this.readerWriterLock.AcquireReaderLock(-1);
				object obj;
				bool flag = this.propertyBag.TryGetValue(typeof(T), out obj);
				value = (flag ? ((T)((object)obj)) : default(T));
				result = flag;
			}
			finally
			{
				try
				{
					this.readerWriterLock.ReleaseReaderLock();
				}
				catch (ApplicationException)
				{
				}
			}
			return result;
		}

		private bool skipToUAndParentalControlCheck;

		private bool showAdminAccessWarning;

		private bool hideAdminAccessWarning;

		private bool activityBasedAuthenticationTimeoutEnabled;

		private bool activityBasedAuthenticationTimeoutWithSingleSignOnEnabled;

		private EnhancedTimeSpan activityBasedAuthenticationTimeoutInterval;

		private string servicePlan;

		private bool isLicensingEnforced;

		private bool isTenantAccessBlocked;

		private Dictionary<Type, object> propertyBag;

		[NonSerialized]
		private ReaderWriterLock readerWriterLock;
	}
}
