using Newtonsoft.Json;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections.Generic;
using PCLCrypto;
namespace PORadnik {
    public class MyClass {
        public string Url { get { return url; } }
        string url = "http://poradnik.mikroprint.pl/get_guides.php?number=15";
        public string urlSlide = "http://poradnik.mikroprint.pl/get_all_slides.php?Id=";
        string authenticationURL = "http://poradnik.mikroprint.pl/get_api_key.php?";
        string imageURL = "http://poradnik.mikroprint.pl/get_images.php?id=";
        string downloadImageURL = "http://poradnik.mikroprint.pl/images/";
        public string searchUrl = "http://poradnik.mikroprint.pl/rest_api.php/guides/search/";
        string username = "username=";
        string hash = "hash=";
        string success = "success";
        string fail = "0";
        string failMessage = "Nie mozna nawiązać poprawnego połączenia";
        public List<Guide> Guides { get; set; }
        public List<Guide> StorageGuides { get; set; } //?? to storage guides when in other views
        public static string api = "";
        public int guideId = 0;
        public MyClass() {
            Guides = new List<Guide>();
            List<string> cat = new List<string>() { "Motoryzacja", "Kuchnia", "ogród" };
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
                    if (jobject.GetValue(success).ToString() != fail) {
                        IList<JToken> results = jobject["guides"].Children().ToList();
                        IList<Guide> guideResults = new List<Guide>();
                        foreach (JToken result in results) {
                            Guide guide = JsonConvert.DeserializeObject<Guide>(result.ToString());
                            guideResults.Add(guide);
                        }
                        Guides = (List<Guide>)guideResults;
                    }
                    else {
                        return failMessage;
                    }
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
                    if (jobject.GetValue(success).ToString() != fail) {
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

    }
}

