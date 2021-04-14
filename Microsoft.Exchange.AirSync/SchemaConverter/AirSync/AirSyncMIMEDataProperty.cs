using System;
using System.Globalization;
using System.IO;
using System.Xml;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	[Serializable]
	internal class AirSyncMIMEDataProperty : AirSyncProperty, IMIMEDataProperty, IMIMERelatedProperty, IProperty
	{
		public AirSyncMIMEDataProperty(string xmlNodeNamespace, bool requiresClientSupport) : base(xmlNodeNamespace, "MIMEData", requiresClientSupport)
		{
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

		protected override void InternalCopyFrom(IProperty srcProperty)
		{
			IMIMEDataProperty imimedataProperty = srcProperty as IMIMEDataProperty;
			if (imimedataProperty == null)
			{
				throw new UnexpectedTypeException("IMIMEDataProperty", srcProperty);
			}
			if (!BodyUtility.IsAskingForMIMEData(imimedataProperty, base.Options))
			{
				return;
			}
			int num = -1;
			bool flag = false;
			imimedataProperty.MIMESize = imimedataProperty.MIMEData.Length;
			if (base.Options.Contains("MIMETruncation"))
			{
				num = (int)base.Options["MIMETruncation"];
			}
			if (num >= 0 && (long)num < imimedataProperty.MIMEData.Length)
			{
				flag = true;
				imimedataProperty.MIMESize = (long)num;
			}
			base.CreateAirSyncNode("MIMETruncated", flag ? "1" : "0");
			if (flag)
			{
				imimedataProperty.MIMESize = (long)num;
				base.CreateAirSyncNode("MIMESize", imimedataProperty.MIMEData.Length.ToString(CultureInfo.InvariantCulture));
			}
			base.CreateAirSyncNode("MIMEData", imimedataProperty.MIMEData, imimedataProperty.MIMESize, XmlNodeType.Text);
		}
	}
}
