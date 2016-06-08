using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Popups;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace PORadnik.WinPhone {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Windows.UI.Xaml.Controls.Page {
        string loginBox = "login";
        const string poradnikTitle = "Poradnik";
        enum PivotItems {
            Guides,
            Search,
            Categories,
            Account,
            Favorites
        };
        PivotItems actualPivotItem;
        MyClass myClass;
        Favorites favorites;
        int returnClickedxTimes;
        public MainPage() {
            this.InitializeComponent();
            //this.NavigationCacheMode = NavigationCacheMode.Required;
            myClass = new MyClass();
            favorites = new Favorites();
            returnClickedxTimes = 0;
            //listOfCategories.ItemsSource = Categories.CategoriesList;
        }
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        //protected override void OnNavigatedTo(NavigationEventArgs e) {
        //    PivotItem pi = (PivotItem)e.Uri
        //    // TODO: Prepare page for display here.

        //    // TODO: If your application contains multiple pages, ensure that you are
        //    // handling the hardware Back button by registering for the
        //    // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
        //    // If you are using the NavigationHelper provided by some templates,
        //    // this event is handled for you.
        //}

        private async void listGuide_ItemClick(object sender, ItemClickEventArgs e) {
            ListView list = null;
            list = (ListView)e.OriginalSource;
            myClass.guideId = 0;
            var guide = (Guide)e.ClickedItem;
            List<Slide> slides = await myClass.GetSlides(myClass.urlSlide, guide);
            Opinion opinion = await myClass.GetOpinion(guide.Id);
            myClass.Guides.First(g => g.Id == guide.Id).Slides = slides;
            try {
                list.ItemsSource = slides;
                if (opinion != null) {
                    PivotApp.Title = opinion.ToString();
                }
                myClass.guideId = guide.Id;
                AppBarButtonsVisibleChange(true, false, false, true);
            }
            catch (InvalidOperationException ioe) {
                MessageDialog msg = new MessageDialog(ioe.Message);
                await msg.ShowAsync();
            }
            list.IsItemClickEnabled = false;
            returnClickedxTimes = 0;
        }

        private async void sha_Click(object sender, RoutedEventArgs e) {
            if (login.Text != string.Empty && login.Text != loginBox
                && password.Password != string.Empty) {
                var apikey = await myClass.Authentication(login.Text, password.Password);
                if (apikey != null) {
                    myClass.user = await myClass.GetUserDetails(apikey.Id);
                    var user = myClass.user;
                    if (user != null) {
                        loginGrid.Visibility = Visibility.Collapsed;
                        loggedGrid.Visibility = Visibility.Visible;
                        string s = "";
                        try {
                             s = "Witaj " + user.Nick.ToString() + "\nTwoje dane:" + "\nImię: " + user.LastName.ToString() +
                                "\nNazwisko: " + user.FirstName.ToString() + "\nE-Mail: " + user.Mail.ToString();
                        }
                        catch (System.NullReferenceException) {
                            s = "Witaj" + user.Nick.ToString();
                        }
                        loggedGridTexbox.Text = s;
                    }
                }
                else {
                    MessageDialog msg = new MessageDialog("Niewłaściwe dane logowania");
                    loginGrid.Visibility = Visibility.Visible;
                    loggedGrid.Visibility = Visibility.Collapsed;
                    myClass.user = null;
                    await msg.ShowAsync();
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
            login.Text = "";
            password.Password = "";
            loginGrid.Visibility = Visibility.Visible;
            loggedGrid.Visibility = Visibility.Collapsed;
            myClass.user = null;
        }
        private void favButton_Click(object sender, RoutedEventArgs e) {
            try {
                favorites.Guides.Add(myClass.Guides.Find(g => g.Id == myClass.guideId));
                jsonView.Text = "DODANO";
                listOfFavorites.ItemsSource = favorites.Guides.ToList();
            }
            catch (ArgumentNullException ane) {
                jsonView.Text = ane.Message;
            }
            catch {
                jsonView.Text = "Reszta błędów";
            }
        }
        private async void favoriteGuides_ItemClick(object sender, ItemClickEventArgs e) {
            var guide = (Guide)e.ClickedItem;
            var slides = favorites.Guides.First(g => g.Id == guide.Id).Slides;
            if (slides.Count != 0) {
                listOfFavorites.ItemsSource = slides;
            }
            else {
                slides = await myClass.GetSlides(myClass.urlSlide, guide);
                listOfFavorites.ItemsSource = slides;
            }
            AppBarButtonsVisibleChange(false, false, false, true);
        }
        /// <summary>
        /// Zarządza "widzialnością" przycisków na dolnej belce. True = widoczny, False = ukryty.
        /// </summary>
        /// <param name="favoriteButton"></param>
        /// <param name="saveButton"></param>
        /// <param name="loadButton"></param>
        /// <param name="returnButton"></param>
        private void AppBarButtonsVisibleChange(bool favoriteButton, bool saveButton, bool loadButton, bool returnButton) {
            this.favButton.Visibility = ChangeVisibility(favoriteButton);
            this.saveFavoritesButton.Visibility = ChangeVisibility(saveButton);
            this.loadFavoritesButton.Visibility = ChangeVisibility(loadButton);
            this.returnButton.Visibility = ChangeVisibility(returnButton);

        }
        private void AppBarButtonsHideAll() {
            AppBarButtonsVisibleChange(false, false, false, false);
        }
        private Visibility ChangeVisibility(bool visible) {
            if (visible)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }
        private void PivotApp_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var pivot = (Pivot)sender;
            var pivotItem = (PivotItem)pivot.SelectedItem;
            PivotApp.Title = poradnikTitle;
            if (pivot != null && pivotItem != null) {
                if (pivotItem == guidesItem) {
                    GuidesPivotLoad();
                    actualPivotItem = PivotItems.Guides;
                    AppBarButtonsHideAll();
                }
                else if (pivotItem == searchItem) {
                    actualPivotItem = PivotItems.Search;
                    AppBarButtonsHideAll();
                    listOfSearched.ItemsSource = null;
                }
                else if (pivotItem == categoriesItem) {
                    actualPivotItem = PivotItems.Categories;
                    AppBarButtonsHideAll();
                    listOfCategories.Visibility = Visibility.Visible;
                    listOfGuidesFromCategory.Visibility = Visibility.Collapsed;
                }
                else if (pivotItem == loginItem) {
                    actualPivotItem = PivotItems.Account;
                    AppBarButtonsHideAll();
                }
                else {
                    actualPivotItem = PivotItems.Favorites;
                    AppBarButtonsVisibleChange(false, true, true, false);
                    listOfFavorites.ItemsSource = favorites.Guides;
                }
            }
        }

        private async void searchButton_Click(object sender, RoutedEventArgs e) {
            var strToFind = searchBox.Text;
            if (strToFind != string.Empty) {
                myClass.SearchedGuides = await myClass.Search(myClass.SearchUrl, strToFind);
                if (myClass.SearchedGuides.Count != 0) {
                    listOfSearched.ItemsSource = myClass.SearchedGuides;
                    listOfSearched.IsItemClickEnabled = true;
                }
                else {
                    MessageDialog message = new MessageDialog("Brak wyników");
                    await message.ShowAsync();
                }
            }
        }

        private async void saveFavoritesButton_Click(object sender, RoutedEventArgs e) {
            MessageDialog msg = new MessageDialog("Zapisano.");
            var json = myClass.SerializeGuideList(favorites.Guides.ToList());
            await myClass.SaveFavorites(json);
            await msg.ShowAsync();
        }

        private async void loadFavoritesButton_Click(object sender, RoutedEventArgs e) {
            try {
                var guides = myClass.DeserializeGuideList(await myClass.LoadFavorites());
                foreach (var guide in guides) {
                    favorites.Guides.Add(guide);
                }
                MessageDialog msg = new MessageDialog("Wczytano");
                await msg.ShowAsync();
                listOfFavorites.ItemsSource = favorites.Guides.ToList();
            }
            catch (PCLStorage.Exceptions.DirectoryNotFoundException) {
                MessageDialog msg = new MessageDialog("Nie znaleziono pliku.");
                await msg.ShowAsync();
            }
            catch (PCLStorage.Exceptions.FileNotFoundException) {
                MessageDialog msg = new MessageDialog("Nie znaleziono pliku.");
                await msg.ShowAsync();
            }

        }
        private async Task GuidesPivotLoad() {
            jsonView.Text = await myClass.GetGuides(myClass.Url);
            listOfGuides.ItemsSource = myClass.Guides;
            listOfGuides.IsItemClickEnabled = true;
        }
        private async Task CategoriesPivotLoad() {
            listOfCategories.ItemsSource = await myClass.GetCategories(myClass.categoriesURL);

        }

        private async void PivotApp_Loaded(object sender, RoutedEventArgs e) {
            // await GuidesPivotLoad();
            await CategoriesPivotLoad();
        }

        private void returnButton_Click(object sender, RoutedEventArgs e) {
            switch (actualPivotItem) {
                case PivotItems.Guides:
                    listOfGuides.ItemsSource = myClass.Guides;
                    listOfGuides.IsItemClickEnabled = true;
                    AppBarButtonsHideAll();
                    break;
                case PivotItems.Search:
                    listOfSearched.ItemsSource = null;
                    listOfSearched.IsItemClickEnabled = true;
                    AppBarButtonsHideAll();
                    listOfSearched.ItemsSource = myClass.SearchedGuides;
                    break;
                case PivotItems.Categories: {
                        ++returnClickedxTimes;
                        if (returnClickedxTimes <= 1) {
                            listOfGuidesFromCategory.ItemsSource = myClass.CategoryGuides;
                            listOfGuidesFromCategory.IsItemClickEnabled = true;
                            ++returnClickedxTimes;
                        }
                        else {
                            listOfCategories.IsItemClickEnabled = true;
                            AppBarButtonsHideAll();
                            listOfCategories.Visibility = Visibility.Visible;
                            listOfGuidesFromCategory.Visibility = Visibility.Collapsed;
                            listOfGuidesFromCategory.ItemsSource = null;
                            listOfGuidesFromCategory.IsItemClickEnabled = true;
                        }
                        break;
                    }
                case PivotItems.Account:
                    AppBarButtonsHideAll();
                    break;
                case PivotItems.Favorites:
                    listOfFavorites.IsItemClickEnabled = true;
                    AppBarButtonsVisibleChange(false, true, true, false);
                    listOfFavorites.ItemsSource = favorites.Guides.ToList();
                    break;
                default:
                    break;
            }
            PivotApp.Title = poradnikTitle;
        }

        private void goToSearchButton_Click(object sender, RoutedEventArgs e) {
            PivotApp.SelectedIndex = (int)PivotItems.Search;
        }

        private async void listOfCategories_ItemClick(object sender, ItemClickEventArgs e) {
            Categories cat = (Categories)e.ClickedItem;
            myClass.CategoryGuides = await myClass.GetGuidesFromCategory(myClass.CategoryGuidesURL, cat);
            listOfGuidesFromCategory.ItemsSource = myClass.CategoryGuides;
            listOfCategories.Visibility = Visibility.Collapsed;
            listOfGuidesFromCategory.Visibility = Visibility.Visible;
            AppBarButtonsVisibleChange(true, false, false, true);

        }

        private async void listOfSearched_ItemClick(object sender, ItemClickEventArgs e) {
            ListView list = null;
            list = (ListView)e.OriginalSource;
            myClass.guideId = 0;
            var guide = (Guide)e.ClickedItem;
            List<Slide> slides = await myClass.GetSlides(myClass.urlSlide, guide);
            myClass.SearchedGuides.First(g => g.Id == guide.Id).Slides = slides;
            try {
                list.ItemsSource = slides;
                myClass.guideId = guide.Id;
                AppBarButtonsVisibleChange(true, false, false, true);
            }
            catch (InvalidOperationException ioe) {
                jsonView.Text = ioe.Message;
            }
            list.IsItemClickEnabled = false;
            returnClickedxTimes = 0;
        }
    }
}