using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

/* TO-DO LIST:
 *     Add an onEnter() for the linking of main / alt accounts
 *     Implement additional tabs for tool management
 *     Add configuration for overwriting or ignoring duplicate 'alt' link attempts
 *     Add smart counting to parse whether or not someone participated in a battle?
 *         We would also have to subsequently store '-1' for battles that the person didn't participate in
 *         
 *     IMPLEMENTATIONS:
 *         Properly implement the check for who was in the battle, and remove non-present battles from averages and whatnot
 *         Ignore battles where you lose (the line "The victors reclaim 5 Lavish Lockers that had been stolen during the fight." will happen)
 *         Fix the engage / disengage messages, which need to be in the following order (in order for the ship to show up):
 *             [20:07:23] You are being pursued by 'Cheap Mako'.
 *             [20:09:21] You have been intercepted by the Cheap Mako!
 *             [20:20:50] Game over.  Winners: 
 *             [20:21:03] Ye received 387 pieces of eight as your initial cut of the booty!
 *             [20:21:05] You are no longer being pursued by 'Cheap Mako'.
 *         
 *         Save logs from a bash
 *         Compilation of basher records
 *         Giant CSV of total results?
 *     CONFIGURATIONS:
 *         Auto-Enrollment of new bashers
 *         Pop-Up when new bashers appear
 *         Ignoring LLs during losses
 *         
 *     RIV'S SUGGESTION: CHECK BOT ADJECTIVES TO FIGURE OUT HOW MANY GREEDIES WERE POSSIBLE ON THE SHIP
 *     DATA RESEARCH:
 *         Gathering all prefixes of greedy pirates into a .txt file
 *         Gathering prefixes of non-greedy pirates into a .txt file (to cross-reference with the greedies)
 */

namespace AmyBashTool {
    public partial class BashTool : Form {
        private string textFileLocation;
        private string currentText;
        private int currentTextIndex;
        private bool processStarted;
        private PirateManager pirateManager;

        public BashTool() {
            InitializeComponent();
            pirateManager = new PirateManager();
            this.textFileLocation = "";
            TabController.SelectedIndexChanged += new EventHandler(TabController_SelectedIndexChanged);

            string linkedAccounts = pirateManager.WindowsFormAccountLinkString();
            if(linkedAccounts.Length != 0) {
                MainAltList.Text = linkedAccounts;
            }
        }

        private void TabController_SelectedIndexChanged(object sender, EventArgs e) {
            switch((sender as TabControl).SelectedIndex) {
                case 0:
                    // Setup Tab
                    break;
                case 1:
                    // Runtime Tab
                    break;
                case 2:
                    // Individual Statistics Tab
                    List<PirateMain> pirateMains = this.pirateManager.GetMainPirateNames();

                    StatisticsAccountSelectionBox.Items.Clear();
                    foreach(PirateMain pirate in pirateMains) {
                        StatisticsAccountSelectionBox.Items.Add(pirate.GetName());
                    }
                    break;
                case 3:
                    // Group Statistics Tab
                    int battleNumber = this.pirateManager.GetBattleNumber();

                    StatisticsBattleSelectionBox.Items.Clear();
                    for(int i = 1; i <= battleNumber; i++) {
                        StatisticsBattleSelectionBox.Items.Add(i);
                    }
                    break;
            }
        }

