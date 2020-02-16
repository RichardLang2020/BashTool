using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmyBashTool {
	static class TextAnalyzer {
		/*
		 * IF A STRIKER EXISTS:		Return "<striker_name_here>" Ex. "Amythrumbler"
		 * IF THE GAME HAS ENDED:	Return "N/A - GAME IS OVER!"
		 * IF A BATTLE STARTED:		Return "BATTLE STARTED"
		 * ELSE:					Return ""
		 */
		public static string IdentifyStrikerFromLine(string line) {
			string pirateName = "";
			string[] partitionedLine = line.Split(' ');
			// CHECK IF THE PARTITIONED LINE IS ACTUALLY A GREEDY LINE
			// "__ performs a powerful attack against __ __, and steals some loot in the process!"
			if(line.Contains("performs a powerful attack against") &&
					line.Contains(", and steals some loot in the process!")) {
				pirateName = partitionedLine[1];
			}
			// "__ executes a masterful strike against __ __, who drops some treasure in surprise!"
			if(line.Contains("executes a masterful strike against") &&
					line.Contains(", who drops some treasure in surprise!")) {
				pirateName = partitionedLine[1];
			}
			// "__ delivers an overwhelming barrage against __ __, causing some treasure to fall from their grip!"
			if(line.Contains("delivers an overwhelming barrage against") &&
					line.Contains(", causing some treasure to fall from their grip!")) {
				pirateName = partitionedLine[1];
			}
			// "__ swings a devious blow against __ __, jarring some treasure loose!"
			if(line.Contains("swings a devious blow against") &&
					line.Contains(", jarring some treasure loose!")) {
				pirateName = partitionedLine[1];
			}
			// "Game over.  Winners:"
			if(line.Contains("Game over.  Winners:")) {
				pirateName = "N/A - GAME IS OVER!";
			}
			// "You are no longer being pursued by '__ __'."
			if(line.Contains("You are no longer being pursued by")) {
				pirateName = partitionedLine[8] + " " + partitionedLine[9];
				pirateName = pirateName.Substring(1, pirateName.Length - 4);

				Console.WriteLine("BATTLE ENDED against " + pirateName);
			}
			// [17:00:21] Lovey-Dovey Maid disengaged from the battle.
			// [03:05:57] The victors reclaim 4 Lavish Lockers that had been stolen during the fight.
			return pirateName;
		}

		/*
		private static void AnalyzeFileContents(string fileContents, int startPoint) {
			string analysisText = fileContents.Substring(startPoint);
			string[] splitText = analysisText.Split('\n');

			for(int lineNumber = 0; lineNumber < splitText.Length; lineNumber++) {
				string strikerName = IdentifyStrikerFromLine(splitText[lineNumber]);
				if(strikerName.Length == 0) {
					continue;
				} else if(strikerName.Contains("N/A - GAME IS OVER!")) {
					// battleNumber++;
					continue;
				} else if(altChecker.containsKey(strikerName)) {
					strikerName = altChecker.get(strikerName);
				}

				// Check if the pirate exists or not. If not, create them and add them to our ongoing list.
				bool pirateExists = false;
				int pirateIndex = -1;
				for(int j = 0; j < pirateList.size(); j++) {
					if(pirateList.get(j).getName().equalsIgnoreCase(strikerName)) {
						pirateExists = true;
						pirateIndex = j;
						break;
					}
				}
				if(!pirateExists) {
					pirateList.add(new PirateMain(strikerName));
					pirateIndex = pirateList.size() - 1;
				}

				// Add the LL to the pirate's count.
				pirateList.get(pirateIndex).addLocker(battleNumber);

				// Lastly...
				splitText[lineNumber] = br.readLine();
			}
			br.close();

			System.out.println("RUN STATISTICS: ");
			for(PirateMain p : pirateList) {
				System.out.println("	" + p.GetName() + ":" + p.GetTotalLockers());
				masterString += "	" + p.GetName() + ":" + p.GetTotalLockers() + "\r\n";
			}
		}
		*/
	}
}
