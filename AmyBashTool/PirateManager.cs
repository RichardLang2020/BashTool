using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AmyBashTool {
    class PirateManager {
        // PRIVATE VARIABLES
        private List<PirateMain> currentPirates;
        private List<PirateAlt> altPirateList;
        private int battleNumber;
        private bool newBasherAlert;

        // CONSTRUCTORS
        public PirateManager() {
            this.currentPirates = new List<PirateMain>();
            this.altPirateList = new List<PirateAlt>();
            this.battleNumber = 1;
            this.newBasherAlert = false;
        }

        // GETTER METHODS
        public List<PirateMain> GetMainPirateNames() {
            return this.currentPirates;
        }
        public PirateMain GetMainPirate(string pirateName) {
            foreach(PirateMain pirate in this.currentPirates) {
                if(pirate.GetName().Equals(pirateName)) {
                    return pirate;
                }
            }

            return null;
        }
        public int GetBattleNumber() {
            return this.battleNumber;
        }
        public int GetTotalLockerCount() {
            int totalLLCount = 0;

            foreach(PirateMain pirate in currentPirates) {
                totalLLCount += pirate.GetTotalLockers();
            }

            return totalLLCount;
        }
        public int GetLockerCount(int battleNumber) {
            int totalLLCount = 0;

            foreach(PirateMain pirate in currentPirates) {
                totalLLCount += pirate.GetLockers(battleNumber);
            }

            return totalLLCount;
        }

        // SETTER METHODS
        public void SetBasherAlert(bool alertNotif) {
            this.newBasherAlert = alertNotif;
        }

        // MAIN METHODS
        public void AddMain(string pirateName) {
            string modifiedPirateName = pirateName.Substring(0, 1).ToUpper() + pirateName.Substring(1).ToLower();

            foreach(PirateMain pirate in this.currentPirates) {
                if(pirate.GetName().Equals(modifiedPirateName)) {
                    return;
                }
            }

            this.currentPirates.Add(new PirateMain(modifiedPirateName));
        }
        /*
         * Takes in an altName (alternate account name) and a mainName to link together
         * - The altName should be one not already taken
         * - The mainName can already be defined, or can be a new one
         */
        public void AddAlt(string altName, string mainName) {
            string modifiedAltName = altName.Substring(0, 1).ToUpper() + altName.Substring(1).ToLower();
            string modifiedMainName = mainName.Substring(0, 1).ToUpper() + mainName.Substring(1).ToLower();
            // First check to make sure that the alt account isn't already enrolled
            // If it does, we can either overwrite it or just immediately break? Right now we just break
            foreach(PirateAlt pirate in this.altPirateList) {
                if(pirate.GetName().Equals(modifiedAltName)) {
                    return;
                }
            }

            PirateMain mainPirate = null;
            foreach(PirateMain tempPirate in this.currentPirates) {
                if(tempPirate.GetName().Equals(modifiedMainName)) {
                    mainPirate = tempPirate;
                    break;
                }
            }
            // If no match was found in the existing list...
            if(mainPirate == null) {
                mainPirate = new PirateMain(modifiedMainName);
                this.currentPirates.Add(mainPirate);
            }
            PirateAlt altPirate = new PirateAlt(modifiedAltName);
            this.altPirateList.Add(altPirate);

            altPirate.LinkMainPirate(mainPirate);
            mainPirate.AddAlt(altPirate);
        }
        public void AddLocker(string pirateName) {
            foreach(PirateMain pirate in this.currentPirates) {
                if(pirate.GetName().Equals(pirateName)) {
                    pirate.AddLocker(this.battleNumber);
                    return;
                }
            }

            // Pirate name not found in main accounts
            foreach(PirateAlt pirate in this.altPirateList) {
                if(pirate.GetName().Equals(pirateName)) {
                    pirate.GetMain().AddLocker(this.battleNumber);
                    return;
                }
            }

            // Pirate name not found in main or alt accounts
            if(newBasherAlert) {
                MessageBox.Show("Unlisted striker " + pirateName + " has appeared!");
            }
            Console.WriteLine("New striker found! " + pirateName);
            // Toggle here for automatically add unlisted pirates as main accounts?
            // Other option is to ignore the unlisted pirates
            PirateMain newPirate = new PirateMain(pirateName);
            this.currentPirates.Add(newPirate);
            newPirate.AddLocker(this.battleNumber);
        }
        public void NextBattle() {
            this.battleNumber++;
        }
        public string WindowsFormAccountLinkString() {
            string output = "";

            foreach(PirateMain pirate in this.currentPirates) {
                output += pirate.GetName() + "\n";

                foreach(PirateAlt pirateAlt in pirate.GetAlts()) {
                    output += "    " + pirateAlt.GetName() + "\n";
                }
            }

            return output;
        }
        // THIS METHOD NEEDS TO BE FIXED UP QUITE A LOT
        public string WindowsFormLockerCountString() {
            string output = "";

            output += (this.battleNumber - 1) + " battles, " + this.GetTotalLockerCount() + " LL's\n";

            this.currentPirates.Sort((x, y) => y.GetTotalLockers().CompareTo(x.GetTotalLockers()));

            foreach(PirateMain pirate in this.currentPirates) {
                output += pirate.GetName() + ": " + pirate.GetTotalLockers() + "\n";
            }

            return output;
        }
        public string WindowsFormLockerCountString(int battleNumber) {
            string output = "";

            output += "Battle #" + battleNumber + "\n";

            this.currentPirates.Sort((x, y) => y.GetLockers(battleNumber).CompareTo(x.GetLockers(battleNumber)));

            foreach(PirateMain pirate in this.currentPirates) {
                output += pirate.GetName() + ": " + pirate.GetLockers(battleNumber) + "\n";
            }

            output += "------------------\nTOTAL: " + this.GetLockerCount(battleNumber) + " LL's\n";

            return output;
        }
    }
}
