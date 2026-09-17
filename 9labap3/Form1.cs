using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace _9labap3
{
    public partial class Form1 : Form
    {
        private DataGridView gridEmployees;
        private DataGridView gridSIZ;
        private DataGridView gridIssued;

        public Form1()
        {
            InitializeComponent();
            DatabaseHelper.Initialize();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "🏭 Учёт СИЗ в цеху";
            this.Size = new Size(950, 650);
            this.StartPosition = FormStartPosition.CenterScreen;

            var tabs = new TabControl { Dock = DockStyle.Fill };

            // ===== Вкладка 1: Сотрудники =====
            var tabEmp = new TabPage("👤 Сотрудники");

            var panelEmp = new Panel { Dock = DockStyle.Top, Height = 50 };
            var btnAddEmp = new Button { Text = "➕ Добавить", Left = 10, Top = 10, Width = 120 };
            var btnEditEmp = new Button { Text = "✏️ Изменить", Left = 140, Top = 10, Width = 120 };
            var btnDelEmp = new Button { Text = "❌ Удалить", Left = 270, Top = 10, Width = 120 };
            var btnRefresh = new Button { Text = "🔄 Обновить", Left = 400, Top = 10, Width = 120 };

            btnAddEmp.Click += (s, e) => AddEmployee();
            btnEditEmp.Click += (s, e) => EditEmployee();
            btnDelEmp.Click += (s, e) => DeleteEmployee();
            btnRefresh.Click += (s, e) => LoadData();

            panelEmp.Controls.AddRange(new Control[] { btnAddEmp, btnEditEmp, btnDelEmp, btnRefresh });

            gridEmployees = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false
            };

            tabEmp.Controls.Add(gridEmployees);
            tabEmp.Controls.Add(panelEmp);

            // ===== Вкладка 2: Каталог СИЗ =====
            var tabSIZ = new TabPage("📚 Каталог СИЗ");
            gridSIZ = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false
            };
            tabSIZ.Controls.Add(gridSIZ);

            // ===== Вкладка 3: Выданные СИЗ =====
            var tabIssued = new TabPage("📦 Выданные СИЗ");

            var panelIssued = new Panel { Dock = DockStyle.Top, Height = 50 };
            var btnIssue = new Button { Text = "📋 Выдать СИЗ", Left = 10, Top = 10, Width = 150 };
            var btnWriteOff = new Button { Text = "🔄 Списать", Left = 170, Top = 10, Width = 120 };
            var btnRefresh2 = new Button { Text = "🔄 Обновить", Left = 300, Top = 10, Width = 120 };

            btnIssue.Click += (s, e) => IssueSIZ();
            btnWriteOff.Click += (s, e) => WriteOffSIZ();
            btnRefresh2.Click += (s, e) => LoadData();

            panelIssued.Controls.AddRange(new Control[] { btnIssue, btnWriteOff, btnRefresh2 });

            gridIssued = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false
            };

            tabIssued.Controls.Add(gridIssued);
            tabIssued.Controls.Add(panelIssued);

            tabs.TabPages.Add(tabEmp);
            tabs.TabPages.Add(tabSIZ);
            tabs.TabPages.Add(tabIssued);

            this.Controls.Add(tabs);
        }

        private void LoadData()
        {
            // Сотрудники
            gridEmployees.DataSource = null;
            gridEmployees.DataSource = DatabaseHelper.GetEmployees()
                .Select(e => new
                {
                    ID = e.Id,
                    ФИО = e.FullName,
                    Должность = e.Position,
                    Отдел = e.Department
                }).ToList();

            // СИЗ
            gridSIZ.DataSource = null;
            gridSIZ.DataSource = DatabaseHelper.GetSIZList()
                .Select(s => new
                {
                    ID = s.Id,
                    Название = s.Name,
                    Размер = s.Size,
                    Срок_мес = s.WearPeriodMonths
                }).ToList();

            // Выдачи
            gridIssued.DataSource = null;
            gridIssued.DataSource = DatabaseHelper.GetIssuedSIZ()
                .Select(i => new
                {
                    ID = i.Id,
                    Сотрудник = i.EmployeeName,
                    СИЗ = i.SIZName,
                    Количество = i.Quantity,
                    Дата_выдачи = i.IssueDate,
                    Статус = i.IsActive ? "✅ Выдан" : "❌ Списан"
                }).ToList();
        }

        private void AddEmployee()
        {
            var form = new Form
            {
                Text = "➕ Новый сотрудник",
                Size = new Size(400, 250),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var lblN = new Label { Text = "ФИО:", Left = 20, Top = 20, Width = 100 };
            var txtN = new TextBox { Left = 130, Top = 20, Width = 220 };
            var lblP = new Label { Text = "Должность:", Left = 20, Top = 60, Width = 100 };
            var txtP = new TextBox { Left = 130, Top = 60, Width = 220 };
            var lblD = new Label { Text = "Отдел/Цех:", Left = 20, Top = 100, Width = 100 };
            var txtD = new TextBox { Left = 130, Top = 100, Width = 220 };
            var btnOk = new Button { Text = "Сохранить", Left = 130, Top = 150, Width = 100, DialogResult = DialogResult.OK };
            var btnCancel = new Button { Text = "Отмена", Left = 250, Top = 150, Width = 100, DialogResult = DialogResult.Cancel };

            form.Controls.AddRange(new Control[] { lblN, txtN, lblP, txtP, lblD, txtD, btnOk, btnCancel });

            if (form.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrWhiteSpace(txtN.Text) || string.IsNullOrWhiteSpace(txtP.Text))
                {
                    MessageBox.Show("ФИО и Должность обязательны!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DatabaseHelper.AddEmployee(txtN.Text, txtP.Text, txtD.Text);
                LoadData();
                MessageBox.Show("Сотрудник добавлен!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void EditEmployee()
        {
            if (gridEmployees.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите сотрудника!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = Convert.ToInt32(gridEmployees.SelectedRows[0].Cells["ID"].Value);
            var emp = DatabaseHelper.GetEmployees().FirstOrDefault(e => e.Id == id);
            if (emp == null) return;

            var form = new Form
            {
                Text = "✏️ Редактировать",
                Size = new Size(400, 250),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var lblN = new Label { Text = "ФИО:", Left = 20, Top = 20, Width = 100 };
            var txtN = new TextBox { Left = 130, Top = 20, Width = 220, Text = emp.FullName };
            var lblP = new Label { Text = "Должность:", Left = 20, Top = 60, Width = 100 };
            var txtP = new TextBox { Left = 130, Top = 60, Width = 220, Text = emp.Position };
            var lblD = new Label { Text = "Отдел/Цех:", Left = 20, Top = 100, Width = 100 };
            var txtD = new TextBox { Left = 130, Top = 100, Width = 220, Text = emp.Department };
            var btnOk = new Button { Text = "Сохранить", Left = 130, Top = 150, Width = 100, DialogResult = DialogResult.OK };
            var btnCancel = new Button { Text = "Отмена", Left = 250, Top = 150, Width = 100, DialogResult = DialogResult.Cancel };

            form.Controls.AddRange(new Control[] { lblN, txtN, lblP, txtP, lblD, txtD, btnOk, btnCancel });

            if (form.ShowDialog() == DialogResult.OK)
            {
                DatabaseHelper.UpdateEmployee(id, txtN.Text, txtP.Text, txtD.Text);
                LoadData();
                MessageBox.Show("Данные обновлены!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void DeleteEmployee()
        {
            if (gridEmployees.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите сотрудника!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = Convert.ToInt32(gridEmployees.SelectedRows[0].Cells["ID"].Value);
            string name = gridEmployees.SelectedRows[0].Cells["ФИО"].Value.ToString();

            if (MessageBox.Show($"Удалить сотрудника '{name}'?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                DatabaseHelper.DeleteEmployee(id);
                LoadData();
                MessageBox.Show("Сотрудник удалён!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void IssueSIZ()
        {
            var emps = DatabaseHelper.GetEmployees();
            var sizs = DatabaseHelper.GetSIZList();

            if (emps.Count == 0 || sizs.Count == 0)
            {
                MessageBox.Show("Нет сотрудников или СИЗ в базе!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var form = new Form
            {
                Text = "📋 Выдача СИЗ",
                Size = new Size(480, 280),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var lblE = new Label { Text = "Сотрудник:", Left = 20, Top = 20, Width = 100 };
            var cmbE = new ComboBox
            {
                Left = 130,
                Top = 20,
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList,
                DataSource = emps,
                DisplayMember = "FullName",
                ValueMember = "Id"
            };

            var lblS = new Label { Text = "СИЗ:", Left = 20, Top = 60, Width = 100 };
            var cmbS = new ComboBox
            {
                Left = 130,
                Top = 60,
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList,
                DataSource = sizs,
                DisplayMember = "Name",
                ValueMember = "Id"
            };

            var lblQ = new Label { Text = "Количество:", Left = 20, Top = 100, Width = 100 };
            var txtQ = new TextBox { Left = 130, Top = 100, Width = 80, Text = "1" };

            var btnOk = new Button { Text = "Выдать", Left = 130, Top = 150, Width = 100, DialogResult = DialogResult.OK };
            var btnCancel = new Button { Text = "Отмена", Left = 250, Top = 150, Width = 100, DialogResult = DialogResult.Cancel };

            form.Controls.AddRange(new Control[] { lblE, cmbE, lblS, cmbS, lblQ, txtQ, btnOk, btnCancel });

            if (form.ShowDialog() == DialogResult.OK)
            {
                if (!int.TryParse(txtQ.Text, out int qty) || qty <= 0)
                {
                    MessageBox.Show("Некорректное количество!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DatabaseHelper.IssueSIZ((int)cmbE.SelectedValue, (int)cmbS.SelectedValue, qty);
                LoadData();
                MessageBox.Show("СИЗ выдан!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void WriteOffSIZ()
        {
            if (gridIssued.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите запись для списания!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = Convert.ToInt32(gridIssued.SelectedRows[0].Cells["ID"].Value);

            if (MessageBox.Show("Списать выбранный СИЗ?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                DatabaseHelper.WriteOffSIZ(id);
                LoadData();
                MessageBox.Show("СИЗ списан!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}