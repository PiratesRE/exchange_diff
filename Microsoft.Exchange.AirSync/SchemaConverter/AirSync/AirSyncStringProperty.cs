using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Diagnostics.Components.SchemaConverter;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	[Serializable]
	internal class AirSyncStringProperty : AirSyncProperty, IStringProperty, IProperty
	{
		public AirSyncStringProperty(string xmlNodeNamespace, string airSyncTagName, bool requiresClientSupport) : base(xmlNodeNamespace, airSyncTagName, requiresClientSupport)
		{
		}

		public string StringData
		{
			get
			{
				return base.XmlNode.InnerText;
			}
		}

		protected override void InternalCopyFrom(IProperty srcProperty)
		{
			IStringProperty stringProperty = srcProperty as IStringProperty;
			if (stringProperty == null)
			{
				throw new UnexpectedTypeException("IStringProperty", srcProperty, base.AirSyncTagNames[0]);
			}
			string text = stringProperty.StringData;
			bool flag = string.Equals(base.AirSyncTagNames[0], "Location");
			if (PropertyState.Modified == srcProperty.State)
			{
				if (string.IsNullOrEmpty(text) && !flag)
				{
					AirSyncDiagnostics.TraceInfo<string>(ExTraceGlobals.AirSyncTracer, this, "Node={0} String data is null or empty.", base.AirSyncTagNames[0]);
					this.InternalSetToDefault(srcProperty);
					return;
				}
				if (text.Length > 32000)
				{
					text = text.Substring(0, 32000);
				}
				base.CreateAirSyncNode(text, flag);
			}
		}

		private const int MaxStringLength = 32000;
	}
}
