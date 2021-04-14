using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.RightsManagement;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public sealed class RmsTemplateIdParameter : IIdentityParameter
	{
		public RmsTemplateIdParameter(string identity)
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			if (identity.Length == 0)
			{
				throw new ArgumentException(Strings.ErrorEmptyParameter(base.GetType().ToString()), "identity");
			}
			this.rawIdentity = identity;
			this.Parse(identity);
		}

		public RmsTemplateIdParameter(RmsTemplatePresentation template)
		{
			if (template == null)
			{
				throw new ArgumentNullException("template");
			}
			RmsTemplateIdentity rmsTemplateIdentity = (RmsTemplateIdentity)template.Identity;
			this.rawIdentity = rmsTemplateIdentity.ToString();
			this.templateId = rmsTemplateIdentity.TemplateId;
			this.templateName = template.Name;
		}

		public RmsTemplateIdParameter(ObjectId objectId)
		{
			if (objectId == null)
			{
				throw new ArgumentNullException("objectId");
			}
			this.rawIdentity = objectId.ToString();
			((IIdentityParameter)this).Initialize(objectId);
		}

		public RmsTemplateIdParameter(INamedIdentity namedIdentity) : this(namedIdentity.Identity)
		{
			this.rawIdentity = namedIdentity.DisplayName;
		}

		public RmsTemplateIdParameter()
		{
		}

		string IIdentityParameter.RawIdentity
		{
			get
			{
				return this.rawIdentity;
			}
		}

		public override string ToString()
		{
			return this.rawIdentity;
		}

		public Guid TemplateId
		{
			get
			{
				return this.templateId;
			}
		}

		public string TemplateName
		{
			get
			{
				return this.templateName;
			}
		}

		IEnumerable<T> IIdentityParameter.GetObjects<T>(ObjectId rootId, IConfigDataProvider session)
		{
			LocalizedString? localizedString;
			return ((IIdentityParameter)this).GetObjects<T>(rootId, session, null, out localizedString);
		}

		IEnumerable<T> IIdentityParameter.GetObjects<T>(ObjectId rootId, IConfigDataProvider session, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			QueryFilter queryFilter = new RmsTemplateQueryFilter(this.templateId, this.templateName);
			notFoundReason = new LocalizedString?(Strings.ErrorManagementObjectNotFound(this.ToString()));
			if (optionalData != null && optionalData.AdditionalFilter != null)
			{
				queryFilter = QueryFilter.AndTogether(new QueryFilter[]
				{
					queryFilter,
					optionalData.AdditionalFilter
				});
			}
			return session.FindPaged<T>(queryFilter, rootId, false, null, 0);
		}

		void IIdentityParameter.Initialize(ObjectId objectId)
		{
			RmsTemplateIdentity rmsTemplateIdentity = objectId as RmsTemplateIdentity;
			if (rmsTemplateIdentity == null)
			{
				throw new ArgumentException(Strings.ErrorInvalidParameterType("objectId", typeof(RmsTemplateIdentity).Name), "objectId");
			}
			this.rawIdentity = rmsTemplateIdentity.ToString();
			this.templateName = rmsTemplateIdentity.TemplateName;
			this.templateId = rmsTemplateIdentity.TemplateId;
		}

		private void Parse(string identity)
		{
			if (!DrmClientUtils.TryParseGuid(identity, out this.templateId))
			{
				this.templateId = Guid.Empty;
			}
			this.templateName = (string.IsNullOrEmpty(identity) ? null : identity.Trim());
		}

		private Guid templateId;

		private string templateName;

		private string rawIdentity;
	}
}
