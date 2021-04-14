using System;

namespace Microsoft.Exchange.Transport.RightsManagement
{
	internal class E4eCssStyleV2 : E4eCssStyle
	{
		internal override string ArrowImgBase64
		{
			get
			{
				return "iVBORw0KGgoAAAANSUhEUgAAABoAAAAaCAMAAACelLz8AAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAyBpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuMC1jMDYwIDYxLjEzNDc3NywgMjAxMC8wMi8xMi0xNzozMjowMCAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvIiB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIgeG1sbnM6c3RSZWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZVJlZiMiIHhtcDpDcmVhdG9yVG9vbD0iQWRvYmUgUGhvdG9zaG9wIENTNSBXaW5kb3dzIiB4bXBNTTpJbnN0YW5jZUlEPSJ4bXAuaWlkOjZBODRDRDVEMDY3QTExRTE5MUNFQ0QwMDBGQzc0RUU1IiB4bXBNTTpEb2N1bWVudElEPSJ4bXAuZGlkOjZBODRDRDVFMDY3QTExRTE5MUNFQ0QwMDBGQzc0RUU1Ij4gPHhtcE1NOkRlcml2ZWRGcm9tIHN0UmVmOmluc3RhbmNlSUQ9InhtcC5paWQ6NkE4NENENUIwNjdBMTFFMTkxQ0VDRDAwMEZDNzRFRTUiIHN0UmVmOmRvY3VtZW50SUQ9InhtcC5kaWQ6NkE4NENENUMwNjdBMTFFMTkxQ0VDRDAwMEZDNzRFRTUiLz4gPC9yZGY6RGVzY3JpcHRpb24+IDwvcmRmOlJERj4gPC94OnhtcG1ldGE+IDw/eHBhY2tldCBlbmQ9InIiPz5VmLPXAAACbVBMVEXs7/BqquFRnN3l6+9bot/t8PDh6O/n7O9YoN7p7vDo7O9uq+Hi6e+NuuXk6+/M3OzN3exMm93J2+xImNyszemtzulWn95npuBPnN1eo9/i6u/f5+9Kmdzm6+9Qnd1/s+Pr7vDq7vBnp+CbwufL3exJmNyDtuRwrOFKmtxNm93U4e3T4e1CldtDltuNu+VurOFSnd1Zod6Rvea/1et9s+NUnt690+urzejU4O2zz+rs8PBzreGKueW70+tQnN1Xn97n7PBOnN251erm6/Dg6e9rquFrquB5seJipeBJmdyoyeiTv+VipuCdw+fh6O7w8fFMmt3r7/DD2euZwOeszOmhxufW5O6AtePV4e7V4u7P3u2Pu+WSvOaIuORgpt+91eqtzOnt7/BnqOBoqeHR4u1XoN6Bs+Ndo9+wz+lOm914r+Lh6e5Un96z0OpSnN3H2uynyejT4e55seO+1evW4u7X4+7d5u/O3u2gxOdxrOFfo9+exOfF2euixujU4u220eqwzum30erZ5O5kpeDf6O7l6vB5sOOJueSTv+ZlpuBWoN5Rnt2MuuXH2eynyOmEtuR1r+K00Org6O9xreHY4+7Z4+640emwzOmSveVxreJTnt5hpODp7fBPm93G2Ovg6O7u7/Db5e7c5u/d5u7P3+2fxueqy+jG2eu20Oq1z+qCtOSJuORhpeDI2uxNmt3s7vDK2+3I2+yGtuS30em81uqQvOVGl9zR4O1jpuCXweamyeiyzumHt+VpqeGvzemDteSZwueWwOVkpuCryujT4O3L2+xtq+BFltzL2u3Q3u2z0eqwzemCteN9suPv8vHydXkhAAAAz3RSTlP//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////wAFdzktAAABxklEQVR42mI4BwGsik1OG4yLVbYzW0FFzjFAKL/cg+LCgkyCupI9BaUoUgLuXcLy+0S2iEzk5TDlnsYFl2I4zSLGW8fMCuJzLpleJWHkDJXyr5VhrJE9Bwflaqp6DhAps8zJFueQAZfetgMMICkppR1HzqECxaK8o0CpwGrhlefQgRaHCec5BmbxCZxQAe3etZOgzApVIVYG4+O2MLVTZ6ZmLYYwF9rHKDDMYGmBmxOZFGAtDWbxhCezM5w1AXGs2ECg9YSbRqUWSCqxQ06AwbCfB8gMYwSDY5KmEmuiQHKnZuUz5KiBpFKYQEBOw0CMiWUzSErUUIhBXX4+kMUnKipibp7RoOQouQJs2a7ujQy84syIYPA1SG8EB6VDO4sUw2GdIJiM9Cpdn3kQZvbsNGaGRfabYFJm3jv5ocwEHUsGBkWbddqwuFsPjdhzss2dzMDgPSlmPQctCBmUPUMYgFIKoTLRC1BkOOvl5NnB8cVuY7eaHTm6+iQOlUETAI+RjPpuF1aoNUKxc7kL4clGystD88zS5RYRJfzLtnLY7ZmClKIUBJQ5BPdyc+u7smju5+dDSYfn2FVsg9vi9Hkt4/lgdgIEGABSfHCQrQseUQAAAABJRU5ErkJggg==";
			}
		}

