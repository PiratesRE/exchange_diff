using System;
using System.IO;
using System.Xml;

namespace Microsoft.Exchange.Compliance.Xml
{
	internal class SafeXmlDocument : XmlDocument
	{
		public SafeXmlDocument()
		{
		}

		public SafeXmlDocument(XmlImplementation imp)
		{
			throw new NotSupportedException("Not supported");
		}

		public SafeXmlDocument(XmlNameTable nt) : base(nt)
		{
		}

		public override void Load(Stream inStream)
		{
			this.Load(new XmlTextReader(inStream)
			{
				EntityHandling = EntityHandling.ExpandCharEntities,
				DtdProcessing = DtdProcessing.Prohibit,
				XmlResolver = null
			});
		}

		public override void Load(string filename)
		{
			using (XmlTextReader xmlTextReader = new XmlTextReader(filename))
			{
				xmlTextReader.EntityHandling = EntityHandling.ExpandCharEntities;
				xmlTextReader.DtdProcessing = DtdProcessing.Prohibit;
				xmlTextReader.XmlResolver = null;
				this.Load(xmlTextReader);
			}
		}

		public override void Load(TextReader txtReader)
		{
			using (XmlTextReader xmlTextReader = new XmlTextReader(txtReader))
			{
				xmlTextReader.EntityHandling = EntityHandling.ExpandCharEntities;
				xmlTextReader.DtdProcessing = DtdProcessing.Prohibit;
				xmlTextReader.XmlResolver = null;
				this.Load(xmlTextReader);
			}
		}

		public override void Load(XmlReader reader)
		{
			if (reader.Settings != null && reader.Settings.DtdProcessing != DtdProcessing.Prohibit)
			{
				throw new XmlDtdException();
			}
			try
			{
				base.Load(reader);
			}
			catch (XmlException ex)
			{
				if (ex.Message.StartsWith("DTD is prohibited in this XML document.", StringComparison.OrdinalIgnoreCase))
				{
					throw new XmlDtdException();
				}
				throw;
			}
		}

		public override void LoadXml(string xml)
		{
			using (XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(xml)))
			{
				xmlTextReader.EntityHandling = EntityHandling.ExpandCharEntities;
				xmlTextReader.DtdProcessing = DtdProcessing.Prohibit;
				xmlTextReader.XmlResolver = null;
				base.Load(xmlTextReader);
			}
		}
	}
}
