using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CourseWork27
{
    /// <summary>
    /// Главная форма.
    /// </summary>
    public partial class MainForm : Form
    {
        #region Поля

        /// <summary>
        /// Делимое А.
        /// </summary>
        private ushort _a;

        /// <summary>
        /// Делитель В.
        /// </summary>
        private uint _b;

        /// <summary>
        /// Управляющий автомат.
        /// </summary>
        private readonly ManageMachine _manageMachine;

        /// <summary>
        /// МикроПрограмма.
        /// </summary>
        private readonly MicroProgram _microProgram;

        #endregion

        /// <summary>
        /// Инициализация.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            _manageMachine = new ManageMachine(this);
            _microProgram = new MicroProgram(this);

            dataGridView_A.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridView_B.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridView_C.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridView_Count.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            dataGridView_A.Font = new Font("Microsoft Sans Serif", 8);
            dataGridView_B.Font = new Font("Microsoft Sans Serif", 8);
            dataGridView_C.Font = new Font("Microsoft Sans Serif", 8);
            dataGridView_Count.Font = new Font("Microsoft Sans Serif", 8);
            const int widthColumn = 25;
            var width = 0;

            for (var i = 4 - 1; i >= 0; i--)
            {
                var index = dataGridView_Count.Columns.Add("column_" + i, i.ToString());
                dataGridView_Count.Columns[index].Width = widthColumn;
                width += widthColumn;
            }

            dataGridView_Count.Height = 45;
            dataGridView_Count.Width = width + 3;
            width = 0;

            for (var i = 16 - 1; i >= 0; i--)
            {
                var index = dataGridView_A.Columns.Add("column_" + i, i.ToString());
                dataGridView_A.Columns[index].Width = widthColumn;
                width += widthColumn;
            }

            dataGridView_A.Height = 45;
            dataGridView_A.Width = width + 3;
            width = 0;

            for (var i = 17 - 1; i >= 0; i--)
            {
                var index = dataGridView_C.Columns.Add("column_" + i, i.ToString());
                dataGridView_C.Columns[index].Width = widthColumn;
                dataGridView_B.Columns.Add("column_" + i, i.ToString());
                dataGridView_B.Columns[index].Width = widthColumn;
                width += widthColumn;
            }
            
            dataGridView_B.Height = 45;
            dataGridView_B.Width = width + 3;
            dataGridView_C.Height = 45;
            dataGridView_C.Width = width + 3;

            dataGridView_A.Rows.Add(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            dataGridView_B.Rows.Add(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            dataGridView_C.Rows.Add(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            dataGridView_Count.Rows.Add(0, 0, 0, 0);

            dataGridView_A.BorderStyle = dataGridView_B.BorderStyle = dataGridView_C.BorderStyle =
                dataGridView_Count.BorderStyle = BorderStyle.FixedSingle;

            dataGridView_A.RowHeadersVisible = dataGridView_B.RowHeadersVisible = dataGridView_C.RowHeadersVisible =
                dataGridView_Count.RowHeadersVisible = false;

            dataGridView_A.RowsDefaultCellStyle.Alignment = dataGridView_B.RowsDefaultCellStyle.Alignment =
                dataGridView_C.RowsDefaultCellStyle.Alignment = dataGridView_Count.RowsDefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleRight;
            
            UpdateStateMemory(0);
        }

        #region Обработчики нажатий

        /// <summary>
        /// Обработчик события: нажате на разрядную сетку делимого (A).
        /// </summary>
        private void dataGridView_A_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;

            var value = (int)dataGridView_A.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            value = value == 0 ? 1 : 0;
            dataGridView_A.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = value;
            var cells = dataGridView_A.Rows[e.RowIndex].Cells;
            var strB = new StringBuilder();
            for (var i = 0; i < cells.Count; i++)
                strB.Append(cells[i].Value);

            var denial = false;
            ushort a = 0;
            if ((int)cells[0].Value == 1)
            {
                a = (ushort)Convert.ToInt16(strB.ToString(), 2);
                strB.Replace("1", "0", 0, 1);
                denial = true;
            }

            _a = (ushort)Convert.ToInt16(strB.ToString(), 2);
            _a = denial ? a : _a;
        }

        /// <summary>
        /// Обработчик события: нажате на разрядную сетку делителя (B).
        /// </summary>
        private void dataGridView_B_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;

            var value = (int)dataGridView_B.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            value = value == 0 ? 1 : 0;
            dataGridView_B.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = value;
            var cells = dataGridView_B.Rows[e.RowIndex].Cells;
            var strB = new StringBuilder();
            for (var i = 0; i < cells.Count; i++)
                strB.Append(cells[i].Value);

            if ((int)cells[0].Value == 1)
            {
                strB.Replace("1", "0", 16, 1);
            }

            _b = (uint)Convert.ToInt32(strB.ToString(), 2);
        }

        #endregion

        #region Отображение данных

        /// <summary>
        /// Обновление визуальной части регистров.
        /// </summary>
        /// <param name="count">Счетчик.</param>
        /// <param name="c">Обновляемый регистр С.</param>
        public void UpdateInfoRegister(uint c, byte count)
        {
            // update Count.
            var result = Convert.ToString(count, 2).PadLeft(4, '0');
            for (var i = 4 - 1; i >= 0; i--)
                dataGridView_Count.Rows[0].Cells[i].Value = result[i];

            // update info about number C.
            result = Convert.ToString(c, 2).PadLeft(32, '0');
            for (int i = 17 - 1, q = 31; i >= 0; i--, q--)
                dataGridView_C.Rows[0].Cells[i].Value = result[q];
        }

        /// <summary>
        /// Обновление состояния автомата.
        /// </summary>
        /// <param name="state">Массив с метками состояний.</param>
        public void UpdateStateMemory(bool[] state)
        {
            radioButton_A0.Checked = state[0];
            radioButton_A1.Checked = state[1];
            radioButton_A2.Checked = state[2];
            radioButton_A3.Checked = state[3];
            radioButton_A4.Checked = state[4];
            radioButton_A5.Checked = state[5];
            radioButton_A6.Checked = state[6];
            radioButton_A7.Checked = state[7];
            radioButton_A8.Checked = state[8];
            radioButton_A9.Checked = state[9];
            radioButton_A10.Checked = state[10];
            radioButton_A11.Checked = state[11];

            for (var i = 0; i < state.Length; i++)
                checkedListBox_A.SetItemChecked(i, state[i]);
        }

        /// <summary>
        /// Обновление состояния автомата.
        /// </summary>
        /// <param name="a">Номер метки.</param>
        public void UpdateStateMemory(ushort a)
        {
            radioButton_A0.Checked = a == 0;
            radioButton_A1.Checked = a == 1;
            radioButton_A2.Checked = a == 2;
            radioButton_A3.Checked = a == 3;
            radioButton_A4.Checked = a == 4;
            radioButton_A5.Checked = a == 5;
            radioButton_A6.Checked = a == 6;
            radioButton_A7.Checked = a == 7;
            radioButton_A8.Checked = a == 8;
            radioButton_A9.Checked = a == 9;
            radioButton_A10.Checked = a == 10;
            radioButton_A11.Checked = a == 11;
        }

        /// <summary>
        /// Обновление значений в комбинационных схемах.
        /// </summary>
        /// <param name="t">Терма.</param>
        /// <param name="y">Выходные сигналы из КСУ.</param>
        /// <param name="d">Выходные сигналы из КСD.</param>
        /// <param name="x">Выходные логические состояния из ОА.</param>
        public void UpdateInfoKc(bool[] t, bool[] y, bool[] d, bool[] x)
        {
            for (var i = 0; i < t.Length; i++)
                checkedListBox_T.SetItemChecked(i, t[i]);
            for (var i = 0; i < y.Length; i++)
                checkedListBox_Y.SetItemChecked(i, y[i]);
            for (var i = 0; i < d.Length; i++)
                checkedListBox_D.SetItemChecked(i, d[i]);
            for (var i = 0; i < x.Length; i++)
                checkedListBox_X.SetItemChecked(i, x[i]);
        }

        /// <summary>
        /// Обновление текущего состояния.
        /// </summary>
        /// <param name="dt">Текущее состояние.</param>
        public void UpdateInfoState(bool[] dt)
        {
            for (var i = 0; i < dt.Length; i++)
                checkedListBox_Dt.SetItemChecked(i, dt[i]);
        }

        /// <summary>
        /// Обновление значений в ПЛУ.
        /// </summary>
        /// <param name="x">Выходные логические состояния из ОА.</param>
        public void UpdateInfoPly(bool[] x)
        {
            for (int i = 0, q = 0; i < x.Length; i++)
            {
                if (i == 0
                    || i == 1
                    || i == 4
                    || i == 5
                    || i == 6)
                {
                    continue;
                }

                checkedListBox_Ply.SetItemChecked(q, x[i]);
                q++;
            }
        }

        private void checkBox_x0_0_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_x0_0.Checked) 
                checkBox_X0_1.Checked = false;
        }

        private void checkBox_X0_1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_X0_1.Checked) 
                checkBox_x0_0.Checked = false;
        }
        
        #endregion

        #region Buttons

        /// <summary>
        /// Сброс данных на регистре A.
        /// </summary>
        private void button_reset_A_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < dataGridView_A.Rows[0].Cells.Count; i++)
            {
                dataGridView_A.Rows[0].Cells[i].Value = 0;
            }

            _a = 0;
        }

        /// <summary>
        /// Сброс данных на регистре B.
        /// </summary>
        private void button_reset_B_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < dataGridView_B.Rows[0].Cells.Count; i++)
            {
                dataGridView_B.Rows[0].Cells[i].Value = 0;
            }

            _b = 0;
        }

        /// <summary>
        /// Обработчик нажатия на кнопку "Такт".
        /// </summary>
        private void button_Step_Click(object sender, EventArgs e)
        {
            if (checkBox_X0_1.Checked)
            {
                if (radioButton_Ya.Checked)
                {
                    if (radioButton_AutoMode.Checked)
                    {
                        _manageMachine.InputData(_a, _b);
                        _manageMachine.AutomaticMode();
                    }
                    else
                    {
                        _manageMachine.InputData(_a, _b);
                        _manageMachine.Step();
                    }
                }
                else
                {
                    if (radioButton_AutoMode.Checked)
                    {
                        _microProgram.InputData(_a, _b);
                        _microProgram.AutomaticMode();
                    }
                    else
                    {
                        _microProgram.InputData(_a, _b);
                        _microProgram.Step();
                    }
                }
            }
        }

        /// <summary>
        /// Сброс данных на регистрах.
        /// </summary>
        private void button_Reset_Click(object sender, EventArgs e)
        {
            _manageMachine.Reset();
            _microProgram.Reset();
            UpdateInfoRegister(0, 0);
            UpdateStateMemory(0);
        }

        #endregion
    }
}