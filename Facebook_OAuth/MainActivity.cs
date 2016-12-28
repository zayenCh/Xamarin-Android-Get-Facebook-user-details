using Android.App;
using Android.Widget;
using Android.OS;
using System;
using Xamarin.Auth;
using System.Collections.Generic;
using Facebook;
using Newtonsoft.Json;
using Android.Graphics;
using Java.Net;
using Java.IO;
using System.Net;

namespace Facebook_OAuth
{
	[Activity(Label = "Facebook_OAuth", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
		string userToken;
		string client_Id = "192987457833095";

		Button button;
		Button buttonPost;
		TextView txtResult;
		ImageView imgView;
		ImageView imgViewCover;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);
	         
			 button = FindViewById<Button>(Resource.Id.myButton);
			 buttonPost = FindViewById<Button>(Resource.Id.myButtonPost);
			 txtResult = FindViewById<TextView>(Resource.Id.txtRes);
			 imgView = FindViewById<ImageView>(Resource.Id.img);
			 imgViewCover = FindViewById<ImageView>(Resource.Id.imgCover);
			 
			button.Click += delegate { FacebookLogin(); };
			buttonPost.Click += ButtonPost_Click;
			buttonPost.Visibility = Android.Views.ViewStates.Invisible;
		}

		void FacebookLogin()
		{
			var authFB = new OAuth2Authenticator(
				clientId: client_Id,
				scope: "",
				authorizeUrl: new System.Uri("https://m.facebook.com/dialog/oauth/"),
				redirectUrl: new System.Uri("http://www.facebook.com/connect/login_success.html"));
				authFB.Completed += AuthFB_Completed;
				StartActivity(authFB.GetUI(this));
		}

		void AuthFB_Completed(object sender, AuthenticatorCompletedEventArgs e)
		{
			if (e.Account != null)
			{
				userToken = e.Account.Properties["access_token"];
				AccountStore.Create().Save(e.Account, "Facebook");

				Toast.MakeText(this, "Facebook login Success " , ToastLength.Short).Show();
				txtResult.Text = "";
				buttonPost.Visibility = Android.Views.ViewStates.Visible;
				foreach (var x in e.Account.Properties.Keys) txtResult.Text += x + ": "+e.Account.Properties[x] +" \n " ;
			}
				else Toast.MakeText(this, "Facebook login Error ", ToastLength.Short).Show();
		}


		 void ButtonPost_Click(object sender, EventArgs e)
		{
			FacebookProfile fbP = new FacebookProfile();
			FacebookClient fb = new FacebookClient(userToken);
			fb.GetTaskAsync("me/?fields=name,picture,friends,age_range,birthday,currency,devices,gender,cover").ContinueWith(t =>
			{
			if (!t.IsFaulted)
			{
					try{

					var result = (IDictionary<string, object>)t.Result;
					fbP = JsonConvert.DeserializeObject<FacebookProfile>(result.ToString());

                    var imageBitmap = Utils.GetImageBitmapFromUrl(fbP.Picture.Data.Url);
					var imageBitmapCover = Utils.GetImageBitmapFromUrl(fbP.Cover.Source);
                    // CONSOLE
				    System.Console.WriteLine(fbP.Id);
					System.Console.WriteLine(fbP.Name);	
					System.Console.WriteLine(fbP.Cover.Source);

						RunOnUiThread(() => {
											  imgView.SetImageBitmap(imageBitmap);
											  imgViewCover.SetImageBitmap(imageBitmapCover);
							                  txtResult.Text = " "; 
											  txtResult.Text += fbP.Name+ " \n ";
											  txtResult.Text += fbP.Gender+ " \n ";
										      txtResult.Text += "Minimum "+fbP. AgeRange.Min + "years \n ";
											  txtResult.Text +=  fbP.friends.summary.total_count+ " Friends \n ";
										    });
					
					     }
					catch (Exception ex)
					{
						System.Console.WriteLine(ex);
					}
				} 
			});
		}


	}
}

