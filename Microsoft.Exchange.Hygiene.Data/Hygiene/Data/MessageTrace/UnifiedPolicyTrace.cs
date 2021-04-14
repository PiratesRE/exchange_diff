using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class UnifiedPolicyTrace : ConfigurablePropertyBag, IComparable<UnifiedPolicyTrace>
	{
		public UnifiedPolicyTrace(Guid organizationalUnitRoot, Guid objectId)
		{
			this.OrganizationalUnitRoot = organizationalUnitRoot;
			this.ObjectId = objectId;
		}

		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(string.Format("{0}\\{1}", this.OrganizationalUnitRoot, this.ObjectId));
			}
		}

		public Guid OrganizationalUnitRoot
		{
			get
			{
				return (Guid)this[UnifiedPolicyTraceSchema.OrganizationalUnitRootProperty];
			}
			set
			{
				this[UnifiedPolicyTraceSchema.OrganizationalUnitRootProperty] = value;
			}
		}

		public Guid ObjectId
		{
			get
			{
				return (Guid)this[UnifiedPolicyTraceSchema.ObjectIdProperty];
			}
			set
			{
				this[UnifiedPolicyTraceSchema.ObjectIdProperty] = value;
			}
		}

		public string DataSource
		{
			get
			{
				return this[UnifiedPolicyTraceSchema.DataSourceProperty] as string;
			}
			set
			{
				this[UnifiedPolicyTraceSchema.DataSourceProperty] = value;
			}
		}

		public Guid FileId
		{
			get
			{
				return (Guid)this[UnifiedPolicyTraceSchema.FileIdProperty];
			}
			set
			{
				this[UnifiedPolicyTraceSchema.FileIdProperty] = value;
			}
		}

		public string FileName
		{
			get
			{
				return this[UnifiedPolicyTraceSchema.FileNameProperty] as string;
			}
			set
			{
				this[UnifiedPolicyTraceSchema.FileNameProperty] = value;
			}
		}

		public long Size
		{
			get
			{
				return (long)this[UnifiedPolicyTraceSchema.SizeProperty];
			}
			set
			{
				this[UnifiedPolicyTraceSchema.SizeProperty] = value;
			}
		}

		public Guid SiteId
		{
			get
			{
				return (Guid)this[UnifiedPolicyTraceSchema.SiteIdProperty];
			}
			set
			{
				this[UnifiedPolicyTraceSchema.SiteIdProperty] = value;
			}
		}

		public string FileUrl
		{
			get
			{
				return this[UnifiedPolicyTraceSchema.FileUrlProperty] as string;
			}
			set
			{
				this[UnifiedPolicyTraceSchema.FileUrlProperty] = value;
			}
		}

		public string Owner
		{
			get
			{
				return this[UnifiedPolicyTraceSchema.OwnerProperty] as string;
			}
			set
			{
				this[UnifiedPolicyTraceSchema.OwnerProperty] = value;
			}
		}

		public bool IsViewableByExternalUsers
		{
			get
			{
				return (bool)this[UnifiedPolicyTraceSchema.IsViewableByExternalUsersProperty];
			}
			set
			{
				this[UnifiedPolicyTraceSchema.IsViewableByExternalUsersProperty] = value;
			}
		}

		public string LastModifiedBy
		{
			get
			{
				return this[UnifiedPolicyTraceSchema.LastModifiedByProperty] as string;
			}
			set
			{
				this[UnifiedPolicyTraceSchema.LastModifiedByProperty] = value;
			}
		}

		public DateTime CreateTime
		{
			get
			{
				return (DateTime)this[UnifiedPolicyTraceSchema.CreateTimeProperty];
			}
			set
			{
				this[UnifiedPolicyTraceSchema.CreateTimeProperty] = value;
			}
		}

		public DateTime LastModifiedTime
		{
			get
			{
				return (DateTime)this[UnifiedPolicyTraceSchema.LastModifiedTimeProperty];
			}
			set
			{
				this[UnifiedPolicyTraceSchema.LastModifiedTimeProperty] = value;
			}
		}

		public DateTime PolicyMatchTime
		{
			get
			{
				return (DateTime)this[UnifiedPolicyTraceSchema.PolicyMatchTimeProperty];
			}
			set
			{
				this[UnifiedPolicyTraceSchema.PolicyMatchTimeProperty] = value;
			}
		}

		public List<UnifiedPolicyRule> Rules
		{
			get
			{
				List<UnifiedPolicyRule> result;
				if ((result = this.rules) == null)
				{
					result = (this.rules = new List<UnifiedPolicyRule>());
				}
				return result;
			}
		}

		public void Add(UnifiedPolicyRule rule)
		{
			if (rule == null)
			{
				throw new ArgumentNullException("rule");
			}
			rule[UnifiedPolicyRuleSchema.OrganizationalUnitRootProperty] = this.OrganizationalUnitRoot;
			rule[UnifiedPolicyRuleSchema.ObjectIdProperty] = this.ObjectId;
			rule[UnifiedPolicyRuleSchema.DataSourceProperty] = this.DataSource;
			this.Rules.Add(rule);
		}

		public override Type GetSchemaType()
		{
			return typeof(UnifiedPolicyTraceSchema);
		}

		public int CompareTo(UnifiedPolicyTrace other)
		{
			if (other == null)
			{
				return 1;
			}
			int num = 0;
			byte[] array = this.ObjectId.ToByteArray();
			byte[] array2 = other.ObjectId.ToByteArray();
			int num2 = 10;
			while (num == 0 && num2 < 16)
			{
				num = array[num2].CompareTo(array2[num2]);
				num2++;
			}
			return num;
		}

		internal static readonly HygienePropertyDefinition[] Properties = new HygienePropertyDefinition[]
		{
			UnifiedPolicyTraceSchema.OrganizationalUnitRootProperty,
			UnifiedPolicyTraceSchema.ObjectIdProperty,
			UnifiedPolicyTraceSchema.DataSourceProperty,
			UnifiedPolicyTraceSchema.FileIdProperty,
			UnifiedPolicyTraceSchema.FileNameProperty,
			UnifiedPolicyTraceSchema.SizeProperty,
			UnifiedPolicyTraceSchema.SiteIdProperty,
			UnifiedPolicyTraceSchema.FileUrlProperty,
			UnifiedPolicyTraceSchema.OwnerProperty,
			UnifiedPolicyTraceSchema.IsViewableByExternalUsersProperty,
			UnifiedPolicyTraceSchema.LastModifiedByProperty,
			UnifiedPolicyTraceSchema.CreateTimeProperty,
			UnifiedPolicyTraceSchema.LastModifiedTimeProperty,
			UnifiedPolicyTraceSchema.PolicyMatchTimeProperty,
			UnifiedPolicyCommonSchema.HashBucketProperty,
			UnifiedPolicyCommonSchema.IntValue1Prop,
			UnifiedPolicyCommonSchema.IntValue2Prop,
			UnifiedPolicyCommonSchema.IntValue3Prop,
			UnifiedPolicyCommonSchema.LongValue1Prop,
			UnifiedPolicyCommonSchema.LongValue2Prop,
			UnifiedPolicyCommonSchema.GuidValue1Prop,
			UnifiedPolicyCommonSchema.GuidValue2Prop,
			UnifiedPolicyCommonSchema.StringValue1Prop,
			UnifiedPolicyCommonSchema.StringValue2Prop,
			UnifiedPolicyCommonSchema.StringValue3Prop,
			UnifiedPolicyCommonSchema.StringValue4Prop,
			UnifiedPolicyCommonSchema.StringValue5Prop,
			UnifiedPolicyCommonSchema.ByteValue1Prop,
			UnifiedPolicyCommonSchema.ByteValue2Prop
		};

		private List<UnifiedPolicyRule> rules;
	}
}
