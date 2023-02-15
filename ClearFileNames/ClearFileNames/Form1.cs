namespace ClearFileNames
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ClearFields();
        }

        private void BlockInput()
        {
            this.Enabled = false;
            //tbPath.Enabled = tbTemplate.Enabled = btnSelect.Enabled = btnApply.Enabled = groupBox.Enabled = false;
        }

        private void UnBlockInput()
        {
            this.Enabled = true;
            //tbPath.Enabled = tbTemplate.Enabled = btnSelect.Enabled = btnApply.Enabled = groupBox.Enabled = true;
        }

        private void ClearFields()
        {
            tbPath.Text = string.Empty;

            tbTemplate.BackColor = Color.Gray;
            tbTemplate.Enabled = false;

            tbTemplate.Text = string.Empty;

            listFiles.Items.Clear();

            groupBox.Enabled = false;
            
            btnApply.Enabled = false;
            btnApply.BackColor = Color.Gray;

        }

        void MakeFieldsAvailable()
        {
            tbTemplate.Enabled = true;
            tbTemplate.BackColor = Color.WhiteSmoke;

            groupBox.Enabled = true;
            
            btnApply.Enabled = true;
            btnApply.BackColor = Color.WhiteSmoke;
        }

        private void ErrorCame()
        {
            MessageBox.Show("Выбраного вами каталога, или файлов в нём уже не существует!\nВыберите путь к новому каталогу.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

            ClearFields();
        }

        private void FillList(string path)
        {
            if (listFiles.Items.Count > 0)
            {
                listFiles.Items.Clear();
            }

            var fileNames = new List<string>();
            foreach (var item in Directory.GetFiles(path))
            {
                if (File.Exists(item))
                {
                    fileNames.Add(Path.GetFileName(item));
                }
            }

            if (fileNames.Count == 0)
            {
                ErrorCame();
                return;
            }

            listFiles.Items.AddRange(fileNames.ToArray());
        }

        void SelectFolder(string path, bool isTextChanged = false)
        {
            if (Directory.Exists(path))
            {
                tbPath.Text = path;
                FillList(path);
                MakeFieldsAvailable();
            }
            else tbPath.Text = string.Empty;
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            using (var folderBrowser = new FolderBrowserDialog())
            {
                if (folderBrowser.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowser.SelectedPath))
                {
                    SelectFolder(folderBrowser.SelectedPath);
                }
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            BlockInput();
            if (Directory.Exists(tbPath.Text))
            {
                foreach (var item in listFiles.Items)
                {
                    var oldFileName = $"{tbPath.Text}\\{item}";
                    if (File.Exists(oldFileName))
                    {
                        var name = Path.GetFileNameWithoutExtension(oldFileName);
                        var extension = Path.GetExtension(oldFileName);
                        if (name != tbTemplate.Text)
                        {
                            var newFileName = $"{tbPath.Text}\\{name.Replace(tbTemplate.Text, "")}{extension}";
                            if (File.Exists(newFileName))
                            {
                                if (rbtnYes.Checked == true) File.Delete(newFileName);
                                else continue;
                            }
                            if (File.Exists(oldFileName)) File.Move(oldFileName, newFileName);
                        }
                    }
                }
                FillList(tbPath.Text);
                MessageBox.Show("Замена имени файлов в указаном каталоге ЗАВЕРШЕНА УСПЕШНО!", "Уведомление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                UnBlockInput();
            }
            else
            {
                ErrorCame();
            }
        }

        private void tbPath_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tbPath.Text)) SelectFolder(tbPath.Text);
        }
    }
}