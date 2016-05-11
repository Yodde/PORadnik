using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using System.Linq;

namespace PORadnik.Droid
{
	[Activity (Label = "PORadnik.Droid", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		int count = 1;
        static MyClass myClass = new MyClass();
        protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.myButton);
            ListView list = FindViewById<ListView>(Resource.Id.listGuide);
            

			button.Click += delegate {
				button.Text = string.Format ("{0} clicks!", count++);
                GuideTable gt = new GuideTable();
                
			};
		}
        public class GuideTable : ListActivity {
            protected override void OnCreate(Bundle savedInstanceState) {
                base.OnCreate(savedInstanceState);
                myClass.GetGuides(myClass.Url);
                ListView list = FindViewById<ListView>(Resource.Id.listGuide);
                var items = new string[] { "Vegetables", "Fruits", "Flower Buds", "Legumes", "Bulbs", "Tubers" };
                ListAdapter = new ArrayAdapter<string>(this,Android.Resource.Layout.SimpleExpandableListItem2,items);
            }
        }
	}
}


