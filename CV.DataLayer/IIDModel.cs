using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.DataLayer
{
    /// <summary>
    /// Items' interface for ID access
    /// </summary>
    public interface IIDModel
    {
        /// <summary>
        /// Item ID value
        /// </summary>
        int ID { get; set; }
    }
}
