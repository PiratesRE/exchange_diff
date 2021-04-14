using System;
using System.Runtime.Serialization;
using System.Web;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SegmentationSettingsType
	{
		public SegmentationSettingsType(ulong segmentationFlags)
		{
			this.segmentationFlags = segmentationFlags;
		}

		internal SegmentationSettingsType(ConfigurationContext configurationContext)
		{
			this.configurationContext = configurationContext;
			this.segmentationFlags = this.configurationContext.SegmentationFlags;
		}

		[DataMember]
		public bool GlobalAddressList
		{
			get
			{
				return this.IsFeatureEnabled(Feature.GlobalAddressList);
			}
			set
			{
				this.SetFeatureEnabled(Feature.GlobalAddressList, value);
			}
		}

		[DataMember]
		public bool ReportJunkEmailEnabled
		{
			get
			{
				if (this.configurationContext == null)
				{
					return false;
				}
				UserContext userContext = UserContextManager.GetMailboxContext(HttpContext.Current, null, true) as UserContext;
				return userContext != null && userContext.FeaturesManager.ServerSettings.ReportJunk.Enabled && this.IsFeatureEnabled(Feature.ReportJunkEmail);
			}
			set
			{
				this.SetFeatureEnabled(Feature.ReportJunkEmail, value);
			}
		}

		[DataMember]
		public bool Calendar
		{
			get
			{
				return this.IsFeatureEnabled(Feature.Calendar);
			}
			set
			{
				this.SetFeatureEnabled(Feature.Calendar, value);
			}
		}

		[DataMember]
		public bool Contacts
		{
			get
			{
				return this.IsFeatureEnabled(Feature.Contacts);
			}
			set
			{
				this.SetFeatureEnabled(Feature.Contacts, value);
			}
		}

		[DataMember]
		public bool Tasks
		{
			get
			{
				return this.IsFeatureEnabled(Feature.Tasks);
			}
			set
			{
				this.SetFeatureEnabled(Feature.Tasks, value);
			}
		}

		[DataMember]
		public bool Todos
		{
			get
			{
				return this.IsFeatureEnabled(Feature.Tasks);
			}
			set
			{
				this.SetFeatureEnabled(Feature.Tasks, value);
			}
		}

		[DataMember]
		public bool Journal
		{
			get
			{
				return this.IsFeatureEnabled(Feature.Journal);
			}
			set
			{
				this.SetFeatureEnabled(Feature.Journal, value);
			}
		}

		[DataMember]
		public bool StickyNotes
		{
			get
			{
				return this.IsFeatureEnabled(Feature.StickyNotes);
			}
			set
			{
				this.SetFeatureEnabled(Feature.StickyNotes, value);
			}
		}

		[DataMember]
		public bool PublicFolders
		{
			get
			{
				return this.IsFeatureEnabled(Feature.PublicFolders);
			}
			set
			{
				this.SetFeatureEnabled(Feature.PublicFolders, value);
			}
		}

		[DataMember]
		public bool Organization
		{
			get
			{
				return this.IsFeatureEnabled(Feature.Organization);
			}
			set
			{
				this.SetFeatureEnabled(Feature.Organization, value);
			}
		}

		[DataMember]
		public bool Notifications
		{
			get
			{
				return this.IsFeatureEnabled(Feature.Notifications);
			}
			set
			{
				this.SetFeatureEnabled(Feature.Notifications, value);
			}
		}

		[DataMember]
		public bool SpellChecker
		{
			get
			{
				return this.IsFeatureEnabled(Feature.SpellChecker);
			}
			set
			{
				this.SetFeatureEnabled(Feature.SpellChecker, value);
			}
		}

		[DataMember]
		public bool SMime
		{
			get
			{
				return this.IsFeatureEnabled(Feature.SMime);
			}
			set
			{
				this.SetFeatureEnabled(Feature.SMime, value);
			}
		}

		[DataMember]
		public bool SearchFolders
		{
			get
			{
				return this.IsFeatureEnabled(Feature.SearchFolders);
			}
			set
			{
				this.SetFeatureEnabled(Feature.SearchFolders, value);
			}
		}

		[DataMember]
		public bool Signature
		{
			get
			{
				return this.IsFeatureEnabled(Feature.Signature);
			}
			set
			{
				this.SetFeatureEnabled(Feature.Signature, value);
			}
		}

		[DataMember]
		public bool Rules
		{
			get
			{
				return this.IsFeatureEnabled(Feature.Rules);
			}
			set
			{
				this.SetFeatureEnabled(Feature.Rules, value);
			}
		}

		[DataMember]
		public bool Themes
		{
			get
			{
				return this.IsFeatureEnabled(Feature.Themes);
			}
			set
			{
				this.SetFeatureEnabled(Feature.Themes, value);
			}
		}

		[DataMember]
		public bool JunkEMail
		{
			get
			{
				return this.IsFeatureEnabled(Feature.JunkEMail);
			}
			set
			{
				this.SetFeatureEnabled(Feature.JunkEMail, value);
			}
		}

		[DataMember]
		public bool UmIntegration
		{
			get
			{
				return this.IsFeatureEnabled(Feature.UMIntegration);
			}
			set
			{
				this.SetFeatureEnabled(Feature.UMIntegration, value);
			}
		}

		[DataMember]
		public bool EasMobileOptions
		{
			get
			{
				return this.IsFeatureEnabled(Feature.EasMobileOptions);
			}
			set
			{
				this.SetFeatureEnabled(Feature.EasMobileOptions, value);
			}
		}

		[DataMember]
		public bool ExplicitLogon
		{
			get
			{
				return this.IsFeatureEnabled(Feature.ExplicitLogon);
			}
			set
			{
				this.SetFeatureEnabled(Feature.ExplicitLogon, value);
			}
		}

		[DataMember]
		public bool AddressLists
		{
			get
			{
				return this.IsFeatureEnabled(Feature.AddressLists);
			}
			set
			{
				this.SetFeatureEnabled(Feature.AddressLists, value);
			}
		}

		[DataMember]
		public bool Dumpster
		{
			get
			{
				return this.IsFeatureEnabled(Feature.Dumpster);
			}
			set
			{
				this.SetFeatureEnabled(Feature.Dumpster, value);
			}
		}

		[DataMember]
		public bool ChangePassword
		{
			get
			{
				return this.IsFeatureEnabled(Feature.ChangePassword);
			}
			set
			{
				this.SetFeatureEnabled(Feature.ChangePassword, value);
			}
		}

		[DataMember]
		public bool InstantMessage
		{
			get
			{
				return this.IsFeatureEnabled(Feature.InstantMessage);
			}
			set
			{
				this.SetFeatureEnabled(Feature.InstantMessage, value);
			}
		}

		[DataMember]
		public bool TextMessage
		{
			get
			{
				return this.IsFeatureEnabled(Feature.TextMessage);
			}
			set
			{
				this.SetFeatureEnabled(Feature.TextMessage, value);
			}
		}

		[DataMember]
		public bool DelegateAccess
		{
			get
			{
				return this.IsFeatureEnabled(Feature.DelegateAccess);
			}
			set
			{
				this.SetFeatureEnabled(Feature.DelegateAccess, value);
			}
		}

		[DataMember]
		public bool Irm
		{
			get
			{
				return this.IsFeatureEnabled((Feature)int.MinValue);
			}
			set
			{
				this.SetFeatureEnabled((Feature)int.MinValue, value);
			}
		}

		[DataMember]
		public bool ForceSaveAttachmentFiltering
		{
			get
			{
				return this.IsFeatureEnabled(Feature.ForceSaveAttachmentFiltering);
			}
			set
			{
				this.SetFeatureEnabled(Feature.ForceSaveAttachmentFiltering, value);
			}
		}

		[DataMember]
		public bool Silverlight
		{
			get
			{
				return this.IsFeatureEnabled(Feature.Silverlight);
			}
			set
			{
				this.SetFeatureEnabled(Feature.Silverlight, value);
			}
		}

		[DataMember]
		public bool DisplayPhotos
		{
			get
			{
				return this.IsFeatureEnabled(Feature.DisplayPhotos);
			}
			set
			{
				this.SetFeatureEnabled(Feature.DisplayPhotos, value);
			}
		}

		[DataMember]
		public bool SetPhoto
		{
			get
			{
				return this.IsFeatureEnabled(Feature.SetPhoto);
			}
			set
			{
				this.SetFeatureEnabled(Feature.SetPhoto, value);
			}
		}

		[DataMember]
		public bool PredictedActions
		{
			get
			{
				return this.IsFeatureEnabled(Feature.PredictedActions);
			}
			set
			{
				this.SetFeatureEnabled(Feature.PredictedActions, value);
			}
		}

		[DataMember]
		public bool UserDiagnosticEnabled
		{
			get
			{
				return this.IsFeatureEnabled(Feature.UserDiagnosticEnabled);
			}
			set
			{
				this.SetFeatureEnabled(Feature.UserDiagnosticEnabled, value);
			}
		}

		[DataMember]
		public bool SkipCreateUnifiedGroupCustomSharepointClassification
		{
			get
			{
				return this.IsFeatureEnabled(Feature.SkipCreateUnifiedGroupCustomSharepointClassification);
			}
			set
			{
				this.SetFeatureEnabled(Feature.SkipCreateUnifiedGroupCustomSharepointClassification, value);
			}
		}

		[DataMember]
		public bool GroupCreationEnabled
		{
			get
			{
				return this.IsFeatureEnabled(Feature.GroupCreationEnabled);
			}
			set
			{
				this.SetFeatureEnabled(Feature.GroupCreationEnabled, value);
			}
		}

		[DataMember]
		public bool FacebookEnabled
		{
			get
			{
				return this.IsFeatureEnabled(Feature.FacebookEnabled);
			}
			set
			{
				this.SetFeatureEnabled(Feature.FacebookEnabled, value);
			}
		}

		[DataMember]
		public bool LinkedInEnabled
		{
			get
			{
				return this.IsFeatureEnabled(Feature.LinkedInEnabled);
			}
			set
			{
				this.SetFeatureEnabled(Feature.LinkedInEnabled, value);
			}
		}

		[DataMember]
		public bool OWALightEnabled
		{
			get
			{
				return this.IsFeatureEnabled(Feature.OWALight);
			}
			set
			{
				this.SetFeatureEnabled(Feature.OWALight, value);
			}
		}

		private bool IsFeatureEnabled(Feature feature)
		{
			return (feature & (Feature)this.segmentationFlags) == feature && (this.configurationContext == null || this.configurationContext.IsFeatureNotRestricted((ulong)feature));
		}

		private void SetFeatureEnabled(Feature feature, bool enabled)
		{
			if (enabled)
			{
				this.segmentationFlags |= (ulong)feature;
				return;
			}
			this.segmentationFlags &= (ulong)(~(ulong)feature);
		}

		private ulong segmentationFlags;

		private ConfigurationContext configurationContext;
	}
}
