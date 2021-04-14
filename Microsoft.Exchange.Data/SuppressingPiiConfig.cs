using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data
{
	[XmlRoot(ElementName = "PiiDataRedaction")]
	public class SuppressingPiiConfig
	{
		[XmlAttribute(AttributeName = "enable")]
		public bool Enable { get; set; }

		[XmlAttribute(AttributeName = "redactorClassName")]
		public string RedactorClassName { get; set; }

		[XmlArrayItem(ElementName = "Schema")]
		[XmlArray(ElementName = "PiiSchemas")]
		public SchemaPiiPropertyDefinition[] SchemaPiiDefinitions
		{
			get
			{
				return this.schemaPiiDefinitions;
			}
			set
			{
				this.schemaPiiDefinitions = value;
				this.redactorDict = this.CreateRedactorDictionary(this.schemaPiiDefinitions);
			}
		}

		[XmlArray(ElementName = "PiiStrings")]
		[XmlArrayItem(ElementName = "Resource")]
		public PiiResource[] PiiResourceDefinitions
		{
			get
			{
				return this.piiResourceDefinitions;
			}
			set
			{
				this.piiResourceDefinitions = value;
				this.piiStringDict = this.CreatePiiStringDictionary(this.piiResourceDefinitions);
			}
		}

		[XmlIgnore]
		public HashSet<Type> ExceptionSchemaTypes { get; private set; }

		[XmlIgnore]
		public HashSet<PropertyDefinition> PropertiesNeedInPiiMap { get; private set; }

		[XmlArray(ElementName = "ExceptionSchemas")]
		[XmlArrayItem(ElementName = "Schema")]
		public SchemaPiiPropertyDefinition[] ExceptionSchemas
		{
			get
			{
				return this.exceptionSchemas;
			}
			set
			{
				this.exceptionSchemas = value;
				this.ExceptionSchemaTypes = new HashSet<Type>(this.ResolveTypeNames(from x in this.exceptionSchemas
				select x.Name));
				this.ExceptionSchemaTypes.ExceptWith(this.definedSchemaHasPii);
			}
		}

		internal string DeserializationError
		{
			get
			{
				if (this.deserializationError.Length != 0)
				{
					return this.deserializationError.ToString();
				}
				return null;
			}
		}

		public bool TryGetRedactor(PropertyDefinition property, out MethodInfo redactor)
		{
			return this.redactorDict.TryGetValue(property, out redactor);
		}

		public int[] GetPiiStringParams(string fullName)
		{
			int[] result;
			this.piiStringDict.TryGetValue(fullName, out result);
			return result;
		}

		public bool NeedAddIntoPiiMap(PropertyDefinition property, object original)
		{
			return original is ObjectId || original is ProxyAddress || original is SmtpAddress || this.PropertiesNeedInPiiMap.Contains(property) || (original is string && SmtpAddress.IsValidSmtpAddress((string)original));
		}

		private Dictionary<PropertyDefinition, MethodInfo> CreateRedactorDictionary(SchemaPiiPropertyDefinition[] piiDefinitions)
		{
			Dictionary<PropertyDefinition, MethodInfo> dictionary = new Dictionary<PropertyDefinition, MethodInfo>();
			this.PropertiesNeedInPiiMap = new HashSet<PropertyDefinition>();
			this.InitLoadedExchangeTypeDict();
			Type type = this.ResolveTypeName(this.RedactorClassName);
			if (type == null)
			{
				this.deserializationError.AppendLine(string.Format("Failed to resolve redactor class name: {0}. PII redaction feature will be disabled.", this.RedactorClassName));
				return dictionary;
			}
			List<Type> list = new List<Type>();
			foreach (SchemaPiiPropertyDefinition schemaPiiPropertyDefinition in piiDefinitions)
			{
				Type type2 = this.ResolveTypeName(schemaPiiPropertyDefinition.Name);
				if (type2 == null)
				{
					this.deserializationError.AppendLine(string.Format("Failed to resolve schema name: {0}. PII redaction on this schema will be skipped.", schemaPiiPropertyDefinition.Name));
				}
				else
				{
					list.Add(type2);
					foreach (PiiPropertyDefinition piiPropertyDefinition in schemaPiiPropertyDefinition.PiiProperties)
					{
						FieldInfo field = type2.GetTypeInfo().GetField(piiPropertyDefinition.Name);
						if (field == null)
						{
							this.deserializationError.AppendLine(string.Format("Cannot resolve property {0} in schema {1}, PII redaction on this property will be skipped.", piiPropertyDefinition.Name, schemaPiiPropertyDefinition.Name));
						}
						else
						{
							PropertyDefinition propertyDefinition = field.GetValue(null) as PropertyDefinition;
							if (propertyDefinition == null)
							{
								this.deserializationError.AppendLine(string.Format("Property {0} in schema {1} isn't a PropertyDefinition type.", piiPropertyDefinition.Name, schemaPiiPropertyDefinition.Name));
							}
							else
							{
								MethodInfo methodInfo = null;
								string text = piiPropertyDefinition.Redactor;
								if (piiPropertyDefinition.Enumerable)
								{
									text = "RedactWithoutHashing";
								}
								else if (string.IsNullOrEmpty(text))
								{
									text = "Redact";
								}
								try
								{
									if (propertyDefinition.Type.IsArray)
									{
										methodInfo = type.GetMethod(text, new Type[]
										{
											propertyDefinition.Type,
											typeof(string[]).MakeByRefType(),
											typeof(string[]).MakeByRefType()
										});
									}
									else
									{
										methodInfo = type.GetMethod(text, new Type[]
										{
											propertyDefinition.Type,
											typeof(string).MakeByRefType(),
											typeof(string).MakeByRefType()
										});
									}
								}
								catch (Exception ex)
								{
									this.deserializationError.AppendLine(string.Format("Specified redactor {0} could not be found. Reason: {1}", text, ex.ToString()));
								}
								if (methodInfo == null)
								{
									this.deserializationError.AppendLine(string.Format("Failed to find a redactor for PII property {0} in schema {1}. PII redaction on this property will be skipped.", piiPropertyDefinition.Name, schemaPiiPropertyDefinition.Name));
								}
								else
								{
									dictionary[propertyDefinition] = methodInfo;
								}
								if (piiPropertyDefinition.AddIntoMap)
								{
									this.PropertiesNeedInPiiMap.Add(propertyDefinition);
								}
							}
						}
					}
				}
			}
			this.definedSchemaHasPii = list.ToArray();
			return dictionary;
		}

		private Dictionary<string, int[]> CreatePiiStringDictionary(PiiResource[] piiResources)
		{
			Dictionary<string, int[]> dictionary = new Dictionary<string, int[]>();
			if (piiResources != null)
			{
				foreach (PiiResource piiResource in piiResources)
				{
					foreach (PiiLocString piiLocString in piiResource.LocStrings)
					{
						if (string.IsNullOrWhiteSpace(piiLocString.Id))
						{
							this.deserializationError.AppendLine("String id is required.");
						}
						else
						{
							string key = piiResource.Name + piiLocString.Id;
							dictionary[key] = piiLocString.Parameters;
						}
					}
				}
			}
			return dictionary;
		}

		private IEnumerable<Type> ResolveTypeNames(IEnumerable<string> names)
		{
			this.InitLoadedExchangeTypeDict();
			using (IEnumerator<string> enumerator = names.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string name = enumerator.Current;
					foreach (Type matchedType in from x in this.loadedExchangeTypeDict
					where Regex.IsMatch(x.Key, name)
					select x.Value)
					{
						yield return matchedType;
					}
				}
			}
			yield break;
		}

		private Type ResolveTypeName(string name)
		{
			Type result;
			if (!this.loadedExchangeTypeDict.TryGetValue(name, out result))
			{
				this.deserializationError.AppendLine(string.Format("Cannot resolve type {0}, please check the spell of the type name.", name));
			}
			return result;
		}

		private void InitLoadedExchangeTypeDict()
		{
			if (this.loadedExchangeTypeDict == null)
			{
				this.loadedExchangeTypeDict = new Dictionary<string, Type>();
				foreach (Type[] array in from x in AppDomain.CurrentDomain.GetAssemblies()
				where x.FullName.StartsWith("Microsoft.Exchange.")
				select SerializationTypeBinder.GetLoadedTypes(x) into x
				where x != null
				select x)
				{
					foreach (Type type in array)
					{
						this.loadedExchangeTypeDict[type.FullName] = type;
					}
				}
			}
		}

		public const string RedactedDataMark = "REDACTED";

		private StringBuilder deserializationError = new StringBuilder();

		private Dictionary<PropertyDefinition, MethodInfo> redactorDict;

		private Dictionary<string, int[]> piiStringDict;

		private SchemaPiiPropertyDefinition[] schemaPiiDefinitions;

		private PiiResource[] piiResourceDefinitions;

		private SchemaPiiPropertyDefinition[] exceptionSchemas;

		private Dictionary<string, Type> loadedExchangeTypeDict;

		private Type[] definedSchemaHasPii;

		public static readonly string RedactedDataPrefix = "REDACTED" + '-';
	}
}
