using System;

namespace CourseWork27
{
    public abstract class AbstractMachine
    {
        #region Свойства

        /// <summary>
        /// Делимое.
        /// </summary>
        protected ushort A { get; set; }

        /// <summary>
        /// Делитель.
        /// </summary>
        protected uint B { get; set; }

        /// <summary>
        /// Частное.
        /// </summary>
        public uint C { get; internal set; }

        /// <summary>
        /// Счетчик.
        /// </summary>
        public byte Count { get; private set; }

        /// <summary>
        /// Переполнение.
        /// </summary>
        private bool OverFlow { get; set; }

        /// <summary>
        /// Конец автоматата.
        /// </summary>
        public bool Run { get; internal set; } = true;

        /// <summary>
        /// Вектор результата логических условий. 
        /// </summary>
        public bool[] X { get; }

        /// <summary>
        /// Микрооперации.
        /// </summary>
        internal readonly Action[] Operations;

        #endregion

        /// <summary>
        /// Инициализация полей.
        /// </summary>
        protected AbstractMachine()
        {
            X = new bool[7];
            X[0] = true;

            Operations = new Action[]
            {
                () => { C = (uint)(A & 0x7fff); }, // y0.
                () =>
                {
                    A &= 0x8000;
                    A |= (ushort)(B & 0x7fff);
                },
                () => { C = (uint)(((~A + 0x1) | 0x18000) + C + 0x1); }, // 0xc000
                () => { C = (uint)(C + ((ushort)(A << 1) >> 1)); }, // y3.

                () => { C <<= 1; }, // y4.
                () => { Count = 0; },
                () => { B &= 0x10000; },
                () => { B = (B << 17 >> 16) | (B & 0x10000) | (~(C << 15 >> 31) & 0x1); }, // y7.

                () =>
                {
                    Count = (byte)(Count == 0
                        ? 15
                        : Count - 1);
                }, // y8.
                () => { C = B & 0xFFFF; },
                () => { C += 2; },
                () => { C |= 0x10000; }, // y11.

                () => { Run = false; }, // y12.
                () => { OverFlow = true; }, // y13.
            };
        }

        /// <summary>
        /// Такт.
        /// </summary>
        public abstract void Step();

        /// <summary>
        /// Вычисление логического результата каждого логического блока. 
        /// </summary>
        internal void LogicalDevice()
        {
            X[1] = (A & 0x7fff) == 0;
            X[2] = C == 1;
            X[3] = C << 15 >> 31 == 1;
            X[4] = Count == 0;
            X[5] = (B & 0x1) == 0;
            X[6] = ((A >> 15) ^ (B >> 16)) == 1;
        }
    }
}