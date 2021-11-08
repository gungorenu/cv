using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.DataLayer
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Deletes an item from database set with ID
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="set">Db set</param>
        /// <param name="id">id of item</param>
        public static void Delete<T>(this DbSet<T> set, int id)
            where T : EntityObject, IIDModel
        {
            if (id == 0) return;
            var v = (from m in set where m.ID == id select m).Single();
            if (v != null)
                set.Remove(v);
        }
    }

}
