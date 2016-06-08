using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PORadnik {
    public class UserApi {
        private string token;

        public string Token {
            get { return token; }
            set { token = value; }
        }
        public int Success { get; set; }
        public string Message { get; set; }
        public int Id { get; set; }
        //public string User { get; set; }
    }
}
