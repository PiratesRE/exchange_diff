using System;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[Serializable]
	public class FailedMSOSyncObject : ADConfigurationObject
	{
		public SyncObjectId ObjectId
		{
			get
			{
				return (SyncObjectId)this.propertyBag[FailedMSOSyncObjectSchema.SyncObjectId];
			}
			set
			{
				this.propertyBag[FailedMSOSyncObjectSchema.SyncObjectId] = value;
			}
		}

		public DateTime? DivergenceTimestamp
		{
			get
			{
				return (DateTime?)this.propertyBag[FailedMSOSyncObjectSchema.DivergenceTimestamp];
			}
			set
			{
				this.propertyBag[FailedMSOSyncObjectSchema.DivergenceTimestamp] = value;
			}
		}

		public int DivergenceCount
		{
			get
			{
				return (int)this.propertyBag[FailedMSOSyncObjectSchema.DivergenceCount];
			}
			set
			{
				this.propertyBag[FailedMSOSyncObjectSchema.DivergenceCount] = value;
			}
		}

		public bool IsTemporary
		{
			get
			{
				return (bool)this.propertyBag[FailedMSOSyncObjectSchema.IsTemporary];
			}
			set
			{
				this.propertyBag[FailedMSOSyncObjectSchema.IsTemporary] = value;
			}
		}

		public bool IsIncrementalOnly
		{
			get
			{
				return (bool)this.propertyBag[FailedMSOSyncObjectSchema.IsIncrementalOnly];
			}
			set
			{
				this.propertyBag[FailedMSOSyncObjectSchema.IsIncrementalOnly] = value;
			}
		}

		public bool IsLinkRelated
		{
			get
			{
				return (bool)this.propertyBag[FailedMSOSyncObjectSchema.IsLinkRelated];
			}
			set
			{
				this.propertyBag[FailedMSOSyncObjectSchema.IsLinkRelated] = value;
			}
		}

		public bool IsIgnoredInHaltCondition
		{
			get
			{
				return (bool)this.propertyBag[FailedMSOSyncObjectSchema.IsIgnoredInHaltCondition];
			}
			set
			{
				this.propertyBag[FailedMSOSyncObjectSchema.IsIgnoredInHaltCondition] = value;
			}
		}

		public bool IsTenantWideDivergence
		{
			get
			{
				return (bool)this.propertyBag[FailedMSOSyncObjectSchema.IsTenantWideDivergence];
			}
			set
			{
				this.propertyBag[FailedMSOSyncObjectSchema.IsTenantWideDivergence] = value;
			}
		}

		public bool IsValidationDivergence
		{
			get
			{
				return (bool)this.propertyBag[FailedMSOSyncObjectSchema.IsValidationDivergence];
			}
			set
			{
				this.propertyBag[FailedMSOSyncObjectSchema.IsValidationDivergence] = value;
			}
		}

		public bool IsRetriable
		{
			get
			{
				return (bool)this.propertyBag[FailedMSOSyncObjectSchema.IsRetriable];
			}
			set
			{
				this.propertyBag[FailedMSOSyncObjectSchema.IsRetriable] = value;
			}
		}

		public MultiValuedProperty<string> Errors
		{
			get
			{
				return (MultiValuedProperty<string>)this.propertyBag[FailedMSOSyncObjectSchema.Errors];
			}
			set
			{
				this.propertyBag[FailedMSOSyncObjectSchema.Errors] = value;
			}
		}

		public string DivergenceInfoXml
		{
			get
			{
				return (string)this[FailedMSOSyncObjectSchema.DivergenceInfoXml];
			}
			internal set
			{
				this[FailedMSOSyncObjectSchema.DivergenceInfoXml] = value;
			}
		}

		public string Comment
		{
			get
			{
				return (string)this.propertyBag[FailedMSOSyncObjectSchema.Comment];
			}
			set
			{
				this.propertyBag[FailedMSOSyncObjectSchema.Comment] = value;
			}
		}

		public string BuildNumber
		{
			get
			{
				return this.GetValueForTag(DivergenceInfo.BuildNumber);
			}
			set
			{
				this.SetValueForTag(DivergenceInfo.BuildNumber, value);
			}
		}

		public string TargetBuildNumber
		{
			get
			{
				return this.GetValueForTag(DivergenceInfo.TargetBuildNumber);
			}
			set
			{
				this.SetValueForTag(DivergenceInfo.TargetBuildNumber, value);
			}
		}

		public string CmdletName
		{
			get
			{
				return this.GetValueForTag(DivergenceInfo.CmdletName);
			}
			set
			{
				this.SetValueForTag(DivergenceInfo.CmdletName, value);
			}
		}

		public string CmdletParameters
		{
			get
			{
				return this.GetValueForTag(DivergenceInfo.CmdletParameters);
			}
			set
			{
				this.SetValueForTag(DivergenceInfo.CmdletParameters, value);
			}
		}

		public string ErrorMessage
		{
			get
			{
				return this.GetValueForTag(DivergenceInfo.ErrorMessage);
			}
			set
			{
				this.SetValueForTag(DivergenceInfo.ErrorMessage, value);
			}
		}

		public string ErrorSymbolicName
		{
			get
			{
				return this.GetValueForTag(DivergenceInfo.ErrorSymbolicName);
			}
			set
			{
				this.SetValueForTag(DivergenceInfo.ErrorSymbolicName, value);
			}
		}

		public string ErrorStringId
		{
			get
			{
				return this.GetValueForTag(DivergenceInfo.ErrorStringId);
			}
			set
			{
				this.SetValueForTag(DivergenceInfo.ErrorStringId, value);
			}
		}

		public string ErrorCategory
		{
			get
			{
				return this.GetValueForTag(DivergenceInfo.ErrorCategory);
			}
			set
			{
				this.SetValueForTag(DivergenceInfo.ErrorCategory, value);
			}
		}

		public string StreamName
		{
			get
			{
				return this.GetValueForTag(DivergenceInfo.StreamName);
			}
			set
			{
				this.SetValueForTag(DivergenceInfo.StreamName, value);
			}
		}

		public string MinDivergenceRetryDatetime
		{
			get
			{
				return this.GetValueForTag(DivergenceInfo.MininumRetryDatetime);
			}
			set
			{
				this.SetValueForTag(DivergenceInfo.MininumRetryDatetime, value);
			}
		}

		public string ServiceInstanceId
		{
			get
			{
				return base.Id.Parent.Parent.Name;
			}
		}

		public override ObjectId Identity
		{
			get
			{
				return new CompoundSyncObjectId(this.ObjectId, new ServiceInstanceId(this.ServiceInstanceId));
			}
		}

		public void LoadDivergenceInfoXml()
		{
			if (this.xmlDoc == null)
			{
				string divergenceInfoXml = this.DivergenceInfoXml;
				this.xmlDoc = new SafeXmlDocument();
				if (string.IsNullOrEmpty(divergenceInfoXml))
				{
					XmlElement newChild = this.xmlDoc.CreateElement("DivergenceInfo");
					this.xmlDoc.AppendChild(newChild);
					return;
				}
				try
				{
					this.xmlDoc.LoadXml(divergenceInfoXml);
				}
				catch (Exception)
				{
					ExTraceGlobals.ADTopologyTracer.TraceError((long)this.GetHashCode(), string.Format("Failed to load Xml blob: \"{0}\". Xml blob will be reset.", divergenceInfoXml));
					this.xmlDoc = null;
					this.DivergenceInfoXml = string.Empty;
					this.LoadDivergenceInfoXml();
				}
			}
		}

		public void SaveDivergenceInfoXml()
		{
			if (this.xmlDoc != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder))
				{
					this.xmlDoc.Save(xmlWriter);
					xmlWriter.Close();
				}
				this.DivergenceInfoXml = stringBuilder.ToString();
			}
		}

		internal override void Initialize()
		{
			this.LoadDivergenceInfoXml();
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return FailedMSOSyncObject.SchemaObject;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return FailedMSOSyncObject.MostDerivedClass;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal string ExternalDirectoryObjectId
		{
			get
			{
				return (string)this.propertyBag[FailedMSOSyncObjectSchema.ObjectId];
			}
			set
			{
				this.propertyBag[FailedMSOSyncObjectSchema.ObjectId] = value;
			}
		}

		internal string ExternalDirectoryOrganizationId
		{
			get
			{
				return (string)this.propertyBag[FailedMSOSyncObjectSchema.ContextId];
			}
			set
			{
				this.propertyBag[FailedMSOSyncObjectSchema.ContextId] = value;
			}
		}

		internal static string GetObjectName(SyncObjectId syncObjectId)
		{
			return string.Format("{0}{1}", FailedMSOSyncObject.GetCompactGuidString(syncObjectId.ContextId), FailedMSOSyncObject.GetCompactGuidString(syncObjectId.ObjectId));
		}

		internal static QueryFilter DivergenceTimestampFilterBuilder(SinglePropertyFilter filter)
		{
			if (!(filter is ComparisonFilter))
			{
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedFilterForProperty(filter.Property.Name, filter.GetType(), typeof(ComparisonFilter)));
			}
			ComparisonFilter comparisonFilter = (ComparisonFilter)filter;
			if (ComparisonOperator.Like == comparisonFilter.ComparisonOperator || ComparisonOperator.IsMemberOf == comparisonFilter.ComparisonOperator)
			{
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedOperatorForProperty(comparisonFilter.Property.Name, comparisonFilter.ComparisonOperator.ToString()));
			}
			long num = ((DateTime)comparisonFilter.PropertyValue).ToFileTimeUtc();
			return new ComparisonFilter(comparisonFilter.ComparisonOperator, FailedMSOSyncObjectSchema.DivergenceTimestampRaw, num);
		}

		internal static object ExternalDirectoryObjectIdGetter(IPropertyBag bag)
		{
			return FailedMSOSyncObject.ExternalDirectoryIdGetter(bag, FailedMSOSyncObjectSchema.RawObjectId);
		}

		internal static void ExternalDirectoryObjectIdSetter(object value, IPropertyBag bag)
		{
			FailedMSOSyncObject.ExternalDirectoryIdSetter(value, bag, FailedMSOSyncObjectSchema.RawObjectId);
		}

		internal static QueryFilter ExternalDirectoryObjectIdFilterBuilder(SinglePropertyFilter filter)
		{
			return FailedMSOSyncObject.ExternalDirectoryObjectIdFilterBuilder(filter, FailedMSOSyncObjectSchema.RawObjectId);
		}

		internal static object ExternalDirectoryOrganizationIdGetter(IPropertyBag bag)
		{
			return FailedMSOSyncObject.ExternalDirectoryIdGetter(bag, FailedMSOSyncObjectSchema.RawContextId);
		}

		internal static void ExternalDirectoryOrganizationIdSetter(object value, IPropertyBag bag)
		{
			FailedMSOSyncObject.ExternalDirectoryIdSetter(value, bag, FailedMSOSyncObjectSchema.RawContextId);
		}

		internal static QueryFilter ExternalDirectoryOrganizationIdFilterBuilder(SinglePropertyFilter filter)
		{
			return FailedMSOSyncObject.ExternalDirectoryObjectIdFilterBuilder(filter, FailedMSOSyncObjectSchema.RawContextId);
		}

		private static object ExternalDirectoryIdGetter(IPropertyBag bag, ADPropertyDefinition externalDirectoryId)
		{
			object obj = bag[externalDirectoryId];
			if (obj != null)
			{
				string text = (string)obj;
				if (text.StartsWith("id:", StringComparison.OrdinalIgnoreCase))
				{
					return text.Substring("id:".Length);
				}
			}
			return obj;
		}

		private string GetValueForTag(DivergenceInfo divergenceInfo)
		{
			if (this.xmlDoc != null && this.xmlDoc.DocumentElement != null)
			{
				XmlNode xmlNode = this.xmlDoc.DocumentElement.SelectSingleNode(Enum.GetName(typeof(DivergenceInfo), divergenceInfo));
				if (xmlNode != null)
				{
					return ((XmlElement)xmlNode).GetAttribute("Value");
				}
			}
			return null;
		}

		private void SetValueForTag(DivergenceInfo divergenceInfo, string value)
		{
			if (this.xmlDoc != null)
			{
				XmlElement xmlElement = this.xmlDoc.DocumentElement;
				if (xmlElement == null)
				{
					xmlElement = this.xmlDoc.CreateElement("DivergenceInfo");
					this.xmlDoc.AppendChild(xmlElement);
				}
				string name = Enum.GetName(typeof(DivergenceInfo), divergenceInfo);
				XmlElement xmlElement2 = (XmlElement)xmlElement.SelectSingleNode(name);
				if (xmlElement2 == null)
				{
					xmlElement2 = this.xmlDoc.CreateElement(name);
					xmlElement.AppendChild(xmlElement2);
				}
				xmlElement2.SetAttribute("Value", value);
			}
		}

		private static void ExternalDirectoryIdSetter(object value, IPropertyBag bag, ADPropertyDefinition externalDirectoryId)
		{
			bag[externalDirectoryId] = value;
		}

		private static string GetMangledId(object rawId)
		{
			return string.Format("{0}{1}", "id:", (string)rawId);
		}

		private static QueryFilter ExternalDirectoryObjectIdFilterBuilder(SinglePropertyFilter filter, ADPropertyDefinition externalDirectoryId)
		{
			if (!(filter is ComparisonFilter) && !(filter is TextFilter))
			{
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedFilterForPropertyMultiple(filter.Property.Name, filter.GetType(), typeof(ComparisonFilter), typeof(TextFilter)));
			}
			ComparisonFilter comparisonFilter = filter as ComparisonFilter;
			QueryFilter queryFilter;
			if (comparisonFilter != null)
			{
				if (comparisonFilter.ComparisonOperator != ComparisonOperator.Equal && ComparisonOperator.NotEqual != comparisonFilter.ComparisonOperator)
				{
					throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedOperatorForProperty(comparisonFilter.Property.Name, comparisonFilter.ComparisonOperator.ToString()));
				}
				queryFilter = new OrFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, externalDirectoryId, comparisonFilter.PropertyValue),
					new ComparisonFilter(ComparisonOperator.Equal, externalDirectoryId, FailedMSOSyncObject.GetMangledId(comparisonFilter.PropertyValue))
				});
				if (comparisonFilter.ComparisonOperator == ComparisonOperator.NotEqual)
				{
					queryFilter = new NotFilter(queryFilter);
				}
			}
			else
			{
				TextFilter textFilter = (TextFilter)filter;
				queryFilter = new OrFilter(new QueryFilter[]
				{
					new TextFilter(externalDirectoryId, textFilter.Text, textFilter.MatchOptions, textFilter.MatchFlags),
					new TextFilter(externalDirectoryId, FailedMSOSyncObject.GetMangledId(textFilter.Text), textFilter.MatchOptions, textFilter.MatchFlags)
				});
			}
			return queryFilter;
		}

		private static string GetCompactGuidString(string guidString)
		{
			return new Guid(guidString).ToString("N");
		}

		private const string RootNodeName = "DivergenceInfo";

		private const string ValueAttribute = "Value";

		private const string IdPrefix = "id:";

		internal static readonly string MostDerivedClass = "msExchMSOForwardSyncDivergence";

		private static readonly FailedMSOSyncObjectSchema SchemaObject = ObjectSchema.GetInstance<FailedMSOSyncObjectSchema>();

		private SafeXmlDocument xmlDoc;
	}
}
