﻿using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	public class StartFindInGALSpeechRecognitionCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal StartFindInGALSpeechRecognitionCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public StartFindInGALSpeechRecognitionResponseMessageType Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (StartFindInGALSpeechRecognitionResponseMessageType)this.results[0];
			}
		}

		private object[] results;
	}
}
