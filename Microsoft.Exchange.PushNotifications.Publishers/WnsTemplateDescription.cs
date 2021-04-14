using System;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	public class WnsTemplateDescription
	{
		private WnsTemplateDescription(string name, int numOfTexts = 0, int numOfImages = 0)
		{
			this.Name = name;
			this.MaxNumOfTexts = numOfTexts;
			this.MaxNumOfImages = numOfImages;
		}

		public string Name { get; private set; }

		public int MaxNumOfTexts { get; private set; }

		public int MaxNumOfImages { get; private set; }

		public static WnsTemplateDescription GetTileDescription(WnsTile tile)
		{
			WnsTemplateDescription wnsTemplateDescription = WnsTemplateDescription.TileDescriptions[(int)tile];
			if (wnsTemplateDescription == null)
			{
				wnsTemplateDescription = WnsTemplateDescription.CreateTileDescription(tile);
				WnsTemplateDescription.TileDescriptions[(int)tile] = wnsTemplateDescription;
			}
			return wnsTemplateDescription;
		}

		public static WnsTemplateDescription GetToastDescription(WnsToast toast)
		{
			WnsTemplateDescription wnsTemplateDescription = WnsTemplateDescription.ToastDescriptions[(int)toast];
			if (wnsTemplateDescription == null)
			{
				wnsTemplateDescription = WnsTemplateDescription.CreateToastDescription(toast);
				WnsTemplateDescription.ToastDescriptions[(int)toast] = wnsTemplateDescription;
			}
			return wnsTemplateDescription;
		}

		public override string ToString()
		{
			return this.Name;
		}

		private static WnsTemplateDescription CreateTileDescription(WnsTile tile)
		{
			switch (tile)
			{
			case WnsTile.SquareImage:
				return new WnsTemplateDescription("TileSquareImage", 0, 1);
			case WnsTile.SquareBlock:
				return new WnsTemplateDescription("TileSquareBlock", 2, 0);
			case WnsTile.SquareText01:
				return new WnsTemplateDescription("TileSquareText01", 4, 0);
			case WnsTile.SquareText02:
				return new WnsTemplateDescription("TileSquareText02", 2, 0);
			case WnsTile.SquareText03:
				return new WnsTemplateDescription("TileSquareText03", 4, 0);
			case WnsTile.SquareText04:
				return new WnsTemplateDescription("TileSquareText04", 1, 0);
			case WnsTile.SquarePeekImageAndText01:
				return new WnsTemplateDescription("TileSquarePeekImageAndText01", 4, 1);
			case WnsTile.SquarePeekImageAndText02:
				return new WnsTemplateDescription("TileSquarePeekImageAndText02", 2, 1);
			case WnsTile.SquarePeekImageAndText03:
				return new WnsTemplateDescription("TileSquarePeekImageAndText03", 4, 1);
			case WnsTile.SquarePeekImageAndText04:
				return new WnsTemplateDescription("TileSquarePeekImageAndText04", 1, 1);
			case WnsTile.WideImageCollection:
				return new WnsTemplateDescription("TileWideImageCollection", 0, 5);
			case WnsTile.WideImageAndText01:
				return new WnsTemplateDescription("TileWideImageAndText01", 1, 1);
			case WnsTile.WideImageAndText02:
				return new WnsTemplateDescription("TileWideImageAndText02", 2, 1);
			case WnsTile.WideImage:
				return new WnsTemplateDescription("TileWideImage", 0, 1);
			case WnsTile.WideBlockAndText01:
				return new WnsTemplateDescription("TileWideBlockAndText01", 6, 0);
			case WnsTile.WideBlockAndText02:
				return new WnsTemplateDescription("TileWideBlockAndText02", 3, 0);
			case WnsTile.WidePeekImageCollection01:
				return new WnsTemplateDescription("TileWidePeekImageCollection01", 2, 5);
			case WnsTile.WidePeekImageCollection02:
				return new WnsTemplateDescription("TileWidePeekImageCollection02", 5, 5);
			case WnsTile.WidePeekImageCollection03:
				return new WnsTemplateDescription("TileWidePeekImageCollection03", 1, 5);
			case WnsTile.WidePeekImageCollection04:
				return new WnsTemplateDescription("TileWidePeekImageCollection04", 1, 5);
			case WnsTile.WidePeekImageCollection05:
				return new WnsTemplateDescription("TileWidePeekImageCollection05", 2, 6);
			case WnsTile.WidePeekImageCollection06:
				return new WnsTemplateDescription("TileWidePeekImageCollection06", 1, 6);
			case WnsTile.WidePeekImageAndText01:
				return new WnsTemplateDescription("TileWidePeekImageAndText01", 1, 1);
			case WnsTile.WidePeekImageAndText02:
				return new WnsTemplateDescription("TileWidePeekImageAndText02", 5, 1);
			case WnsTile.WidePeekImage01:
				return new WnsTemplateDescription("TileWidePeekImage01", 2, 1);
			case WnsTile.WidePeekImage02:
				return new WnsTemplateDescription("TileWidePeekImage02", 5, 1);
			case WnsTile.WidePeekImage03:
				return new WnsTemplateDescription("TileWidePeekImage03", 1, 1);
			case WnsTile.WidePeekImage04:
				return new WnsTemplateDescription("TileWidePeekImage04", 1, 1);
			case WnsTile.WidePeekImage05:
				return new WnsTemplateDescription("TileWidePeekImage05", 2, 2);
			case WnsTile.WidePeekImage06:
				return new WnsTemplateDescription("TileWidePeekImage06", 1, 2);
			case WnsTile.WideSmallImageAndText01:
				return new WnsTemplateDescription("TileWideSmallImageAndText01", 1, 1);
			case WnsTile.WideSmallImageAndText02:
				return new WnsTemplateDescription("TileWideSmallImageAndText02", 5, 1);
			case WnsTile.WideSmallImageAndText03:
				return new WnsTemplateDescription("TileWideSmallImageAndText03", 1, 1);
			case WnsTile.WideSmallImageAndText04:
				return new WnsTemplateDescription("TileWideSmallImageAndText04", 2, 1);
			case WnsTile.WideSmallImageAndText05:
				return new WnsTemplateDescription("TileWideSmallImageAndText05", 2, 1);
			case WnsTile.WideText01:
				return new WnsTemplateDescription("TileWideText01", 5, 0);
			case WnsTile.WideText02:
				return new WnsTemplateDescription("TileWideText02", 9, 0);
			case WnsTile.WideText03:
				return new WnsTemplateDescription("TileWideText03", 1, 0);
			case WnsTile.WideText04:
				return new WnsTemplateDescription("TileWideText04", 1, 0);
			case WnsTile.WideText05:
				return new WnsTemplateDescription("TileWideText05", 5, 0);
			case WnsTile.WideText06:
				return new WnsTemplateDescription("TileWideText06", 10, 0);
			case WnsTile.WideText07:
				return new WnsTemplateDescription("TileWideText07", 9, 0);
			case WnsTile.WideText08:
				return new WnsTemplateDescription("TileWideText08", 10, 0);
			case WnsTile.WideText09:
				return new WnsTemplateDescription("TileWideText09", 2, 0);
			case WnsTile.WideText10:
				return new WnsTemplateDescription("TileWideText10", 9, 0);
			case WnsTile.WideText11:
				return new WnsTemplateDescription("TileWideText11", 10, 0);
			default:
				throw new NotSupportedException(string.Format("{0} tile template not supported", tile.ToString()));
			}
		}

		private static WnsTemplateDescription CreateToastDescription(WnsToast toast)
		{
			switch (toast)
			{
			case WnsToast.ImageAndText01:
				return new WnsTemplateDescription("ToastImageAndText01", 1, 1);
			case WnsToast.ImageAndText02:
				return new WnsTemplateDescription("ToastImageAndText02", 2, 1);
			case WnsToast.ImageAndText03:
				return new WnsTemplateDescription("ToastImageAndText03", 2, 1);
			case WnsToast.ImageAndText04:
				return new WnsTemplateDescription("ToastImageAndText04", 3, 1);
			case WnsToast.Text01:
				return new WnsTemplateDescription("ToastText01", 1, 0);
			case WnsToast.Text02:
				return new WnsTemplateDescription("ToastText02", 2, 0);
			case WnsToast.Text03:
				return new WnsTemplateDescription("ToastText03", 2, 0);
			case WnsToast.Text04:
				return new WnsTemplateDescription("ToastText04", 3, 0);
			default:
				throw new NotSupportedException(string.Format("{0} toast template not supported", toast.ToString()));
			}
		}

		private static readonly WnsTemplateDescription[] TileDescriptions = new WnsTemplateDescription[Enum.GetNames(typeof(WnsTile)).Length];

		private static readonly WnsTemplateDescription[] ToastDescriptions = new WnsTemplateDescription[Enum.GetNames(typeof(WnsToast)).Length];
	}
}
