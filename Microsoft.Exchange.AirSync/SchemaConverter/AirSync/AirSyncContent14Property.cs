using System;
using System.Collections.Generic;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Diagnostics.Components.SchemaConverter;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	internal class AirSyncContent14Property : AirSyncContentProperty, IContent14Property, IContentProperty, IMIMEDataProperty, IMIMERelatedProperty, IProperty
	{
		public AirSyncContent14Property(string xmlNodeNamespace, string airSyncTagName, bool requiresClientSupport) : base(xmlNodeNamespace, airSyncTagName, requiresClientSupport)
		{
		}

		public string Preview
		{
			get
			{
				throw new NotImplementedException("Preview should not be called");
			}
		}

		protected override void InternalCopyFrom(IProperty sourceProperty)
		{
			List<BodyPartPreference> list = base.Options["BodyPartPreference"] as List<BodyPartPreference>;
			if (list != null && list.Count > 0)
			{
				List<BodyPreference> list2 = base.Options["BodyPreference"] as List<BodyPreference>;
				if (list2 == null || list2.Count <= 0)
				{
					AirSyncDiagnostics.TraceInfo(ExTraceGlobals.AirSyncTracer, this, "No <BodyPreference> in the request while <BodyPartPreference> presends. Skip generating <Body>");
					return;
				}
			}
			base.InternalCopyFrom(sourceProperty);
			int previewLength = this.GetPreviewLength();
			if (previewLength == 0)
			{
				return;
			}
			string text = ((IContent14Property)sourceProperty).Preview;
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			if (text.Length > previewLength)
			{
				text = text.Remove(previewLength);
			}
			text = text.TrimEnd(new char[0]);
			if (!string.IsNullOrEmpty(text))
			{
				base.AppendChildNode(base.XmlNode, "Preview", text);
			}
		}

		protected int GetPreviewLength()
		{
			int result = 0;
			List<BodyPreference> list = base.Options["BodyPreference"] as List<BodyPreference>;
			AirSyncDiagnostics.Assert(list != null);
			int i = 0;
			while (i < list.Count)
			{
				if (list[i].Type == base.BodyType)
				{
					if (list[i].Preview == 0)
					{
						return 0;
					}
					result = list[i].Preview;
					break;
				}
				else
				{
					i++;
				}
			}
			if (base.Truncated && (base.Data == null || base.Data.Length == 0L))
			{
				return result;
			}
			if (base.BodyType == BodyType.PlainText)
			{
				return 0;
			}
			return result;
		}
	}
}
