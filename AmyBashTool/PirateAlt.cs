using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmyBashTool {
    class PirateAlt {
        // PRIVATE VARIABLES
        private string name;
        private PirateMain mainPirate;

        // CONSTRUCTORS
        public PirateAlt(string name) {
            this.name = name;
        }

        // GETTER METHODS
        public string GetName() {
            return this.name;
        }
        public PirateMain GetMain() {
            return this.mainPirate;
        }

        // MAIN METHODS
        public void LinkMainPirate(PirateMain mainPirate) {
            this.mainPirate = mainPirate;
        }
    }
}
