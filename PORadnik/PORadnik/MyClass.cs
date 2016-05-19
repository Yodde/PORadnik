using Newtonsoft.Json;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections.Generic;
using PCLCrypto;
using System.Threading.Tasks;
using PCLStorage;

namespace PORadnik {
    public class MyClass {
        public string Url { get { return url; } }
        string url = "http://poradnik.mikroprint.pl/rest_api.php/guides";
        public string urlSlide = "http://poradnik.mikroprint.pl/rest_api.php/slides/";
        string authenticationURL = "http://poradnik.mikroprint.pl/get_api_key.php?";
        string imageURL = "http://poradnik.mikroprint.pl/get_images.php?id=";
        string downloadImageURL = "http://poradnik.mikroprint.pl/images/";
        public string SearchUrl = "http://poradnik.mikroprint.pl/rest_api.php/guides/search/";
        public string categoriesURL = "http://poradnik.mikroprint.pl/api.php/category/";
        public string CategoryGuidesURL = "http://poradnik.mikroprint.pl/rest_api.php/guides/category/";
        const string Username = "username=";
        const string Hash = "hash=";
        const string Success = "success";
        const string Fail = "0";
        const string FailMessage = "Nie mozna nawiązać poprawnego połączenia";
        const string FolderName = "Saved";
        const string FileName = "Favorites";
        public List<Guide> Guides { get; set; }
        public List<Guide> CategoryGuides { get; set; }
        public List<Guide> SearchedGuides { get; set; }
        public static string api = "";
        public int guideId = 0;

        public MyClass() {
            Guides = new List<Guide>();
        }
        public async Task<string> GetGuides(string url) {
            string json = "";
            using (var http = new HttpClient()) {
                var response = await http.GetAsync(url);
                if (response.IsSuccessStatusCode) {
                    var content = response.Content;
                    json = await content.ReadAsStringAsync();
                    JObject jobject = JObject.Parse(json);
                    IList<JToken> results = jobject["guides"].Children().ToList();
                    IList<Guide> guideResults = new List<Guide>();
                    foreach (JToken result in results) {
                        Guide guide = JsonConvert.DeserializeObject<Guide>(result.ToString());
                        guideResults.Add(guide);
                    }
                    Guides = (List<Guide>)guideResults;
                }
                else
                    return FailMessage;
                return json;
            }
        }

        public async Task<List<Slide>> GetSlides(string url, Guide guide) {
            List<Slide> slides = new List<Slide>();
            using (var http = new HttpClient()) {
                var response = await http.GetAsync(url + guide.Id);
                if (response.IsSuccessStatusCode) {
                    var content = response.Content;
                    var json = await content.ReadAsStringAsync();
                    JObject jobject = JObject.Parse(json);
                    IList<JToken> results = jobject["slides"].Children().ToList();
                    foreach (JToken result in results) {
                        Slide slide = JsonConvert.DeserializeObject<Slide>(result.ToString());
                        slide = await GetImage(slide);
                        slides.Add(slide);
                    }
                }
                else {
                    slides = null;
                }
            }
            return slides;
        }
        public async Task<List<Guide>> Search(string url, string strToFind) {
            List<Guide> foundGuides = new List<Guide>();
            using (var http = new HttpClient()) {
                var response = await http.GetAsync(url + strToFind);////do poprawy!!!
                if (response.IsSuccessStatusCode) {
                    var content = response.Content;
                    var json = await content.ReadAsStringAsync();
                    JObject jobject = JObject.Parse(json);
                    IList<JToken> results = jobject["guides"].Children().ToList();
                    foreach (var result in results) {
                        foundGuides.Add(JsonConvert.DeserializeObject<Guide>(result.ToString()));
                    }
                }
                else {
                    foundGuides = null;
                }
            }
            return foundGuides;
        }
        public async Task<Slide> GetImage(Slide slide) {
            using (var imageHTTP = new HttpClient()) {
                var responseImg = await imageHTTP.GetAsync(imageURL + slide.Id);
                if (responseImg.IsSuccessStatusCode) {
                    var imgContent = responseImg.Content;
                    var jsonImg = await imgContent.ReadAsStringAsync();
                    JObject imgJo = JObject.Parse(jsonImg);
                    if (imgJo.GetValue(Success).ToString() != Fail) {
                        IList<JToken> imgs = imgJo["image"].Children().ToList();
                        ImageClass ic = new ImageClass();
                        ic = JsonConvert.DeserializeObject<ImageClass>(imgs[0].ToString());
                        slide.ImageSos = downloadImageURL + ic.Name;
                    }
                    else
                        slide.ImageSos = "";
                }
                else
                    slide.ImageSos = "";
            }
            return slide;
        }
        public async Task<List<Categories>> GetCategories(string url) {
            List<Categories> categoriesList = new List<Categories>();
            using (var categoryHttp = new HttpClient()) {
                var response = await categoryHttp.GetAsync(categoriesURL);
                if (response.IsSuccessStatusCode) {
                    var responseContent = response.Content;
                    var json = await responseContent.ReadAsStringAsync();
                    // JObject jObject = JObject.Parse(json);
                    JArray jArray = JArray.Parse(json);
                    IList<JToken> categories = jArray.Children().ToList();
                    foreach (var category in categories) {
                        categoriesList.Add(JsonConvert.DeserializeObject<Categories>(category.ToString()));
                    }
                }
            }
            return categoriesList;
        }

