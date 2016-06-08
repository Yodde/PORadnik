using System;

namespace PORadnik {
    public class Slide {
        const string ImageUrl = "http://91.134.138.82/Poradnik/images/";
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
        private string imageName;

        public string ImageName {
            get { return imageName; }
            set { imageName = ImageUrl + value; }
        }

        public string Name { get; set; }

        string guide;
        DateTime createdAt;
        DateTime updatedAt;
        bool enabled;
        public Slide(int id, string description, string guide) {
            this.id = id;
            this.description = description;
            this.guide = guide;
        }
        public Slide() {
            //_image = new Image { Aspect = Aspect.AspectFit };
        }
        //public override string ToString() {
        //    return id + "\n" + description + "\n";
        //}
    }
}
