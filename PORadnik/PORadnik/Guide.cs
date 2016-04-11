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

        List<Slide> slides;
        public Guide(int id, string description) {
            this.id = id;
            this.description = description;
            slides = new List<Slide>();
        }

        public Guide() {
        }

        public void addToList(Slide s) {
            slides.Add(s);
        }
        public override string ToString() {
            return id.ToString() + " " + description;
        }
    }
}
