using System;

namespace CourseWork27
{
    /// <summary>
    /// Операционный автомат.
    /// </summary>
    public class OperationMachine : AbstractMachine
    {
        /// <summary>
        /// Инициализация полей.
        /// </summary>
        /// <param name="a">Делимое А.</param>
        /// <param name="b">Делитель В.</param>
        public OperationMachine(ushort a, uint b)
        {
            A = a;
            B = b;
            X[0] = true;
        }
        
        /// <summary>
        /// Такт.
        /// </summary>
        /// <param name="signals">Вектор сигналов из КСУ.</param>
        public void Step(bool[] signals)
        {
            for (var index = 0; index < signals.Length; index++)
            {
                if (signals[index])
                {
                    Operations[index]();
                }
            }
            
            LogicalDevice();
        }

        public override void Step() => Step(Array.Empty<bool>());
    }
}