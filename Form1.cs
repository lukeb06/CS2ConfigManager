using System.IO;

namespace ConfigManager
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitLabel1();
            InitButton2();
            InitComboBox1();
        }

        private void InitLabel1()
        {
            string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CS2ConfigManager");

            if (Directory.Exists(appDataPath) == false)
            {
                Directory.CreateDirectory(appDataPath);
            }

            string tsFilePath = Path.Combine(appDataPath, "last_backup.txt");
            if (File.Exists(tsFilePath))
            {
                string lastBackupTime = File.ReadAllText(tsFilePath);
                label1.Text = $"Last Backup: {lastBackupTime}";
            }
            else
            {
                label1.Text = "Last Backup: NEVER";
            }
        }

        private void InitButton2()
        {
            string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CS2ConfigManager");

            if (Directory.Exists(appDataPath) == false)
            {
                Directory.CreateDirectory(appDataPath);
            }

            string tsFilePath = Path.Combine(appDataPath, "last_backup.txt");

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

        private void InitComboBox1()
        {
            string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CS2ConfigManager");

            if (Directory.Exists(appDataPath) == false)
            {
                Directory.CreateDirectory(appDataPath);
            }

            string userFilePath = Path.Combine(appDataPath, "last_user.txt");

            if (File.Exists(userFilePath))
            {
                string lastUser = File.ReadAllText(userFilePath);
                comboBox1.Text = lastUser;
            }

            string steamPath = @"C:\Program Files (x86)\Steam";
            string gamePath = Path.Combine(steamPath, "steamapps", "common", "Counter-Strike Global Offensive");

            string gameUserDataFilePath = Path.Combine(steamPath, "userdata");

            string[] options = Directory.GetDirectories(gameUserDataFilePath);

            foreach (string option in options)
            {
                string folderName = option.Split(@"\").Last();
                comboBox1.Items.Add(folderName);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void CopyDirectory(string path, string newPath)
        {
            if (!Directory.Exists(newPath)) return;

            foreach (string file in Directory.GetFiles(path))
            {
                string fileName = Path.GetFileName(file);
                string newFilePath = Path.Combine(newPath, fileName);
                if (File.Exists(newFilePath))
                {
                    File.Delete(newFilePath);
                }
                File.Copy(file, newFilePath);

                foreach (string dir in Directory.GetDirectories(path))
                {
                    string dirName = dir.Split(@"\").Last();
                    string newDir = Path.Combine(newPath, dirName);
                    Directory.CreateDirectory(newDir);

                    CopyDirectory(Path.Combine(path, dirName), newDir);
                }
            }
        }

        // Backup Button
        private void button1_Click(object sender, EventArgs e)
        {
            string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CS2ConfigManager");
            string steamPath = @"C:\Program Files (x86)\Steam";
            string gamePath = Path.Combine(steamPath, "steamapps", "common", "Counter-Strike Global Offensive");

            string gameCfgFilePath = Path.Combine(gamePath, "game", "csgo", "cfg");
            string gameUserDataFilePath = Path.Combine(steamPath, "userdata");

            if (Directory.Exists(appDataPath) == false)
            {
                Directory.CreateDirectory(appDataPath);
            }

            string tsFilePath = Path.Combine(appDataPath, "last_backup.txt");
            string cfgFilePath = Path.Combine(appDataPath, "cfg");
            string s30FilePath = Path.Combine(appDataPath, "730");

            Directory.CreateDirectory(cfgFilePath);
            Directory.CreateDirectory(s30FilePath);

            File.WriteAllText(tsFilePath, DateTime.Now.ToShortDateString());

            label1.Text = $"Last Backup: {DateTime.Now.ToShortDateString()}";

            button2.BackColor = Color.White;
            button2.ForeColor = Color.Black;
            button2.Enabled = true;



            // Move Files
            CopyDirectory(gameCfgFilePath, cfgFilePath);

            string userId = comboBox1.SelectedItem != null ? comboBox1.SelectedItem.ToString() : comboBox1.Text != null ? comboBox1.Text : "null";
            string userDataPath = Path.Combine(gameUserDataFilePath, userId, "730");
            
            if (!Directory.Exists(userDataPath))
            {
                MessageBox.Show("User ID not found. Please select a valid user from the dropdown.");
                return;
            }

            CopyDirectory(userDataPath, s30FilePath);

            MessageBox.Show("Done!");
        }

        // Restore Button
        private void button2_Click(object sender, EventArgs e)
        {
            string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CS2ConfigManager");
            string steamPath = @"C:\Program Files (x86)\Steam";
            string gamePath = Path.Combine(steamPath, "steamapps", "common", "Counter-Strike Global Offensive");

            if (Directory.Exists(appDataPath) == false)
            {
                Directory.CreateDirectory(appDataPath);
            }

            string tsFilePath = Path.Combine(appDataPath, "last_backup.txt");
            string cfgFilePath = Path.Combine(appDataPath, "cfg");
            string s30FilePath = Path.Combine(appDataPath, "730");

            string gameCfgFilePath = Path.Combine(gamePath, "game", "csgo", "cfg");
            string gameUserDataFilePath = Path.Combine(steamPath, "userdata");

            CopyDirectory(cfgFilePath, gameCfgFilePath);
            
            foreach (string dir in Directory.GetDirectories(gameUserDataFilePath))
            {
                CopyDirectory(s30FilePath, Path.Combine(dir, "730"));
            }

            MessageBox.Show("Done! You can now launch the game.");
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CS2ConfigManager");
            string userFilePath = Path.Combine(appDataPath, "last_user.txt");

            if (Directory.Exists(appDataPath) == false)
            {
                Directory.CreateDirectory(appDataPath);
            }

            File.WriteAllText(userFilePath, comboBox1.SelectedItem.ToString());

            // string item = comboBox1.SelectedItem.ToString();

            // label1.Text = item;
        }
    }
}
