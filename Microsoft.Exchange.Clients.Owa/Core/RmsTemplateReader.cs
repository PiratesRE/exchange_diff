using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.RightsManagement;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class RmsTemplateReader
	{
		internal bool IsInternalLicensingEnabled
		{
			get
			{
				bool result;
				try
				{
					result = RmsClientManager.IRMConfig.IsInternalLicensingEnabledForTenant(this.organizationId);
				}
				catch (ExchangeConfigurationException innerException)
				{
					throw new RightsManagementTransientException(ServerStrings.RmExceptionGenericMessage, innerException);
				}
				catch (RightsManagementException ex)
				{
					if (ex.IsPermanent)
					{
						throw new RightsManagementPermanentException(ServerStrings.RmExceptionGenericMessage, ex);
					}
					throw new RightsManagementTransientException(ServerStrings.RmExceptionGenericMessage, ex);
				}
				return result;
			}
		}

		internal bool IsExternalLicensingEnabled
		{
			get
			{
				bool result;
				try
				{
					result = RmsClientManager.IRMConfig.IsExternalLicensingEnabledForTenant(this.organizationId);
				}
				catch (ExchangeConfigurationException innerException)
				{
					throw new RightsManagementTransientException(ServerStrings.RmExceptionGenericMessage, innerException);
				}
				catch (RightsManagementException ex)
				{
					if (ex.IsPermanent)
					{
						throw new RightsManagementPermanentException(ServerStrings.RmExceptionGenericMessage, ex);
					}
					throw new RightsManagementTransientException(ServerStrings.RmExceptionGenericMessage, ex);
				}
				return result;
			}
		}

		internal bool TemplateAcquisitionFailed
		{
			get
			{
				return this.errorAcquiringTemplates;
			}
		}

		internal RmsTemplateReader(OrganizationId organizationId)
		{
			this.organizationId = organizationId;
			this.errorAcquiringTemplates = false;
		}

		public IEnumerable<RmsTemplate> GetRmsTemplates()
		{
			this.errorAcquiringTemplates = false;
			if (this.IsInternalLicensingEnabled)
			{
				try
				{
					return RmsClientManager.AcquireRmsTemplates(this.organizationId, false);
				}
				catch (ExchangeConfigurationException arg)
				{
					ExTraceGlobals.CoreTracer.TraceError<ExchangeConfigurationException>(0L, "ExchangeConfigurationException while loading RMS templates: {0}", arg);
					this.errorAcquiringTemplates = true;
				}
				catch (RightsManagementException arg2)
				{
					ExTraceGlobals.CoreTracer.TraceError<RightsManagementException>(0L, "RightsManagementException while Loading RMS templates: {0}", arg2);
					this.errorAcquiringTemplates = true;
				}
			}
			return RmsTemplateReader.EmptyRmsTemplateList;
		}

		public RmsTemplate LookupRmsTemplate(Guid guid)
		{
			if (guid == Guid.Empty)
			{
				return null;
			}
			foreach (RmsTemplate rmsTemplate in this.GetRmsTemplates())
			{
				if (rmsTemplate.Id == guid)
				{
					return rmsTemplate;
				}
			}
			return null;
		}

		internal string GetDescription(Guid guid, CultureInfo locale)
		{
			ComplianceReader.ThrowOnNullArgument(locale, "locale");
			return this.GetDescription(this.LookupRmsTemplate(guid), locale);
		}

		private string GetDescription(RmsTemplate template, CultureInfo locale)
		{
			ComplianceReader.ThrowOnNullArgument(locale, "locale");
			string result = string.Empty;
			if (template != null)
			{
				string name = template.GetName(locale);
				string description = template.GetDescription(locale);
				if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(description))
				{
					if (!string.IsNullOrEmpty(name))
					{
						result = name;
					}
					else if (!string.IsNullOrEmpty(description))
					{
						result = description;
					}
				}
				else
				{
					result = name + " - " + description;
				}
			}
			return result;
		}

		private static readonly List<RmsTemplate> EmptyRmsTemplateList = new List<RmsTemplate>();

		private readonly OrganizationId organizationId;

		private bool errorAcquiringTemplates;
	}
}