		internal override string LockImgBase64
		{
			get
			{
				return "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAyBpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuMC1jMDYwIDYxLjEzNDc3NywgMjAxMC8wMi8xMi0xNzozMjowMCAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvIiB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIgeG1sbnM6c3RSZWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZVJlZiMiIHhtcDpDcmVhdG9yVG9vbD0iQWRvYmUgUGhvdG9zaG9wIENTNSBXaW5kb3dzIiB4bXBNTTpJbnN0YW5jZUlEPSJ4bXAuaWlkOkU2QjZFRDJFOTk0OTExRTE5MEFFQUY3RkJFODY1Qzc3IiB4bXBNTTpEb2N1bWVudElEPSJ4bXAuZGlkOkU2QjZFRDJGOTk0OTExRTE5MEFFQUY3RkJFODY1Qzc3Ij4gPHhtcE1NOkRlcml2ZWRGcm9tIHN0UmVmOmluc3RhbmNlSUQ9InhtcC5paWQ6RTZCNkVEMkM5OTQ5MTFFMTkwQUVBRjdGQkU4NjVDNzciIHN0UmVmOmRvY3VtZW50SUQ9InhtcC5kaWQ6RTZCNkVEMkQ5OTQ5MTFFMTkwQUVBRjdGQkU4NjVDNzciLz4gPC9yZGY6RGVzY3JpcHRpb24+IDwvcmRmOlJERj4gPC94OnhtcG1ldGE+IDw/eHBhY2tldCBlbmQ9InIiPz4P2R/9AAAAwklEQVR42mL8//8/AyWABV2goqKCE0jVAXEsEEsD8VMgngLEfR0dHb8IGgAE64DYHohXAvFxILaEGqgOxInoihmRvQC03QNIbQfiXKBtU5DEc4DUZCDWA4pfRjaACc1ASyi9A018K5Q2Q3cBugGiUPo+mvgjKM2G0wtAZx5DcgE+cBzoDStsLrAkMuYs8XkBGdQAbWIE0fhMw2fARDSaZAPy0WiSDWhBo0k2gCiA1wBg1P4nxYAFRFrahDMvkAMAAgwA6hUy/dUMmhQAAAAASUVORK5CYII=";
			}
		}

		internal override string RegularTextStyle
		{
			get
			{
				return "font:13px Segoe UI Semilight; font-family: Segoe UI Semilight, Segoe UI, Segoe, Helvetica, Tahoma, Arial, sans-serif; color:#333333;";
			}
		}

