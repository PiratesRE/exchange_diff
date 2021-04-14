﻿using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	public class SyncFolderItemsCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal SyncFolderItemsCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public SyncFolderItemsResponseType Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (SyncFolderItemsResponseType)this.results[0];
			}
		}

		private object[] results;
	}
}
