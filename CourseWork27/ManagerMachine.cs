namespace CourseWork27
{
    /// <summary>
    /// Управляющий автомат.
    /// </summary>
    public class ManageMachine
    {
        #region Поля

        /// <summary>
        /// Текущее состояние автомата.
        /// </summary>
        private bool[] _a;

        /// <summary>
        /// Сигналы из КСД.
        /// </summary>
        private bool[] _d;

        /// <summary>
        /// Термы.
        /// </summary>
        private readonly bool[] _t;

        /// <summary>
        /// Сигналы из КСУ.
        /// </summary>
        private readonly bool[] _y;

        /// <summary>
        /// Предыдущая метка автомата.
        /// </summary>
        private byte _lastState;

        /// <summary>
        /// Операционный автомат.
        /// </summary>
        private OperationMachine _operationMachine;

        /// <summary>
        /// Форма для отображения.
        /// </summary>
        private readonly MainForm _mainForm;

        /// <summary>
        /// Данные внесены в автомат.
        /// </summary>
        private bool _installData;

        /// <summary>
        /// Автомат работает.
        /// </summary>
        private bool _run = true;

        #endregion

        public ManageMachine(MainForm form)
        {
            _mainForm = form;
            _t = new bool[21];
            _a = new bool[12];
            _d = new bool[4];
            _y = new bool[14];
        }

        /// <summary>
        /// Внесение данные. Если данные еще не внесесы, то добавляются.
        /// </summary>
        /// <param name="a">Делимое А.</param>
        /// <param name="b">Делитель В.</param>
        public void InputData(ushort a, uint b)
        {
            if (!_installData)
            {
                _operationMachine = new OperationMachine(a, b);
                _installData = true;
            }
        }

        /// <summary>
        /// Автоматический режим.
        /// </summary>
        public void AutomaticMode()
        {
            while (_run)
            {
                Step();
            }
            
            _mainForm.UpdateStateMemory(0);
        }

        /// <summary>
        /// Такт.
        /// </summary>
        public void Step()
        {
            if (!_run)
            {
                _mainForm.UpdateStateMemory(0);
                return;
            }

            var x = _operationMachine.X;
            _mainForm.UpdateInfoPly(x);
            _mainForm.UpdateInfoState(_d);

            StateMemory(Decoder());
            KC_T(x);
            KC_Y();
            KC_D();
            _operationMachine.Step(_y);

            _run = _operationMachine.Run;
            // Вывод информации на остальные схемы.
            _mainForm.UpdateInfoRegister(_operationMachine.C, _operationMachine.Count, _operationMachine.B);
            _mainForm.UpdateStateMemory(_a);
            _mainForm.UpdateInfoKc(_t, _y, _d, _operationMachine.X);
        }

        /// <summary>
        /// Комбинационная схема Т (Терма).
        /// </summary>
        private void KC_T(bool[] x)
        {
            _t[0] = _a[0] && !x[0];
            _t[1] = _a[1] && !x[1] && x[2];
            _t[2] = _a[9] && !x[5] && !x[6];
            _t[3] = _a[10] && !x[6];
            _t[4] = _a[11];
            _t[5] = _a[0] && x[0];
            _t[6] = _a[1] && !x[1] && !x[2];
            _t[7] = _a[2] && x[3];
            _t[8] = _a[3];
            _t[9] = _a[4];
            _t[10] = _a[8] && !x[4];
            _t[11] = _a[5];
            _t[12] = _a[6] && x[3];
            _t[13] = _a[6] && !x[3];
            _t[14] = _a[7];
            _t[15] = _a[8] && x[4];
            _t[16] = _a[9] && x[5];
            _t[17] = _a[1] && x[1];
            _t[18] = _a[2] && !x[3];
            _t[19] = _a[9] && !x[5] && x[6];
            _t[20] = _a[10] && x[6];
        }

        /// <summary>
        /// Комбинационная схема D.
        /// </summary>
        private void KC_D()
        {
            _d[0] = _t[5] || _t[7] || _t[9] || _t[10] || _t[12] || _t[15] || _t[17] || _t[18] || _t[19] || _t[20];
            _d[1] = _t[6] || _t[7] || _t[11] || _t[12] || _t[16] || _t[17] || _t[18] || _t[19] || _t[20];
            _d[2] = _t[8] || _t[9] || _t[10] || _t[11] || _t[12];
            _d[3] = _t[13] || _t[14] || _t[15] || _t[16] || _t[17] || _t[18] || _t[19] || _t[20];
        }

        /// <summary>
        /// Комбинационная схема Y.
        /// </summary>
        private void KC_Y()
        {
            _y[0] = _y[1] = _t[5];
            _y[2] = _t[6] || _t[9] || _t[10];
            _y[3] = _t[7] || _t[12];
            _y[4] = _t[8] || _t[13] || _t[14];
            _y[5] = _y[6] = _t[8];
            _y[7] = _t[11];
            _y[8] = _t[13] || _t[14];
            _y[9] = _t[15];
            _y[10] = _t[16];
            _y[11] = _t[19] || _t[20];
            _y[12] = _t[1] || _t[2] || _t[3] || _t[4];
            _y[13] = _t[17] || _t[18];
        }

        /// <summary>
        /// Дешифратор.
        /// </summary>
        private byte Decoder()
        {
            byte number = 0;

            if (_d[0])
                number = 1;
            if (_d[1])
                number += 2;
            if (_d[2])
                number += 4;
            if (_d[3])
                number += 8;

            return number;
        }

        /// <summary>
        /// Память состояний.
        /// </summary>
        /// <param name="newState">Индекс нового состояния.</param>
        private void StateMemory(byte newState)
        {
            _a[_lastState] = false;
            // Запоминаем состояние, чтобы можно было его установить в false на след такте.
            _lastState = newState;
            _a[newState] = true;
        }

        /// <summary>
        /// Сброс данных.
        /// </summary>
        public void Reset()
        {
            _run = true;
            _installData = false;
            _a = new bool[12];
            _a[0] = true;
            _d = new bool[4];
            _operationMachine = new OperationMachine(0, 0);
            _lastState = 0;
        }
    }
}