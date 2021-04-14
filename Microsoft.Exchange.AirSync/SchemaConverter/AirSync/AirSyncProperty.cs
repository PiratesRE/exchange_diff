using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.AirSync.Wbxml;
using Microsoft.Exchange.Diagnostics.Components.SchemaConverter;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	[Serializable]
	internal abstract class AirSyncProperty : PropertyBase
	{
		public AirSyncProperty(string xmlNodeNamespace, string airSyncTagName, bool requiresClientSupport)
		{
			if (string.IsNullOrEmpty(airSyncTagName))
			{
				throw new ArgumentNullException("airSyncTagName");
			}
			this.airSyncTagNames = new string[1];
			bool[] array = new bool[1];
			this.tagBound = array;
			this.airSyncTagNames[0] = airSyncTagName;
			this.xmlNodeNamespace = xmlNodeNamespace;
			this.requiresClientSupport = requiresClientSupport;
		}

		public AirSyncProperty(string xmlNodeNamespace, string airSyncTagName1, string airSyncTagName2, bool requiresClientSupport)
		{
			if (string.IsNullOrEmpty(airSyncTagName1))
			{
				throw new ArgumentNullException("airSyncTagName1");
			}
			if (string.IsNullOrEmpty(airSyncTagName2))
			{
				throw new ArgumentNullException("airSyncTagName2");
			}
			this.airSyncTagNames = new string[2];
			bool[] array = new bool[2];
			this.tagBound = array;
			this.airSyncTagNames[0] = airSyncTagName1;
			this.airSyncTagNames[1] = airSyncTagName2;
			this.xmlNodeNamespace = xmlNodeNamespace;
			this.requiresClientSupport = requiresClientSupport;
		}

		public AirSyncProperty(string xmlNodeNamespace, string[] airSyncTagNames, bool requiresClientSupport)
		{
			if (airSyncTagNames == null)
			{
				throw new ArgumentNullException("airSyncTagNames");
			}
			foreach (string value in airSyncTagNames)
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentNullException("A tag in airSyncTagNames");
				}
			}
			this.airSyncTagNames = airSyncTagNames;
			this.tagBound = new bool[airSyncTagNames.Length];
			this.xmlNodeNamespace = xmlNodeNamespace;
			this.requiresClientSupport = requiresClientSupport;
		}

		public string[] AirSyncTagNames
		{
			get
			{
				return this.airSyncTagNames;
			}
		}

		public bool ClientChangeTracked
		{
			get
			{
				return this.clientChangeTracked;
			}
			set
			{
				this.clientChangeTracked = value;
			}
		}

		public string Namespace
		{
			get
			{
				return this.xmlNodeNamespace;
			}
		}

		public IDictionary Options
		{
			get
			{
				return this.options;
			}
			set
			{
				this.options = value;
			}
		}

		public bool RequiresClientSupport
		{
			get
			{
				return this.requiresClientSupport;
			}
		}

		protected XmlNode XmlNode
		{
			get
			{
				return this.xmlNode;
			}
			set
			{
				this.xmlNode = value;
			}
		}

		protected XmlNode XmlParentNode
		{
			get
			{
				return this.xmlParentNode;
			}
			set
			{
				this.xmlParentNode = value;
			}
		}

		protected XmlNamespaceManager NamespaceManager
		{
			get
			{
				if (this.nsmgr == null && this.xmlNode != null)
				{
					this.nsmgr = new XmlNamespaceManager(this.xmlNode.OwnerDocument.NameTable);
					this.nsmgr.AddNamespace("X", this.xmlNodeNamespace);
				}
				return this.nsmgr;
			}
		}

		public void Bind(XmlNode xmlNode)
		{
			if (xmlNode == null)
			{
				throw new ArgumentNullException("xmlNode");
			}
			bool flag = false;
			int i = 0;
			while (i < this.airSyncTagNames.Length)
			{
				if (xmlNode.Name == this.airSyncTagNames[i])
				{
					if (!this.tagBound[i])
					{
						this.tagBound[i] = true;
						flag = true;
						break;
					}
					throw new ConversionException("There are multiple nodes with name " + xmlNode.Name);
				}
				else
				{
					i++;
				}
			}
			if (!flag)
			{
				throw new ConversionException("Cannot bind property to node " + xmlNode.InnerXml);
			}
			if (this.xmlNode == null)
			{
				this.xmlNode = xmlNode;
				this.xmlParentNode = xmlNode.ParentNode;
				base.State = PropertyState.Modified;
			}
		}

		public void BindToParent(XmlNode xmlParentNode)
		{
			if (xmlParentNode == null)
			{
				throw new ArgumentNullException("xmlParentNode");
			}
			base.State = PropertyState.Uninitialized;
			this.xmlParentNode = xmlParentNode;
			this.xmlNode = null;
			AirSyncDiagnostics.Assert(PropertyState.Uninitialized == base.State);
		}

		public override void CopyFrom(IProperty srcProperty)
		{
			if (srcProperty == null)
			{
				throw new ArgumentNullException("srcProperty");
			}
			if (srcProperty.SchemaLinkId != base.SchemaLinkId)
			{
				throw new ConversionException("Schema link id's must match in CopyFrom()");
			}
			if (this.airSyncTagNames[0] == null)
			{
				throw new ConversionException("this.airSyncTagNames[0] is null");
			}
			if (!this.IsBound())
			{
				throw new ConversionException(string.Format(CultureInfo.InvariantCulture, "Property must be bound before we can copy to it; PropertyTag: {0}, NS: {1}", new object[]
				{
					this.airSyncTagNames[0],
					this.xmlNodeNamespace
				}));
			}
			PropertyState state = srcProperty.State;
			if (state != PropertyState.Uninitialized)
			{
				switch (state)
				{
				case PropertyState.Unmodified:
					goto IL_97;
				case PropertyState.NotSupported:
					return;
				}
				if (this.xmlNode != null)
				{
					this.xmlParentNode.RemoveChild(this.xmlNode);
					this.xmlNode = null;
				}
				if (srcProperty.State == PropertyState.SetToDefault)
				{
					this.InternalSetToDefault(srcProperty);
					return;
				}
				this.InternalCopyFrom(srcProperty);
				return;
			}
			IL_97:
			throw new ConversionException("Unexpected property state " + srcProperty.State + " in AirSyncProperty.CopyFrom()");
		}

		public bool IsBound()
		{
			return this.xmlParentNode != null;
		}

		public bool IsBoundToEmptyTag()
		{
			if (this.xmlNode == null)
			{
				AirSyncDiagnostics.TraceInfo<AirSyncProperty>(ExTraceGlobals.AirSyncTracer, this, "AirSyncProperty={0} IsBoundToEmptyTag=false due to null xmlNode", this);
				return false;
			}
			if (this.xmlNode.ChildNodes.Count == 0 && this.xmlNode.InnerText.Length == 0)
			{
				AirSyncDiagnostics.TraceInfo<AirSyncProperty>(ExTraceGlobals.AirSyncTracer, this, "AirSyncProperty={0} IsBoundToEmptyTag=true", this);
				return true;
			}
			AirSyncDiagnostics.TraceInfo<AirSyncProperty, int, int>(ExTraceGlobals.AirSyncTracer, this, "AirSyncProperty={0} IsBoundToEmptyTag=false, ChildNodes count={1}, InnerText length={2}", this, this.xmlNode.ChildNodes.Count, this.xmlNode.InnerText.Length);
			return false;
		}

		public void OutputEmptyNode()
		{
			if (!this.IsBound())
			{
				throw new ConversionException("Cannot output empty node without being bound to a parent");
			}
			XmlNode newChild = this.xmlParentNode.OwnerDocument.CreateElement(this.airSyncTagNames[0], this.xmlNodeNamespace);
			this.xmlParentNode.AppendChild(newChild);
		}

		public override string ToString()
		{
			string text = this.CreateDebugString();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("(Base: {0}, ", base.ToString());
			stringBuilder.AppendFormat("xmlNodeNamespace: {0}, ", (this.xmlNodeNamespace != null) ? this.xmlNodeNamespace : "null");
			stringBuilder.AppendFormat("State: {0}, ", base.State);
			stringBuilder.AppendFormat("SchemaLinkId: {0}, ", base.SchemaLinkId);
			stringBuilder.AppendFormat("xmlParentNode: {0}, ", (this.xmlParentNode != null && this.xmlParentNode.Name != null) ? this.xmlParentNode.Name : "null");
			if (this.AirSyncTagNames != null)
			{
				for (int i = 0; i < this.airSyncTagNames.Length; i++)
				{
					stringBuilder.AppendFormat("airSyncTagNames[{0}]: {1}, ", i, (this.airSyncTagNames[i] != null) ? this.airSyncTagNames[i] : "null");
				}
			}
			stringBuilder.AppendFormat("clientChangeTracked: {0}, ", this.clientChangeTracked);
			stringBuilder.AppendFormat("AdditionalInfo ({0})) ", (text != null) ? text : "null");
			return stringBuilder.ToString();
		}

		public override void Unbind()
		{
			try
			{
				this.xmlNode = null;
				this.xmlParentNode = null;
				this.nsmgr = null;
				base.State = PropertyState.Uninitialized;
				for (int i = 0; i < this.tagBound.Length; i++)
				{
					this.tagBound[i] = false;
				}
			}
			finally
			{
				base.Unbind();
			}
		}

		protected void AppendChildCData(XmlNode parentNode, string elementName, string value)
		{
			AirSyncDiagnostics.TraceInfo<string, string, string>(ExTraceGlobals.AirSyncTracer, this, "Creating CData Node={0} Value={1} to {2}", elementName, AirSyncProperty.GetValueToTrace(value, elementName), parentNode.Name);
			XmlNode xmlNode = this.xmlParentNode.OwnerDocument.CreateElement(elementName, this.xmlNodeNamespace);
			XmlCDataSection newChild = this.xmlParentNode.OwnerDocument.CreateCDataSection(value);
			xmlNode.AppendChild(newChild);
			parentNode.AppendChild(xmlNode);
		}

		protected void AppendChildNode(string elementName, string value)
		{
			AirSyncDiagnostics.TraceInfo<string, string>(ExTraceGlobals.AirSyncTracer, this, "Creating Node={0} Value={1}", elementName, AirSyncProperty.GetValueToTrace(value, elementName));
			XmlNode xmlNode = this.xmlParentNode.OwnerDocument.CreateElement(elementName, this.xmlNodeNamespace);
			xmlNode.InnerText = value;
			this.xmlNode.AppendChild(xmlNode);
		}

		protected void AppendChildNode(XmlNode parentNode, string elementName, string value)
		{
			this.AppendChildNode(parentNode, elementName, value, this.xmlNodeNamespace);
		}

		protected void AppendChildNode(XmlNode parentNode, string elementName, string value, string nodeNamespace)
		{
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.AirSyncTracer, this, "Appending Namespace = {0} Node={1} Value={2} to {3}", new object[]
			{
				nodeNamespace,
				elementName,
				AirSyncProperty.GetValueToTrace(value, elementName),
				parentNode.Name
			});
			XmlNode xmlNode = this.xmlParentNode.OwnerDocument.CreateElement(elementName, nodeNamespace);
			xmlNode.InnerText = value;
			parentNode.AppendChild(xmlNode);
		}

		protected XmlNode AppendChildNode(XmlNode parentNode, string elementName, Stream streamData, long dataSize, XmlNodeType orginalNodeType)
		{
			AirSyncDiagnostics.TraceInfo<string, string>(ExTraceGlobals.AirSyncTracer, this, "Appending Node={0} to {1}", elementName, parentNode.Name);
			AirSyncBlobXmlNode airSyncBlobXmlNode = new AirSyncBlobXmlNode(null, elementName, this.xmlNodeNamespace, this.xmlParentNode.OwnerDocument);
			airSyncBlobXmlNode.Stream = streamData;
			airSyncBlobXmlNode.StreamDataSize = dataSize;
			airSyncBlobXmlNode.OriginalNodeType = orginalNodeType;
			parentNode.AppendChild(airSyncBlobXmlNode);
			return airSyncBlobXmlNode;
		}

		protected XmlNode ReplaceValueOrCreateNode(XmlNode parentNode, string nodeName, string value)
		{
			XmlNode xmlNode = parentNode.SelectSingleNode("X:" + nodeName, this.NamespaceManager);
			if (xmlNode != null)
			{
				xmlNode.InnerText = value;
			}
			else
			{
				this.AppendChildNode(parentNode, nodeName, value);
			}
			return xmlNode;
		}

		protected bool RemoveNode(XmlNode parentNode, string nodeName)
		{
			XmlNode xmlNode = parentNode.SelectSingleNode("X:" + nodeName, this.NamespaceManager);
			if (xmlNode != null)
			{
				parentNode.RemoveChild(xmlNode);
				return true;
			}
			return false;
		}

		protected void CreateAirSyncNode(string strData)
		{
			this.CreateAirSyncNode(strData, false);
		}

		protected void CreateAirSyncNode(string strData, bool allowEmptyNode)
		{
			AirSyncDiagnostics.TraceInfo<string, string>(ExTraceGlobals.AirSyncTracer, this, "Creating Node={0} Value={1}", this.airSyncTagNames[0], AirSyncProperty.GetValueToTrace(strData, this.airSyncTagNames[0]));
			if (allowEmptyNode || !string.IsNullOrEmpty(strData))
			{
				this.xmlNode = this.xmlParentNode.OwnerDocument.CreateElement(this.airSyncTagNames[0], this.xmlNodeNamespace);
				this.xmlNode.InnerText = strData;
				this.xmlParentNode.AppendChild(this.xmlNode);
			}
		}

		protected void CreateAirSyncNode(string airSyncTagName, string strData)
		{
			AirSyncDiagnostics.TraceInfo<string, string>(ExTraceGlobals.AirSyncTracer, this, "Creating Node={0} Value={1}", airSyncTagName, AirSyncProperty.GetValueToTrace(strData, airSyncTagName));
			this.xmlNode = this.xmlParentNode.OwnerDocument.CreateElement(airSyncTagName, this.xmlNodeNamespace);
			this.xmlNode.InnerText = strData;
			this.xmlParentNode.AppendChild(this.xmlNode);
		}

		protected void CreateAirSyncNode(string airSyncTagName, Stream mimeData, long mimeSize, XmlNodeType originalNodeType)
		{
			AirSyncDiagnostics.TraceInfo<string>(ExTraceGlobals.AirSyncTracer, this, "Creating Node={0}", airSyncTagName);
			AirSyncBlobXmlNode airSyncBlobXmlNode = new AirSyncBlobXmlNode(null, airSyncTagName, this.xmlNodeNamespace, this.xmlParentNode.OwnerDocument);
			airSyncBlobXmlNode.Stream = mimeData;
			airSyncBlobXmlNode.StreamDataSize = mimeSize;
			airSyncBlobXmlNode.OriginalNodeType = originalNodeType;
			this.xmlParentNode.AppendChild(airSyncBlobXmlNode);
		}

		protected void CreateAirSyncNode(string airSyncTagName, byte[] data)
		{
			AirSyncDiagnostics.TraceInfo<string>(ExTraceGlobals.AirSyncTracer, this, "Creating Node={0}", airSyncTagName);
			AirSyncBlobXmlNode airSyncBlobXmlNode = new AirSyncBlobXmlNode(null, airSyncTagName, this.xmlNodeNamespace, this.xmlParentNode.OwnerDocument);
			airSyncBlobXmlNode.ByteArray = data;
			this.xmlParentNode.AppendChild(airSyncBlobXmlNode);
		}

		protected virtual string CreateDebugString()
		{
			return string.Empty;
		}

		protected abstract void InternalCopyFrom(IProperty srcProperty);

		protected virtual void InternalSetToDefault(IProperty srcProperty)
		{
		}

		private static string GetValueToTrace(string value, string elementName)
		{
			if (!GlobalSettings.EnableMailboxLoggingVerboseMode)
			{
				foreach (string text in AirSyncUtility.PiiTags)
				{
					if (text.Equals(elementName, StringComparison.Ordinal))
					{
						return string.Format("{0} bytes", (value == null) ? 0 : value.Length);
					}
				}
			}
			return value;
		}

		private string[] airSyncTagNames;

		private bool clientChangeTracked;

		[NonSerialized]
		private IDictionary options;

		[NonSerialized]
		private XmlNode xmlNode;

		private string xmlNodeNamespace;

		private XmlNamespaceManager nsmgr;

		[NonSerialized]
		private XmlNode xmlParentNode;

		private bool requiresClientSupport;

		private bool[] tagBound;
	}
}
