using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Filtering;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class OwaFilteringServiceInvokerRequest : FilteringServiceInvokerRequest
	{
		private OwaFilteringServiceInvokerRequest(string organizationId, TimeSpan scanTimeout, int textScanLimit, MapiFipsDataStreamFilteringRequest mapiFipsDataStreamFilteringRequest) : base(organizationId, scanTimeout, textScanLimit, mapiFipsDataStreamFilteringRequest)
		{
		}

		public static OwaFilteringServiceInvokerRequest CreateInstance(Item item, IExtendedMapiFilteringContext context)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			string text = item.Session.OrganizationId.ToExternalDirectoryOrganizationId();
			if (string.IsNullOrEmpty(text))
			{
				text = Guid.Empty.ToString();
			}
			TimeSpan scanTimeout = OwaFilteringServiceInvokerRequest.GetScanTimeout(item, context);
			MapiFipsDataStreamFilteringRequest mapiFipsDataStreamFilteringRequest = MapiFipsDataStreamFilteringRequest.CreateInstance(item, context);
			return new OwaFilteringServiceInvokerRequest(text, scanTimeout, 153600, mapiFipsDataStreamFilteringRequest);
		}

		private static TimeSpan GetScanTimeout(Item item, IExtendedMapiFilteringContext context)
		{
			List<KeyValuePair<string, double>> list = null;
			IList<AttachmentHandle> allHandles = item.AttachmentCollection.GetAllHandles();
			if (allHandles != null && allHandles.Count > 0)
			{
				foreach (AttachmentHandle handle in allHandles)
				{
					using (Attachment attachment = item.AttachmentCollection.Open(handle))
					{
						if (context.NeedsClassificationScan(attachment))
						{
							if (list == null)
							{
								list = new List<KeyValuePair<string, double>>();
							}
							list.Add(new KeyValuePair<string, double>(attachment.FileName, (double)attachment.Size));
						}
					}
				}
			}
			if (list == null && !context.NeedsClassificationScan())
			{
				return new TimeSpan(0L);
			}
			RulesScanTimeout rulesScanTimeout = new RulesScanTimeout(OwaFilteringServiceInvokerRequest.DefaultScanVelocities, 60000);
			return rulesScanTimeout.GetTimeout(context.NeedsClassificationScan() ? ((double)item.Body.Size) : 0.0, list);
		}

		private const int MinFipsTimeoutInMilliseconds = 60000;

		private const int MaxTextScanLimit = 153600;

		private static readonly Dictionary<string, uint> DefaultScanVelocities = new Dictionary<string, uint>
		{
			{
				".",
				30U
			},
			{
				"doc",
				1292U
			},
			{
				"docx",
				92U
			},
			{
				"xls",
				166U
			},
			{
				"xlsx",
				30U
			},
			{
				"ppt",
				7000U
			},
			{
				"pptx",
				400U
			},
			{
				"htm",
				120U
			},
			{
				"html",
				120U
			},
			{
				"pdf",
				840U
			}
		};
	}
}
