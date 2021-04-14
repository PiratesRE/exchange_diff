using System;
using System.Collections;
using System.Globalization;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class ReferredByHeaderHandler
	{
		protected Hashtable ParseHeader(string referredBy)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "ReferredByHeaderHandler::ParseHeader() SIP header '{0}': {1}", new object[]
			{
				"Referred-By",
				referredBy
			});
			Hashtable hashtable = new Hashtable();
			Hashtable result;
			try
			{
				PlatformSignalingHeader platformSignalingHeader = Platform.Builder.CreateSignalingHeader("Referred-By", referredBy);
				PlatformSipUri platformSipUri = platformSignalingHeader.ParseUri();
				foreach (PlatformSipUriParameter platformSipUriParameter in platformSipUri.GetParametersThatHaveValues())
				{
					if (!hashtable.ContainsKey(platformSipUriParameter.Name))
					{
						hashtable.Add(platformSipUriParameter.Name.ToLowerInvariant(), platformSipUriParameter.Value);
					}
				}
				result = hashtable;
			}
			catch (ArgumentException ex)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "ReferredByHeaderHandler::ParseHeader() Invalid SIP header '{0}': {1}", new object[]
				{
					"Referred-By",
					ex
				});
				result = hashtable;
			}
			return result;
		}

		protected PlatformSipUri FrameHeader(Hashtable paramsToBeAdded)
		{
			PlatformSipUri platformSipUri = Platform.Builder.CreateSipUri(string.Format(CultureInfo.InvariantCulture, "sip:{0}", new object[]
			{
				string.IsNullOrEmpty(this.referredByHostUri) ? Utils.GetOwnerHostFqdn() : this.referredByHostUri
			}));
			foreach (object obj in paramsToBeAdded.Keys)
			{
				string text = (string)obj;
				platformSipUri.AddParameter(text, (string)paramsToBeAdded[text]);
			}
			return platformSipUri;
		}

		protected string referredByHostUri;
	}
}
