using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource("EditAccountProperties", "Microsoft.Exchange.Management.ControlPanel.Client.Users.js")]
	public sealed class EditAccountProperties : Properties
	{
		public WebServiceMethod CancelPhotoWebServiceMethod { get; private set; }

		public WebServiceMethod SavePhotoWebServiceMethod { get; private set; }

		public WebServiceMethod RemovePhotoWebServiceMethod { get; private set; }

		public WebServiceReference UserPhotoServiceUrl { get; set; }

		public string PhotoSectionId { get; set; }

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			this.CancelPhotoWebServiceMethod = new WebServiceMethod();
			this.CancelPhotoWebServiceMethod.ID = "CancelUserPhoto";
			this.CancelPhotoWebServiceMethod.ServiceUrl = this.UserPhotoServiceUrl;
			this.CancelPhotoWebServiceMethod.Method = "CancelPhoto";
			this.CancelPhotoWebServiceMethod.ParameterNames = WebServiceParameterNames.Identity;
			this.Controls.Add(this.CancelPhotoWebServiceMethod);
			this.SavePhotoWebServiceMethod = new WebServiceMethod();
			this.SavePhotoWebServiceMethod.ID = "SaveUserPhoto";
			this.SavePhotoWebServiceMethod.ServiceUrl = this.UserPhotoServiceUrl;
			this.SavePhotoWebServiceMethod.Method = "SavePhoto";
			this.SavePhotoWebServiceMethod.ParameterNames = WebServiceParameterNames.Identity;
			this.Controls.Add(this.SavePhotoWebServiceMethod);
			this.RemovePhotoWebServiceMethod = new WebServiceMethod();
			this.RemovePhotoWebServiceMethod.ID = "RemoveUserPhoto";
			this.RemovePhotoWebServiceMethod.ServiceUrl = this.UserPhotoServiceUrl;
			this.RemovePhotoWebServiceMethod.Method = "RemovePhoto";
			this.RemovePhotoWebServiceMethod.ParameterNames = WebServiceParameterNames.Identity;
			this.Controls.Add(this.RemovePhotoWebServiceMethod);
		}

		protected override void OnInit(EventArgs e)
		{
			this.ApplySimplifiedPhotoExperienceLayout();
			base.OnInit(e);
		}

		protected override void OnPreRender(EventArgs e)
		{
			this.ApplySimplifiedPhotoExperienceLayout();
			base.OnPreRender(e);
			foreach (ImageUploader imageUploader in this.GetVisibleImageUploaders(base.Sections[this.PhotoSectionId]))
			{
				if (this.SimplifiedPhotoExperience())
				{
					imageUploader.InitStateAsEditClicked = true;
				}
				imageUploader.CancelWebServiceMethod = this.CancelPhotoWebServiceMethod;
				imageUploader.SaveWebServiceMethod = this.SavePhotoWebServiceMethod;
				imageUploader.RemoveWebServiceMethod = this.RemovePhotoWebServiceMethod;
			}
		}

		protected override ScriptControlDescriptor GetScriptDescriptor()
		{
			ScriptControlDescriptor scriptDescriptor = base.GetScriptDescriptor();
			scriptDescriptor.Type = "EditAccountProperties";
			return scriptDescriptor;
		}

		private IEnumerable<ImageUploader> GetVisibleImageUploaders(Control parent)
		{
			if (parent.Visible)
			{
				ImageUploader pl = parent as ImageUploader;
				if (pl != null)
				{
					yield return pl;
				}
				foreach (object obj in parent.Controls)
				{
					Control subControl = (Control)obj;
					foreach (ImageUploader c in this.GetVisibleImageUploaders(subControl))
					{
						yield return c;
					}
				}
			}
			yield break;
		}

		private void ApplySimplifiedPhotoExperienceLayout()
		{
			if (!this.SimplifiedPhotoExperience())
			{
				return;
			}
			this.RemoveSectionsExceptPhoto();
			base.CaptionTextField = "ChangePhotoCaption";
			((BaseForm)this.Page).HideFieldValidationAssistant = true;
		}

		private void RemoveSectionsExceptPhoto()
		{
			if (base.Sections == null || base.Sections.Count == 0)
			{
				return;
			}
			for (int i = base.Sections.Count - 1; i >= 0; i--)
			{
				if (!this.PhotoSectionId.Equals(base.Sections[i].ID, StringComparison.OrdinalIgnoreCase))
				{
					base.Sections.RemoveAt(i);
				}
			}
		}

		private bool SimplifiedPhotoExperience()
		{
			return this.Page.Request.QueryString.AllKeys.Contains("chgPhoto", StringComparer.OrdinalIgnoreCase);
		}

		private const string SimplifiedPhotoExperienceCaption = "ChangePhotoCaption";
	}
}
