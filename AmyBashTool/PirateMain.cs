using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmyBashTool {
	class PirateMain {
		// PRIVATE VARIABLES
		private string name;
		private List<int> currentBashHistory;
		private List<PirateAlt> altNames;

		// CONSTRUCTORS
		public PirateMain(string name) {
			this.name = name;
			this.currentBashHistory = new List<int>();
			this.currentBashHistory.Add(0);
			this.altNames = new List<PirateAlt>();
		}

		// GETTER METHODS
		public string GetName() {
			return this.name;
		}
		public int GetTotalLockers() {
			return this.currentBashHistory.Sum();
		}
		public int GetTotalBattles() {
			int output = 0;

			for(int i = 0; i < this.currentBashHistory.Count; i++) {
				if(this.currentBashHistory[i] != 0) {
					output++;
				}
			}

			return output;
		}
		public int GetMaximumLockers() {
			return this.currentBashHistory.Max();
		}
		public List<PirateAlt> GetAlts() {
			return this.altNames;
		}
		public int GetLockers(int battleNumber) {
			try {
				return this.currentBashHistory[battleNumber - 1];
			} catch(Exception) {
				return 0;
			}
		}

		// MAIN METHODS
		public void LogBattle(int lavishLockerCount, int battleNumber) {
			this.currentBashHistory[battleNumber - 1] = lavishLockerCount;
		}
		public void AddLocker(int battleNumber) {
			if(this.currentBashHistory.Count() < battleNumber) {
				Console.WriteLine("Currently working on " + this.name + "'s list! Looks like " + this.currentBashHistory);
				Console.WriteLine("Trying to add a new entry to battle number " + battleNumber);
				while(this.currentBashHistory.Count() < battleNumber) {
					this.currentBashHistory.Add(0);
				}
				Console.WriteLine("List now looks like: " + this.currentBashHistory.ToString());
			}

			this.currentBashHistory[battleNumber - 1]++;

			Console.WriteLine("After all adding, " + this.name + "'s list looks like: " + this.currentBashHistory);
		}
		public void AddAlt(PirateAlt altPirate) {
			this.altNames.Add(altPirate);
		}
		public List<string> CsvOutput() {
			List<string> output = new List<string>();

			output.Add(this.name);
			for(int i = 0; i < this.currentBashHistory.Count; i++) {
				output.Add("" + this.currentBashHistory[i]);
			}

			return output;
		}
		public string StatisticsOutput() {
			string output = "";
			output += "~~" + this.name.ToUpper() + "'S STATISTICS~~\n";

			output += "Counts: \n";
			for(int i = 0; i < this.currentBashHistory.Count(); i++) {
				output += "    Battle " + (i + 1) + ": " + this.currentBashHistory[i] + " LLs\n";
			}

			output += "\n";
			output += "Total LLs: " + this.GetTotalLockers() + "\n";
			output += "Maximum LLs: " + this.GetMaximumLockers() + "\n";
			output += "Average LLs: " + (this.GetTotalLockers() * 1.0) / this.currentBashHistory.Count() + "\n";

			return output;
		}
	}
}
