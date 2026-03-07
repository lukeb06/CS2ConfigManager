using System.IO;

namespace ConfigManager
{
    public partial class Form1 : Form
    {
        private string getAppDataPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CS2ConfigManager");
        }

        private string getTsFilePath()
        {
            return Path.Combine(getAppDataPath(), "last_backup.txt");
        }

        private string getUserFilePath()
        {
            return Path.Combine(getAppDataPath(), "last_user.txt");
        }

        private string getSteamPath()
        {
            return @"C:\Program Files (x86)\Steam";
        }
        private string getGamePath()
        {
            return Path.Combine(getSteamPath(), "steamapps", "common", "Counter-Strike Global Offensive");
        }

        private string getGameUserDataFilePath()
        {
            return Path.Combine(getSteamPath(), "userdata");
        }

        private string getGameCfgFilePath()
        {
            return Path.Combine(getGamePath(), "game", "csgo", "cfg");
        }

        private string getCfgFilePath()
        {
            return Path.Combine(getAppDataPath(), "cfg");
        }
        
        private string getS30FilePath()
        {
            return Path.Combine(getAppDataPath(), "730");
        }

        private string getSelectedUserId()
        {
            return comboBox1.SelectedItem.ToString();
        }

        private string getUserId()
        {
            return comboBox1.SelectedItem != null ? getSelectedUserId() : comboBox1.Text != null ? comboBox1.Text : "null";
        }

        private string getUserDataPath(string userId)
        {
            return Path.Combine(getGameUserDataFilePath(), userId, "730");
        }

        public Form1()
        {
            string appDataPath = getAppDataPath();
            string tsFilePath = getTsFilePath();
            string userFilePath = getUserFilePath();

            string steamPath = getSteamPath();
            string gamePath = getGamePath();

            string gameUserDataFilePath = getGameUserDataFilePath();

            string gameCfgFilePath = getGameCfgFilePath();

            string cfgFilePath = getCfgFilePath();
            string s30FilePath = getS30FilePath();


            Directory.CreateDirectory(appDataPath);
            Directory.CreateDirectory(cfgFilePath);
            Directory.CreateDirectory(s30FilePath);

            InitializeComponent();
            InitLabel1(tsFilePath);
            InitButton2(tsFilePath);
            InitComboBox1(userFilePath, gameUserDataFilePath);
        }

        private void ReloadButton2(string tsFilePath)
        {
            if (File.Exists(tsFilePath))
            {
                button2.BackColor = Color.White;
                button2.ForeColor = Color.Black;
                button2.Enabled = true;
            }
            else
            {
                button2.BackColor = Color.DarkGray;
                button2.ForeColor = Color.DimGray;
                button2.Enabled = false;
            }
        }

        private void InitLabel1(string tsFilePath)
        {
            if (File.Exists(tsFilePath)) label1.Text = $"Last Backup: {File.ReadAllText(tsFilePath)}";
        }

        private void InitButton2(string tsFilePath)
        {
            ReloadButton2(tsFilePath);
        }

        private void InitComboBox1(string userFilePath, string gameUserDataFilePath)
        {
            if (File.Exists(userFilePath))
            {
                string lastUser = File.ReadAllText(userFilePath);
                comboBox1.Text = lastUser;
            }

            string[] options = Directory.GetDirectories(gameUserDataFilePath);

            foreach (string option in options)
            {
                string folderName = option.Split(@"\").Last();
                comboBox1.Items.Add(folderName);
            }
        }

        private void CopyFile(string file, string newPath)
        {
            if (!Directory.Exists(newPath)) return;

            string fileName = Path.GetFileName(file);
            string newFilePath = Path.Combine(newPath, fileName);
            if (File.Exists(newFilePath))
            {
                File.Delete(newFilePath);
            }
            File.Copy(file, newFilePath);
        }

        private void CopyDirectory(string path, string newPath)
        {
            if (!Directory.Exists(newPath)) return;

            foreach (string file in Directory.GetFiles(path))
            {
                CopyFile(file, newPath);

                foreach (string dir in Directory.GetDirectories(path))
                {
                    string dirName = dir.Split(@"\").Last();
                    string newDir = Path.Combine(newPath, dirName);
                    Directory.CreateDirectory(newDir);

                    CopyDirectory(Path.Combine(path, dirName), newDir);
                }
            }
        }

        private void Backup()
        {
            string date = DateTime.Now.ToShortDateString();

            File.WriteAllText(getTsFilePath(), date);

            label1.Text = $"Last Backup: {date}";

            button2.BackColor = Color.White;
            button2.ForeColor = Color.Black;
            button2.Enabled = true;

            // Move Files
            CopyDirectory(getGameCfgFilePath(), getCfgFilePath());

            string userId = getUserId();
            string userDataPath = getUserDataPath(userId);

            if (!Directory.Exists(userDataPath))
            {
                MessageBox.Show("User ID not found. Please select a valid user from the dropdown.");
                return;
            }

            CopyDirectory(userDataPath, getS30FilePath());
        }

        private void Restore()
        {
            CopyDirectory(getCfgFilePath(), getGameCfgFilePath());

            if (checkBox1.Checked)
            {
                foreach (string dir in Directory.GetDirectories(getGameUserDataFilePath()))
                {
                    CopyDirectory(getS30FilePath(), Path.Combine(dir, "730"));
                }
            }
            else
            {
                string userId = getUserId();
                string userDataPath = getUserDataPath(userId);
                CopyDirectory(getS30FilePath(), userDataPath);
            }
        }

        // Backup Button
        private void button1_Click(object sender, EventArgs e)
        {
            Backup();
            MessageBox.Show("Done!");
        }

        // Restore Button
        private void button2_Click(object sender, EventArgs e)
        {
            Restore();
            MessageBox.Show("Done! You can now launch the game.");
        }

        // User ID Selector
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            File.WriteAllText(getUserFilePath(), getSelectedUserId());
        }
    }
}
