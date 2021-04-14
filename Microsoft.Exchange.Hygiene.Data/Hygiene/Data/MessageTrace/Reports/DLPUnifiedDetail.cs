using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace.Reports
{
	internal class DLPUnifiedDetail : Schema
	{
		public string OrganizationalUnitRootId
		{
			get
			{
				return this[DLPUnifiedDetail.OrganizationalUnitRootIdDefinition] as string;
			}
			set
			{
				this[DLPUnifiedDetail.OrganizationalUnitRootIdDefinition] = value;
			}
		}

		public string DataSource
		{
			get
			{
				return this[DLPUnifiedDetail.DataSourceDefinition] as string;
			}
			set
			{
				this[DLPUnifiedDetail.DataSourceDefinition] = value;
			}
		}

		public DateTime PolicyMatchTime
		{
			get
			{
				return (DateTime)this[DLPUnifiedDetail.PolicyMatchTimeDefinition];
			}
			set
			{
				this[DLPUnifiedDetail.PolicyMatchTimeDefinition] = value;
			}
		}

		public string Title
		{
			get
			{
				return this[DLPUnifiedDetail.TitleDefinition] as string;
			}
			set
			{
				this[DLPUnifiedDetail.TitleDefinition] = value;
			}
		}

		public long Size
		{
			get
			{
				return (long)this[DLPUnifiedDetail.SizeDefinition];
			}
			set
			{
				this[DLPUnifiedDetail.SizeDefinition] = value;
			}
		}

		public string Location
		{
			get
			{
				return this[DLPUnifiedDetail.LocationDefinition] as string;
			}
			set
			{
				this[DLPUnifiedDetail.LocationDefinition] = value;
			}
		}

		public string Actor
		{
			get
			{
				return this[DLPUnifiedDetail.ActorDefinition] as string;
			}
			set
			{
				this[DLPUnifiedDetail.ActorDefinition] = value;
			}
		}

		public string PolicyName
		{
			get
			{
				return this[DLPUnifiedDetail.PolicyNameDefinition] as string;
			}
			set
			{
				this[DLPUnifiedDetail.PolicyNameDefinition] = value;
			}
		}

		public string TransportRuleName
		{
			get
			{
				return this[DLPUnifiedDetail.TransportRuleNameDefinition] as string;
			}
			set
			{
				this[DLPUnifiedDetail.TransportRuleNameDefinition] = value;
			}
		}

		public string Severity
		{
			get
			{
				return this[DLPUnifiedDetail.SeverityDefinition] as string;
			}
			set
			{
				this[DLPUnifiedDetail.SeverityDefinition] = value;
			}
		}

		public string OverrideType
		{
			get
			{
				return this[DLPUnifiedDetail.OverrideTypeDefinition] as string;
			}
			set
			{
				this[DLPUnifiedDetail.OverrideTypeDefinition] = value;
			}
		}

		public string OverrideJustification
		{
			get
			{
				return this[DLPUnifiedDetail.OverrideJustificationDefinition] as string;
			}
			set
			{
				this[DLPUnifiedDetail.OverrideJustificationDefinition] = value;
			}
		}

		public string DataClassification
		{
			get
			{
				return this[DLPUnifiedDetail.DataClassificationDefinition] as string;
			}
			set
			{
				this[DLPUnifiedDetail.DataClassificationDefinition] = value;
			}
		}

		public int ClassificationConfidence
		{
			get
			{
				return (int)this[DLPUnifiedDetail.ClassificationConfidenceDefinition];
			}
			set
			{
				this[DLPUnifiedDetail.ClassificationConfidenceDefinition] = value;
			}
		}

		public int ClassificationCount
		{
			get
			{
				return (int)this[DLPUnifiedDetail.ClassificationCountDefinition];
			}
			set
			{
				this[DLPUnifiedDetail.ClassificationCountDefinition] = value;
			}
		}

		public string EventType
		{
			get
			{
				return this[DLPUnifiedDetail.EventTypeDefinition] as string;
			}
			set
			{
				this[DLPUnifiedDetail.EventTypeDefinition] = value;
			}
		}

		public string Action
		{
			get
			{
				return this[DLPUnifiedDetail.ActionDefinition] as string;
			}
			set
			{
				this[DLPUnifiedDetail.ActionDefinition] = value;
			}
		}

		public Guid ObjectId
		{
			get
			{
				return (Guid)this[DLPUnifiedDetail.ObjectIdDefinition];
			}
			set
			{
				this[DLPUnifiedDetail.ObjectIdDefinition] = value;
			}
		}

		internal static readonly HygienePropertyDefinition OrganizationalUnitRootIdDefinition = new HygienePropertyDefinition("OrganizationalUnitRootId", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition DataSourceDefinition = new HygienePropertyDefinition("DataSource", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition PolicyMatchTimeDefinition = new HygienePropertyDefinition("PolicyMatchTime", typeof(DateTime), DateTime.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition TitleDefinition = new HygienePropertyDefinition("Title", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition SizeDefinition = new HygienePropertyDefinition("Size", typeof(long), 0L, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition LocationDefinition = new HygienePropertyDefinition("Location", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition SeverityDefinition = new HygienePropertyDefinition("Severity", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition ActorDefinition = new HygienePropertyDefinition("Actor", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition PolicyNameDefinition = new HygienePropertyDefinition("PolicyName", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition TransportRuleNameDefinition = new HygienePropertyDefinition("TransportRuleName", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition DataClassificationDefinition = new HygienePropertyDefinition("DataClassification", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition ClassificationConfidenceDefinition = new HygienePropertyDefinition("ClassificationConfidence", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition ClassificationCountDefinition = new HygienePropertyDefinition("ClassificationCount", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition OverrideJustificationDefinition = new HygienePropertyDefinition("OverrideJustification", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition OverrideTypeDefinition = new HygienePropertyDefinition("OverrideType", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition EventTypeDefinition = new HygienePropertyDefinition("EventType", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition ActionDefinition = new HygienePropertyDefinition("Action", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition ObjectIdDefinition = new HygienePropertyDefinition("ObjectId", typeof(Guid), Guid.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
