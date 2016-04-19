using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PORadnik {
    class UserApi {
        private string api_key;

        public string Api_key {
            get { return api_key; }
            set { api_key = value; }
        }
        public int Success { get; set; }
        public string Message { get; set; }
    }
}
