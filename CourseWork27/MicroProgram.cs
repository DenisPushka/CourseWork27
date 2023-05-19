namespace CourseWork27
{
    /// <summary>
    /// МикроПрограмма.
    /// </summary>
    public class MicroProgram : AbstractMachine
    {
        /// <summary>
        /// Текущее состояние автомата.
        /// </summary>
        private byte _state;

        /// <summary>
        /// Форма отображеия.
        /// </summary>
        private readonly MainForm _form;

        /// <summary>
        /// Данные внесены в автомат.
        /// </summary>
        private bool _installData;

        public MicroProgram(MainForm form)
        {
            _state = 0;
            _form = form;
        }

        /// <summary>
        /// Автоматический режим.
        /// </summary>
        public void AutomaticMode()
        {
            while (Run)
            {
                Step();
            }
        }

        /// <summary>
        /// Такт.
        /// </summary>
        public override void Step()
        {
            switch (_state)
            {
                case 0:
                    if (X[0])
                    {
                        Operations[0]();
                        Operations[1]();
                        _state = 1;
                    }

                    break;
                case 1:
                    if (X[1])
                    {
                        Operations[13]();
                        _state = 11;
                    }
                    else if (X[2])
                    {
                        Operations[12]();
                        _state = 0;
                    }
                    else
                    {
                        Operations[2]();
                        _state = 2;
                    }

                    break;
                case 2:
                    if (X[3])
                    {
                        Operations[3]();
                        _state = 3;
                    }
                    else
                    {
                        Operations[13]();
                        _state = 11;
                    }

                    break;
                case 3:
                    Operations[4]();
                    Operations[5]();
                    Operations[6]();
                    _state = 4;

                    break;
                case 4:
                    Operations[2]();
                    _state = 5;

                    break;
                case 5:
                    Operations[7]();
                    _state = 6;

                    break;
                case 6:
                    if (X[3])
                    {
                        Operations[3]();
                        _state = 7;
                    }
                    else
                    {
                        Operations[4]();
                        Operations[8]();
                        _state = 8;
                    }

                    break;
                case 7:
                    Operations[4]();
                    Operations[8]();
                    _state = 8;

                    break;
                case 8:
                    if (X[4])
                    {
                        Operations[9]();
                        _state = 9;
                    }
                    else
                    {
                        Operations[2]();
                        _state = 5;
                    }

                    break;
                case 9:
                    if (X[5])
                    {
                        Operations[10]();
                        _state = 10;
                    }
                    else if (X[6])
                    {
                        Operations[11]();
                        _state = 11;
                    }
                    else
                    {
                        Operations[12]();
                        _state = 0;
                    }

                    break;
                case 10:
                    if (X[6])
                    {
                        Operations[11]();
                        _state = 11;
                    }
                    else
                    {
                        Operations[12]();
                        _state = 0;
                    }

                    break;
                case 11:
                    Operations[12]();
                    _state = 0;
                    
                    break;
            }

            // Отображение данных.
            _form.UpdateInfoRegister(C, Count, B);
            _form.UpdateStateMemory(_state);
            LogicalDevice();
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
                A = a;
                B = b;
                _installData = true;
            }
        }

        /// <summary>
        /// Сброс данных.
        /// </summary>
        public void Reset()
        {
            Run = true;
            _installData = false;
            A = 0;
            B = 0;
            C = 0;
            _state = 0;
        }
    }
}