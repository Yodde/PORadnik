using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Xamarin.Forms;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace PORadnik.WinPhone {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Windows.UI.Xaml.Controls.Page {
        string loginBox = "login";
        public MainPage() {
            this.InitializeComponent();
            //this.NavigationCacheMode = NavigationCacheMode.Required;
            
        }

        MyClass m = new MyClass();
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        //protected override void OnNavigatedTo(NavigationEventArgs e) {
        //    // TODO: Prepare page for display here.

        //    // TODO: If your application contains multiple pages, ensure that you are
        //    // handling the hardware Back button by registering for the
        //    // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
        //    // If you are using the NavigationHelper provided by some templates,
        //    // this event is handled for you.
        //}

        private void button_Click(object sender, RoutedEventArgs e) {
           // if (MyClass.api != "0" && MyClass.api != "") {
                jsonView.Text = m.GetJson(m.Url);
                listGuide.ItemsSource = m.G;
                listGuide.IsItemClickEnabled = true;
                
            //}
        }

        private void listGuide_ItemClick(object sender, ItemClickEventArgs e) {
            listGuide.IsItemClickEnabled = false;
            var guide = (Guide)e.ClickedItem;
            jsonView.Text = m.GetJson(m.urlSlide, guide);
            listGuide.ItemsSource = m.S;

        }

        private void sha_Click(object sender, RoutedEventArgs e) {
            if (login.Text != string.Empty && login.Text != loginBox
                && password.Password != string.Empty)
                jsonView.Text = m.Authentication(login.Text, password.Password);
            //if (MyClass.api != "0" && MyClass.api != "") {
                
            //}
            //else
            //    dataDownload.IsEnabled = false;
        }

        private void login_GotFocus(object sender, RoutedEventArgs e) {
            var textbox = (TextBox)sender;
            textbox.Text = string.Empty;
        }

        private void login_LostFocus(object sender, RoutedEventArgs e) {
            if (login.Text == string.Empty)
                login.Text = loginBox;
        }

    }
}
