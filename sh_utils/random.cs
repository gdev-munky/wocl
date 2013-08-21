using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WOCL.Shared.Utils
{
    /// <summary>
    /// This class is for comfortable use of pseudorandom stuff
    /// Wherever you want, usage is "Rnd.r[...];" (Dont forget to use WOCL.Shared.Utils)
    /// </summary>
    public class Rnd
    {
        public static Rnd r = new Rnd();
        private Random rand = new Random();
        /// <summary>
        /// Random int of [min, max] (all inclusive)
        /// </summary>
        /// <param name="min">Min. possible value</param>
        /// <param name="max">Max. possible value</param>
        /// <returns></returns>
        public int this[int min, int max]
        {
            get
            {
                return rand.Next(min, max + 1);
            }
        }
        /// <summary>
        /// Random double of [min, max] (all inclusive)
        /// </summary>
        /// <param name="min">Min. possible value</param>
        /// <param name="max">Max. possible value</param>
        /// <returns></returns>
        public double this[double min, double max]
        {
            get
            {
                return rand.NextDouble() * (max - min) + min;
            }
        }
        /// <summary>
        /// Converts double representation of probability ([0..1]) to boolean
        /// So, if you want cyclic event to be raised with probabilty of 25%, use "if (Rnd.r[0.25]) {//25%} else {//75%}"
        /// </summary>
        /// <param name="chance">Probability [0..1]</param>
        /// <returns></returns>
        public bool this[double chance]
        {
            get
            {
                if (chance >= 1)
                    return true;
                if (chance <= 0)
                    return false;
                return rand.NextDouble() < chance;
            }
        }
    }
}
