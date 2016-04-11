using System;
using System.Net;
using Newtonsoft.Json;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections.Generic;

namespace PORadnik {
    public class MyClass {
        public string Url { get { return url; } }
        string url = "http://poradnik.mikroprint.pl/get_guides.php?number=2";
        public string urlSlide = "http://poradnik.mikroprint.pl/get_all_slides.php?Id=";
        public List<Guide> G { get; set; }
        public List<Slide> S { get; set; }
        public MyClass() {
           // g = new Guide();
            //g = new Guide(1, "Por", "Jak chodowac pora");
           // g.addToList(new Slide(1, "Krok 1.", "jak chodowac pora", "Kup pora"));
        }
        public string getJson(string url) {
            string json = "";
            using (var http = new HttpClient()){
                var response = http.GetAsync(url).Result;
                if (response.IsSuccessStatusCode) {
                    var content = response.Content;
                    json = content.ReadAsStringAsync().Result;
                    JObject jo = JObject.Parse(json);
                    IList<JToken> results = jo["guides"].Children().ToList();
                    IList<Guide> guideResults = new List<Guide>();
                    foreach (JToken result in results) {
                        Guide guide = JsonConvert.DeserializeObject<Guide>(result.ToString());
                        guideResults.Add(guide);
                    }
                    G = (List<Guide>)guideResults;
                }
                else
                    return "Nie mozna nawiązać poprawnego połączenia";
                return json;
            }
        }
        public string getJson(string url,Guide guide) {
            string json = "";
            
            using (var http = new HttpClient()) {
                var response = http.GetAsync(url+guide.Id).Result;
                if (response.IsSuccessStatusCode) {
                    var content = response.Content;
                    json = content.ReadAsStringAsync().Result;
                    JObject jobject = JObject.Parse(json);
                    IList<JToken> results = jobject["slides"].Children().ToList();
                    var slidesResult = new List<Slide>();
                    foreach (JToken result in results) {
                        Slide slide = JsonConvert.DeserializeObject<Slide>(result.ToString());
                        slidesResult.Add(slide);
                    }
                    S = slidesResult;
                }
                else
                    return "Nie mozna nawiązać poprawnego połączenia";
            }
            return json;
        }

    }
}