		internal override string DisclaimerTextStyle
		{
			get
			{
				return "font: 11px Segoe UI Regular; font-family: Segoe UI Regular, Segoe UI, Segoe, Helvetica, Tahoma, Arial, sans-serif; color:#666666;";
			}
		}

		internal override string HostedTextStyle
		{
			get
			{
				return "font:13px Segoe UI Semilight;  font-family: Segoe UI Semilight, Segoe UI, Segoe, Helvetica, Tahoma, Arial, sans-serif; color:#666666;";
			}
		}

		internal override string AnchorTagStyle
		{
			get
			{
				return "font: 11px Segoe UI Regular; font-family: Segoe UI Regular, Segoe UI, Segoe, Helvetica, Tahoma, Arial, sans-serif; color:#0072C6; text-decoration:none;";
			}
		}

		internal override string EmailTextAnchorStyle
		{
			get
			{
				return "text-decoration:none; cursor:text;";
			}
		}

		internal override string LogoSizeStyle
		{
			get
			{
				return "height:70px; width:170px;";
			}
		}

		internal override string LockSizeStyle
		{
			get
			{
				return "height:16px; width:16px;";
			}
		}

		internal override string BoldTextStyle
		{
			get
			{
				return "font: 13px Segoe UI Semibold; font-family: Segoe UI Semibold, Segoe UI, Segoe, Helvetica, Tahoma, Arial, sans-serif; color:#000000;";
			}
		}

		internal override string HeaderDivStyle
		{
			get
			{
				return "padding: 10px 18px 10px 18px; background-color:rgb(0, 114, 198); line-height:30px;";
			}
		}

		internal override string HeaderTextStyle
		{
			get
			{
				return "font: 13px Segoe UI Regular; font-family: Segoe UI Regular, Segoe UI, Segoe, Helvetica, Tahoma, Arial, sans-serif; color: #ffffff;";
			}
		}

		internal override string ViewMessageOTPButtonStyle
		{
			get
			{
				return "font: 14px Segoe UI Regular; color:#0072C6; font-family: Segoe UI Regular, Segoe UI, Segoe, Helvetica, Tahoma, Arial, sans-serif; background-color: transparent; border: none; cursor: pointer; white-space:normal; text-align:left;";
			}
		}

		internal override string ViewportMetaTag
		{
			get
			{
				return "<meta name='viewport' content='user-scalable=0, width=device-width, initial-scale=1.0, maximum-scale=1.0'>";
			}
		}

		internal override string MainContentDivStyle
		{
			get
			{
				return "max-width:700px; margin-left:auto; margin-right:auto; padding-left:10px; padding-right:10px;";
			}
		}

		internal override string EncryptedMessageDivStyle
		{
			get
			{
				return "padding-top:14%; padding-bottom:20px; font: 22px Segoe UI Semilight; color:#000000; font-family: Segoe UI Semilight, Segoe UI, Segoe, Helvetica, Tahoma, Arial, sans-serif; line-height:125%;";
			}
		}

		internal override string ViewMessageButtonDivStyle
		{
			get
			{
				return "padding-bottom:21%";
			}
		}

		internal override string HostedMessageTableStyle
		{
			get
			{
				return "padding-top:8px; padding-bottom:15px; border-top: 1px solid #EAECEE; width:100%;";
			}
		}

		internal override string EmailAddressSpanStyle
		{
			get
			{
				return "word-wrap:break-word";
			}
		}

		internal override string ButtonStyle(string base64Image)
		{
			return string.Format("font:14px Segoe UI Regular;  font-family: Segoe UI Regular, Segoe UI, Segoe, Helvetica, Tahoma, Arial, sans-serif; color:#0072C6; cursor: pointer; background: #ffffff url(data:{0};base64,{1}) no-repeat; border: none; padding-left: 34px; min-height:26px; white-space:normal; text-align:left;", "image/png", base64Image);
		}
	}
}
