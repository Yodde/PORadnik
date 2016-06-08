using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PORadnik {
    public class Opinion {
        public int UserNumber { get; set; }
        public float Average { get; set; }
        public int GuideID { get; set; }
        public int UserID { get; set; }
        public int Value { get; set; }

        public override string ToString() {
            if (UserNumber != 0 && Value != 0) {
                return "Średnia ocen: " + Average + ". Oceniło: " + UserNumber + ". Twoja ocena: " + Value;
            }
            else if (UserNumber != 0 && Value == 0) {
                return "Średnia ocen: " + Average + ". Oceniło: " + UserNumber + ".";
            }
            else
                return "Brak ocen";
        }
    }
}
