﻿using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	public class DisconnectPhoneCallCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal DisconnectPhoneCallCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public DisconnectPhoneCallResponseMessageType Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (DisconnectPhoneCallResponseMessageType)this.results[0];
			}
		}

		private object[] results;
	}
}