        // SETUP TAB
        private void LinkSubmitButton_Click(object sender, EventArgs e) {
            if(!LinkMainName.Text.Equals("") && !LinkAltName.Text.Equals("")) {
                pirateManager.AddMain(LinkMainName.Text);
                pirateManager.AddAlt(LinkAltName.Text, LinkMainName.Text);
            }

            MainAltList.Text = pirateManager.WindowsFormAccountLinkString();
        }
        private void LoadFileButton_Click(object sender, EventArgs e) {
            if (OpenFileDialog.ShowDialog() == DialogResult.OK) {
                textFileLocation = OpenFileDialog.FileName;
                LoadFileText.Text = "Loading...";
            }

            // this.currentText = File.ReadAllText(this.textFileLocation);

            FileStream logFileStream = new FileStream(this.textFileLocation, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader logFileReader = new StreamReader(logFileStream);
            this.currentText = logFileReader.ReadToEnd();
            logFileReader.Close();
            logFileStream.Close();

            LoadFileText.Text = "Completed!";
        }
        private void StartButton_Click(object sender, EventArgs e) {
            if(processStarted) {
                MessageBox.Show("Already running an analysis!");
                return;
            }

            FileStream logFileStream = new FileStream(this.textFileLocation, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader logFileReader = new StreamReader(logFileStream);
            this.currentText = logFileReader.ReadToEnd();
            logFileReader.Close();
            logFileStream.Close();
            string[] splitText = currentText.Split('\n');
            this.currentTextIndex = splitText.Length;
            ShipNamesAndLLs.Text = "";

            if(NewBasherAlertConfig.Checked) {
                pirateManager.SetBasherAlert(true);
            } else {
                pirateManager.SetBasherAlert(false);
            }

            if(FullFileAnalysisConfig.Checked) {
                foreach(string lineOfText in splitText) {
                    string strikerName = TextAnalyzer.IdentifyStrikerFromLine(lineOfText);

                    if(strikerName.Equals("N/A - GAME IS OVER!")) {
                        pirateManager.NextBattle();
                    } else if(strikerName.Contains(" ")) {
                        // Ship name
                        ShipNamesAndLLs.Text += (pirateManager.GetBattleNumber() - 1) + ". " + strikerName + ": " + pirateManager.GetLockerCount(pirateManager.GetBattleNumber() - 1) + " LL's\n";
                    } else if(!strikerName.Equals("")) {
                        pirateManager.AddLocker(strikerName);
                        continue;
                    }
                }
            }

            TabController.SelectedIndex = 1;
            MainProcess.RunWorkerAsync();
        }

        // RUNTIME TAB
        private void MainProcess_DoWork(object sender, DoWorkEventArgs e) {
            this.processStarted = true;
            while(true) {
                try {
                    // this.currentText = File.ReadAllText(this.textFileLocation);

                    FileStream logFileStream = new FileStream(this.textFileLocation, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    StreamReader logFileReader = new StreamReader(logFileStream);
                    this.currentText = logFileReader.ReadToEnd();
                    logFileReader.Close();
                    logFileStream.Close();
                } catch(Exception) {
                    continue;
                }
                string[] splitText = currentText.Split('\n');
                if(splitText.Length == this.currentTextIndex) {
                    System.Threading.Thread.Sleep(500);
                    continue;
                }

                string runtimeString = "";
                for(int i = splitText.Length - 6; i < splitText.Length - 1; i++) {
                    try {
                        runtimeString += splitText[i] + "\n";
                    } catch(Exception) {

                    }
                }

                RuntimeLatestLines.Invoke((MethodInvoker)delegate {
                    RuntimeLatestLines.Text = runtimeString;
                });

                for(int i = this.currentTextIndex - 1; i < splitText.Length - 1; i++) {
                    string strikerName = TextAnalyzer.IdentifyStrikerFromLine(splitText[i]);

                    if(strikerName.Equals("N/A - GAME IS OVER!")) {
                        pirateManager.NextBattle();
                        Console.WriteLine("LIVE BATTLE END LOGGED - LINE IS " + splitText[i]);
                    } else if(strikerName.Contains(" ")) {
                        // Ship name
                        ShipNamesAndLLs.Invoke((MethodInvoker)delegate {
                            ShipNamesAndLLs.Text += (pirateManager.GetBattleNumber() - 1) + ". " + strikerName + ": " + pirateManager.GetLockerCount(pirateManager.GetBattleNumber() - 1) + " LL's\n";
                        });
                        
                        Console.WriteLine((pirateManager.GetBattleNumber() - 1) + ". " + strikerName + ": " + pirateManager.GetLockerCount(pirateManager.GetBattleNumber() - 1) + " LL's\n");
                    } else if(!strikerName.Equals("")) {
                        pirateManager.AddLocker(strikerName);
                        Console.WriteLine("LIVE GREEDY STRIKE LOGGED - LINE IS " + splitText[i]);
                        CurrentBattleCounts.Invoke((MethodInvoker)delegate {
                            CurrentBattleCounts.Text = this.pirateManager.WindowsFormLockerCountString(this.pirateManager.GetBattleNumber());
                        });

                        this.currentTextIndex = splitText.Length;
                    }
                }

                this.currentTextIndex = splitText.Length;
            }
        }
        private void ShipNamesAndLLs_Click(object sender, EventArgs e) {
            Clipboard.SetText(ShipNamesAndLLs.Text);
        }
        private void CurrentBattleCounts_Click(object sender, EventArgs e) {
            Clipboard.SetText(CurrentBattleCounts.Text);
        }

        // INDIVIDUAL STATISTICS TAB
        private void SearchStatisticsButton_Click(object sender, EventArgs e) {
            PirateMain selectedPirate = pirateManager.GetMainPirate(StatisticsAccountSelectionBox.SelectedItem.ToString());
            IndividualStatisticsText.Text = selectedPirate.StatisticsOutput();
        }
        private void IndividualStatisticsText_Click(object sender, EventArgs e) {
            Clipboard.SetText(IndividualStatisticsText.Text);
        }

        // GROUP STATISTICS TAB
        private void SearchBattleStatisticsButton_Click(object sender, EventArgs e) {
            int battleNumber = -1;
            Int32.TryParse(StatisticsBattleSelectionBox.SelectedItem.ToString(), out battleNumber);
            BattleStatisticsText.Text = this.pirateManager.WindowsFormLockerCountString(battleNumber);
        }
        private void FullStatisticsButton_Click(object sender, EventArgs e) {
            BattleStatisticsText.Text = pirateManager.WindowsFormLockerCountString();
        }
        private void BattleStatisticsText_Click(object sender, EventArgs e) {
            Clipboard.SetText(BattleStatisticsText.Text);
        }
    }
}
