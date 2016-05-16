using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace PORadnik.WinPhone {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Windows.UI.Xaml.Controls.Page {
        string loginBox = "login";
        MyClass myClass;
        Favorites favorites;
        public MainPage() {
            this.InitializeComponent();
            //this.NavigationCacheMode = NavigationCacheMode.Required;
            listOfCategories.ItemsSource = Categories.CategoriesList;
            myClass = new MyClass();
            favorites = new Favorites();
        }
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
            jsonView.Text = myClass.GetGuides(myClass.Url);
            listGuide.ItemsSource = myClass.Guides;
            listGuide.IsItemClickEnabled = true;
            favButton.Visibility = Visibility.Collapsed;
            //}
        }

        private void listGuide_ItemClick(object sender, ItemClickEventArgs e) {
            ListView list = (ListView)e.OriginalSource;
            list.ItemsSource = null;
            list.IsItemClickEnabled = false;
            myClass.guideId = 0;
            var guide = (Guide)e.ClickedItem;
            List<Slide> slides = myClass.GetSlides(myClass.urlSlide, guide);
            myClass.Guides.First(g => g.Id == guide.Id).Slides = slides;
            try {
                list.ItemsSource = slides;
                myClass.guideId = guide.Id;
                favButton.Visibility = Visibility.Visible;
            }
            catch (InvalidOperationException ioe) {
                jsonView.Text = ioe.Message;
            }
        }

        private void sha_Click(object sender, RoutedEventArgs e) {
            if (login.Text != string.Empty && login.Text != loginBox
                && password.Password != string.Empty) {
                var apikey = myClass.Authentication(login.Text, password.Password);
                jsonView.Text = apikey;
                if (apikey != "0") {
                    loginGrid.Visibility = Visibility.Collapsed;
                    loggedGridTexbox.Text = "Witaj " + login.Text;
                    loggedGrid.Visibility = Visibility.Visible;
                }
                else {
                    loginGrid.Visibility = Visibility.Visible;
                    loggedGrid.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void login_GotFocus(object sender, RoutedEventArgs e) {
            var textbox = (TextBox)sender;
            textbox.Text = string.Empty;
        }

        private void login_LostFocus(object sender, RoutedEventArgs e) {
            if (login.Text == string.Empty)
                login.Text = loginBox;
        }

        private void logout_Click(object sender, RoutedEventArgs e) {
            MyClass.api = "0";
            loginGrid.Visibility = Visibility.Visible;
            loggedGrid.Visibility = Visibility.Collapsed;
        }

        private void favButton_Click(object sender, RoutedEventArgs e) {
            try {
                favorites.Guides.Add(myClass.Guides.Find(g => g.Id == myClass.guideId));
                jsonView.Text = "DODANO";
            }
            catch (ArgumentNullException ane) {
                jsonView.Text = ane.Message;
            }
            catch {
                jsonView.Text = "Reszta błędów";
            }
        }
        private void favoriteGuides_ItemClick(object sender, ItemClickEventArgs e) {
            var guide = (Guide)e.ClickedItem;
            favoriteGuides.ItemsSource = favorites.Guides.First(g => g.Id == guide.Id).Slides;
        }

        private void PivotApp_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var pivot = (Pivot)sender;
            var pivotItem = (PivotItem)pivot.SelectedItem;
            if (pivot != null && pivotItem != null)
                if (pivotItem.Name == "favoritesView") {
                    favoriteGuides.ItemsSource = favorites.Guides.ToList();
                }
        }

        private void searchButton_Click(object sender, RoutedEventArgs e) {
            var strToFind = searchBox.Text;
            if (strToFind != string.Empty) {
                myClass.Guides = myClass.Search(myClass.searchUrl, strToFind);
                if (myClass.Guides != null) {
                    searchGuideList.ItemsSource = myClass.Guides;
                }
                else
                    searchGuideList.ItemsSource = "Brak wynikow";
            }
        }

        private void PivotApp_Loaded(object sender, RoutedEventArgs e) {
            listOfCategories.ItemsSource = Categories.CategoriesList;
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e) {
            var json = myClass.SerializeGuideList(favorites.Guides.ToList());
            await myClass.SaveFavorites(json);
        }

        private async void loadFavoritesButton_Click(object sender, RoutedEventArgs e) {
            var guides = myClass.DeserializeGuideList(await myClass.LoadFavorites());
            foreach (var guide in guides) {
                favorites.Guides.Add(guide);
            }
        }
    }
}