using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using Test.Model;
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

            // Olay bağlantılarını kontrol edin
            this.personalInfoToolStripMenuItem.Click += personalInfoToolStripMenuItem_Click;
            this.statisticsToolStripMenuItem.Click += statisticsToolStripMenuItem_Click;
        }

        private void MainPageForm_Load(object sender, EventArgs e)
        {
            playersDataGridView.AutoGenerateColumns = false;

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

            playersDataGridView.DataSource = playerRepository.FindAll();

            ResetPlayer();

            // Başlangıçta Personal Info alanlarını görünür yapıyoruz
            personalInfoToolStripMenuItem_Click(this, EventArgs.Empty);
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
            attackTextBox.DataBindings.Add(new Binding("Text", PPlayer, nameof(PPlayer.Attack), true, DataSourceUpdateMode.OnPropertyChanged));
            defenseTextBox.DataBindings.Add(new Binding("Text", PPlayer, nameof(PPlayer.Defense), true, DataSourceUpdateMode.OnPropertyChanged));
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (!IsValidString(nameTextBox.Text) || !IsValidString(surnameTextBox.Text))
            {
                MessageBox.Show("Name and Surname should contain only alphabetic characters.");
                return; // İşlemi durdur
            }

            if (string.IsNullOrWhiteSpace(nameTextBox.Text) ||
                string.IsNullOrWhiteSpace(surnameTextBox.Text) ||
                string.IsNullOrWhiteSpace(heightTextBox.Text) ||
                string.IsNullOrWhiteSpace(weightTextBox.Text) ||
                string.IsNullOrWhiteSpace(attackTextBox.Text) ||
                string.IsNullOrWhiteSpace(defenseTextBox.Text))
            {
                MessageBox.Show("All fields must be filled.");
                return; // İşlemi durdur
            }

            playerRepository.Add(PPlayer);
            MessageBox.Show("Player saved successfully!");
            playersDataGridView.DataSource = null;
            playersDataGridView.DataSource = playerRepository.FindAll();
            ResetPlayer();
        }

        private bool IsValidString(string input)
        {
            return Regex.IsMatch(input, @"^[a-zA-Z]+$");
        }

        private void personalInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowPersonalInfo(true);
            ShowStatistics(false);
        }

        private void statisticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowStatistics(true);
            ShowPersonalInfo(false);
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
            var json = JsonConvert.SerializeObject(playerRepository.FindAll());
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
                        var players = JsonConvert.DeserializeObject<List<Player>>(reader.ReadToEnd());
                        foreach (var player in players)
                        {
                            playerRepository.Add(player);
                        }
                    }
                    playersDataGridView.DataSource = null;
                    playersDataGridView.DataSource = playerRepository.FindAll();
                    MessageBox.Show("Data successfully imported from: " + openFileDialog.FileName);
                }
            }
        }

        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            var searchText = searchTextBox.Text;
            var filteredPlayers = playerRepository.Like(searchText);
            playersDataGridView.DataSource = null;
            playersDataGridView.DataSource = filteredPlayers;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var cellValue = playersDataGridView[e.ColumnIndex, e.RowIndex].Value;
                MessageBox.Show($"Clicked on cell value: {cellValue}");
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void weightTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
