using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	internal class AirSyncBodyProperty : AirSyncProperty, IBodyProperty, IMIMERelatedProperty, IProperty
	{
		public AirSyncBodyProperty(string xmlNodeNamespace, string textBodyTag, string bodyTruncatedTag, string bodySizeTag, bool forceBodyTruncatedTag, bool writeBodyTruncatedLast, bool requiresClientSupport) : base(xmlNodeNamespace, textBodyTag, requiresClientSupport)
		{
			base.ClientChangeTracked = true;
			this.textBodyTag = textBodyTag;
			this.forceBodyTruncatedTag = forceBodyTruncatedTag;
			this.bodyTruncatedTag = bodyTruncatedTag;
			this.bodySizeTag = bodySizeTag;
			this.writeBodyTruncatedLast = writeBodyTruncatedLast;
		}

		public AirSyncBodyProperty(string xmlNodeNamespace, string textBodyTag, string rtfBodyTag, string bodyTruncatedTag, string bodySizeTag, bool forceBodyTruncatedTag, bool writeBodyTruncatedLast, bool requiresClientSupport) : this(xmlNodeNamespace, textBodyTag, rtfBodyTag, bodyTruncatedTag, bodySizeTag, forceBodyTruncatedTag, writeBodyTruncatedLast, requiresClientSupport, false)
		{
		}

		public AirSyncBodyProperty(string xmlNodeNamespace, string textBodyTag, string rtfBodyTag, string bodyTruncatedTag, string bodySizeTag, bool forceBodyTruncatedTag, bool writeBodyTruncatedLast, bool requiresClientSupport, bool cacheRtf) : base(xmlNodeNamespace, textBodyTag, rtfBodyTag, requiresClientSupport)
		{
			base.ClientChangeTracked = true;
			this.textBodyTag = textBodyTag;
			if (string.IsNullOrEmpty(rtfBodyTag))
			{
				throw new ArgumentNullException("rtfBodyTag");
			}
			this.rtfBodyTag = rtfBodyTag;
			this.cacheRtf = cacheRtf;
			this.forceBodyTruncatedTag = forceBodyTruncatedTag;
			this.bodyTruncatedTag = bodyTruncatedTag;
			this.bodySizeTag = bodySizeTag;
			this.writeBodyTruncatedLast = writeBodyTruncatedLast;
		}

		public bool IsOnSMIMEMessage
		{
			get
			{
				throw new NotImplementedException("IsOnSMIMEMessage should not be called");
			}
		}

		public Stream RtfData
		{
			get
			{
				if (!this.RtfPresent)
				{
					throw new ConversionException("Cannot return RtfData when no RTF is present");
				}
				return new MemoryStream(Convert.FromBase64String(base.XmlNode.InnerText));
			}
		}

		public bool RtfPresent
		{
			get
			{
				return base.XmlNode.Name == this.rtfBodyTag;
			}
		}

		public int RtfSize
		{
			get
			{
				if (!this.RtfPresent)
				{
					throw new ConversionException("Cannot return RtfSize when no RTF is present");
				}
				return base.XmlNode.InnerText.Length / 4 * 3;
			}
		}

		public Stream TextData
		{
			get
			{
				if (!this.TextPresent)
				{
					throw new ConversionException("Cannot return TextData no text is present");
				}
				return new MemoryStream(Encoding.UTF8.GetBytes(base.XmlNode.InnerText));
			}
		}

		public bool TextPresent
		{
			get
			{
				return base.XmlNode.Name == this.textBodyTag;
			}
		}

		public int TextSize
		{
			get
			{
				if (!this.TextPresent)
				{
					throw new ConversionException("Cannot return TextData when no text is present");
				}
				return base.XmlNode.InnerText.Length;
			}
		}

		public void CreateRtfNode()
		{
			if (this.cachedRtfNode != null)
			{
				base.XmlParentNode.AppendChild(this.cachedRtfNode);
			}
		}

		public Stream GetTextData(int length)
		{
			throw new NotImplementedException("Programming Error!! This method should never be called");
		}

		protected override void InternalCopyFrom(IProperty srcProperty)
		{
			IBodyProperty bodyProperty = srcProperty as IBodyProperty;
			if (bodyProperty == null)
			{
				throw new UnexpectedTypeException("IBodyProperty", bodyProperty);
			}
			if (BodyUtility.IsAskingForMIMEData(bodyProperty, base.Options))
			{
				return;
			}
			bool flag = false;
			bool flag2 = false;
			int num = 0;
			bool flag3 = false;
			this.cachedRtfNode = null;
			if (base.Options != null)
			{
				if (base.Options.Contains("ClientSupportsRtf"))
				{
					flag = (bool)base.Options["ClientSupportsRtf"];
				}
				if (base.Options.Contains("MaxTextBodySize"))
				{
					flag2 = true;
					num = (int)base.Options["MaxTextBodySize"];
				}
				if (base.Options.Contains("MaxRtfBodySize") && bodyProperty.RtfPresent)
				{
					int num2 = (int)base.Options["MaxRtfBodySize"];
					flag3 = (bodyProperty.RtfSize > (num2 / 3 + 1) * 4);
				}
			}
			if (flag && this.rtfBodyTag != null && bodyProperty.RtfPresent && !flag3)
			{
				AirSyncStream airSyncStream = bodyProperty.RtfData as AirSyncStream;
				if (airSyncStream == null || airSyncStream.Length == 0L)
				{
					return;
				}
				airSyncStream.DoBase64Conversion = true;
				if (flag2)
				{
					base.CreateAirSyncNode(this.bodyTruncatedTag, "0");
				}
				XmlNode xmlNode = base.AppendChildNode(base.XmlParentNode, this.rtfBodyTag, airSyncStream, -1L, XmlNodeType.Text);
				if (this.cacheRtf)
				{
					this.cachedRtfNode = xmlNode;
					return;
				}
			}
			else
			{
				if (!bodyProperty.TextPresent)
				{
					return;
				}
				if (!flag2 || bodyProperty.TextSize <= num)
				{
					Stream textData = bodyProperty.TextData;
					if (bodyProperty.TextSize == 0)
					{
						return;
					}
					if (!this.writeBodyTruncatedLast && this.forceBodyTruncatedTag)
					{
						base.CreateAirSyncNode(this.bodyTruncatedTag, "0");
					}
					base.AppendChildNode(base.XmlParentNode, this.textBodyTag, textData, -1L, XmlNodeType.Text);
					if (this.writeBodyTruncatedLast && this.forceBodyTruncatedTag)
					{
						base.CreateAirSyncNode(this.bodyTruncatedTag, "0");
						return;
					}
				}
				else
				{
					Stream textData2 = bodyProperty.GetTextData(num);
					if (bodyProperty.TextSize == 0)
					{
						return;
					}
					if (!this.writeBodyTruncatedLast)
					{
						base.CreateAirSyncNode(this.bodyTruncatedTag, "1");
					}
					if (this.bodySizeTag != null)
					{
						base.CreateAirSyncNode(this.bodySizeTag, bodyProperty.TextSize.ToString(CultureInfo.InvariantCulture));
					}
					base.AppendChildNode(base.XmlParentNode, this.textBodyTag, textData2, -1L, XmlNodeType.Text);
					if (this.writeBodyTruncatedLast)
					{
						base.CreateAirSyncNode(this.bodyTruncatedTag, "1");
					}
				}
			}
		}

		public const string OptionClientSupportsRtf = "ClientSupportsRtf";

		public const string OptionMaxRtfBodySize = "MaxRtfBodySize";

		public const string OptionMaxTextBodySize = "MaxTextBodySize";

		private string bodySizeTag;

		private string bodyTruncatedTag;

		private XmlNode cachedRtfNode;

		private bool cacheRtf;

		private bool forceBodyTruncatedTag;

		private string rtfBodyTag;

		private string textBodyTag;

		private bool writeBodyTruncatedLast;
	}
}
