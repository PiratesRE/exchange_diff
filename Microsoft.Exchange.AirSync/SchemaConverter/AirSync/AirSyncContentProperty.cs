using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.SchemaConverter;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	internal class AirSyncContentProperty : AirSyncProperty, IContentProperty, IMIMEDataProperty, IMIMERelatedProperty, IProperty, IAirSyncAttachments
	{
		public AirSyncContentProperty(string xmlNodeNamespace, string airSyncTagName, bool requiresClientSupport) : base(xmlNodeNamespace, airSyncTagName, requiresClientSupport)
		{
			base.ClientChangeTracked = true;
		}

		public IEnumerable<AirSyncAttachmentInfo> Attachments
		{
			get
			{
				return this.attachments;
			}
		}

		public Stream Body
		{
			get
			{
				XmlNode xmlNode = base.XmlNode.SelectSingleNode("X:Data", base.NamespaceManager);
				if (xmlNode == null)
				{
					return null;
				}
				return new MemoryStream(Encoding.UTF8.GetBytes(xmlNode.InnerText));
			}
		}

		public bool IsOnSMIMEMessage
		{
			get
			{
				throw new NotImplementedException("IsOnSMIMEMessage should not be called");
			}
		}

		public Stream MIMEData { get; set; }

		public long MIMESize { get; set; }

		public bool IsIrmErrorMessage
		{
			get
			{
				throw new NotImplementedException("IsIrmErrorMessage should not be called");
			}
		}

		public void PreProcessProperty()
		{
			throw new NotImplementedException("PreProcessProperty should not be called");
		}

		public void PostProcessProperty()
		{
			throw new NotImplementedException("PostProcessProperty should not be called");
		}

		public long Size
		{
			get
			{
				XmlNode xmlNode = base.XmlNode.SelectSingleNode("X:EstimatedDataSize", base.NamespaceManager);
				long result;
				if (xmlNode != null && !string.IsNullOrEmpty(xmlNode.InnerText) && long.TryParse(xmlNode.InnerText, out result))
				{
					return result;
				}
				return 0L;
			}
		}

		protected BodyType BodyType
		{
			get
			{
				return this.bodyType;
			}
		}

		protected Stream Data
		{
			get
			{
				return this.data;
			}
		}

		protected bool Truncated
		{
			get
			{
				return this.truncated;
			}
			set
			{
				this.truncated = value;
			}
		}

		public Stream GetData(BodyType type, long truncationSize, out long totalDataSize, out IEnumerable<AirSyncAttachmentInfo> attachments)
		{
			throw new ConversionException("Conversion of BodyType needs to be handled from the XSO side");
		}

		public BodyType GetNativeType()
		{
			XmlNode xmlNode = base.XmlNode.SelectSingleNode("X:Type", base.NamespaceManager);
			if (xmlNode == null)
			{
				return BodyType.None;
			}
			string innerText;
			if ((innerText = xmlNode.InnerText) != null)
			{
				if (innerText == "1")
				{
					return BodyType.PlainText;
				}
				if (innerText == "2")
				{
					return BodyType.Html;
				}
				if (innerText == "3")
				{
					return BodyType.Rtf;
				}
				if (innerText == "4")
				{
					return BodyType.Mime;
				}
			}
			return BodyType.None;
		}

		public override void Unbind()
		{
			base.Unbind();
			this.attachments = null;
			this.bodyType = BodyType.None;
			this.data = null;
			this.MIMEData = null;
			this.MIMESize = 0L;
			this.Truncated = true;
		}

		protected override void InternalCopyFrom(IProperty sourceProperty)
		{
			IContentProperty contentProperty = sourceProperty as IContentProperty;
			if (contentProperty == null)
			{
				throw new UnexpectedTypeException("IContentProperty", sourceProperty);
			}
			contentProperty.PreProcessProperty();
			try
			{
				long num = 0L;
				bool flag;
				long num3;
				if (BodyUtility.IsAskingForMIMEData(contentProperty, base.Options) && this.IsAcceptable(BodyType.Mime, out num, out flag))
				{
					int num2 = -1;
					this.bodyType = BodyType.Mime;
					this.MIMEData = contentProperty.MIMEData;
					if (base.Options.Contains("MIMETruncation"))
					{
						num2 = (int)base.Options["MIMETruncation"];
					}
					if (num2 >= 0 && (long)num2 < this.MIMEData.Length)
					{
						this.Truncated = true;
						this.MIMESize = (long)num2;
					}
					else
					{
						this.Truncated = false;
						this.MIMESize = this.MIMEData.Length;
					}
					num3 = contentProperty.MIMEData.Length;
				}
				else
				{
					this.bodyType = contentProperty.GetNativeType();
					if (this.bodyType == BodyType.None)
					{
						return;
					}
					num3 = contentProperty.Size;
					foreach (BodyType bodyType in AirSyncContentProperty.GetPrioritizedBodyTypes(this.bodyType))
					{
						try
						{
							long num4;
							if (this.ClientAccepts(contentProperty, bodyType, out num4, out num))
							{
								this.bodyType = bodyType;
								num3 = num4;
								break;
							}
						}
						catch (ObjectNotFoundException arg)
						{
							AirSyncDiagnostics.TraceInfo<BodyType, ObjectNotFoundException>(ExTraceGlobals.AirSyncTracer, this, "ClientAccepts({0}) has thrown {1}", bodyType, arg);
							num3 = 0L;
							this.bodyType = BodyType.None;
						}
					}
				}
				if (contentProperty.IsIrmErrorMessage)
				{
					this.Truncated = true;
				}
				base.XmlNode = base.XmlParentNode.OwnerDocument.CreateElement(base.AirSyncTagNames[0], base.Namespace);
				base.XmlParentNode.AppendChild(base.XmlNode);
				XmlNode xmlNode = base.XmlNode;
				string elementName = "Type";
				int num5 = (int)this.bodyType;
				base.AppendChildNode(xmlNode, elementName, num5.ToString(CultureInfo.InvariantCulture));
				base.AppendChildNode(base.XmlNode, "EstimatedDataSize", num3.ToString(CultureInfo.InvariantCulture));
				if (this.Truncated)
				{
					base.AppendChildNode(base.XmlNode, "Truncated", "1");
				}
				this.CopyData();
			}
			finally
			{
				contentProperty.PostProcessProperty();
			}
		}

		protected virtual bool ClientAccepts(IContentProperty srcProperty, BodyType type, out long estimatedDataSize, out long truncationSize)
		{
			estimatedDataSize = 0L;
			bool flag;
			if (this.IsAcceptable(type, out truncationSize, out flag))
			{
				if (truncationSize >= 0L && flag)
				{
					this.data = srcProperty.GetData(type, -1L, out estimatedDataSize, out this.attachments);
					if (this.data.Length >= truncationSize && estimatedDataSize > truncationSize)
					{
						this.data = null;
						this.Truncated = true;
						return false;
					}
					this.Truncated = false;
					estimatedDataSize = this.data.Length;
				}
				else
				{
					this.data = srcProperty.GetData(type, truncationSize, out estimatedDataSize, out this.attachments);
					if (truncationSize < 0L)
					{
						this.Truncated = false;
					}
					else if (this.data.Length >= truncationSize && estimatedDataSize > truncationSize)
					{
						this.Truncated = true;
					}
					else
					{
						this.Truncated = false;
					}
				}
				return true;
			}
			this.data = null;
			this.Truncated = true;
			return false;
		}

		protected virtual void CopyData()
		{
			if (this.MIMEData != null && this.MIMESize > 0L)
			{
				base.AppendChildNode(base.XmlNode, "Data", this.MIMEData, this.MIMESize, XmlNodeType.Text);
				return;
			}
			if (this.data != null && this.data.Length > 0L)
			{
				base.AppendChildNode(base.XmlNode, "Data", this.data, -1L, XmlNodeType.Text);
			}
		}

		protected bool IsAcceptable(BodyType type, out long truncationSize, out bool allOrNone)
		{
			allOrNone = false;
			truncationSize = -1L;
			List<BodyPreference> list = base.Options["BodyPreference"] as List<BodyPreference>;
			AirSyncDiagnostics.Assert(list != null);
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Type == type)
				{
					allOrNone = list[i].AllOrNone;
					truncationSize = list[i].TruncationSize;
					return true;
				}
			}
			return false;
		}

		private static List<BodyType> GetPrioritizedBodyTypes(BodyType nativeType)
		{
			List<BodyType> list = new List<BodyType>(5);
			list.Add(nativeType);
			BodyType item = nativeType;
			while (BodyUtility.GetLowerBodyType(ref item))
			{
				list.Add(item);
			}
			item = nativeType;
			while (BodyUtility.GetHigherBodyType(ref item))
			{
				list.Add(item);
			}
			return list;
		}

		private BodyType bodyType;

		private Stream data;

		private bool truncated = true;

		private IEnumerable<AirSyncAttachmentInfo> attachments;
	}
}
