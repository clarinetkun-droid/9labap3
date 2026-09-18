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
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9);

            var tabs = new TabControl { Dock = DockStyle.Fill };

            // ============================================
            // ВКЛАДКА 1: СОТРУДНИКИ
            // ============================================
            var tabEmp = new TabPage("👤 Сотрудники");

            var panelEmp = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(245, 245, 245) };
            var btnAddEmp = CreateButton("➕ Добавить", 10, Color.FromArgb(76, 175, 80));
            var btnEditEmp = CreateButton("✏️ Изменить", 150, Color.FromArgb(255, 152, 0));
            var btnDelEmp = CreateButton("❌ Удалить", 290, Color.FromArgb(244, 67, 54));
            var btnRefreshEmp = CreateButton("🔄 Обновить", 430, Color.FromArgb(33, 150, 243));

            btnAddEmp.Click += (s, e) => AddEmployee();
            btnEditEmp.Click += (s, e) => EditEmployee();
            btnDelEmp.Click += (s, e) => DeleteEmployee();
            btnRefreshEmp.Click += (s, e) => LoadData();

            panelEmp.Controls.AddRange(new Control[] { btnAddEmp, btnEditEmp, btnDelEmp, btnRefreshEmp });

            gridEmployees = CreateGrid();
            tabEmp.Controls.Add(gridEmployees);
            tabEmp.Controls.Add(panelEmp);

            // ============================================
            // ВКЛАДКА 2: КАТАЛОГ СИЗ
            // ============================================
            var tabSIZ = new TabPage("📚 Каталог СИЗ");

            var panelSIZ = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(245, 245, 245) };
            var btnAddSIZ = CreateButton("➕ Добавить", 10, Color.FromArgb(76, 175, 80));
            var btnEditSIZ = CreateButton("✏️ Изменить", 150, Color.FromArgb(255, 152, 0));
            var btnDelSIZ = CreateButton("❌ Удалить", 290, Color.FromArgb(244, 67, 54));
            var btnRefreshSIZ = CreateButton("🔄 Обновить", 430, Color.FromArgb(33, 150, 243));

            btnAddSIZ.Click += (s, e) => AddSIZ();
            btnEditSIZ.Click += (s, e) => EditSIZ();
            btnDelSIZ.Click += (s, e) => DeleteSIZ();
            btnRefreshSIZ.Click += (s, e) => LoadData();

            panelSIZ.Controls.AddRange(new Control[] { btnAddSIZ, btnEditSIZ, btnDelSIZ, btnRefreshSIZ });

            gridSIZ = CreateGrid();
            tabSIZ.Controls.Add(gridSIZ);
            tabSIZ.Controls.Add(panelSIZ);

            // ============================================
            // ВКЛАДКА 3: ВЫДАННЫЕ СИЗ
            // ============================================
            var tabIssued = new TabPage("📦 Выданные СИЗ");

            var panelIssued = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(245, 245, 245) };
            var btnIssue = CreateButton("📋 Выдать СИЗ", 10, Color.FromArgb(76, 175, 80));
            var btnWriteOff = CreateButton("🔄 Списать", 150, Color.FromArgb(244, 67, 54));
            var btnRefreshIssued = CreateButton("🔄 Обновить", 290, Color.FromArgb(33, 150, 243));

            btnIssue.Click += (s, e) => IssueSIZ();
            btnWriteOff.Click += (s, e) => WriteOffSIZ();
            btnRefreshIssued.Click += (s, e) => LoadData();

            panelIssued.Controls.AddRange(new Control[] { btnIssue, btnWriteOff, btnRefreshIssued });

            gridIssued = CreateGrid();
            tabIssued.Controls.Add(gridIssued);
            tabIssued.Controls.Add(panelIssued);

            tabs.TabPages.Add(tabEmp);
            tabs.TabPages.Add(tabSIZ);
            tabs.TabPages.Add(tabIssued);

            this.Controls.Add(tabs);
        }

        // ============================================
        // ХЕЛПЕРЫ
        // ============================================

        private Button CreateButton(string text, int left, Color backColor)
        {
            var btn = new Button
            {
                Text = text,
                Left = left,
                Top = 12,
                Width = 130,
                Height = 36,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private DataGridView CreateGrid()
        {
            return new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 9),
                ColumnHeadersHeight = 35,
                EnableHeadersVisualStyles = false,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(25, 118, 210),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleLeft
                }
            };
        }

        private void LoadData()
        {
            gridEmployees.DataSource = null;
            gridEmployees.DataSource = DatabaseHelper.GetEmployees()
                .Select(e => new
                {
                    ID = e.Id,
                    ФИО = e.FullName,
                    Должность = e.Position,
                    Отдел = e.Department
                }).ToList();

            gridSIZ.DataSource = null;
            gridSIZ.DataSource = DatabaseHelper.GetSIZList()
                .Select(s => new
                {
                    ID = s.Id,
                    Название = s.Name,
                    Размер = s.Size,
                    Срок_мес = s.WearPeriodMonths
                }).ToList();

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

        // ============================================
        // СОТРУДНИКИ
        // ============================================

        private void AddEmployee()
        {
            var form = CreateForm("➕ Новый сотрудник", 420, 280);

            var txtN = AddInputField(form, "ФИО:", 20);
            var txtP = AddInputField(form, "Должность:", 70);
            var txtD = AddInputField(form, "Отдел/Цех:", 120);

            var btnOk = new Button
            {
                Text = "💾 Сохранить",
                Left = 130,
                Top = 180,
                Width = 120,
                Height = 35,
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };
            btnOk.FlatAppearance.BorderSize = 0;

            var btnCancel = new Button
            {
                Text = "❌ Отмена",
                Left = 260,
                Top = 180,
                Width = 120,
                Height = 35,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };
            btnCancel.FlatAppearance.BorderSize = 0;

            form.Controls.Add(btnOk);
            form.Controls.Add(btnCancel);
            form.AcceptButton = btnOk;
            form.CancelButton = btnCancel;

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

            var form = CreateForm("✏️ Редактировать", 420, 280);

            var txtN = AddInputField(form, "ФИО:", 20, emp.FullName);
            var txtP = AddInputField(form, "Должность:", 70, emp.Position);
            var txtD = AddInputField(form, "Отдел/Цех:", 120, emp.Department);

            var btnOk = new Button
            {
                Text = "💾 Сохранить",
                Left = 130,
                Top = 180,
                Width = 120,
                Height = 35,
                BackColor = Color.FromArgb(255, 152, 0),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };
            btnOk.FlatAppearance.BorderSize = 0;

            var btnCancel = new Button
            {
                Text = "❌ Отмена",
                Left = 260,
                Top = 180,
                Width = 120,
                Height = 35,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };
            btnCancel.FlatAppearance.BorderSize = 0;

            form.Controls.Add(btnOk);
            form.Controls.Add(btnCancel);

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

        // ============================================
        // КАТАЛОГ СИЗ
        // ============================================

        private void AddSIZ()
        {
            var form = CreateForm("➕ Новый СИЗ", 420, 280);

            var txtName = AddInputField(form, "Название:", 20);
            var txtSize = AddInputField(form, "Размер:", 70);
            var txtPeriod = AddInputField(form, "Срок носки (мес):", 120, "12");

            var btnOk = new Button
            {
                Text = "💾 Сохранить",
                Left = 130,
                Top = 180,
                Width = 120,
                Height = 35,
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };
            btnOk.FlatAppearance.BorderSize = 0;

            var btnCancel = new Button
            {
                Text = "❌ Отмена",
                Left = 260,
                Top = 180,
                Width = 120,
                Height = 35,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };
            btnCancel.FlatAppearance.BorderSize = 0;

            form.Controls.Add(btnOk);
            form.Controls.Add(btnCancel);

            if (form.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("Название обязательно!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!int.TryParse(txtPeriod.Text, out int period) || period <= 0)
                {
                    MessageBox.Show("Некорректный срок носки!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DatabaseHelper.AddSIZ(txtName.Text, txtSize.Text, period);
                LoadData();
                MessageBox.Show("СИЗ добавлен!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void EditSIZ()
        {
            if (gridSIZ.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите СИЗ!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = Convert.ToInt32(gridSIZ.SelectedRows[0].Cells["ID"].Value);
            var siz = DatabaseHelper.GetSIZList().FirstOrDefault(s => s.Id == id);
            if (siz == null) return;

            var form = CreateForm("✏️ Редактировать СИЗ", 420, 280);

            var txtName = AddInputField(form, "Название:", 20, siz.Name);
            var txtSize = AddInputField(form, "Размер:", 70, siz.Size);
            var txtPeriod = AddInputField(form, "Срок носки (мес):", 120, siz.WearPeriodMonths.ToString());

            var btnOk = new Button
            {
                Text = "💾 Сохранить",
                Left = 130,
                Top = 180,
                Width = 120,
                Height = 35,
                BackColor = Color.FromArgb(255, 152, 0),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };
            btnOk.FlatAppearance.BorderSize = 0;

            var btnCancel = new Button
            {
                Text = "❌ Отмена",
                Left = 260,
                Top = 180,
                Width = 120,
                Height = 35,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };
            btnCancel.FlatAppearance.BorderSize = 0;

            form.Controls.Add(btnOk);
            form.Controls.Add(btnCancel);

            if (form.ShowDialog() == DialogResult.OK)
            {
                if (!int.TryParse(txtPeriod.Text, out int period) || period <= 0)
                {
                    MessageBox.Show("Некорректный срок носки!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DatabaseHelper.UpdateSIZ(id, txtName.Text, txtSize.Text, period);
                LoadData();
                MessageBox.Show("СИЗ обновлён!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void DeleteSIZ()
        {
            if (gridSIZ.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите СИЗ!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = Convert.ToInt32(gridSIZ.SelectedRows[0].Cells["ID"].Value);
            string name = gridSIZ.SelectedRows[0].Cells["Название"].Value.ToString();

            if (MessageBox.Show($"Удалить СИЗ '{name}'?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                DatabaseHelper.DeleteSIZ(id);
                LoadData();
                MessageBox.Show("СИЗ удалён!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // ============================================
        // ВЫДАЧА СИЗ
        // ============================================

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

            var form = CreateForm("📋 Выдача СИЗ", 500, 320);

            var lblE = new Label { Text = "Сотрудник:", Left = 20, Top = 20, Width = 100 };
            var cmbE = new ComboBox
            {
                Left = 130,
                Top = 20,
                Width = 320,
                DropDownStyle = ComboBoxStyle.DropDownList,
                DataSource = emps,
                DisplayMember = "FullName",
                ValueMember = "Id"
            };

            var lblS = new Label { Text = "СИЗ:", Left = 20, Top = 65, Width = 100 };
            var cmbS = new ComboBox
            {
                Left = 130,
                Top = 65,
                Width = 320,
                DropDownStyle = ComboBoxStyle.DropDownList,
                DataSource = sizs,
                DisplayMember = "Name",
                ValueMember = "Id"
            };

            var lblQ = new Label { Text = "Количество:", Left = 20, Top = 110, Width = 100 };
            var txtQ = new TextBox { Left = 130, Top = 110, Width = 100, Text = "1" };

            var btnOk = new Button
            {
                Text = "📋 Выдать",
                Left = 130,
                Top = 180,
                Width = 120,
                Height = 35,
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };
            btnOk.FlatAppearance.BorderSize = 0;

            var btnCancel = new Button
            {
                Text = "❌ Отмена",
                Left = 260,
                Top = 180,
                Width = 120,
                Height = 35,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };
            btnCancel.FlatAppearance.BorderSize = 0;

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

        // ============================================
        // ВСПОМОГАТЕЛЬНЫЕ
        // ============================================

        private Form CreateForm(string title, int width, int height)
        {
            return new Form
            {
                Text = title,
                Size = new Size(width, height),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.WhiteSmoke
            };
        }

        private TextBox AddInputField(Form form, string label, int top, string defaultValue = "")
        {
            var lbl = new Label
            {
                Text = label,
                Left = 20,
                Top = top,
                Width = 110,
                Height = 25
            };

            var txt = new TextBox
            {
                Left = 140,
                Top = top,
                Width = 240,
                Height = 28,
                Text = defaultValue
            };

            form.Controls.Add(lbl);
            form.Controls.Add(txt);
            return txt;
        }
    }
}