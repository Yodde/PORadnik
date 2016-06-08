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
        string url = "http://91.134.138.82/rest.php/guides";
        public string urlSlide = "http://91.134.138.82/rest.php/slides/";
        string authenticationURL = "http://91.134.138.82/rest.php/auth/";
        string getUserDetails = "http://91.134.138.82/rest.php/user/";
        public string SearchUrl = "http://91.134.138.82/rest.php/guides/search/";
        public string categoriesURL = "http://91.134.138.82/rest.php/categories";
        public string CategoryGuidesURL = "http://91.134.138.82/rest.php/guides/category/";
        public string OpinionURL = "http://91.134.138.82/rest.php/opinion/";
        const string UserOpinion = "/user/";
        const string Success = "success";
        const string Fail = "0";
        const string FailMessage = "Nie mozna nawiązać poprawnego połączenia";
        const string FolderName = "Saved";
        const string FileName = "Favorites";
        public List<Guide> Guides { get; set; }
        public List<Guide> CategoryGuides { get; set; }
        public List<Guide> SearchedGuides { get; set; }
        //   public static string api = "";
        public User user = null;
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
                        ///slide = await GetImage(slide);
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
        //public async Task<Slide> GetImage(Slide slide) {
        //    using (var imageHTTP = new HttpClient()) {
        //        var responseImg = await imageHTTP.GetAsync(imageURL + slide.Id);
        //        if (responseImg.IsSuccessStatusCode) {
        //            var imgContent = responseImg.Content;
        //            var jsonImg = await imgContent.ReadAsStringAsync();
        //            JObject imgJo = JObject.Parse(jsonImg);
        //            if (imgJo.GetValue(Success).ToString() != Fail) {
        //                IList<JToken> imgs = imgJo["image"].Children().ToList();
        //                ImageClass ic = new ImageClass();
        //                ic = JsonConvert.DeserializeObject<ImageClass>(imgs[0].ToString());
        //                slide.ImageSos = downloadImageURL + ic.Name;
        //            }
        //            else
        //                slide.ImageSos = "";
        //        }
        //        else
        //            slide.ImageSos = "";
        //    }
        //    return slide;
        //}
        public async Task<List<Categories>> GetCategories(string url) {
            List<Categories> categoriesList = new List<Categories>();
            using (var categoryHttp = new HttpClient()) {
                var response = await categoryHttp.GetAsync(categoriesURL);
                if (response.IsSuccessStatusCode) {
                    var responseContent = response.Content;
                    var json = await responseContent.ReadAsStringAsync();
                    JObject jObject = JObject.Parse(json);
                    IList<JToken> results = jObject["categories"].Children().ToList();
                    foreach (var category in results) {
                        categoriesList.Add(JsonConvert.DeserializeObject<Categories>(category.ToString()));
                    }
                }
            }
            return categoriesList;
        }

        public async Task<List<Guide>> GetGuidesFromCategory(string url, Categories category) {
            List<Guide> guides = new List<Guide>();
            using (var http = new HttpClient()) {
                var response = await http.GetAsync(url + category.CategoryId);
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

        public async Task<Opinion> GetOpinion(int guideId) {
            Opinion opinon = new Opinion();
            try {
                using (var http = new HttpClient()) {
                    var response = await http.GetAsync(OpinionURL + guideId);
                    if (response.IsSuccessStatusCode) {
                        var content = response.Content;
                        var json = await content.ReadAsStringAsync();
                        JObject jobject = JObject.Parse(json);
                        IList<JToken> result = jobject["opinion"].Children().ToList();
                        var opinion = JsonConvert.DeserializeObject<Opinion>(result[0].ToString());
                        if (user != null) {
                            opinion = await GetUserOpinion(opinion, guideId, user.UserID);
                        }
                        return opinion;
                    }
                    else
                        return null;
                }
            }
            catch (System.ArgumentNullException e) {
                return null;
            }
            catch (System.ArgumentOutOfRangeException e) {
                return null;
            }
        }
        public async Task<Opinion> GetUserOpinion(Opinion opinion, int guideId, int userId) {
            using (var http = new HttpClient()) {
                var response = await http.GetAsync(OpinionURL + guideId + UserOpinion + userId);
                if (response.IsSuccessStatusCode) {
                    var content = response.Content;
                    var json = await content.ReadAsStringAsync();
                    JObject jObject = JObject.Parse(json);
                    try {
                        IList<JToken> result = jObject["opinion"].Children().ToList();
                        var o = JsonConvert.DeserializeObject<Opinion>(result[0].ToString());
                        opinion.UserID = o.UserID;
                        opinion.GuideID = o.GuideID;
                        opinion.Value = o.Value;
                    }
                    catch (System.ArgumentOutOfRangeException) {
                        return opinion;
                    }
                }
            }
            return opinion;
        }

        public async Task<UserApi> GetUserApi(string url) {
            string json = "";
            UserApi api = new UserApi();
            using (var http = new HttpClient()) {
                var response = await http.GetAsync(url);
                if (response.IsSuccessStatusCode) {
                    var content = response.Content;
                    json = await content.ReadAsStringAsync();
                    try {
                        JObject jobject = JObject.Parse(json);
                        api = JsonConvert.DeserializeObject<UserApi>(jobject.ToString());
                    }
                    catch (JsonReaderException) {
                        
                        return null;
                    }
                }
                else
                    return null;
                return api;
            }
        }

        public async Task<User> GetUserDetails(int id) {
            string json = "";
            using (var http = new HttpClient()) {
                var response = await http.GetAsync(getUserDetails + id);
                if (response.IsSuccessStatusCode) {
                    var content = response.Content;
                    json = await content.ReadAsStringAsync();
                    JObject jObject = JObject.Parse(json);
                    IList<JToken> result = jObject["user"].Children().ToList();
                    return JsonConvert.DeserializeObject<User>(result[0].ToString());
                }
                else
                    return null;
            }
        }

        public async Task<UserApi> Authentication(string username, string password) {
            UserApi token = new UserApi();
            var hashedPassword = await GenerateSHA(password);
            token = await GetUserApi(authenticationURL + username.ToLower() + "/" + hashedPassword);
            return token;
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

        //Zapisywanie i wczytywanie pliku

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

