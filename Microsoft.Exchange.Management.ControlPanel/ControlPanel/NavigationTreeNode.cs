using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class NavigationTreeNode
	{
		public NavigationTreeNode()
		{
		}

		public NavigationTreeNode(SiteMapNode siteMapNode)
		{
			this.ID = siteMapNode["id"];
			this.Title = siteMapNode.Title;
			this.Format = Util.GetPageTitleFormat(siteMapNode);
			this.Url = EcpUrl.ProcessUrl(siteMapNode.Url);
			this.HybridRole = siteMapNode["hybridRole"];
			bool noCache;
			bool.TryParse(siteMapNode["noCache"], out noCache);
			this.NoCache = noCache;
			bool flag;
			bool.TryParse(siteMapNode["contentPage"], out flag);
			this.hasContentPage = flag;
			this.isContentPage = flag;
		}

		[DataMember(EmitDefaultValue = false)]
		public int Selected { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string Title { get; private set; }

		[DataMember(EmitDefaultValue = false)]
		public string Format { get; private set; }

		[DataMember(EmitDefaultValue = false)]
		public string Url { get; internal set; }

		[DataMember(EmitDefaultValue = false)]
		public string ID { get; private set; }

		[DataMember(EmitDefaultValue = false)]
		public string Sprite { get; private set; }

		[DataMember(EmitDefaultValue = false)]
		public bool NoCache { get; private set; }

		[DataMember(EmitDefaultValue = false)]
		public string HybridRole { get; private set; }

		public List<NavigationTreeNode> Children
		{
			get
			{
				return this.children ?? NavigationTreeNode.emptyCollection;
			}
		}

		internal bool HasContentPage
		{
			get
			{
				if (!this.hasContentPage)
				{
					foreach (NavigationTreeNode navigationTreeNode in this.Children)
					{
						if (navigationTreeNode.HasContentPage)
						{
							this.hasContentPage = true;
							break;
						}
					}
				}
				return this.hasContentPage;
			}
		}

		internal string AggregateHybridRole()
		{
			if (!this.hybridRoleCalculated && string.IsNullOrEmpty(this.HybridRole) && !this.isContentPage)
			{
				StringBuilder stringBuilder = null;
				int selected = 0;
				int num = 0;
				foreach (NavigationTreeNode navigationTreeNode in this.Children)
				{
					string value = navigationTreeNode.AggregateHybridRole();
					if (string.IsNullOrEmpty(value))
					{
						stringBuilder = null;
						selected = num;
						break;
					}
					if (stringBuilder == null)
					{
						stringBuilder = new StringBuilder();
					}
					else
					{
						stringBuilder.Append(',');
					}
					stringBuilder.Append(value);
					num++;
				}
				if (stringBuilder != null)
				{
					this.HybridRole = stringBuilder.ToString();
				}
				this.hybridRoleCalculated = true;
				this.Selected = selected;
			}
			return this.HybridRole;
		}

		public void AddChild(NavigationTreeNode node)
		{
			if (this.children == null)
			{
				this.children = new List<NavigationTreeNode>();
			}
			this.children.Add(node);
		}

		private readonly bool isContentPage;

		private static List<NavigationTreeNode> emptyCollection = new List<NavigationTreeNode>();

		[DataMember(Name = "Children", EmitDefaultValue = false)]
		private List<NavigationTreeNode> children;

		private bool hasContentPage;

		private bool hybridRoleCalculated;
	}
}
