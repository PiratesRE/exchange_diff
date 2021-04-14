using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal sealed class ActivityContextLogger : ExtensibleLogger
	{
		private ActivityContextLogger() : base(new ActivityContextLogConfiguration())
		{
		}

		public static ActivityContextLogger Instance
		{
			get
			{
				if (ActivityContextLogger.instance == null)
				{
					lock (ActivityContextLogger.syncRoot)
					{
						if (ActivityContextLogger.instance == null)
						{
							ActivityContext.RegisterMetadata(typeof(ActivityContextLoggerMetaData));
							ActivityContextLogger.instance = new ActivityContextLogger();
						}
					}
				}
				return ActivityContextLogger.instance;
			}
		}

		protected override ICollection<KeyValuePair<string, object>> GetComponentSpecificData(IActivityScope activityScope, string eventId)
		{
			Dictionary<string, object> dictionary = null;
			if (activityScope != null)
			{
				HttpContext httpContext = HttpContext.Current;
				if (httpContext != null)
				{
					string sourceCafeServer = CafeHelper.GetSourceCafeServer(httpContext.Request);
					if (!string.IsNullOrEmpty(sourceCafeServer))
					{
						activityScope.SetProperty(ActivityContextLoggerMetaData.FrontEndServer, sourceCafeServer);
					}
					string requestUrlForLog = httpContext.GetRequestUrlForLog();
					activityScope.SetProperty(ActivityContextLoggerMetaData.Url, requestUrlForLog);
				}
				RbacPrincipal current = RbacPrincipal.GetCurrent(false);
				if (current != null)
				{
					string value;
					if (current.RbacConfiguration.DelegatedPrincipal != null)
					{
						value = current.RbacConfiguration.DelegatedPrincipal.UserId;
					}
					else
					{
						SmtpAddress executingUserPrimarySmtpAddress = current.RbacConfiguration.ExecutingUserPrimarySmtpAddress;
						value = (executingUserPrimarySmtpAddress.IsValidAddress ? executingUserPrimarySmtpAddress.ToString() : current.RbacConfiguration.ExecutingUserPrincipalName);
					}
					if (!string.IsNullOrEmpty(value))
					{
						activityScope.SetProperty(ActivityContextLoggerMetaData.PrimarySmtpAddress, value);
					}
					OrganizationId organizationId = current.RbacConfiguration.OrganizationId;
					if (organizationId != null && organizationId.OrganizationalUnit != null)
					{
						string name = organizationId.OrganizationalUnit.Name;
						activityScope.SetProperty(ActivityContextLoggerMetaData.Organization, name);
					}
				}
				dictionary = new Dictionary<string, object>(ActivityContextLogger.EnumToShortKeyMapping.Count);
				ExtensibleLogger.CopyPIIProperty(activityScope, dictionary, ActivityContextLoggerMetaData.PrimarySmtpAddress, ActivityContextLogger.PrimarySmtpAddressKey);
				ExtensibleLogger.CopyProperties(activityScope, dictionary, ActivityContextLogger.EnumToShortKeyMapping);
			}
			return dictionary;
		}

		private static readonly Dictionary<Enum, string> EnumToShortKeyMapping = new Dictionary<Enum, string>
		{
			{
				ActivityContextLoggerMetaData.Organization,
				"ORG"
			},
			{
				ActivityContextLoggerMetaData.FrontEndServer,
				"FE"
			},
			{
				ActivityContextLoggerMetaData.Url,
				"URL"
			}
		};

		private static string PrimarySmtpAddressKey = "PSA";

		private static object syncRoot = new object();

		private static ActivityContextLogger instance;
	}
}
