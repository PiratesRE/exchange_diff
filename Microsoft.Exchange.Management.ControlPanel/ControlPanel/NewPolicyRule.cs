using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.MessagingPolicies.Rules.Tasks;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class NewPolicyRule : BaseForm
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (!base.IsPostBack)
			{
				this.CheckIfCopyMode();
				if (!string.IsNullOrEmpty(base.Request.QueryString["mode"]))
				{
					RuleMode ruleMode = (RuleMode)Enum.Parse(typeof(RuleMode), base.Request.QueryString["mode"], true);
					if (ruleMode == RuleMode.Audit || ruleMode == RuleMode.AuditAndNotify)
					{
						this.transportRuleEditor.AdvModeDialogMinSize = new Size(780, 575);
						this.transportRuleEditor.ForceDisclaimerOnNew = true;
					}
					this.transportRuleEditor.DlpPolicyMode = ruleMode.ToString();
				}
				if (!string.IsNullOrEmpty(base.Request.QueryString["name"]))
				{
					this.transportRuleEditor.DlpPolicy = HttpUtility.UrlDecode(base.Request.QueryString["name"]);
				}
				if (!string.IsNullOrEmpty(base.Request.QueryString["preconfig"]))
				{
					string text = base.Request.QueryString["preconfig"].ToLower();
					if (!string.IsNullOrEmpty(text))
					{
						List<string> list = new List<string>();
						List<string> list2 = new List<string>();
						this.transportRuleEditor.DefaultValues.Add("SentToScope", ToUserScope.NotInOrganization.ToString());
						string a;
						if ((a = text) != null)
						{
							if (!(a == "notifysender"))
							{
								if (!(a == "rejectwithoverride"))
								{
									if (!(a == "rejectnooverride"))
									{
										if (a == "rejectmessageallowoverrides")
										{
											list.AddRange(new string[]
											{
												"SentToScope",
												"MessageContainsDataClassifications"
											});
											list2.AddRange(new string[]
											{
												"NotifySender",
												"GenerateIncidentReport"
											});
											this.transportRuleEditor.DefaultValues.Add("IncidentReportOriginalMail", IncidentReportOriginalMail.IncludeOriginalMail.ToString());
											this.transportRuleEditor.DefaultValues.Add("SenderNotifySettings", new SenderNotifySettings
											{
												NotifySender = NotifySenderType.RejectUnlessSilentOverride.ToString()
											}.ToJsonString(null));
										}
									}
									else
									{
										list.AddRange(new string[]
										{
											"SentToScope",
											"MessageContainsDataClassifications"
										});
										list2.AddRange(new string[]
										{
											"GenerateIncidentReport",
											"NotifySender"
										});
										this.transportRuleEditor.DefaultValues.Add("IncidentReportOriginalMail", IncidentReportOriginalMail.IncludeOriginalMail.ToString());
										this.transportRuleEditor.DefaultValues.Add("SenderNotifySettings", new SenderNotifySettings
										{
											NotifySender = NotifySenderType.RejectMessage.ToString()
										}.ToJsonString(null));
									}
								}
								else
								{
									list.AddRange(new string[]
									{
										"SentToScope",
										"MessageContainsDataClassifications"
									});
									list2.AddRange(new string[]
									{
										"GenerateIncidentReport",
										"NotifySender"
									});
									this.transportRuleEditor.DefaultValues.Add("IncidentReportOriginalMail", IncidentReportOriginalMail.IncludeOriginalMail.ToString());
									this.transportRuleEditor.DefaultValues.Add("SenderNotifySettings", new SenderNotifySettings
									{
										NotifySender = NotifySenderType.RejectUnlessExplicitOverride.ToString()
									}.ToJsonString(null));
								}
							}
							else
							{
								list.AddRange(new string[]
								{
									"SentToScope",
									"MessageContainsDataClassifications"
								});
								list2.AddRange(new string[]
								{
									"GenerateIncidentReport",
									"NotifySender"
								});
								this.transportRuleEditor.DefaultValues.Add("IncidentReportOriginalMail", IncidentReportOriginalMail.IncludeOriginalMail.ToString());
								this.transportRuleEditor.DefaultValues.Add("SenderNotifySettings", new SenderNotifySettings
								{
									NotifySender = NotifySenderType.NotifyOnly.ToString()
								}.ToJsonString(null));
							}
						}
						this.transportRuleEditor.DefaultConditions = list.ToArray();
						this.transportRuleEditor.DefaultActions = list2.ToArray();
					}
				}
			}
		}

		private Properties FindPropertiesControl()
		{
			return (Properties)base.ContentControl;
		}

		private void CheckIfCopyMode()
		{
			if (base.Request.QueryString["action"] == "copy")
			{
				Properties properties = this.FindPropertiesControl();
				if (properties != null)
				{
					base.Title = Strings.CopyTransportRule;
					properties.UseSetObject = false;
					properties.GetObjectForNew = true;
					properties.CaptionTextField = null;
					this.transportRuleEditor.IsCopyMode = true;
				}
			}
		}

		protected DLPRuleEditor transportRuleEditor;
	}
}
