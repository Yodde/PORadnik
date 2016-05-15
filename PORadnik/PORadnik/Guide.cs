using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PORadnik {
    public class Guide {
        private int id;
        public int Id {
            get { return id; }
            set { id = value; }
        }
        private string description;

        public string Description {
            get { return description; }
            set { description = value; }
        }
        private List<Slide> slides;

        public List<Slide> Slides {
            get { return slides; }
            set { slides = value; }
        }
        public Guide(int id, string description) {
            this.id = id;
            this.description = description;
            slides = new List<Slide>();
        }

        public Guide() {
            slides = new List<Slide>();
        }

        public void addToList(Slide s) {
            slides.Add(s);
        }
        public override string ToString() {
            return id.ToString() + " " + description;
        }
        public override bool Equals(object obj) {
            Guide g = obj as Guide;
            return g != null && g.id.Equals(this.id) && g.description.Equals(this.description);
        }
        public override int GetHashCode() {
            int hash = 13;
            hash = (hash * 7) + this.id.GetHashCode();
            hash = (hash * 7) + this.description.GetHashCode();
            return hash; 
        }
    }
}
