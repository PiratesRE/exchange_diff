using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.Tasks;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.ManagementEndpoint
{
	[Cmdlet("Get", "ManagementEndpoint")]
	public sealed class GetManagementEndpoint : ManagementEndpointBase
	{
		[Parameter(Mandatory = true, Position = 0)]
		[ValidateNotNullOrEmpty]
		public SmtpDomain DomainName
		{
			get
			{
				return (SmtpDomain)base.Fields["DomainName"];
			}
			set
			{
				base.Fields["DomainName"] = value;
			}
		}

		protected override ITaskModuleFactory CreateTaskModuleFactory()
		{
			return new GetManagementEndpointTaskModuleFactory();
		}

		protected override string GetRedirectionTemplate()
		{
			if (GetManagementEndpoint.PodTemplate == null)
			{
				string podRedirectionTemplate = ExchangePropertyContainer.GetPodRedirectionTemplate(base.SessionState);
				if (!string.IsNullOrEmpty(podRedirectionTemplate))
				{
					GetManagementEndpoint.PodTemplate = podRedirectionTemplate;
				}
				else
				{
					string text = null;
					using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ExchangeCrossForest\\"))
					{
						if (registryKey != null)
						{
							text = (string)registryKey.GetValue("RemotePowershellManagementEndpointUrlTemplate");
							if (text != null)
							{
								string[] array = text.Split(new char[]
								{
									'/'
								}, StringSplitOptions.RemoveEmptyEntries);
								if (array.Length > 0 && array[1].Contains("{0}"))
								{
									text = string.Format(array[1], "pod{0}");
								}
							}
						}
					}
					GetManagementEndpoint.PodTemplate = (text ?? "{0}");
				}
			}
			return GetManagementEndpoint.PodTemplate;
		}

		internal override void ProcessRedirectionEntry(IGlobalDirectorySession session)
		{
			string memberName = string.Format("E5CB63F56E8B4b69A1F70C192276D6AD@{0}", this.DomainName);
			string redirectServer = session.GetRedirectServer(memberName);
			if (string.IsNullOrEmpty(redirectServer))
			{
				base.WriteError(new CannotDetermineManagementEndpointException(Strings.ErrorCannotDetermineEndpointForTenant(this.DomainName.ToString())), ErrorCategory.InvalidData, null);
			}
			string remotePowershellUrl = string.Format("https://{0}/PowerShell/", redirectServer);
			ManagementEndpoint managementEndpoint = new ManagementEndpoint(remotePowershellUrl, ManagementEndpointVersion.Version3)
			{
				DomainName = this.DomainName
			};
			Guid guid;
			string resourceForest;
			string accountPartition;
			string smtpNextHopDomain;
			string text;
			bool flag;
			if (((GlsMServDirectorySession.GlsLookupMode != GlsLookupMode.MServOnly && session is GlsMServDirectorySession) || session is GlsDirectorySession) && session.TryGetTenantForestsByDomain(this.DomainName.ToString(), out guid, out resourceForest, out accountPartition, out smtpNextHopDomain, out text, out flag) && Guid.Empty != guid)
			{
				managementEndpoint.AccountPartition = accountPartition;
				managementEndpoint.SmtpNextHopDomain = smtpNextHopDomain;
				managementEndpoint.ResourceForest = resourceForest;
				managementEndpoint.ExternalDirectoryOrganizationId = guid;
			}
			ExTraceGlobals.LogTracer.Information<string, ManagementEndpointVersion>(0L, "Get-ManagementEndPoint URL/Version {0}/{1}", managementEndpoint.RemotePowershellUrl, managementEndpoint.Version);
			base.WriteObject(managementEndpoint);
		}

		private const string RemotePowershellUrlTemplate = "https://{0}/PowerShell/";

		private const string ExchangeCrossForestKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ExchangeCrossForest\\";

		private static string PodTemplate;
	}
}
