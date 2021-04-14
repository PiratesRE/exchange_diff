using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.ConfigurationSettings;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings
{
	[Serializable]
	public sealed class SettingsGroup : XMLSerializableDictionary<string, string>
	{
		public SettingsGroup()
		{
			this.Scopes = new List<SettingsScope>();
		}

		public SettingsGroup(string name, SettingsScope scope)
		{
			this.Name = name;
			this.Scopes = new List<SettingsScope>(1)
			{
				scope
			};
			this.LastModified = DateTime.UtcNow;
			this.Priority = scope.DefaultPriority;
		}

		public SettingsGroup(string name, string scopeFilter, int priority)
		{
			this.Name = name;
			this.LastModified = DateTime.UtcNow;
			this.Priority = priority;
			this.ScopeFilter = scopeFilter;
			this.HasExplicitScopeFilter = true;
			this.Scopes = SettingsGroup.AlwaysFalseScope;
		}

		[XmlAttribute(AttributeName = "HasSF")]
		public bool HasExplicitScopeFilter { get; set; }

		[XmlElement(ElementName = "SF")]
		public string ScopeFilter
		{
			get
			{
				if (!this.HasExplicitScopeFilter)
				{
					Exception ex;
					QueryFilter queryFilter = this.TryParseFilter(null, null, out ex);
					return queryFilter.GenerateInfixString(FilterLanguage.Monad);
				}
				return this.scopeFilter;
			}
			set
			{
				this.scopeFilter = value;
				this.parsedFilter = null;
			}
		}

		[XmlAttribute(AttributeName = "ED")]
		public DateTime ExpirationDate { get; set; }

		[XmlAttribute(AttributeName = "Nm")]
		public string Name { get; set; }

		[XmlAttribute(AttributeName = "Pr")]
		public int Priority { get; set; }

		[XmlAttribute(AttributeName = "LM")]
		public DateTime LastModified { get; set; }

		[XmlAttribute(AttributeName = "En")]
		public bool Enabled { get; set; }

		[XmlAttribute(AttributeName = "UB")]
		public string UpdatedBy { get; set; }

		[XmlAttribute(AttributeName = "Rs")]
		public string Reason { get; set; }

		[XmlArrayItem("OSc", typeof(SettingsOrganizationScope))]
		[XmlArrayItem("ASc", typeof(SettingsDagScope))]
		[XmlArrayItem("DSc", typeof(SettingsDatabaseScope))]
		[XmlArrayItem("FSc", typeof(SettingsForestScope))]
		[XmlArrayItem("GSc", typeof(SettingsGenericScope))]
		[XmlArrayItem("PSc", typeof(SettingsProcessScope))]
		[XmlArrayItem("SSc", typeof(SettingsServerScope))]
		[XmlArrayItem("USc", typeof(SettingsUserScope))]
		public List<SettingsScope> Scopes { get; set; }

		public static implicit operator SettingsGroup(string xml)
		{
			return XMLSerializableBase.Deserialize<SettingsGroup>(xml, InternalExchangeSettingsSchema.ConfigurationXMLRaw);
		}

		public SettingsGroup Clone()
		{
			SettingsGroup result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(SettingsGroup));
				xmlSerializer.Serialize(memoryStream, this);
				memoryStream.Seek(0L, SeekOrigin.Begin);
				result = (SettingsGroup)xmlSerializer.Deserialize(memoryStream);
			}
			return result;
		}

		public override string ToString()
		{
			bool flag = true;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(string.Format("[{0}]", this.Name));
			if (!this.Enabled)
			{
				stringBuilder.AppendLine("; ============================================");
				stringBuilder.AppendLine("; WARNING -- this SettingsGroup is disabled!!!");
				stringBuilder.AppendLine("; ============================================");
				flag = false;
			}
			else if (this.ExpirationDate != DateTime.MinValue && DateTime.UtcNow > this.ExpirationDate)
			{
				stringBuilder.AppendLine("; ============================================");
				stringBuilder.AppendLine("; WARNING -- this SettingsGroup has expired!!!");
				stringBuilder.AppendLine("; ============================================");
				flag = false;
			}
			stringBuilder.AppendLine(string.Format("; Priority: {0}; Expiration: {1}; LastModified: {2}; ModifiedBy: {3}", new object[]
			{
				this.Priority,
				(this.ExpirationDate == DateTime.MinValue) ? "(never)" : this.ExpirationDate.ToString(),
				this.LastModified,
				this.UpdatedBy
			}));
			stringBuilder.AppendLine(string.Format("; Scope: {0}", this.ScopeFilter));
			foreach (KeyValuePair<string, string> keyValuePair in this)
			{
				stringBuilder.AppendLine(string.Format("{0}{1}={2}", flag ? "" : "; ", keyValuePair.Key, keyValuePair.Value));
			}
			return stringBuilder.ToString();
		}

		internal bool Matches(IConfigSchema schema, ISettingsContext context)
		{
			if (!this.Enabled || (this.ExpirationDate != DateTime.MinValue && DateTime.UtcNow > this.ExpirationDate))
			{
				return false;
			}
			Exception ex;
			QueryFilter filter = this.TryParseFilter(schema, null, out ex);
			return OpathFilterEvaluator.FilterMatches(filter, null, (PropertyDefinition pdef) => ((SettingsScopeFilterSchema.ScopeFilterPropertyDefinition)pdef).RetrieveValue(context));
		}

		internal void Validate(IConfigSchema schema, QueryParser.EvaluateVariableDelegate evalDelegate)
		{
			if (this.Priority <= -1)
			{
				throw new ConfigurationSettingsException(DirectoryStrings.ConfigurationSettingsInvalidPriority(this.Priority));
			}
			if (!this.HasExplicitScopeFilter)
			{
				if (this.Scopes.Count <= 0)
				{
					throw new ConfigurationSettingsRestrictionExpectedException("");
				}
				HashSet<Type> hashSet = new HashSet<Type>();
				foreach (SettingsScope settingsScope in this.Scopes)
				{
					if (hashSet.Contains(settingsScope.GetType()))
					{
						throw new ConfigurationSettingsDuplicateRestrictionException(settingsScope.GetType().Name.ToString(), this.Name);
					}
					hashSet.Add(settingsScope.GetType());
					settingsScope.Validate(schema);
				}
				if (hashSet.Contains(typeof(SettingsForestScope)) && hashSet.Count > 1)
				{
					this.Scopes.RemoveAll((SettingsScope x) => x is SettingsForestScope);
					return;
				}
			}
			else
			{
				Exception ex;
				this.TryParseFilter(schema, evalDelegate, out ex);
				if (ex != null)
				{
					throw ex;
				}
			}
		}

		private QueryFilter TryParseFilter(IConfigSchema schema, QueryParser.EvaluateVariableDelegate evalDelegate, out Exception ex)
		{
			ex = null;
			if (this.parsedFilter == null)
			{
				if (!this.HasExplicitScopeFilter)
				{
					if (this.Scopes == null || this.Scopes.Count == 0)
					{
						this.parsedFilter = QueryFilter.True;
					}
					else
					{
						List<QueryFilter> list = new List<QueryFilter>(1);
						foreach (SettingsScope settingsScope in this.Scopes)
						{
							list.Add(settingsScope.ConstructScopeFilter(schema));
						}
						this.parsedFilter = (QueryFilter.AndTogether(list.ToArray()) ?? QueryFilter.False);
					}
				}
				else if (string.IsNullOrWhiteSpace(this.ScopeFilter))
				{
					this.parsedFilter = QueryFilter.True;
					this.scopeFilter = null;
				}
				else
				{
					try
					{
						SettingsScopeFilterSchema scopeFilterSchema = SettingsScopeFilterSchema.GetSchemaInstance(schema);
						QueryParser queryParser = new QueryParser(this.ScopeFilter, QueryParser.Capabilities.All, new QueryParser.LookupPropertyDelegate(scopeFilterSchema.LookupSchemaProperty), () => scopeFilterSchema.AllFilterableProperties, evalDelegate, new QueryParser.ConvertValueFromStringDelegate(MailboxProvisioningConstraint.ConvertValueFromString));
						this.parsedFilter = queryParser.ParseTree;
					}
					catch (InvalidCastException ex2)
					{
						ex = ex2;
					}
					catch (ParsingException ex3)
					{
						ex = ex3;
					}
					catch (ArgumentOutOfRangeException ex4)
					{
						ex = ex4;
					}
					if (ex != null)
					{
						ex = new ConfigurationSettingsInvalidScopeFilter(ex.Message, ex);
						this.parsedFilter = QueryFilter.False;
					}
					else
					{
						this.scopeFilter = this.parsedFilter.GenerateInfixString(FilterLanguage.Monad);
					}
				}
			}
			return this.parsedFilter;
		}

		internal const int InvalidPriority = -1;

		private static readonly List<SettingsScope> AlwaysFalseScope = new List<SettingsScope>(1)
		{
			new SettingsProcessScope("This settings group has a ScopeFilter, it cannot be processed by this downlevel server.")
		};

		private string scopeFilter;

		private QueryFilter parsedFilter;
	}
}
