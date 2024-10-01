using Newtonsoft.Json;
using PlayerManagement.Model;
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
            nameTextBox.KeyPress += nameTextBox_KeyPress;
            surnameTextBox.KeyPress += surnameTextBox_KeyPress;
        }

        private void MainPageForm_Load(object sender, EventArgs e)
        {
            playersDataGridView.Dock = DockStyle.Fill;
            playersDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
           //playersDataGridView.AutoGenerateColumns = false;
            playersDataGridView.ReadOnly = true;

            //playersDataGridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "Name", HeaderText = "Name", DataPropertyName = "Name" });
            //playersDataGridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "Surname", HeaderText = "Surname", DataPropertyName = "Surname" });
            //playersDataGridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "Height", HeaderText = "Height", DataPropertyName = "Height" });
            //playersDataGridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "Weight", HeaderText = "Weight", DataPropertyName = "Weight" });
            //playersDataGridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "Attack", HeaderText = "Attack", DataPropertyName = "Attack" });
            //playersDataGridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "Defense", HeaderText = "Defense", DataPropertyName = "Defense" });

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
            else
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
            if (!IsValidString(nameTextBox.Text) || !IsValidString(surnameTextBox.Text) || string.IsNullOrWhiteSpace(nameTextBox.Text) || string.IsNullOrWhiteSpace(surnameTextBox.Text) || string.IsNullOrWhiteSpace(heightTextBox.Text) || string.IsNullOrWhiteSpace(weightTextBox.Text))
            {
                MessageBox.Show("All fields must be filled with valid values.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            PPlayer.Attack = double.TryParse(attackTextBox.Text, out double attack) ? attack : 0;
            PPlayer.Defense = double.TryParse(defenseTextBox.Text, out double defense) ? defense : 0;

            var playerList = playerRepository.FindAll().ToList();
            if (!playerList.Exists(p => p.Name == PPlayer.Name && p.Surname == PPlayer.Surname))
            {
                playerRepository.Add(PPlayer);
                MessageBox.Show("Player saved successfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                playersDataGridView.DataSource = new BindingList<Player>(playerRepository.FindAll());
                ResetPlayer();
            }
            else
            {
                MessageBox.Show("A player with the same name and surname already exists.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private bool IsValidString(string input) => Regex.IsMatch(input, @"^[a-zA-ZğüşıİçÖĞÜŞ\s]*$");

        private void nameTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && e.KeyChar != ' ')
            {
                e.Handled = true;
            }
        }

        private void surnameTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && e.KeyChar != ' ')
            {
                e.Handled = true;
            }
        }

        private void ShowPersonalInfo(bool show)
        {
            label1.Visible = show;
            nameTextBox.Visible = show;
            label2.Visible = show;
            surnameTextBox.Visible = show;
            label3.Visible = show;
            heightTextBox.Visible = show;
            label4.Visible = show;
            weightTextBox.Visible = show;
        }

        private void ShowStatistics(bool show)
        {
            attackLabel.Visible = show;
            attackTextBox.Visible = show;
            defenseLabel.Visible = show;
            defenseTextBox.Visible = show;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var selectedPlayer = playersDataGridView.Rows[e.RowIndex].DataBoundItem as Player;
                if (selectedPlayer != null)
                {
                    PPlayer = selectedPlayer;
                    SetPlayerDataBindings();
                }
            }
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
                playerDataModel.PlayerStats.Add(new PlayerStatistics { Attack = player.Attack, Defense = player.Defense });
            }

            var json = JsonConvert.SerializeObject(playerDataModel, Formatting.Indented);
            using (var sf = new SaveFileDialog { Filter = "Json files (*.json)|*.json", RestoreDirectory = true })
            {
                if (sf.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(sf.FileName, json);
                    MessageBox.Show("Data successfully saved to: " + sf.FileName, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private void attackTextBox_TextChanged(object sender, EventArgs e)
        {
        
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
        
        }

        private void Fill(object sender, EventArgs e)
        {
            
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            ResetPlayer()
        }

        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            
        }


        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            playerRepository.RemoveAll();
            using (var openFileDialog = new OpenFileDialog { Filter = "Json files (*.json)|*.json" })
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var json = File.ReadAllText(openFileDialog.FileName);
                    var playerDataModel = JsonConvert.DeserializeObject<PlayerDataModel>(json);
                    foreach (var player in playerDataModel.Players)
                    {
                        playerRepository.Add(player);
                    }

                    playersDataGridView.DataSource = new BindingList<Player>(playerRepository.FindAll());
                    MessageBox.Show("Data imported successfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
