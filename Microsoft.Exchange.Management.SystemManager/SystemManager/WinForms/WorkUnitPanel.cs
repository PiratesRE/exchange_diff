using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Management.Automation;
using System.Text;
using System.Windows.Forms;
using Microsoft.Exchange.CommonHelpProvider;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.ManagementGUI;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class WorkUnitPanel : CollapsiblePanel
	{
		public new int Top
		{
			get
			{
				return base.Top;
			}
			set
			{
				base.Top = value;
				base.UpdateBounds(base.Left, value, base.Width, base.Height);
			}
		}

		public WorkUnitPanel()
		{
			this.InitializeComponent();
			this.descriptionLabel.Click += delegate(object param0, EventArgs param1)
			{
				this.descriptionLabel.Select();
			};
		}

		protected override void OnParentChanged(EventArgs e)
		{
			base.OnParentChanged(e);
			this.Refresh();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.TimerEnabled = false;
				this.WorkUnitPanelItem = null;
			}
			base.Dispose(disposing);
		}

		[DefaultValue(null)]
		internal WorkUnit WorkUnit
		{
			get
			{
				if (this.WorkUnitPanelItem == null)
				{
					return null;
				}
				return this.WorkUnitPanelItem.WorkUnit;
			}
		}

		[DefaultValue(null)]
		internal WorkUnitPanelItem WorkUnitPanelItem
		{
			get
			{
				return this.workUnitPanelItem;
			}
			set
			{
				if (value != this.WorkUnitPanelItem)
				{
					if (this.WorkUnitPanelItem != null)
					{
						this.WorkUnitPanelItem.WorkUnitPropertyChanged -= new PropertyChangedEventHandler(this.WorkUnit_PropertyChanged);
					}
					this.workUnitPanelItem = value;
					if (this.WorkUnitPanelItem != null)
					{
						this.WorkUnitPanelItem.WorkUnitPropertyChanged += new PropertyChangedEventHandler(this.WorkUnit_PropertyChanged);
					}
				}
				this.Refresh();
			}
		}

		internal static Size MeasureExpandedSizeForPanelItem(WorkUnitPanel templatePanel, WorkUnitPanelItem panelItem)
		{
			templatePanel.SuspendLayout();
			templatePanel.WorkUnitPanelItem = panelItem;
			templatePanel.FastSetIsMinimized(false);
			templatePanel.ResumeLayout();
			return templatePanel.Size;
		}

		internal static Size MeasureCollapsedSizeForPanelItem(WorkUnitPanel templatePanel, WorkUnitPanelItem panelItem)
		{
			Size size = templatePanel.Size;
			size.Height = templatePanel.CaptionStrip.Height;
			return size;
		}

		private void WorkUnit_PropertyChanged(object sender, EventArgs e)
		{
			this.Refresh();
		}

		public override void Refresh()
		{
			if (!base.InvokeRequired)
			{
				this.UpdatePanel();
				base.Refresh();
				return;
			}
			if (this.TimerEnabled)
			{
				return;
			}
			base.Invoke(new MethodInvoker(this.Refresh));
		}

		private void UpdatePanel()
		{
			base.SuspendLayout();
			base.CaptionStrip.SuspendLayout();
			try
			{
				WorkUnit workUnit = this.WorkUnit;
				if (workUnit != null)
				{
					this.Text = workUnit.Text;
					base.Icon = workUnit.Icon;
					if (this.descriptionLabel.Links.Count > 0)
					{
						this.descriptionLabel.Links.Clear();
					}
					WorkUnitStatus status = workUnit.Status;
					this.UpdateProgressBar(status);
					bool flag = true;
					bool flag2 = true;
					TaskState taskState = 0;
					if (base.Parent is WorkUnitsPanel)
					{
						taskState = ((WorkUnitsPanel)base.Parent).TaskState;
						flag = (taskState != null && status != WorkUnitStatus.NotStarted);
						flag2 = (taskState == null || (taskState == 1 && status == WorkUnitStatus.InProgress) || (taskState == 1 && status == WorkUnitStatus.NotStarted));
					}
					base.StatusVisible = (status != WorkUnitStatus.InProgress);
					string status2 = LocalizedDescriptionAttribute.FromEnum(typeof(WorkUnitStatus), status);
					StringBuilder stringBuilder = new StringBuilder(2048);
					if (flag2)
					{
						if (taskState == 1 && status == WorkUnitStatus.InProgress)
						{
							string text = (workUnit.StatusDescription == null) ? null : workUnit.StatusDescription.Trim();
							if (!string.IsNullOrEmpty(text))
							{
								stringBuilder.AppendLine(Strings.StatusDescription(text));
							}
						}
						else
						{
							string value = (workUnit.Description == null) ? null : workUnit.Description.Trim();
							if (!string.IsNullOrEmpty(value))
							{
								stringBuilder.AppendLine(value);
							}
						}
					}
					string executedCommandTextForWorkUnit = workUnit.ExecutedCommandTextForWorkUnit;
					switch (status)
					{
					case WorkUnitStatus.NotStarted:
						base.StatusImage = null;
						base.Status = "";
						if (taskState == 1)
						{
							base.Status = Strings.WorkUnitStatusPending;
						}
						else if (taskState == 2)
						{
							base.Status = Strings.WorkUnitStatusCancelled;
							base.StatusImage = WorkUnitPanel.warning;
							if (workUnit.CanShowExecutedCommand && !string.IsNullOrEmpty(executedCommandTextForWorkUnit))
							{
								stringBuilder.AppendLine(Strings.MshCommandExecutedFailed(executedCommandTextForWorkUnit));
							}
						}
						break;
					case WorkUnitStatus.InProgress:
						base.Status = "";
						base.StatusImage = null;
						break;
					case WorkUnitStatus.Completed:
						if (!flag2)
						{
							base.Status = status2;
							base.StatusImage = ((workUnit.Warnings.Count == 0) ? WorkUnitPanel.completed : WorkUnitPanel.warning);
							if (workUnit.Warnings.Count > 0)
							{
								stringBuilder.AppendLine(workUnit.WarningsDescription);
							}
							if (workUnit.CanShowExecutedCommand && !string.IsNullOrEmpty(executedCommandTextForWorkUnit))
							{
								stringBuilder.AppendLine(Strings.MshCommandExecutedSuccessfully(executedCommandTextForWorkUnit));
							}
						}
						break;
					case WorkUnitStatus.Failed:
						if (!flag2)
						{
							base.FastSetIsMinimized(false);
							base.Status = status2;
							base.StatusImage = WorkUnitPanel.failed;
							if (workUnit.Errors.Count > 0)
							{
								for (int i = 0; i < workUnit.Errors.Count; i++)
								{
									stringBuilder.AppendLine(Strings.WorkUnitError);
									ErrorRecord errorRecord = workUnit.Errors[i];
									ErrorDetails errorDetails = errorRecord.ErrorDetails;
									string text2 = null;
									if (errorDetails != null && !string.IsNullOrEmpty(errorDetails.RecommendedAction))
									{
										text2 = errorDetails.RecommendedAction;
									}
									else
									{
										LocalizedException ex = errorRecord.Exception as LocalizedException;
										if (ex != null)
										{
											Uri uri = null;
											if (Microsoft.Exchange.CommonHelpProvider.HelpProvider.TryGetErrorAssistanceUrl(ex, out uri))
											{
												text2 = uri.ToString();
											}
										}
									}
									if (errorDetails != null)
									{
										if (!string.IsNullOrEmpty(errorDetails.Message))
										{
											stringBuilder.AppendLine(errorDetails.Message);
										}
									}
									else
									{
										Exception ex2 = errorRecord.Exception;
										string text3 = "";
										while (ex2 != null)
										{
											if (ex2.Message != text3)
											{
												text3 = ex2.Message;
												stringBuilder.AppendLine(text3);
											}
											ex2 = ex2.InnerException;
											if (ex2 != null)
											{
												stringBuilder.AppendLine();
											}
										}
									}
									if (!string.IsNullOrEmpty(text2))
									{
										string value2 = Strings.WorkUnitErrorAssistanceLink;
										LinkLabel.Link link = new LinkLabel.Link();
										link.LinkData = text2;
										link.Start = new StringInfo(stringBuilder.ToString()).LengthInTextElements;
										link.Length = new StringInfo(value2).LengthInTextElements;
										this.descriptionLabel.Links.Add(link);
										stringBuilder.AppendLine(value2);
									}
									if (i < workUnit.Errors.Count - 1)
									{
										stringBuilder.AppendLine();
									}
								}
							}
							if (workUnit.Warnings.Count > 0)
							{
								if (workUnit.Errors.Count > 0)
								{
									stringBuilder.AppendLine();
								}
								stringBuilder.AppendLine(workUnit.WarningsDescription);
							}
							if (workUnit.CanShowExecutedCommand && !string.IsNullOrEmpty(executedCommandTextForWorkUnit))
							{
								if (workUnit.Warnings.Count > 0 || workUnit.Errors.Count > 0)
								{
									stringBuilder.AppendLine();
								}
								stringBuilder.AppendLine(Strings.MshCommandExecutedFailed(executedCommandTextForWorkUnit));
							}
						}
						break;
					}
					if (workUnit.CanShowElapsedTime && flag)
					{
						stringBuilder.AppendLine();
						stringBuilder.AppendLine(workUnit.ElapsedTimeText);
					}
					this.Description = stringBuilder.ToString().Trim();
					this.TimerEnabled = (status == WorkUnitStatus.InProgress);
				}
			}
			finally
			{
				base.CaptionStrip.ResumeLayout(false);
				base.CaptionStrip.PerformLayout();
				base.ResumeLayout();
			}
		}

		private void InitializeComponent()
		{
			this.descriptionLabel = new AutoHeightLabel();
			base.SuspendLayout();
			this.descriptionLabel.Dock = DockStyle.Top;
			this.descriptionLabel.Location = new Point(0, 25);
			this.descriptionLabel.Name = "descriptionLabel";
			this.descriptionLabel.Padding = new Padding(21, 4, 4, 4);
			this.descriptionLabel.Size = new Size(16, 17);
			this.descriptionLabel.TabIndex = 1;
			this.descriptionLabel.UseMnemonic = false;
			this.descriptionLabel.LinkClicked += this.descriptionLabel_LinkLabelLinkClickedEventHandler;
			this.AutoSize = true;
			base.Controls.Add(this.descriptionLabel);
			base.Name = "WorkUnitPanel";
			base.Controls.SetChildIndex(this.descriptionLabel, 0);
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		[RefreshProperties(RefreshProperties.All)]
		[EditorBrowsable(EditorBrowsableState.Always)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Browsable(true)]
		[Bindable(true)]
		[DefaultValue("")]
		public string Description
		{
			get
			{
				return this.descriptionLabel.Text;
			}
			set
			{
				this.descriptionLabel.Text = value;
			}
		}

		public string GetSummaryText()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(this.Text);
			stringBuilder.AppendLine(base.Status);
			stringBuilder.AppendLine();
			if (this.descriptionLabel.Links.Count > 0)
			{
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				while (num3 < this.descriptionLabel.Links.Count && num2 < this.Description.Length)
				{
					LinkLabel.Link link = this.descriptionLabel.Links[num3];
					num2 = link.Start;
					stringBuilder.Append(this.Description.ToString().Substring(num, num2 - num));
					stringBuilder.AppendFormat("{0} {1}", Strings.WorkUnitErrorAssistanceLink, (string)link.LinkData);
					num = num2 + link.Length;
					num3++;
				}
				stringBuilder.Append(this.Description.ToString().Substring(num));
			}
			else
			{
				stringBuilder.Append(this.Description);
			}
			return stringBuilder.ToString();
		}

		private void descriptionLabel_LinkLabelLinkClickedEventHandler(object sender, LinkLabelLinkClickedEventArgs e)
		{
			string text = e.Link.LinkData as string;
			if (!string.IsNullOrEmpty(text))
			{
				try
				{
					Process.Start(text);
				}
				catch (Win32Exception)
				{
					base.ShowError(Strings.WorkUnitRecommendedActionLinkError(text));
				}
				catch (FileNotFoundException)
				{
					base.ShowError(Strings.WorkUnitRecommendedActionLinkError(text));
				}
			}
		}

		private bool TimerEnabled
		{
			get
			{
				return null != this.updateTimer;
			}
			set
			{
				if (value != this.TimerEnabled)
				{
					if (value)
					{
						this.updateTimer = new Timer();
						this.updateTimer.Interval = 200;
						this.updateTimer.Tick += this.WorkUnit_PropertyChanged;
						this.updateTimer.Enabled = true;
						return;
					}
					this.updateTimer.Dispose();
					this.updateTimer = null;
				}
			}
		}

		private void UpdateProgressBar(WorkUnitStatus workUnitStatus)
		{
			if (workUnitStatus != WorkUnitStatus.InProgress)
			{
				if (this.progressBar != null)
				{
					base.CaptionStrip.SuspendLayout();
					base.CaptionStrip.Items.Remove(this.progressBar);
					base.CaptionStrip.ResumeLayout();
					this.progressBar.Dispose();
					this.progressBar = null;
				}
				return;
			}
			if (this.progressBar == null)
			{
				base.CaptionStrip.SuspendLayout();
				this.progressBar = new ExchangeToolStripProgressBar();
				this.progressBar.Alignment = ToolStripItemAlignment.Right;
				this.progressBar.AutoSize = false;
				this.progressBar.Name = "progressBar";
				this.progressBar.Minimum = 0;
				this.progressBar.Maximum = 100;
				this.progressBar.Size = new Size(100, 12);
				this.progressBar.Overflow = ToolStripItemOverflow.Never;
				base.CaptionStrip.Items.Add(this.progressBar);
				base.CaptionStrip.ResumeLayout();
			}
			int percentComplete = this.WorkUnit.PercentComplete;
			this.progressBar.Value = percentComplete;
			if ((percentComplete > 0 && percentComplete < 100) || !this.TimerEnabled)
			{
				this.progressBar.Style = ProgressBarStyle.Continuous;
				return;
			}
			this.progressBar.Style = ProgressBarStyle.Marquee;
		}

		private AutoHeightLabel descriptionLabel;

		private static Bitmap completed = IconLibrary.ToSmallBitmap(Icons.Complete);

		private static Bitmap failed = IconLibrary.ToSmallBitmap(Icons.Error);

		private static Bitmap warning = IconLibrary.ToSmallBitmap(Icons.Warning);

		private WorkUnitPanelItem workUnitPanelItem;

		private Timer updateTimer;

		private ExchangeToolStripProgressBar progressBar;
	}
}
