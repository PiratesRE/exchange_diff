﻿using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	public class CopyItemCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal CopyItemCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public CopyItemResponseType Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (CopyItemResponseType)this.results[0];
			}
		}

		private object[] results;
	}
}
