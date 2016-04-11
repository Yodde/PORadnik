using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PORadnik {
    public class Slide {
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

        string guide;
        DateTime createdAt;
        DateTime updatedAt;
        bool enabled;
        public Slide(int id, string description, string guide) {
            this.id = id;
            this.description = description;
            this.guide = guide;
        }
        public Slide() { }
        public override string ToString() {
            return id + "\n" + description + "\n";
        }
    }
}
