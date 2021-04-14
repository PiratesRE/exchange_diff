using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class SchemaValidator
	{
		public SchemaValidator(SchemaValidator.ThrowSchemaValidationFaultDelegate faultDelegate)
		{
			this.faultDelegate = faultDelegate;
		}

		private static XmlSchemaSet LoadSchemaSet(XmlSchemaSetPreprocessDelegate preprocessDelegate, params string[] schemaNames)
		{
			XmlSchemaSet xmlSchemaSet = new XmlSchemaSet();
			try
			{
				SchemaValidator.LoadSchemaFromResourceFile(xmlSchemaSet, "SOAP1.1.xsd");
				foreach (string resourceName in schemaNames)
				{
					SchemaValidator.LoadSchemaFromResourceFile(xmlSchemaSet, resourceName);
				}
			}
			catch (Exception exception)
			{
				throw ExceptionHandlerBase.HandleInternalServerError(null, exception);
			}
			if (preprocessDelegate != null)
			{
				preprocessDelegate(xmlSchemaSet);
			}
			try
			{
				xmlSchemaSet.Compile();
			}
			catch (XmlSchemaException ex)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError<XmlSchemaException>(0L, "Failed to compile schema set. Error: {0}.", ex);
				throw ExceptionHandlerBase.HandleInternalServerError(null, ex);
			}
			return xmlSchemaSet;
		}

		protected static void LoadSchemaFromResourceFile(XmlSchemaSet schemaSet, string resourceName)
		{
			using (Stream manifestResourceStream = Assembly.GetCallingAssembly().GetManifestResourceStream(resourceName))
			{
				XmlSchema schema = SafeXmlSchema.Read(manifestResourceStream, null);
				schemaSet.Add(schema);
			}
		}

		private static XmlSchema FixupExchangeSchema(XmlSchemaSet xmlSchemaSet, string schemaId, Func<XmlSchema, XmlSchema> fixupFunc)
		{
			foreach (object obj in xmlSchemaSet.Schemas())
			{
				XmlSchema xmlSchema = (XmlSchema)obj;
				if (xmlSchema.Id == schemaId)
				{
					return fixupFunc(xmlSchema);
				}
			}
			return null;
		}

		private static void FixupExchange2010SchemaSet(XmlSchemaSet xmlSchemaSet)
		{
			SchemaValidator.FixupExchangeSchema(xmlSchemaSet, "types", (XmlSchema schema) => SchemaValidator.AddServerVersionSupportToSchema(ExchangeVersionType.Exchange2009, schema));
		}

		private static void FixupExchange2013SchemaSet(XmlSchemaSet xmlSchemaSet)
		{
			SchemaValidator.FixupExchangeSchema(xmlSchemaSet, "types", (XmlSchema schema) => SchemaValidator.AddServerVersionSupportToSchema(ExchangeVersionType.Exchange2012, schema));
		}

		private static void FixupLatestExchangeSchemaSet(XmlSchemaSet xmlSchemaSet)
		{
			SchemaValidator.FixupExchange2013SchemaSet(xmlSchemaSet);
		}

		private static XmlSchema AddServerVersionSupportToSchema(ExchangeVersionType version, XmlSchema schema)
		{
			XmlSchemaSimpleType xmlSchemaSimpleType = schema.Items.OfType<XmlSchemaSimpleType>().First((XmlSchemaSimpleType type) => type.Name == "ExchangeVersionType");
			XmlSchemaSimpleTypeRestriction xmlSchemaSimpleTypeRestriction = (XmlSchemaSimpleTypeRestriction)xmlSchemaSimpleType.Content;
			XmlSchemaEnumerationFacet xmlSchemaEnumerationFacet = new XmlSchemaEnumerationFacet();
			xmlSchemaEnumerationFacet.Value = version.ToString();
			xmlSchemaSimpleTypeRestriction.Facets.Add(xmlSchemaEnumerationFacet);
			SchemaValidator.RemoveFixedAttributeFromRequestServerVersion(schema);
			return schema;
		}

		private static XmlSchema RemoveFixedAttributeFromRequestServerVersion(XmlSchema schema)
		{
			XmlSchemaElement xmlSchemaElement = schema.Items.OfType<XmlSchemaElement>().First((XmlSchemaElement elem) => elem.Name == "RequestServerVersion");
			XmlSchemaComplexType xmlSchemaComplexType = (XmlSchemaComplexType)xmlSchemaElement.SchemaType;
			XmlSchemaAttribute xmlSchemaAttribute = xmlSchemaComplexType.Attributes.OfType<XmlSchemaAttribute>().First((XmlSchemaAttribute attr) => attr.Name == "Version");
			xmlSchemaAttribute.FixedValue = null;
			return schema;
		}

		internal static XmlSchemaSet GetSchemaSetForVersion(ExchangeVersion version)
		{
			if (version.Version > ExchangeVersionType.Exchange2013)
			{
				version = ExchangeVersion.Exchange2013;
			}
			LazyMember<XmlSchemaSet> lazyMember;
			if (!SchemaValidator.cachedSchemaSets.Member.TryGetValue(version, out lazyMember))
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<ExchangeVersion>(0L, "[SchemaValidator.GetSchemaSetForVersion] Schema version is not loaded: {0}", version);
				throw FaultExceptionUtilities.CreateFault(new SchemaValidationException(new ArgumentException("Unsupported schema version"), 0, 0, "Unsupported request version"), FaultParty.Receiver);
			}
			return lazyMember.Member;
		}

		private XmlReaderSettings GetValidatingReaderSettings(ExchangeVersion version, bool treatWarningsAsErrors, bool checkCharacters)
		{
			XmlSchemaSet schemaSetForVersion = SchemaValidator.GetSchemaSetForVersion(version);
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings
			{
				CheckCharacters = checkCharacters,
				ValidationType = ValidationType.Schema,
				IgnoreComments = true,
				IgnoreWhitespace = true,
				IgnoreProcessingInstructions = true,
				CloseInput = true
			};
			if (treatWarningsAsErrors)
			{
				xmlReaderSettings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
			}
			xmlReaderSettings.Schemas = schemaSetForVersion;
			xmlReaderSettings.ValidationEventHandler += this.HandleValidationEvent;
			return xmlReaderSettings;
		}

		internal SoapSavvyReader GetValidatingReader(Stream streamToValidate, ExchangeVersion version, bool treatWarningsAsErrors, bool checkCharacters)
		{
			this.soapSection = SoapSavvyReader.SoapSection.Unknown;
			XmlReaderSettings validatingReaderSettings = this.GetValidatingReaderSettings(version, treatWarningsAsErrors, checkCharacters);
			XmlReader innerReader = SafeXmlFactory.CreateSafeXmlReader(streamToValidate, validatingReaderSettings);
			return new SoapSavvyReader(innerReader, new SoapSavvyReader.OnSoapSectionChange(this.HandleSoapSectionChange));
		}

		private void HandleSoapSectionChange(SoapSavvyReader reader, SoapSavvyReader.SoapSection section)
		{
			this.soapSection = section;
		}

		internal void ValidateMessage(Stream streamToValidate, ExchangeVersion version, bool treatWarningsAsErrors, bool checkCharacters)
		{
			XmlReader validatingReader = this.GetValidatingReader(streamToValidate, version, treatWarningsAsErrors, checkCharacters);
			try
			{
				this.ValidateMessage(validatingReader);
			}
			catch (XmlException ex)
			{
				XmlSchemaException exception = new XmlSchemaException(ex.Message, ex, ex.LineNumber, ex.LinePosition);
				this.faultDelegate(exception, this.soapSection);
			}
			finally
			{
				validatingReader.Close();
			}
		}

		private void ValidateMessage(XmlReader validatingReader)
		{
			while (validatingReader.Read())
			{
			}
		}

		internal SoapSavvyReader.SoapSection SoapSection
		{
			get
			{
				return this.soapSection;
			}
		}

		private void HandleValidationEvent(object sender, ValidationEventArgs e)
		{
			ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string>((long)this.GetHashCode(), "[SchemaValidator::HandleValidationEvent] Validation event received: {0}", e.Message);
			if (e.Exception != null && (this.soapSection == SoapSavvyReader.SoapSection.Body || e.Severity == XmlSeverityType.Error))
			{
				this.faultDelegate(e.Exception, this.soapSection);
			}
		}

		private static LazyMember<XmlSchemaSet> e12SchemaSet = new LazyMember<XmlSchemaSet>(() => SchemaValidator.LoadSchemaSet(null, new string[]
		{
			"types-e12.xsd",
			"messages-e12.xsd"
		}));

		private static LazyMember<XmlSchemaSet> e12SP1SchemaSet = new LazyMember<XmlSchemaSet>(() => SchemaValidator.LoadSchemaSet(null, new string[]
		{
			"types-e12sp1.xsd",
			"messages-e12sp1.xsd"
		}));

		private static LazyMember<XmlSchemaSet> e14SchemaSet = new LazyMember<XmlSchemaSet>(() => SchemaValidator.LoadSchemaSet(new XmlSchemaSetPreprocessDelegate(SchemaValidator.FixupExchange2010SchemaSet), new string[]
		{
			"types-e14.xsd",
			"messages-e14.xsd"
		}));

		private static LazyMember<XmlSchemaSet> e14SP1SchemaSet = new LazyMember<XmlSchemaSet>(() => SchemaValidator.LoadSchemaSet(null, new string[]
		{
			"types-e14sp1.xsd",
			"messages-e14sp1.xsd"
		}));

		private static LazyMember<XmlSchemaSet> e14SP2SchemaSet = new LazyMember<XmlSchemaSet>(() => SchemaValidator.LoadSchemaSet(null, new string[]
		{
			"types-e14sp2.xsd",
			"messages-e14sp2.xsd"
		}));

		private static LazyMember<XmlSchemaSet> e15SchemaSet = new LazyMember<XmlSchemaSet>(() => SchemaValidator.LoadSchemaSet(new XmlSchemaSetPreprocessDelegate(SchemaValidator.FixupExchange2013SchemaSet), new string[]
		{
			"types-e15.xsd",
			"messages-e15.xsd"
		}));

		private static LazyMember<XmlSchemaSet> e15SP1SchemaSet = new LazyMember<XmlSchemaSet>(() => SchemaValidator.LoadSchemaSet(new XmlSchemaSetPreprocessDelegate(SchemaValidator.FixupLatestExchangeSchemaSet), new string[]
		{
			"types.xsd",
			"messages.xsd"
		}));

		private static LazyMember<Dictionary<ExchangeVersion, LazyMember<XmlSchemaSet>>> cachedSchemaSets = new LazyMember<Dictionary<ExchangeVersion, LazyMember<XmlSchemaSet>>>(() => new Dictionary<ExchangeVersion, LazyMember<XmlSchemaSet>>
		{
			{
				ExchangeVersion.Exchange2007,
				SchemaValidator.e12SchemaSet
			},
			{
				ExchangeVersion.Exchange2007SP1,
				SchemaValidator.e12SP1SchemaSet
			},
			{
				ExchangeVersion.Exchange2010,
				SchemaValidator.e14SchemaSet
			},
			{
				ExchangeVersion.Exchange2010SP1,
				SchemaValidator.e14SP1SchemaSet
			},
			{
				ExchangeVersion.Exchange2010SP2,
				SchemaValidator.e14SP2SchemaSet
			},
			{
				ExchangeVersion.Exchange2013,
				SchemaValidator.e15SchemaSet
			}
		});

		private SchemaValidator.ThrowSchemaValidationFaultDelegate faultDelegate;

		private SoapSavvyReader.SoapSection soapSection;

		internal delegate void ThrowSchemaValidationFaultDelegate(XmlSchemaException exception, SoapSavvyReader.SoapSection soapSection);
	}
}
