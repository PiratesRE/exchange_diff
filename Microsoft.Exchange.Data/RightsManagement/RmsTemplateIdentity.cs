using System;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Data.RightsManagement
{
	[Serializable]
	public sealed class RmsTemplateIdentity : ObjectId, IEquatable<RmsTemplateIdentity>
	{
		public RmsTemplateIdentity(Guid templateId, string templateName, RmsTemplateType templateType)
		{
			this.templateId = templateId;
			this.templateName = templateName;
			this.templateType = templateType;
		}

		public RmsTemplateIdentity(Guid templateId, string templateName) : this(templateId, templateName, RmsTemplateType.Distributed)
		{
		}

		public RmsTemplateIdentity(Guid templateId) : this(templateId, string.Empty)
		{
		}

		public RmsTemplateIdentity() : this(Guid.Empty, string.Empty)
		{
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

		public RmsTemplateType Type
		{
			get
			{
				return this.templateType;
			}
		}

		public override byte[] GetBytes()
		{
			return this.templateId.ToByteArray();
		}

		public bool Equals(RmsTemplateIdentity other)
		{
			return other != null && this.templateId.Equals(other.templateId);
		}

		public override bool Equals(object other)
		{
			return this.Equals(other as RmsTemplateIdentity);
		}

		public override int GetHashCode()
		{
			return this.templateId.GetHashCode();
		}

		public override string ToString()
		{
			return this.templateName;
		}

		private Guid templateId;

		private string templateName;

		private RmsTemplateType templateType;
	}
}
