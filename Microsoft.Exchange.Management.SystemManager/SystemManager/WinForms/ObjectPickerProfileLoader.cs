using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.ManagementGUI;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ObjectPickerProfileLoader
	{
		public ObjectPickerProfileLoader() : this(0)
		{
		}

		public ObjectPickerProfileLoader(DataDrivenCategory dataDrivenCategory)
		{
			this.stringIDToLocalizedStringConverter = new StringIDToLocalizedStringConverter();
			this.dataDrivenCategory = dataDrivenCategory;
		}

		public ResultsLoaderProfile GetProfile(string profileName)
		{
			Stream manifestResource = WinformsHelper.GetManifestResource(this.dataDrivenCategory);
			XmlTextReader reader = SafeXmlFactory.CreateSafeXmlTextReader(manifestResource);
			XElement po = XElement.Load(reader);
			DataTable inputTable;
			var <>f__AnonymousType = (from dataSource in po.Elements("ResultsLoader")
			where (string)dataSource.Attribute("Name") == profileName
			select new
			{
				Configuration = this.CreateUIPresentationProfile(po, dataSource.Element("Configuration")),
				DataTable = (from dataTable in dataSource.Elements("DataColumns")
				select new
				{
					DataColumns = dataTable.Elements("Column").Union(dataTable.Elements("RefColumns").SelectMany((XElement r) => (from c in po.Element("RefColumnsSection").Elements("RefColumns")
					where c.Attribute("Name").Value == r.Attribute("Name").Value
					select c).Elements("Column"))),
					PrimaryKey = (this.HasValue(dataTable.Attribute("PrimaryKey")) ? dataTable.Attribute("PrimaryKey").Value.Split(new char[]
					{
						','
					}) : new string[0]),
					NameProperty = (this.HasValue(dataTable.Attribute("NameProperty")) ? dataTable.Attribute("NameProperty").Value : "Name"),
					DataColumnsCalculator = (IDataColumnsCalculator)(this.HasValue(dataTable.Attribute("DataColumnsCalculator")) ? ObjectPickerProfileLoader.CreateObject(dataTable.Attribute("DataColumnsCalculator").Value) : null),
					WholeObjectProperty = (this.HasValue(dataTable.Attribute("WholeObjectProperty")) ? dataTable.Attribute("WholeObjectProperty").Value : string.Empty),
					DistinguishIdentity = (this.HasValue(dataTable.Attribute("DistinguishIdentity")) ? dataTable.Attribute("DistinguishIdentity").Value : string.Empty)
				}).First(),
				InputTable = (this.HasValue(dataSource.Element("InputColumns")) ? (from inputTable in dataSource.Elements("InputColumns")
				select new
				{
					DataColumns = inputTable.Elements("Column").Union(inputTable.Elements("RefColumns").SelectMany((XElement r) => (from c in po.Element("RefColumnsSection").Elements("RefColumns")
					where c.Attribute("Name").Value == r.Attribute("Name").Value
					select c).Elements("Column"))),
					PartialOrderComparer = (IPartialOrderComparer)(this.HasValue(inputTable.Attribute("PartialOrderComparer")) ? ObjectPickerProfileLoader.CreateObject(inputTable.Attribute("PartialOrderComparer").Value) : null)
				}).First() : null),
				DataReader = (from dataReader in dataSource.Elements("DataTableFillers")
				select new
				{
					DataTableFillers = dataReader.Elements(),
					BatchSize = (this.HasValue(dataReader.Attribute("BatchSize")) ? dataReader.Attribute("BatchSize").Value : ResultsLoaderProfile.DefaultBatchSize.ToString()),
					FillType = (this.HasValue(dataReader.Attribute("FillType")) ? ((FillType)Enum.Parse(typeof(FillType), dataReader.Attribute("FillType").Value)) : 0),
					LoadableFromProfilePredicate = (ILoadableFromProfile)(this.HasValue(dataReader.Attribute("LoadableFromProfilePredicate")) ? ObjectPickerProfileLoader.CreateObject(dataReader.Attribute("LoadableFromProfilePredicate").Value) : null),
					PostRefreshAction = (PostRefreshActionBase)(this.HasValue(dataReader.Attribute("PostRefreshAction")) ? ObjectPickerProfileLoader.CreateObject(dataReader.Attribute("PostRefreshAction").Value) : null)
				}).First()
			}).First();
			DataTable resultTable = this.GetTableSchema(<>f__AnonymousType.DataTable.DataColumns, true);
			resultTable.PrimaryKey = (from c in <>f__AnonymousType.DataTable.PrimaryKey
			select resultTable.Columns[c]).ToArray<DataColumn>();
			resultTable.TableName = profileName;
			inputTable = ((<>f__AnonymousType.InputTable != null) ? this.GetTableSchema(<>f__AnonymousType.InputTable.DataColumns, false) : new DataTable());
			ResultsLoaderProfile resultsLoaderProfile = new ResultsLoaderProfile(<>f__AnonymousType.Configuration, inputTable, resultTable)
			{
				Name = profileName,
				InputTablePartialOrderComparer = ((<>f__AnonymousType.InputTable == null) ? null : <>f__AnonymousType.InputTable.PartialOrderComparer),
				WholeObjectProperty = <>f__AnonymousType.DataTable.WholeObjectProperty,
				NameProperty = <>f__AnonymousType.DataTable.NameProperty,
				DataColumnsCalculator = <>f__AnonymousType.DataTable.DataColumnsCalculator,
				LoadableFromProfilePredicate = <>f__AnonymousType.DataReader.LoadableFromProfilePredicate,
				PostRefreshAction = <>f__AnonymousType.DataReader.PostRefreshAction,
				BatchSize = ((string.Compare(<>f__AnonymousType.DataReader.BatchSize, "MaxValue", true) == 0) ? int.MaxValue : int.Parse(<>f__AnonymousType.DataReader.BatchSize)),
				DistinguishIdentity = <>f__AnonymousType.DataTable.DistinguishIdentity,
				FillType = <>f__AnonymousType.DataReader.FillType
			};
			this.AddFillerCollection(resultsLoaderProfile, <>f__AnonymousType.DataReader.DataTableFillers);
			return resultsLoaderProfile;
		}

		private UIPresentationProfile CreateUIPresentationProfile(XElement po, XElement configuration)
		{
			UIPresentationProfile result;
			if (configuration == null)
			{
				result = new UIPresentationProfile();
			}
			else
			{
				IEnumerable<XElement> query = configuration.Elements("DisplayedColumns").Elements("RefColumns").SelectMany((XElement r) => (from c in po.Element("RefColumnsSection").Elements("RefColumns")
				where c.Attribute("Name").Value == r.Attribute("Name").Value
				select c).Elements("Column")).Union(configuration.Elements("DisplayedColumns").Elements("Column"));
				FilterColumnProfile[] filterColumnCollection = new FilterColumnProfile[0];
				ObjectSchema filterObjectSchema = null;
				FilterLanguage filterLanguage = FilterLanguage.Ado;
				XElement xelement = configuration.Element("FilterColumns");
				if (this.HasValue(xelement))
				{
					string text = (string)xelement.Attribute("ObjectSchema");
					filterColumnCollection = this.GetFilterProfile(xelement.Elements("Column"), text);
					filterObjectSchema = ObjectSchema.GetInstance(ObjectSchemaLoader.GetTypeByString(text));
					filterLanguage = (this.HasValue(xelement.Attribute("FilterLanguage")) ? ((FilterLanguage)Enum.Parse(typeof(FilterLanguage), (string)xelement.Attribute("FilterLanguage"))) : FilterLanguage.Ado);
				}
				result = new UIPresentationProfile(this.GetColumnProfile(query), filterColumnCollection)
				{
					DisplayName = this.GetLocalizedString(configuration.Element("Caption").Value),
					HideIcon = (this.HasValue(configuration.Element("HideIcon")) && string.Equals("true", (string)configuration.Element("HideIcon"), StringComparison.InvariantCultureIgnoreCase)),
					ImageProperty = (this.HasValue(configuration.Element("ImageColumn")) ? configuration.Element("ImageColumn").Value : string.Empty),
					UseTreeViewForm = (this.HasValue(configuration.Element("UseTreeView")) && bool.Parse((string)configuration.Element("UseTreeView"))),
					SortProperty = (this.HasValue(configuration.Element("SortProperty")) ? configuration.Element("SortProperty").Value : string.Empty),
					HelpTopic = (this.HasValue(configuration.Element("HelpTopic")) ? configuration.Element("HelpTopic").Value : string.Empty),
					ScopeSupportingLevel = (this.HasValue(configuration.Element("ScopeSupportingLevel")) ? ((ScopeSupportingLevel)Enum.Parse(typeof(ScopeSupportingLevel), (string)configuration.Element("ScopeSupportingLevel"))) : ScopeSupportingLevel.NoScoping),
					FilterObjectSchema = filterObjectSchema,
					FilterLanguage = filterLanguage,
					SerializationLevel = (this.HasValue(configuration.Element("SerializationLevel")) ? ((ExchangeRunspaceConfigurationSettings.SerializationLevel)Enum.Parse(typeof(ExchangeRunspaceConfigurationSettings.SerializationLevel), (string)configuration.Element("SerializationLevel"))) : ExchangeRunspaceConfigurationSettings.SerializationLevel.Partial),
					MultiSelect = (!this.HasValue(configuration.Element("MultiSelect")) || bool.Parse((string)configuration.Element("MultiSelect")))
				};
			}
			return result;
		}

		private void AddFillerCollection(ResultsLoaderProfile objectPickerProfile, IEnumerable<XElement> dataTableFillers)
		{
			foreach (XElement xelement in dataTableFillers)
			{
				if ("MonadAdapterFiller" == xelement.Name || xelement.Name == "DAGNetworkDataFiller" || xelement.Name == "SupervisionListFiller" || xelement.Name == "DatabaseStatusFiller" || xelement.Name == "SecurityIdentifierResolveFiller")
				{
					IExchangeScopeBuilder exchangeScopeBuilder = null;
					if (this.HasValue(xelement.Attribute("ScopeBuilder")))
					{
						string name = (string)xelement.Attribute("ScopeBuilder");
						exchangeScopeBuilder = (IExchangeScopeBuilder)ObjectSchemaLoader.GetTypeByString(name).GetConstructor(new Type[0]).Invoke(new object[0]);
					}
					IExchangeCommandFilterBuilder filterBuilder = null;
					if (this.HasValue(xelement.Attribute("FilterBuilder")))
					{
						string name2 = (string)xelement.Attribute("FilterBuilder");
						filterBuilder = (IExchangeCommandFilterBuilder)ObjectSchemaLoader.GetTypeByString(name2).GetConstructor(new Type[0]).Invoke(new object[0]);
					}
					string resolveCommandText = null;
					if (this.HasValue(xelement.Attribute("ResolveCommand")))
					{
						resolveCommandText = (string)xelement.Attribute("ResolveCommand");
					}
					ExchangeCommandBuilder exchangeCommandBuilder = new ExchangeCommandBuilder(filterBuilder);
					if (exchangeScopeBuilder != null)
					{
						exchangeCommandBuilder.ScopeBuilder = exchangeScopeBuilder;
					}
					if (this.HasValue(xelement.Attribute("SearchType")))
					{
						exchangeCommandBuilder.SearchType = (ExchangeCommandBuilderSearch)Enum.Parse(typeof(ExchangeCommandBuilderSearch), (string)xelement.Attribute("SearchType"));
					}
					if (this.HasValue(xelement.Attribute("UseFilterToResolveNonId")))
					{
						exchangeCommandBuilder.UseFilterToResolveNonId = bool.Parse((string)xelement.Attribute("UseFilterToResolveNonId"));
					}
					exchangeCommandBuilder.NamePropertyFilter = objectPickerProfile.NameProperty;
					if ("MonadAdapterFiller" == xelement.Name)
					{
						MonadAdapterFiller monadAdapterFiller = new MonadAdapterFiller((string)xelement.Attribute("Command"), exchangeCommandBuilder)
						{
							ResolveCommandText = resolveCommandText
						};
						IEnumerable<XElement> enumerable = xelement.Elements("Column");
						foreach (XElement xelement2 in enumerable)
						{
							string value = xelement2.Attribute("Name").Value;
							string value2 = xelement2.Attribute("Value").Value;
							ICustomTextConverter customTextConverter = objectPickerProfile.DataTable.Columns[value].ExtendedProperties.ContainsKey("TextConverter") ? ((ICustomTextConverter)objectPickerProfile.DataTable.Columns[value].ExtendedProperties["TextConverter"]) : new TextConverter();
							monadAdapterFiller.FixedValues[value] = customTextConverter.Parse(objectPickerProfile.DataTable.Columns[value].DataType, value2, null);
						}
						if (xelement.Element("Parameters") != null)
						{
							IEnumerable<XElement> enumerable2 = xelement.Element("Parameters").Elements("Parameter");
							foreach (XElement xelement3 in enumerable2)
							{
								monadAdapterFiller.Parameters[xelement3.Attribute("Name").Value] = xelement3.Attribute("Value").Value;
							}
							IEnumerable<XElement> enumerable3 = xelement.Element("Parameters").Elements("AdditionalParameter");
							foreach (XElement xelement4 in enumerable3)
							{
								monadAdapterFiller.AddtionalParameters.Add(xelement4.Attribute("Name").Value);
							}
						}
						string runnableLambdaExpression = string.Empty;
						if (this.HasValue(xelement.Attribute("RunnableLambdaExpression")))
						{
							runnableLambdaExpression = (string)xelement.Attribute("RunnableLambdaExpression");
						}
						objectPickerProfile.AddTableFiller(monadAdapterFiller, runnableLambdaExpression);
					}
					else if ("SupervisionListFiller" == xelement.Name)
					{
						objectPickerProfile.AddTableFiller(new SupervisionListFiller((string)xelement.Attribute("Command")));
					}
					else if ("DatabaseStatusFiller" == xelement.Name)
					{
						string runnableLambdaExpression2 = string.Empty;
						if (this.HasValue(xelement.Attribute("RunnableLambdaExpression")))
						{
							runnableLambdaExpression2 = (string)xelement.Attribute("RunnableLambdaExpression");
						}
						objectPickerProfile.AddTableFiller(new DatabaseStatusFiller((string)xelement.Attribute("Command"), exchangeCommandBuilder)
						{
							ResolveCommandText = resolveCommandText
						}, runnableLambdaExpression2);
					}
					else if ("DAGNetworkDataFiller" == xelement.Name)
					{
						objectPickerProfile.AddTableFiller(new DAGNetworkDataFiller((string)xelement.Attribute("Command"), exchangeCommandBuilder)
						{
							ResolveCommandText = resolveCommandText
						});
					}
					else
					{
						string sidColumn = (string)xelement.Attribute("SidColumn");
						string userColumn = (string)xelement.Attribute("UserColumn");
						objectPickerProfile.AddTableFiller(new SecurityIdentifierResolveFiller(sidColumn, userColumn));
					}
				}
				else if ("FixedDataFiller" == xelement.Name)
				{
					FixedDataFiller fixedDataFiller = new FixedDataFiller();
					fixedDataFiller.DataTable = objectPickerProfile.DataTable.Clone();
					IEnumerable<XElement> enumerable4 = xelement.Elements("Row");
					foreach (XElement xelement5 in enumerable4)
					{
						DataRow dataRow = fixedDataFiller.DataTable.NewRow();
						IEnumerable<XElement> enumerable5 = xelement5.Elements("Column");
						foreach (XElement xelement6 in enumerable5)
						{
							string value3 = xelement6.Attribute("Name").Value;
							string value4 = xelement6.Attribute("Value").Value;
							ICustomTextConverter customTextConverter2 = fixedDataFiller.DataTable.Columns[value3].ExtendedProperties.ContainsKey("TextConverter") ? ((ICustomTextConverter)fixedDataFiller.DataTable.Columns[value3].ExtendedProperties["TextConverter"]) : new TextConverter();
							dataRow[value3] = customTextConverter2.Parse(fixedDataFiller.DataTable.Columns[value3].DataType, value4, null);
						}
						fixedDataFiller.DataTable.Rows.Add(dataRow);
					}
					objectPickerProfile.AddTableFiller(fixedDataFiller);
				}
			}
		}

		private bool HasValue(XElement element)
		{
			return element != null && !string.IsNullOrEmpty(element.Value);
		}

		private bool HasValue(XAttribute element)
		{
			return element != null && !string.IsNullOrEmpty(element.Value);
		}

		private DataTable GetTableSchema(IEnumerable<XElement> query, bool armedWithLambdaExpression)
		{
			DataTable dataTable = new DataTable();
			foreach (XElement xelement in query)
			{
				DataColumn dataColumn = new DataColumn((string)xelement.Attribute("Name"));
				if (this.HasValue(xelement.Attribute("Type")))
				{
					dataColumn.DataType = ObjectSchemaLoader.GetTypeByString((string)xelement.Attribute("Type"));
				}
				ICustomTextConverter customTextConverter = new TextConverter();
				if (this.HasValue(xelement.Attribute("TextConverter")))
				{
					customTextConverter = (ICustomTextConverter)ObjectPickerProfileLoader.CreateObject(xelement.Attribute("TextConverter").Value);
				}
				else if (dataColumn.DataType.Equals(typeof(LocalizedString)))
				{
					customTextConverter = this.stringIDToLocalizedStringConverter;
				}
				dataColumn.ExtendedProperties["TextConverter"] = customTextConverter;
				if (this.HasValue(xelement.Attribute("DefaultValue")))
				{
					dataColumn.DefaultValue = customTextConverter.Parse(dataColumn.DataType, (string)xelement.Attribute("DefaultValue"), null);
				}
				if (this.HasValue(xelement.Attribute("AutoIncrement")))
				{
					dataColumn.AutoIncrement = bool.Parse((string)xelement.Attribute("AutoIncrement"));
				}
				if (this.HasValue(xelement.Attribute("ExpectedType")))
				{
					dataColumn.ExtendedProperties["ExpectedType"] = ObjectSchemaLoader.GetTypeByString((string)xelement.Attribute("ExpectedType"));
				}
				if (armedWithLambdaExpression && this.HasValue(xelement.Attribute("LambdaExpression")))
				{
					dataColumn.ExtendedProperties["LambdaExpression"] = (string)xelement.Attribute("LambdaExpression");
				}
				dataTable.Columns.Add(dataColumn);
			}
			return dataTable;
		}

		private FilterColumnProfile[] GetFilterProfile(IEnumerable<XElement> query, string filterObjectSchemaTypeName)
		{
			List<FilterColumnProfile> list = new List<FilterColumnProfile>();
			foreach (XElement xelement in query)
			{
				string text = (string)xelement.Attribute("Name");
				Type type = this.HasValue(xelement.Attribute("ColumnType")) ? ObjectSchemaLoader.GetTypeByString((string)xelement.Attribute("ColumnType")) : null;
				string text2 = (string)xelement.Attribute("Operators");
				string value = (string)xelement.Attribute("FormatMode");
				string text3 = this.HasValue(xelement.Attribute("ObjectSchemaField")) ? ((string)xelement.Attribute("ObjectSchemaField")) : text;
				ProviderPropertyDefinition providerPropertyDefinition = (type == null) ? ((ProviderPropertyDefinition)ObjectSchemaLoader.GetStaticField(filterObjectSchemaTypeName, text3)) : FilterControlHelper.GenerateEmptyPropertyDefinition(text3, type);
				List<FilterColumnProfile> list2 = list;
				FilterColumnProfile filterColumnProfile = new FilterColumnProfile();
				filterColumnProfile.ColumnType = type;
				filterColumnProfile.FilterableListSource = this.GetFilterableListSource(providerPropertyDefinition.Type, (string)xelement.Attribute("FilterableListSource"));
				FilterColumnProfile filterColumnProfile2 = filterColumnProfile;
				PropertyFilterOperator[] operators;
				if (!string.IsNullOrEmpty(text2))
				{
					operators = (from opera in text2.Split(new char[]
					{
						','
					})
					select (PropertyFilterOperator)Enum.Parse(typeof(PropertyFilterOperator), opera)).ToArray<PropertyFilterOperator>();
				}
				else
				{
					operators = FilterControlHelper.GetFilterOperators(providerPropertyDefinition.Type);
				}
				filterColumnProfile2.Operators = operators;
				filterColumnProfile.DisplayMember = (string)xelement.Attribute("DisplayMember");
				filterColumnProfile.FormatMode = (string.IsNullOrEmpty(value) ? 0 : ((DisplayFormatMode)Enum.Parse(typeof(DisplayFormatMode), value)));
				filterColumnProfile.Name = text;
				filterColumnProfile.PickerProfile = (string)xelement.Attribute("PickerProfile");
				filterColumnProfile.ValueMember = (string)xelement.Attribute("ValueMember");
				filterColumnProfile.PropertyDefinition = ((providerPropertyDefinition.Name == text) ? providerPropertyDefinition : FilterControlHelper.CopyPropertyDefinition(text, providerPropertyDefinition));
				filterColumnProfile.RefDisplayedColumn = (this.HasValue(xelement.Attribute("RefDisplayedColumn")) ? ((string)xelement.Attribute("RefDisplayedColumn")) : text);
				list2.Add(filterColumnProfile);
			}
			return list.ToArray();
		}

		private ObjectListSource GetFilterableListSource(Type columnType, string filterStrings)
		{
			ObjectListSource result = null;
			if (!string.IsNullOrEmpty(filterStrings))
			{
				if (columnType.IsEnum)
				{
					IEnumerable<object> source = from str in filterStrings.Split(new char[]
					{
						','
					})
					select Enum.Parse(columnType, str);
					result = new EnumListSource(source.ToArray<object>(), columnType);
				}
				else
				{
					IEnumerable<string> source2 = from str in filterStrings.Split(new char[]
					{
						','
					})
					select this.GetLocalizedString(str).ToString();
					result = new ObjectListSource(source2.ToArray<string>());
				}
			}
			return result;
		}

		private ResultsColumnProfile[] GetColumnProfile(IEnumerable<XElement> query)
		{
			List<ResultsColumnProfile> list = new List<ResultsColumnProfile>();
			foreach (XElement xelement in query)
			{
				string text = null;
				if (this.HasValue(xelement.Attribute("Text")))
				{
					text = this.GetLocalizedString((string)xelement.Attribute("Text"));
				}
				bool isDefault = false;
				if (this.HasValue(xelement.Attribute("Mandatory")))
				{
					isDefault = bool.Parse((string)xelement.Attribute("Mandatory"));
				}
				SortMode sortMode = SortMode.NotSpecified;
				if (this.HasValue(xelement.Attribute("SortMode")))
				{
					sortMode = (SortMode)Enum.Parse(typeof(SortMode), (string)xelement.Attribute("SortMode"));
				}
				IComparer customComparer = null;
				if (this.HasValue(xelement.Attribute("CustomComparer")))
				{
					customComparer = (IComparer)ObjectSchemaLoader.GetTypeByString((string)xelement.Attribute("CustomComparer")).GetConstructor(new Type[0]).Invoke(new object[0]);
				}
				ICustomFormatter customFormatter = null;
				if (this.HasValue(xelement.Attribute("CustomFormatter")))
				{
					customFormatter = (ICustomFormatter)ObjectSchemaLoader.GetTypeByString((string)xelement.Attribute("CustomFormatter")).GetConstructor(new Type[0]).Invoke(new object[0]);
				}
				if (customFormatter == null)
				{
					customFormatter = TextConverter.DefaultConverter;
				}
				IFormatProvider formatProvider = null;
				if (this.HasValue(xelement.Attribute("FormatProvider")))
				{
					formatProvider = (IFormatProvider)ObjectSchemaLoader.GetTypeByString((string)xelement.Attribute("FormatProvider")).GetConstructor(new Type[0]).Invoke(new object[0]);
				}
				string formatString = null;
				if (this.HasValue(xelement.Attribute("FormatString")))
				{
					formatString = (string)xelement.Attribute("FormatString");
				}
				string defaultEmptyText = "";
				if (this.HasValue(xelement.Attribute("DefaultEmptyText")))
				{
					defaultEmptyText = this.GetLocalizedString((string)xelement.Attribute("DefaultEmptyText"));
				}
				IToColorFormatter colorFormatter = null;
				if (this.HasValue(xelement.Attribute("ColorFormatter")))
				{
					colorFormatter = (IToColorFormatter)ObjectSchemaLoader.GetTypeByString((string)xelement.Attribute("ColorFormatter")).GetConstructor(new Type[0]).Invoke(new object[0]);
				}
				list.Add(new ResultsColumnProfile((string)xelement.Attribute("Name"), isDefault, text)
				{
					SortMode = sortMode,
					CustomComparer = customComparer,
					CustomFormatter = customFormatter,
					FormatProvider = formatProvider,
					FormatString = formatString,
					DefaultEmptyText = defaultEmptyText,
					ColorFormatter = colorFormatter
				});
			}
			return list.ToArray();
		}

		private LocalizedString GetLocalizedString(string stringID)
		{
			return (LocalizedString)this.stringIDToLocalizedStringConverter.Parse(typeof(LocalizedString), stringID, null);
		}

		private static object CreateObject(string type)
		{
			return ObjectSchemaLoader.GetTypeByString(type).GetConstructor(new Type[0]).Invoke(new object[0]);
		}

		private ICustomTextConverter stringIDToLocalizedStringConverter;

		private DataDrivenCategory dataDrivenCategory;
	}
}
