using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Audit;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Servicelets.AuditLogSearch
{
	public class AuditLogSearchHealth : HealthHandlerResult
	{
		public Search[] Searches
		{
			get
			{
				Search[] result;
				lock (this.syncRoot)
				{
					result = this.searches.ToArray();
				}
				return result;
			}
			set
			{
				lock (this.syncRoot)
				{
					this.searches = new List<Search>(value ?? Array<Search>.Empty);
				}
			}
		}

		public string[] Tenants
		{
			get
			{
				string[] result;
				lock (this.syncRoot)
				{
					result = this.tenants.ToArray();
				}
				return result;
			}
			set
			{
				lock (this.syncRoot)
				{
					this.tenants = new List<string>(value ?? Array<string>.Empty);
				}
			}
		}

		public string[] RetryTenants
		{
			get
			{
				string[] result;
				lock (this.syncRoot)
				{
					result = this.retryTenants.ToArray();
				}
				return result;
			}
			set
			{
				lock (this.syncRoot)
				{
					this.retryTenants = new List<string>(value ?? Array<string>.Empty);
				}
			}
		}

		public ExceptionDetails[] Exceptions
		{
			get
			{
				ExceptionDetails[] result;
				lock (this.syncRoot)
				{
					result = this.exceptions.ToArray();
				}
				return result;
			}
			set
			{
				lock (this.syncRoot)
				{
					this.exceptions.Clear();
					if (value != null && value.Length > 0)
					{
						for (int i = 0; i < value.Length; i++)
						{
							ExceptionDetails item = value[i];
							if (this.exceptions.Count >= 25)
							{
								break;
							}
							this.exceptions.Add(item);
						}
					}
				}
			}
		}

		public DateTime ProcessStartTime { get; set; }

		public DateTime? ProcessEndTime { get; set; }

		public DateTime? NextSearchTime { get; set; }

		public int RetryIteration { get; set; }

		public SslPolicyInfo SslValidationInfo
		{
			get
			{
				return SslPolicyInfo.Instance;
			}
			set
			{
			}
		}

		public SslConfig SslConfig
		{
			get
			{
				return new SslConfig
				{
					AllowInternalUntrustedCerts = SslConfiguration.AllowInternalUntrustedCerts,
					AllowExternalUntrustedCerts = SslConfiguration.AllowExternalUntrustedCerts
				};
			}
			set
			{
			}
		}

		internal void Clear()
		{
			lock (this.syncRoot)
			{
				this.searches.Clear();
				this.tenants.Clear();
				this.retryTenants.Clear();
			}
		}

		internal void ClearRetry()
		{
			lock (this.syncRoot)
			{
				this.retryTenants.Clear();
			}
		}

		internal void AddSearch(Search search)
		{
			if (search != null)
			{
				lock (this.syncRoot)
				{
					this.searches.Add(search);
				}
			}
		}

		internal void AddTenant(ADUser tenant)
		{
			if (tenant != null)
			{
				lock (this.syncRoot)
				{
					this.tenants.Add(tenant.UserPrincipalName);
				}
			}
		}

		internal void AddException(Exception e)
		{
			if (AuditLogSearchHealth.IsExceptionInterestingForDiagnostics(e))
			{
				ExceptionDetails exceptionDetails = ExceptionDetails.Create(e);
				if (exceptionDetails != null)
				{
					lock (this.syncRoot)
					{
						if (this.exceptions.Count == 25)
						{
							this.exceptions.RemoveAt(0);
						}
						this.exceptions.Add(exceptionDetails);
					}
				}
			}
		}

		internal void AddRetryTenant(ADUser tenant)
		{
			if (tenant != null)
			{
				lock (this.syncRoot)
				{
					this.retryTenants.Add(tenant.UserPrincipalName);
				}
			}
		}

		private static bool IsExceptionInterestingForDiagnostics(Exception exception)
		{
			return exception != null && !(exception is TenantAccessBlockedException) && !(exception is CannotResolveTenantNameException) && (exception.InnerException == null || !(exception.InnerException is CannotResolveTenantNameException));
		}

		internal const int MaxExceptions = 25;

		private readonly object syncRoot = new object();

		private List<Search> searches = new List<Search>();

		private List<string> tenants = new List<string>();

		private readonly List<ExceptionDetails> exceptions = new List<ExceptionDetails>(25);

		private List<string> retryTenants = new List<string>();
	}
}
