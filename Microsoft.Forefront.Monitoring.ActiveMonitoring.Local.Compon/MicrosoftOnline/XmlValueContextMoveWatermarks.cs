﻿using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class XmlValueContextMoveWatermarks
	{
		public ContextMoveWatermarksValue Watermarks
		{
			get
			{
				return this.watermarksField;
			}
			set
			{
				this.watermarksField = value;
			}
		}

		private ContextMoveWatermarksValue watermarksField;
	}
}
