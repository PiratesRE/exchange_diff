﻿using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	public class ArchiveItemCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal ArchiveItemCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public ArchiveItemResponseType Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (ArchiveItemResponseType)this.results[0];
			}
		}

		private object[] results;
	}
}
