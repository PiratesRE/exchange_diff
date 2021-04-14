using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[KnownType(typeof(OwaMailboxPolicyFeatureInfo))]
	[DataContract]
	[KnownType(typeof(UMMailboxFeatureInfo))]
	[KnownType(typeof(LitigationHoldFeatureInfo))]
	[KnownType(typeof(EASMailboxFeatureInfo))]
	public abstract class MailboxFeatureInfo : BaseRow
	{
		public MailboxFeatureInfo(Identity id) : base(id, null)
		{
		}

		public MailboxFeatureInfo(ADObject objectId) : base(objectId)
		{
			if (objectId == null)
			{
				throw new ArgumentNullException("objectId");
			}
			this.UseModalDialogForEdit = true;
			this.UseModalDialogForEnable = false;
		}

		[DataMember]
		public string Name { get; protected set; }

		[DataMember]
		public string EnableCommandUrl { get; protected set; }

		[DataMember]
		public string EditCommandUrl { get; protected set; }

		[DataMember]
		public int? EnableWizardDialogHeight { get; protected set; }

		[DataMember]
		public int? EnableWizardDialogWidth { get; protected set; }

		[DataMember]
		public int? PropertiesDialogHeight { get; protected set; }

		[DataMember]
		public int? PropertiesDialogWidth { get; protected set; }

		[DataMember]
		public virtual string Status { get; protected set; }

		[DataMember]
		public virtual bool CanChangeStatus { get; protected set; }

		[DataMember]
		public virtual string SpriteId { get; protected set; }

		[DataMember]
		public virtual string IconAltText { get; protected set; }

		[DataMember]
		public bool UseModalDialogForEdit { get; protected set; }

		[DataMember]
		public bool UseModalDialogForEnable { get; protected set; }

		public virtual bool Visible
		{
			get
			{
				return !this.IsReadOnly || this.ShowReadOnly;
			}
		}

		protected bool IsReadOnly
		{
			get
			{
				bool flag = this.Status == ClientStrings.EnabledDisplayText && this.EditCommandUrl != null;
				return !this.CanChangeStatus && !flag;
			}
		}

		protected bool ShowReadOnly { get; set; }

		internal static string GetStatusText(bool isEnabled)
		{
			if (!isEnabled)
			{
				return ClientStrings.DisabledDisplayText;
			}
			return ClientStrings.EnabledDisplayText;
		}

		internal static bool? IsEnabled(string statusText)
		{
			if (statusText == ClientStrings.EnabledPendingDisplayText)
			{
				return new bool?(true);
			}
			if (statusText == ClientStrings.DisabledPendingDisplayText)
			{
				return new bool?(false);
			}
			return null;
		}

		protected bool IsInRole(string[] roles)
		{
			if (roles == null)
			{
				throw new ArgumentException("roles is null");
			}
			foreach (string role in roles)
			{
				if (!RbacPrincipal.Current.IsInRole(role))
				{
					return false;
				}
			}
			return true;
		}
	}
}
