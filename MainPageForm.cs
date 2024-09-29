using Newtonsoft.Json;
using PlayerManagement.Model; // Düzgün namespace kullanımı
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Test.Repository;

namespace Test
{
    public partial class MainPageForm : Form
    {
        private readonly PlayerRepository playerRepository;
        public Player PPlayer { get; set; }

        public MainPageForm()
        {
            InitializeComponent();
            playerRepository = PlayerRepository.GetInstance();
            this.Load += MainPageForm_Load;
            playerTabControl.SelectedIndexChanged += PlayerTabControl_SelectedIndexChanged;

            // KeyPress olaylarını bağla
            nameTextBox.KeyPress += nameTextBox_KeyPress;
            surnameTextBox.KeyPress += surnameTextBox_KeyPress;
        }

        private void MainPageForm_Load(object sender, EventArgs e)
        {
            playersDataGridView.Dock = DockStyle.Fill;
            playersDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            playersDataGridView.AutoGenerateColumns = false;
            playersDataGridView.ReadOnly = true; // DataGridView'i yalnızca okunur yap

            playersDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Name",
                HeaderText = "Name",
                DataPropertyName = "Name"
            });

            playersDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Surname",
                HeaderText = "Surname",
                DataPropertyName = "Surname"
            });

            playersDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Height",
                HeaderText = "Height",
                DataPropertyName = "Height"
            });

            playersDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Weight",
                HeaderText = "Weight",
                DataPropertyName = "Weight"
            });

            playersDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Attack",
                HeaderText = "Attack",
                DataPropertyName = "Attack"
            });

            playersDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Defense",
                HeaderText = "Defense",
                DataPropertyName = "Defense"
            });

            playersDataGridView.DataSource = new BindingList<Player>(playerRepository.FindAll());

            ResetPlayer();

            playerTabControl.SelectedTab = tabPage1;
            ShowPersonalInfo(true);
            ShowStatistics(false);
        }

        private void PlayerTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (playerTabControl.SelectedTab == tabPage1)
            {
                ShowPersonalInfo(true);
                ShowStatistics(false);
            }
            else if (playerTabControl.SelectedTab == tabPage2)
            {
                ShowPersonalInfo(false);
                ShowStatistics(true);
            }
        }

        private void ResetPlayer()
        {
            PPlayer = new Player();
            SetPlayerDataBindings();
        }

        private void SetPlayerDataBindings()
        {
            nameTextBox.DataBindings.Clear();
            surnameTextBox.DataBindings.Clear();
            heightTextBox.DataBindings.Clear();
            weightTextBox.DataBindings.Clear();
            attackTextBox.DataBindings.Clear();
            defenseTextBox.DataBindings.Clear();

            nameTextBox.DataBindings.Add(new Binding("Text", PPlayer, nameof(PPlayer.Name), true, DataSourceUpdateMode.OnPropertyChanged));
            surnameTextBox.DataBindings.Add(new Binding("Text", PPlayer, nameof(PPlayer.Surname), true, DataSourceUpdateMode.OnPropertyChanged));
            heightTextBox.DataBindings.Add(new Binding("Text", PPlayer, nameof(PPlayer.Height), true, DataSourceUpdateMode.OnPropertyChanged));
            weightTextBox.DataBindings.Add(new Binding("Text", PPlayer, nameof(PPlayer.Weight), true, DataSourceUpdateMode.OnPropertyChanged));
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            // Düzenli ifadeyi güncelledik
            if (!IsValidString(nameTextBox.Text) || !IsValidString(surnameTextBox.Text))
            {
                MessageBox.Show("Name and Surname should contain only alphabetic characters, including Turkish letters and spaces.");
                return;
            }

            if (string.IsNullOrWhiteSpace(nameTextBox.Text) ||
                string.IsNullOrWhiteSpace(surnameTextBox.Text) ||
                string.IsNullOrWhiteSpace(heightTextBox.Text) ||
                string.IsNullOrWhiteSpace(weightTextBox.Text))
            {
                MessageBox.Show("All fields must be filled.");
                return;
            }

            PPlayer.Attack = double.TryParse(attackTextBox.Text, out double attack) ? attack : 0;
            PPlayer.Defense = double.TryParse(defenseTextBox.Text, out double defense) ? defense : 0;

            playerRepository.Add(PPlayer);
            MessageBox.Show("Player saved successfully!");
            playersDataGridView.DataSource = null;
            playersDataGridView.DataSource = new BindingList<Player>(playerRepository.FindAll());
            ResetPlayer();
        }

        private bool IsValidString(string input)
        {
            // Türk alfabesindeki harfleri ve boşluğu kabul eden regex
            return Regex.IsMatch(input, @"^[a-zA-ZğüşıİçÖĞÜŞ\s]*$");
        }

        private void nameTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Harfler ve boşluk karakterine izin ver
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && e.KeyChar != ' ')
            {
                e.Handled = true; // Geçersiz karakterse işlemi iptal et
            }
        }

        private void surnameTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Harfler ve boşluk karakterine izin ver
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && e.KeyChar != ' ')
            {
                e.Handled = true; // Geçersiz karakterse işlemi iptal et
            }
        }

        private void ShowPersonalInfo(bool show)
        {
            label1.Visible = show;  // Name label
            nameTextBox.Visible = show;
            label2.Visible = show;  // Surname label
            surnameTextBox.Visible = show;
            label3.Visible = show;  // Height label
            heightTextBox.Visible = show;
            label4.Visible = show;  // Weight label
            weightTextBox.Visible = show;
        }

        private void ShowStatistics(bool show)
        {
            attackLabel.Visible = show;  // Attack label
            attackTextBox.Visible = show;  // Attack TextBox
            defenseLabel.Visible = show;  // Defense label
            defenseTextBox.Visible = show;  // Defense TextBox
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            ResetPlayer();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var playerDataModel = new PlayerDataModel
            {
                SavingDate = DateTime.Now.ToString("dd.MM.yyyy"),
                Players = new BindingList<Player>(playerRepository.FindAll())
            };

            foreach (var player in playerDataModel.Players)
            {
                playerDataModel.PlayerStats.Add(new PlayerStatistics
                {
                    Attack = player.Attack,
                    Defense = player.Defense
                });
            }

            var json = JsonConvert.SerializeObject(playerDataModel, Formatting.Indented);

            using (var sf = new SaveFileDialog())
            {
                sf.Filter = "Json files (*.json)|*.json";
                sf.RestoreDirectory = true;
                if (sf.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(sf.FileName, json);
                    MessageBox.Show("Data successfully saved to: " + sf.FileName);
                }
            }
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            playerRepository.RemoveAll();

            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Json files (*.json)|*.json";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (var reader = new StreamReader(openFileDialog.FileName))
                    {
                        var playerDataModel = JsonConvert.DeserializeObject<PlayerDataModel>(reader.ReadToEnd());

                        foreach (var player in playerDataModel.Players)
                        {
                            playerRepository.Add(player);
                        }

                        foreach (var stats in playerDataModel.PlayerStats)
                        {
                            var player = playerRepository.FindAll().Last();
                            player.Attack = stats.Attack;
                            player.Defense = stats.Defense;
                        }
                    }

                    playersDataGridView.DataSource = null;
                    playersDataGridView.DataSource = new BindingList<Player>(playerRepository.FindAll());
                    MessageBox.Show("Data successfully imported from: " + openFileDialog.FileName);
                }
            }
        }

        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            var searchText = searchTextBox.Text;
            var filteredPlayers = playerRepository.Like(searchText);
            playersDataGridView.DataSource = null;
            playersDataGridView.DataSource = new BindingList<Player>(filteredPlayers);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var cellValue = playersDataGridView[e.ColumnIndex, e.RowIndex].Value;
                MessageBox.Show($"Clicked on cell value: {cellValue}");
            }
        }

        private void attackTextBox_TextChanged(object sender, EventArgs e) { }
        private void Fill(object sender, EventArgs e) { }

        // Menü elemanına bağlanacak metot
        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Buraya dosya menüsü ile ilgili işlemler ekleyebilirsin.
        }
    }
}
