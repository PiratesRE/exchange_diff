using System;
using System.IO;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	[Serializable]
	internal class AirSyncRtfBodyProperty : AirSyncProperty, IBodyProperty, IMIMERelatedProperty, IProperty
	{
		public AirSyncRtfBodyProperty(string xmlNodeNamespace, string textBodyTag, bool requiresClientSupport, AirSyncBodyProperty bodyProperty) : base(xmlNodeNamespace, textBodyTag, requiresClientSupport)
		{
			this.bodyProperty = bodyProperty;
			base.ClientChangeTracked = true;
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
				return true;
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
				throw new NotImplementedException("Programming Error!! This method should never be called");
			}
		}

		public bool TextPresent
		{
			get
			{
				return false;
			}
		}

		public int TextSize
		{
			get
			{
				throw new NotImplementedException("Programming Error!! This method should never be called");
			}
		}

		public Stream GetTextData(int length)
		{
			throw new NotImplementedException("Programming Error!! This method should never be called");
		}

		protected override void InternalCopyFrom(IProperty srcProperty)
		{
			this.bodyProperty.CreateRtfNode();
		}

		private AirSyncBodyProperty bodyProperty;
	}
}
