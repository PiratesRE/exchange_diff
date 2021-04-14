using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11")]
	[Serializable]
	public class ContextRecoveryInfo
	{
		[XmlElement(DataType = "base64Binary", IsNullable = true, Order = 0)]
		public byte[] FilteredContextSyncCookie
		{
			get
			{
				return this.filteredContextSyncCookieField;
			}
			set
			{
				this.filteredContextSyncCookieField = value;
			}
		}

		[XmlElement(DataType = "base64Binary", IsNullable = true, Order = 1)]
		public byte[] ContextRecoveryToken
		{
			get
			{
				return this.contextRecoveryTokenField;
			}
			set
			{
				this.contextRecoveryTokenField = value;
			}
		}

		[XmlAttribute]
		public int StatusCode
		{
			get
			{
				return this.statusCodeField;
			}
			set
			{
				this.statusCodeField = value;
			}
		}

		private byte[] filteredContextSyncCookieField;

		private byte[] contextRecoveryTokenField;

		private int statusCodeField;
	}
}
