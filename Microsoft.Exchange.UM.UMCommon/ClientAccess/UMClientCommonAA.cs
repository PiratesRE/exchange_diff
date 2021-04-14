using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.ClientAccess
{
	internal sealed class UMClientCommonAA : UMClientCommonBase
	{
		public UMClientCommonAA()
		{
		}

		public UMClientCommonAA(UMAutoAttendant aa, string userName, string fileName)
		{
			if (aa == null || userName == null || fileName == null)
			{
				throw new ArgumentNullException();
			}
			this.aa = aa;
			this.userToUseForPublishingAAprompt = userName;
			this.fileNameToStoreAARecording = fileName;
			base.TracePrefix = string.Format(CultureInfo.InvariantCulture, "{0}({1}): ", new object[]
			{
				base.GetType().Name,
				this.aa.Name
			});
		}

		public string PlayOnPhoneAAGreeting(string dialString)
		{
			string result;
			try
			{
				base.DebugTrace("PlayOnPhoneAAGreeting({0})", new object[]
				{
					dialString
				});
				if (UMClientCommonBase.Counters != null)
				{
					UMClientCommonBase.Counters.TotalPlayOnPhoneRequests.Increment();
				}
				this.ValidateAAGreetingParameters();
				UMServerProxy serverByDialplan = UMServerManager.GetServerByDialplan(this.aa.UMDialPlan);
				IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromOrganizationId(this.aa.OrganizationId);
				Guid externalDirectoryOrganizationId = iadsystemConfigurationLookup.GetExternalDirectoryOrganizationId();
				string sessionId = serverByDialplan.PlayOnPhoneAAGreeting(this.aa, externalDirectoryOrganizationId, dialString, this.userToUseForPublishingAAprompt, this.fileNameToStoreAARecording);
				result = base.EncodeCallId(serverByDialplan.Fqdn, sessionId);
			}
			catch (LocalizedException exception)
			{
				base.LogException(exception);
				if (UMClientCommonBase.Counters != null)
				{
					UMClientCommonBase.Counters.TotalPlayOnPhoneErrors.Increment();
				}
				throw;
			}
			return result;
		}

		protected override string GetUserContext()
		{
			if (this.aa != null)
			{
				return this.aa.Name;
			}
			return string.Empty;
		}

		protected override void DisposeMe()
		{
		}

		private void ValidateAAGreetingParameters()
		{
			if (this.aa == null || this.fileNameToStoreAARecording == null || this.userToUseForPublishingAAprompt == null)
			{
				throw new InvalidDataException();
			}
		}

		private string userToUseForPublishingAAprompt;

		private string fileNameToStoreAARecording;

		private UMAutoAttendant aa;
	}
}
