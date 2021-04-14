using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	[Serializable]
	internal class AirSyncPictureProperty : AirSyncProperty, IPictureProperty, IProperty
	{
		public AirSyncPictureProperty(string xmlNodeNamespace, string pictureBodyTag, bool requiresClientSupport) : base(xmlNodeNamespace, pictureBodyTag, requiresClientSupport)
		{
			base.ClientChangeTracked = true;
		}

		public string PictureData
		{
			get
			{
				return base.XmlNode.InnerText;
			}
		}

		protected override void InternalCopyFrom(IProperty srcProperty)
		{
			IPictureProperty pictureProperty = srcProperty as IPictureProperty;
			if (pictureProperty == null)
			{
				throw new UnexpectedTypeException("IPictureProperty", srcProperty);
			}
			string pictureData = pictureProperty.PictureData;
			if (PropertyState.Modified == srcProperty.State && pictureData != null)
			{
				base.CreateAirSyncNode(pictureData);
			}
		}
	}
}
