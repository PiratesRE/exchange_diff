using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("New", "ServiceEndpoint")]
	public sealed class NewServiceEndpoint : NewSystemConfigurationObjectTask<ADServiceConnectionPoint>
	{
		[Parameter(Mandatory = false)]
		public Uri Url
		{
			get
			{
				if (this.DataObject.ServiceBindingInformation != null && this.DataObject.ServiceBindingInformation.Count > 0)
				{
					return new Uri(this.DataObject.ServiceBindingInformation[0]);
				}
				return null;
			}
			set
			{
				if (value != null)
				{
					this.DataObject.ServiceBindingInformation = new MultiValuedProperty<string>();
					this.DataObject.ServiceBindingInformation.Add(value.ToString());
				}
			}
		}

		[Parameter(Mandatory = false)]
		public string UrlTemplate
		{
			get
			{
				if (this.DataObject.Keywords != null && this.DataObject.Keywords.Count > 0)
				{
					foreach (string text in this.DataObject.Keywords)
					{
						if (text.StartsWith(ServiceEndpointContainer.UriTemplateKey, StringComparison.OrdinalIgnoreCase))
						{
							return text.Substring(ServiceEndpointContainer.UriTemplateKey.Length);
						}
					}
				}
				return null;
			}
			set
			{
				string text = (value != null) ? value.Trim() : null;
				if (!string.IsNullOrEmpty(text))
				{
					if (this.DataObject.Keywords == null)
					{
						this.DataObject.Keywords = new MultiValuedProperty<string>();
					}
					this.DataObject.Keywords.Add(ServiceEndpointContainer.UriTemplateKey + text);
				}
			}
		}

		[Parameter(Mandatory = false)]
		public string Token
		{
			get
			{
				if (this.DataObject.Keywords != null && this.DataObject.Keywords.Count > 0)
				{
					foreach (string text in this.DataObject.Keywords)
					{
						if (text.StartsWith(ServiceEndpointContainer.TokenKey, StringComparison.OrdinalIgnoreCase))
						{
							return text.Substring(ServiceEndpointContainer.TokenKey.Length);
						}
					}
				}
				return null;
			}
			set
			{
				string text = (value != null) ? value.Trim() : null;
				if (!string.IsNullOrEmpty(text))
				{
					if (this.DataObject.Keywords == null)
					{
						this.DataObject.Keywords = new MultiValuedProperty<string>();
					}
					this.DataObject.Keywords.Add(ServiceEndpointContainer.TokenKey + text);
				}
			}
		}

		[Parameter(Mandatory = false)]
		public string CertificateSubjectName
		{
			get
			{
				if (this.DataObject.Keywords != null && this.DataObject.Keywords.Count > 0)
				{
					foreach (string text in this.DataObject.Keywords)
					{
						if (text.StartsWith(ServiceEndpointContainer.CertSubjectKey, StringComparison.OrdinalIgnoreCase))
						{
							return text.Substring(ServiceEndpointContainer.CertSubjectKey.Length);
						}
					}
				}
				return null;
			}
			set
			{
				string text = (value != null) ? value.Trim() : null;
				if (!string.IsNullOrEmpty(text))
				{
					if (this.DataObject.Keywords == null)
					{
						this.DataObject.Keywords = new MultiValuedProperty<string>();
					}
					this.DataObject.Keywords.Add(ServiceEndpointContainer.CertSubjectKey + text);
				}
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			ADObjectId childId = base.RootOrgContainerId.GetChildId(ServiceEndpointContainer.DefaultName).GetChildId(this.DataObject.Name);
			this.DataObject.SetId(childId);
			base.InternalValidate();
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			IConfigurationSession configurationSession = (IConfigurationSession)base.DataSession;
			ADServiceConnectionPoint adserviceConnectionPoint = configurationSession.Read<ADServiceConnectionPoint>(this.DataObject.Id);
			if (adserviceConnectionPoint != null)
			{
				adserviceConnectionPoint.ServiceBindingInformation = this.DataObject.ServiceBindingInformation;
				adserviceConnectionPoint.Keywords = this.DataObject.Keywords;
				configurationSession.Save(adserviceConnectionPoint);
				base.WriteObject(adserviceConnectionPoint);
			}
			else
			{
				base.InternalProcessRecord();
				adserviceConnectionPoint = configurationSession.Read<ADServiceConnectionPoint>(this.DataObject.Id);
			}
			base.WriteVerbose(Strings.VerboseUpdatedServiceEndpoint(adserviceConnectionPoint.Name, adserviceConnectionPoint.OriginatingServer));
			TaskLogger.LogExit();
		}
	}
}