        public async Task<List<Guide>> GetGuidesFromCategory(string url, Categories category) {
            List<Guide> guides = new List<Guide>();
            using (var http = new HttpClient()) {
                var response = await http.GetAsync(url+category.CategoryId);
                if (response.IsSuccessStatusCode) {
                    var content = response.Content;
                    var json = await content.ReadAsStringAsync();
                    JObject jobject = JObject.Parse(json);
                    IList<JToken> results = jobject["guides"].Children().ToList();
                    IList<Guide> guideResults = new List<Guide>();
                    foreach (JToken result in results) {
                        Guide guide = JsonConvert.DeserializeObject<Guide>(result.ToString());
                        guideResults.Add(guide);
                    }
                    guides = (List<Guide>)guideResults;
                }
                return guides;
            }
        }

        public async Task<string> GetUserApiKey(string url) {
            string json = "";
            UserApi api = new UserApi();
            string apikey = "";
            using (var http = new HttpClient()) {
                var response = await http.GetAsync(url);
                if (response.IsSuccessStatusCode) {
                    var content = response.Content;
                    json = await content.ReadAsStringAsync();
                    JObject jobject = JObject.Parse(json);
                    api = JsonConvert.DeserializeObject<UserApi>(jobject.ToString());
                    if (api.Success == 1) {
                        apikey = api.Api_key;
                    }
                    else
                        apikey = Fail;
                }
                MyClass.api = apikey;
                return apikey;
            }
        }
        public async Task<string> Authentication(string username, string password) {
            string apikey = string.Empty;
            var hashedPassword = await GenerateSHA(password);
            apikey = await GetUserApiKey(authenticationURL + Username + username.ToLower() + "&" + Hash + hashedPassword);
            return apikey;
        }
        private async Task<string> GenerateSHA(string password) {
            var data = System.Text.Encoding.UTF8.GetBytes(password);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            Task t = Task.Run(() => {
                var hasher = WinRTCrypto.HashAlgorithmProvider.OpenAlgorithm(HashAlgorithm.Sha256);
                byte[] hashbyte = hasher.HashData(data);
                for (int i = 0; i < hashbyte.Length; i++) {
                    sb.Append(hashbyte[i].ToString("X2"));
                }
            });
            await t;
            return sb.ToString().ToLower();

        }
        public string SerializeGuideList(List<Guide> g) {
            return JsonConvert.SerializeObject(g);
        }
        public List<Guide> DeserializeGuideList(string json) {
            var s = JsonConvert.DeserializeObject<List<Guide>>(json);
            return s;
        }

        public async Task SaveFavorites(string json) {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFolder folder = await rootFolder.CreateFolderAsync(FolderName, CreationCollisionOption.OpenIfExists);
            IFile file = await folder.CreateFileAsync(FileName, CreationCollisionOption.ReplaceExisting);
            await file.WriteAllTextAsync(json);
        }
        public async Task<string> LoadFavorites() {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFolder folder = await rootFolder.GetFolderAsync(FolderName);
            IFile file = await folder.GetFileAsync(FileName);
            return await file.ReadAllTextAsync();

        }
    }
}

