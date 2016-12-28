using System;
using System.Net;
using Android.Graphics;

namespace Facebook_OAuth
{
	public static class Utils
	{
		public static Bitmap GetImageBitmapFromUrl(string url)
		{
			Bitmap imageBitmap = null;
			using (var webClient = new WebClient())
			{
				var imageBytes = webClient.DownloadData(url);
				if (imageBytes != null && imageBytes.Length > 0)
				{
					imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
				}
			}
			return imageBitmap;
		}
	}
}
