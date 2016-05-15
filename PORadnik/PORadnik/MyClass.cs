<<<<<<< Updated upstream
﻿using System;

namespace PORadnik {
    public class MyClass {
        public MyClass() {
        }
=======
﻿using Newtonsoft.Json;
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
        public string searchUrl = "http://poradnik.mikroprint.pl/rest_api.php/guides/search/";
        string username = "username=";
        string hash = "hash=";
        string success = "success";
        string fail = "0";
        string failMessage = "Nie mozna nawiązać poprawnego połączenia";
        const string folderName = "Saved";
        const string fileName = "Favorites";
        public List<Guide> Guides { get; set; }
        public List<Guide> StorageGuides { get; set; } //?? to storage guides when in other views
        public static string api = "";
        public int guideId = 0;
        public MyClass() {
            Guides = new List<Guide>();
            List<string> cat = new List<string>() { "Motoryzacja", "Kuchnia", "Ogród" };
            Categories.CategoriesList = cat;
        }
        public string GetGuides(string url) {
            string json = "";
            using (var http = new HttpClient()) {
                var response = http.GetAsync(url).Result;
                if (response.IsSuccessStatusCode) {
                    var content = response.Content;
                    json = content.ReadAsStringAsync().Result;
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
                    return failMessage;
                return json;
            }
        }
        public List<Slide> GetSlides(string url, Guide guide) {
            List<Slide> slides = new List<Slide>();
            using (var http = new HttpClient()) {
                var response = http.GetAsync(url + guide.Id).Result;
                if (response.IsSuccessStatusCode) {
                    var content = response.Content;
                    var json = content.ReadAsStringAsync().Result;
                    JObject jobject = JObject.Parse(json);
                    IList<JToken> results = jobject["slides"].Children().ToList();
                    foreach (JToken result in results) {
                        Slide slide = JsonConvert.DeserializeObject<Slide>(result.ToString());
                        GetImage(slide, out slide);
                        slides.Add(slide);
                    }
                }
                else {
                    slides = null;
                }
            }
            return slides;
        }
        public List<Guide> Search(string url, string strToFind) {
            List<Guide> foundGuides = new List<Guide>();
            using (var http = new HttpClient()) {
                var response = http.GetAsync(url + strToFind).Result;////do poprawy!!!
                if (response.IsSuccessStatusCode) {
                    var content = response.Content;
                    var json = content.ReadAsStringAsync().Result;
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
        public void GetImage(Slide slide, out Slide slideOut) {
            slideOut = slide;
            using (var imageHTTP = new HttpClient()) {
                var responseImg = imageHTTP.GetAsync(imageURL + slide.Id).Result;
                if (responseImg.IsSuccessStatusCode) {
                    var imgContent = responseImg.Content;
                    var jsonImg = imgContent.ReadAsStringAsync().Result;
                    JObject imgJo = JObject.Parse(jsonImg);
                    if (imgJo.GetValue(success).ToString() != fail) {
                        IList<JToken> imgs = imgJo["image"].Children().ToList();
                        ImageClass ic = new ImageClass();
                        ic = JsonConvert.DeserializeObject<ImageClass>(imgs[0].ToString());
                        slideOut.ImageSos = downloadImageURL + ic.Name;
                    }
                }
            }
        }
        public string GetUserApiKey(string url) {
            string json = "";
            UserApi api = new UserApi();
            string apikey = "";
            using (var http = new HttpClient()) {
                var response = http.GetAsync(url).Result;
                if (response.IsSuccessStatusCode) {
                    var content = response.Content;
                    json = content.ReadAsStringAsync().Result;
                    JObject jobject = JObject.Parse(json);
                    api = JsonConvert.DeserializeObject<UserApi>(jobject.ToString());
                    if (api.Success == 1) {
                        apikey = api.Api_key;
                    }
                    else
                        apikey = fail;
                }
                MyClass.api = apikey;
                return apikey;
            }
        }
        public string Authentication(string username, string password) {
            string apikey = string.Empty;
            var hashedPassword = GenerateSHA(password);
            apikey = GetUserApiKey(authenticationURL + this.username + username.ToLower() + "&" + this.hash + hashedPassword);
            return apikey;
        }
        private string GenerateSHA(string password) {
            var data = System.Text.Encoding.UTF8.GetBytes(password);
            var hasher = WinRTCrypto.HashAlgorithmProvider.OpenAlgorithm(HashAlgorithm.Sha256);
            byte[] hashbyte = hasher.HashData(data);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < hashbyte.Length; i++) {
                sb.Append(hashbyte[i].ToString("X2"));
            }
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
            IFolder folder = await rootFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);
            IFile file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            await file.WriteAllTextAsync(json);
        }
        public async Task<string> LoadFavorites() {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFolder folder = await rootFolder.GetFolderAsync(folderName);
            IFile file = await folder.GetFileAsync(fileName);
            return await file.ReadAllTextAsync();
        }
>>>>>>> Stashed changes
    }
}

