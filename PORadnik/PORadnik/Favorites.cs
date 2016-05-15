using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PORadnik {
   public class Favorites {
        // public static HashSet<Guide> guides = new HashSet<Guide>();
        private HashSet<Guide> guides;

        public HashSet<Guide> Guides {
            get { return guides; }
            set { guides = value; }
        }
        public Favorites() {
            guides = new HashSet<Guide>();
        }
        // equal override
    }
}
